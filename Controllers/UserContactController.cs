using ContactKeeper.Services.Repositories;using ContactKeeper.Data;
using ContactKeeper.Services.Interfaces;
using ContactKeeper.Models;
using ContactKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace ContactKeeper.Controllers
{

    [ApiController]
    [Route("UserContact")]
    public class UserContactController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IunitOfWork _iunitOfWork;
        private readonly IUserContactRepository _userContactRepository;

        public UserContactController
        (
            DataContext context,
            IUserContactRepository userContactRepository,
            IunitOfWork iunitOfWork
        )
        {
            _context = context;
            _userContactRepository = userContactRepository;
            _iunitOfWork = iunitOfWork;
        }

        #region [addUser]
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Add a new contact for the logged-in user",
            Description = "Creates a contact only for the authenticated user"
        )]
        public async Task<ActionResult<UserContact>> AddUserContact([FromBody] UserContact userContact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _iunitOfWork.BeginTransaction();
            await _userContactRepository.AddUserContact(userContact);
            await _iunitOfWork.CommitAsync();

            return Ok(userContact);
        }
        #endregion

        #region [getallUsersContacts]
        [HttpGet]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Get all users contacts",
            Description = "Get all users contacts from the database"
        )]
        public async Task<ActionResult<List<UserContact>>> GetUserContact()
        {
            
            try
            {
                var users = await _userContactRepository.GetUserContact();
                return Ok(users);
            }
            catch(Exception ex)    

            {
                return BadRequest(new { message = "can't list contact users", error = ex.Message });
            }
        }
        #endregion

        #region [getUserContactById]
        [HttpGet]
        [Route("id/{userId:int}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Get user contact by id",
            Description = "Get any user on table by your id"
        )]
        public async Task<ActionResult<IEnumerable<UserContact>>> GetUserContactById(int userId)
        {
            try
            {
                var user = await _userContactRepository.GetUserContactById(userId);
                if(user == null)
                    return NotFound(new{Message= "ID not found!"});

                return Ok(user);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Can't handle Id user Contact", error = ex.Message });
            }
        }
        #endregion

        #region [GetUsersByDdd]
        [Route("ddd/{ddd:int}")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "[AUTH REQUIRED] Get users by DDD",
            Description = "Get users from the database by their DDD"
        )]
        [HttpGet]
        public async Task<IActionResult> GetUsersByDdd(int ddd)
        {
            var users = await _userContactRepository.GetInfoUserByDdd(ddd);
            if (users == null || users.Count == 0)
            {
            return NotFound(new { message = "No users found with the specified DDD." });
            }
            return Ok(users);
        }            
        #endregion


    }
}