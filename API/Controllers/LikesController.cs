using System.Net;
using System.Reflection.Metadata;
using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Interfaces;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    public class LikesController: BaseApicontroller
    {
        private readonly ILikesRepository _repo;
        private readonly IUserRepository _repoUser;

        public LikesController(ILikesRepository repostory, IUserRepository userRepository){
            _repo=repostory;
            _repoUser= userRepository;
        }


        [HttpPost("{username}")]
        public async Task<ActionResult> addLike(string username)
        {

            var sourceId = User.GetUserId();
            var likedUser= await _repoUser.GetUserByUsernameAsync(username);
            var sourceUser= await _repo.GetUserWithLikes(sourceId);

            if(likedUser == null) return null;

            if(sourceUser.UserName == username) return BadRequest("you can't like yourself in application");   

            var userLike = await _repo.GetUserLike(sourceId, likedUser.Id);
            if(userLike != null) return BadRequest("You already like this user");

            userLike= new Entites.UserLike{
                SourceUSerId=sourceId,
                LikedUserId= likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if(await _repoUser.SaveAllAsync())  return Ok();

            return BadRequest("Failed to like user"); 
            
        }



        [HttpGet]
        public  async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {   
            likeParams.userId=User.GetUserId();
            var users = await _repo.GetUserLikes(likeParams);

            Response.AddPageHeader(users.CurrentPage, users.PageSize, users.TotalCount,users.TotalPages);
            return Ok(users);
        }
        
    }
}