using System.Text;
using System.Text.Json;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Interfaces;
using ContactKeeper.Microservices;
using ContactKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using Prometheus;
using Swashbuckle.AspNetCore.Annotations;

namespace ContactKeeper.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IunitOfWork _iunitOfWork;
        private readonly IuserRepository _userRepository;
        private readonly HalfOpenCircuit _circuitBreakerPolicy;
        private readonly string _authTriggerUrl;
        private readonly IConfiguration _configuration;

        public UserController
        (
            DataContext context,
            IuserRepository userRepository,
            IunitOfWork iunitOfWork,
            IConfiguration configuration,
            HalfOpenCircuit circuitBreakerPolicy
        )
        {
            _context = context;
            _userRepository = userRepository;
            _iunitOfWork = iunitOfWork;
            _configuration = configuration;
            _circuitBreakerPolicy = circuitBreakerPolicy;
            _authTriggerUrl = _configuration["AuthTriggerUrl"];
        }

        #region [prometheus metrics]
        // Contador para logins bem-sucedidos
        private static readonly Counter LoginSuccessCounter = Metrics
            .CreateCounter("app_logins_success_total", "Número total de logins bem-sucedidos");

        // Contador para logins com falha
        private static readonly Counter LoginFailureCounter = Metrics
            .CreateCounter("app_logins_failure_total", "Número total de logins com falha");

        // Métrica de latência para autenticação
        private static readonly Histogram AuthLatencyHistogram = Metrics
            .CreateHistogram("app_auth_latency_seconds_count", "Latência de autenticação em segundos");
        #endregion

        #region [getallUsers]
        [HttpGet]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Get all users",
            Description = "Get all users from the database"
        )]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                var users = await _iunitOfWork.UserRepository.GetUsers();
                return Ok(users);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "can't list users", error = ex.Message });
            }
        }
        #endregion

        #region [getUsersById]
        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Get user by ID",
            Description = "Get a user from the database by their ID"
        )]
        public async Task<ActionResult<User>> GetUserById(int id)
        {

            try
            {
                var user = await _iunitOfWork.UserRepository.GetUserById(id);
                if (user == null)
                    return NotFound(new { message = "user not found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "cant list user", error = ex.Message });
            }
        }
        #endregion       

        #region [addUser]
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Add a new user",
            Description = "Add a new user to the database"
        )]
        public async Task<ActionResult<User>> AddUser(
            [FromBody] User user
            )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _iunitOfWork.BeginTransaction();
            await _userRepository.AddUser(user);
            await _iunitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        #endregion

        // #region [Login]
        // [HttpPost]
        // [Route("login")]
        // [AllowAnonymous]
        // public async Task<ActionResult<dynamic>> Authenticate(
        //     [FromBody] UserContact model,
        //     [FromServices] DataContext context
        // )
        // {
        //     var user = await _iunitOfWork.UserRepository.Authenticate(model.UserId);
        //     if (user == null)
        //         return NotFound(new { message = "User or password invalid" });

        //     var token = TokenService.GenerateToken(user);
        //     user.Password = "";

        //     return new
        //     {
        //         user = user,
        //         token = token
        //     };
        // }
        // #endregion

        #region [Authenticate on Azure]
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Authenticate user",
            Description = "Authenticate a user and return a JWT token for further requests"
        )]
        public async Task<ActionResult<dynamic>> Authenticate(
        [FromBody] UserDto model,
        [FromServices] DataContext context)
        {
            try
            {
                ActionResult<dynamic> result = null;
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {

                    using (AuthLatencyHistogram.NewTimer())
                    {
                        var user = await _userRepository.Authenticate(model.Username, model.Role, model.Password);
                        if (user == null)
                        {
                            LoginFailureCounter.Inc(); // Increment failure on invalid login
                            result = NotFound(new { message = "User or password invalid" });
                            return;
                        }
                        else
                        {
                            LoginSuccessCounter.Inc(); // Increment success on valid login
                        }

                        var payload = new
                        {
                            Username = user.Username,
                            Role = user.Role,
                        };

                        using var httpClient = new HttpClient();
                        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(_authTriggerUrl, content);
                        if (!response.IsSuccessStatusCode)
                        {
                            result = StatusCode((int)response.StatusCode, "Token generation failed");
                            return;
                        }

                        var tokenJson = await response.Content.ReadAsStringAsync();
                        user.Password = "";
                        var token = JsonDocument.Parse(tokenJson).RootElement.GetProperty("token").GetString();
                        result = Ok(new { token = token, user = user });
                    }

                });
                return result;
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(503, "Service temporarily unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region [UpdateUser] 
        [HttpPut]
        [Route("update/{id:int}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Update a user",
            Description = "Update an existing user in the database"
        )]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest(new { message = "id not match" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna erros de validação padrão
            }

            try
            {
                _iunitOfWork.BeginTransaction();
                await _iunitOfWork.UserRepository.UpdateUser(user);
                await _iunitOfWork.CommitAsync();
                return Ok(user);
            }
            catch
            {
                _iunitOfWork.Rollback();
                return BadRequest(new { message = "cant update user" });
            }
        }
        #endregion

        #region [DeleteUser]
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(
        Summary = "[AUTH REQUIRED] Delete a user",
        Description = "Delete a user from the database by their ID"
        )]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _iunitOfWork.BeginTransaction();
                await _iunitOfWork.UserRepository.DeleteUser(id);
                await _iunitOfWork.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _iunitOfWork.Rollback();
                return BadRequest(ex.Message);
            }
        }
        #endregion

    }
}