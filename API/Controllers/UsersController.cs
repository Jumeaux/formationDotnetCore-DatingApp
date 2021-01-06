using System.Security.Claims;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Data;
using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [Authorize]
    public class UsersController : BaseApicontroller
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repo, IMapper mapper)
        {

            _repo = repo;
            _mapper = mapper;
        }


        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> AllUser()
        {

            var users = await _repo.GetMembersAsync();
            return Ok(users);
        }



        // api/users/username
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetByUsernamne(string username)
        {
            Console.WriteLine("test");
            return await _repo.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _repo.GetUserByUsernameAsync(username);
            _mapper.Map(memberUpdateDto, user);
            _repo.UpdateUSer(user);

            if (await _repo.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }




    } 
}