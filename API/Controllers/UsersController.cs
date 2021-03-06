using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using API.Extensions;
using API.Entites;
using API.Helpers;

namespace Controllers
{
    [Authorize]
    public class UsersController : BaseApicontroller
    {
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _photoService=photoService;
            _unitOfWork=unitOfWork;
            _mapper = mapper;
        }


        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {

            var gender = await _unitOfWork.UserRepository.GetGender(User.GetUsername());
            userParams.CurrentUSername=User.GetUsername();

            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender== "male" ? "female" : "male";
            
            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddPageHeader(users.CurrentPage, users.PageSize, users.TotalCount,users.TotalPages);
            return Ok(users);
        }



        
        [HttpGet("{username}",Name="GetUSer")]
        public async Task<ActionResult<MemberDto>> GetByUsernamne(string username)
        {
            var currentUsername= User.GetUsername();
            return await _unitOfWork.UserRepository.GetMemberAsync(username, currentUser: currentUsername==username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
         
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.UpdateUSer(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user");
        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> addPohto(IFormFile  file)
        {

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null)  return BadRequest(result.Error.Message);

            var photo= new Photo
            {
                 Url= result.SecureUrl.AbsoluteUri,
                 PublicId=result.PublicId
            };

            // if(user.Photos.Count == 0) photo.isMain=true;
            user.Photos.Add(photo);

            if (await _unitOfWork.Complete()) return  CreatedAtRoute("GetUser", new {username= user.UserName}, _mapper.Map<PhotoDto>(photo)) ;

            return BadRequest("Echec de l'ajout");
        }



        [HttpPut("set-main-photo/{idPhoto}")]
        public async Task<ActionResult> SetMainPhoto(int idPhoto)
        {
            var user= await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo=  user.Photos.FirstOrDefault(u=>u.Id == idPhoto);

            if (photo.isMain) return BadRequest("cette photo est dèjà la principal");

            var currentMain= user.Photos.FirstOrDefault(u=>u.isMain);
            if (currentMain!= null) currentMain.isMain= false;

            photo.isMain=true;

            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Faild to set main photo");
        }


        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x=> x.Id ==photoId);

            if( photo == null) return NotFound(" photo not found");

            if(photo.isMain) return BadRequest("You can't remove the main photo");

            if(photo.PublicId != null){
                
                var res= await _photoService.DeletePhotoAsync(photo.PublicId);
                if(res.Error !=null)  return BadRequest(res.Error.Message);
            }

            user.Photos.Remove(photo);
            if(await _unitOfWork.Complete()) return Ok();

            return BadRequest("Faild to delete photo");
        }
    } 
}