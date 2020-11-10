using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using Data;
using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{

   
    public class UsersController:BaseApicontroller
    {
        private DataContext _context;

        public UsersController(DataContext context){

            _context=context;
        }


        // api/users
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetOneUser(int id) => await _context.Users.FindAsync(id);

        [AllowAnonymous]
        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> AllUser()=> await _context.Users.ToListAsync();
    }
}