using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Data;
using Entites;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApicontroller
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService) {
             _context=context;
             _tokenService=tokenService;
        }



        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto)
        {

            if ( await UserExists(registerDto.Username)) return BadRequest("Username is taken");

           using (HMACSHA512 hmac= new HMACSHA512()){

                var user= new AppUser{
                   UserName=registerDto.Username.ToLower(),
                   PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                   PasswordSalt=hmac.Key
               };
               _context.Users.Add(user);
               await _context.SaveChangesAsync();
               
                return new UserDto
                { 
                    Username= user.UserName, 
                    token= _tokenService.CreateToken(user)
                };
            }
        }



        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginUser(LoginDto loginDto){

            var user = await _context.Users.SingleOrDefaultAsync(x=> x.UserName == loginDto.Username);

            if (user==null) return Unauthorized("invalid user");

            using ( var hmac= new HMACSHA512(user.PasswordSalt)){

                var computedHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i]!= user.PasswordHash[i]) return Unauthorized("invalid password");
                }

                return new UserDto
                { 
                    Username= user.UserName, 
                    token= _tokenService.CreateToken(user)
                };
            } 
          
        }

        
        public async Task<bool> UserExists(string Username) => await _context.Users.AnyAsync(u=>u.UserName == Username.ToLower());
        

    
    }
}