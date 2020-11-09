using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UsersController:ControllerBase
    {
        private DataContext _context;

        public UsersController(DataContext context){

            _context=context;
        }

        // api/users
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetOneUser(int id) => await _context.Users.FindAsync(id);


        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> AllUser()=> await _context.Users.ToListAsync();
    }
}