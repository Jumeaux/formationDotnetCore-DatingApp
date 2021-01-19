using System;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Data;
using Entites;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public  class AccountController : BaseApicontroller
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper) {
             _context=context;
             _tokenService=tokenService;
             _mapper=mapper;
        }



        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto)
        {

            if ( await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user= _mapper.Map<AppUser>(registerDto);

           using var hmac= new HMACSHA512();
          
                user.UserName=registerDto.Username.ToLower();
                user.PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password));
                user.PasswordSalt=hmac.Key;
               _context.Users.Add(user);
               
               await _context.SaveChangesAsync();
               
                return new UserDto
                { 
                    Username= user.UserName, 
                    token= _tokenService.CreateToken(user),
                    KnownAs= user.KnownAs

                };
           

               
                
            
        }

 

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginUser(LoginDto loginDto){

            var user = await _context.Users
            .Include(p=>p.Photos)
            .SingleOrDefaultAsync(x=> x.UserName == loginDto.Username);

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
                    token= _tokenService.CreateToken(user),
                    PhotoUrl= user.Photos.FirstOrDefault(p=>p.isMain)?.Url,
                    KnownAs= user.KnownAs
                };
            } 
          
        }

        public async Task<bool> UserExists(string Username) => await _context.Users.AnyAsync(u=>u.UserName == Username.ToLower());
    

    
    }
}