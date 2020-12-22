
using Data;
using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggController: BaseApicontroller
    {
        private readonly DataContext _dataContext;

        public BuggController(DataContext  dataContext){

            _dataContext= dataContext;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "unauthorize";
        }


      
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetnotFound()
        {
            var thing= _dataContext.Users.Find(-1);

            if(thing == null) return NotFound();
            return Ok(thing);
        }


      
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing= _dataContext.Users.Find(-1);
            var thingReturn= thing.ToString();

            return thingReturn;
        }


       
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("this was not a good request");
        }
     
    }
}