using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Interfaces;
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
        //[Authorize(Roles = "teste")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Add a new user contact",
            Description = "Add a new user contact on users to the database" 
        )]
        public async Task<ActionResult<UserContact>> AddUserContact([FromBody] UserContact userContact)
        {

            if (!ModelState.IsValid)
            {
            return BadRequest(ModelState); // Retorna erros de validação padrão
            }                                 

            _iunitOfWork.BeginTransaction();
            await _iunitOfWork.UserContactRepository.AddUserContact(userContact);
            await _iunitOfWork.CommitAsync();
            return Ok(userContact);

        }
        #endregion
       
        #region [getallUsersContacts]
        [HttpGet]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(
            Summary = "Get all users contacts",
            Description = "Get all users contacts from the database"
            //Description = "Get all users from the database"
        )]   
        public async Task<ActionResult<List<UserContact>>> GetUserContact()
        {
            var users = await _iunitOfWork.UserContactRepository.GetUserContact();
            try
            {
                return Ok(users);
            }
            catch
            {
                return BadRequest(new{message="cant  list contact users"});  
            }
        }
        #endregion

    
    }
}