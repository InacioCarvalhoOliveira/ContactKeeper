using ContactKeeper.Contracts;
using ContactKeeper.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Swashbuckle.AspNetCore.Annotations;

namespace ContactKeeper.Controller
{
    [ApiController]
    [Route("Health")]
    public class HealthController : ControllerBase
    {
        private readonly DataContext _context;
        public HealthController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("HealthDataBase")]
        [SwaggerOperation(
            Summary = "Get connection to the database",
            Description = "Check if the connection to the database is working"
        )]
        public ActionResult<string> HealthDataBase()
        {
            try
            {


                bool canConnect = _context.Database.CanConnect();
                if (canConnect == true)
                {
                    return Ok($"Connection available on |{Environment.MachineName}|  connected at |{DateTime.Now}| " +
                     $"Server: {_context.Database.GetDbConnection().DataSource}| Database: {_context.Database.GetDbConnection().Database}|");
                }
                else
                {
                    return BadRequest(new { message = "Connection to the database isn't working" });
                }
            }
            catch
            {
                return BadRequest(new { message = "service isn't avaliable, check database provider" });
            }
        }
    }
}
