using System.Net;
using System.Reflection.Metadata;
using System.Collections.Generic;

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
        private  readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork){
            _unitOfWork=unitOfWork;
        }


        [HttpPost("{username}")]
        public async Task<ActionResult> addLike(string username)
        {

            var sourceId = User.GetUserId();
            var likedUser= await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser= await _unitOfWork.LikesRepository.GetUserWithLikes(sourceId);

            if(likedUser == null) return null;

            if(sourceUser.UserName == username) return BadRequest("you can't like yourself in application");   

            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceId, likedUser.Id);
            if(userLike != null) return BadRequest("You already like this user");

            userLike= new Entites.UserLike{
                SourceUSerId=sourceId,
                LikedUserId= likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if(await _unitOfWork.Complete())  return Ok();

            return BadRequest("Failed to like user"); 
            
        }



        [HttpGet]
        public  async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {   
            likeParams.userId=User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likeParams);

            Response.AddPageHeader(users.CurrentPage, users.PageSize, users.TotalCount,users.TotalPages);
            return Ok(users);
        }
        
    }
}