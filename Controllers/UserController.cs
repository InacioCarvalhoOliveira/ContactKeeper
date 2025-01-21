using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Interfaces;
using ContactKeeper.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ContactKeeper.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly IuserRepository _userRepository;
        public UserController(DataContext context,
        IuserRepository userRepository,
        UnitOfWork UnitOfWork)
        {
            _context = context;            
            _userRepository = userRepository;
            _unitOfWork = UnitOfWork;
        }

        #region [getallUsers]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Get all users from the database"
        )]   
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _unitOfWork.UserRepository.GetUsers();
            try
            {
                return Ok(users);
            }
            catch
            {
                return BadRequest(new{message="cant list users"});  
            }
        }
        #endregion

        #region [getUsersById]
        [HttpGet]
        [Route("{id:int}")]
        [SwaggerOperation(
            Summary = "Get user by ID",
            Description = "Get a user from the database by their ID"
        )]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(id);
            if(user == null)
            return NotFound(new { message = "user not found" });
            
            try
            {
            return Ok(user);
            }
            catch
            {
            return BadRequest(new { message = "cant list user" });
            }  
        }   
        #endregion

        #region [GetUsersByDdd]
        [Route("ddd/{ddd:int}")]
        [SwaggerOperation(
            Summary = "Get users by DDD",
            Description = "Get users from the database by their DDD"
        )]
        [HttpGet]
        public async Task<IActionResult> GetUsersByDdd(int ddd)
        {
            var users = await _userRepository.GetInfoUserByDdd(ddd);
            if (users == null || users.Count == 0)
            {
            return NotFound(new { message = "No users found with the specified DDD." });
            }
            return Ok(users);
        }            
        #endregion    

        #region [addUser]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Add a new user",
            Description = "Add a new user to the database"
        )]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
            return BadRequest(ModelState); // Retorna erros de validação padrão
            }                      

            _unitOfWork.BeginTransaction();
            await _unitOfWork.UserRepository.AddUser(user);
            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        #endregion

        #region [UpdateUser] 
        [HttpPut]
        [Route("{id:int}")]
        [SwaggerOperation(
            Summary = "Update a user",
            Description = "Update an existing user in the database"
        )]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
           if(id != user.Id)
           {
               return BadRequest(new { message = "id not match" });
           }
              if (!ModelState.IsValid)
              {
            return BadRequest(ModelState); // Retorna erros de validação padrão
              }

            try
            {
                _unitOfWork.BeginTransaction();
                await _unitOfWork.UserRepository.UpdateUser(user);
                await _unitOfWork.CommitAsync();
                return Ok(user);
            }
            catch
            {
                _unitOfWork.Rollback();
                return BadRequest(new { message = "cant update user" });
            }
        }
        #endregion

        #region [DeleteUser]
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerOperation(
        Summary = "Delete a user",
        Description = "Delete a user from the database by their ID"
        )]
        public async Task<IActionResult> DeleteUser(int id)
        {
           try
           {
            _unitOfWork.BeginTransaction();
            await _unitOfWork.UserRepository.DeleteUser(id);
            await _unitOfWork.CommitAsync();
            return NoContent();
           }
           catch(Exception ex)
           {
               _unitOfWork.Rollback();
               return BadRequest( ex.Message);
           }
        }
        #endregion
        
    }
}