using System.Runtime.Intrinsics.X86;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController:BaseApicontroller
    {
        private UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager){

            _userManager=userManager;
        }

        [Authorize(Policy="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRole(){
           
            var users= await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy( u => u.UserName)
                .Select( u => new {
                    u.Id,
                    Username= u.UserName,
                    Roles =u.UserRoles.Select(r =>r.Role.Name).ToList()

                }).ToListAsync();

                return Ok(users);
        }


        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditeRole(string username,  [FromQuery] string roles)
        {
            var selectRoles= roles.Split(",").ToArray();
            var user= await _userManager.FindByNameAsync(username);

            if(user == null)  return NotFound("user not found");

            var userRoles= await _userManager.GetRolesAsync(user);

            var res= await _userManager.AddToRolesAsync(user,selectRoles.Except(userRoles));
            if(!res.Succeeded) return BadRequest("faild to add roles");

            var removeRole=  await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectRoles));
            if(!removeRole.Succeeded) return BadRequest("Failed to remove from roles");
       
            return Ok(await _userManager.GetRolesAsync(user));
        }


        [Authorize(Policy="ModeratePhotoRole")]
        [HttpGet("photo-moderate")]
        public ActionResult GetPhotoForModerate(){
            return Ok("Admin or Moderate acesss");
        }
    }
}