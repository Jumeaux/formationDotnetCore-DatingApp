using System.Collections;
using System.Runtime.Intrinsics.X86;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;
using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using API.DTOs;
using API.Interfaces;


namespace API.Controllers
{
    public class AdminController : BaseApicontroller
    {
        private UserManager<AppUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private IPhotoService _photoService;
        private readonly IPhotoService _mapper;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {

            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _photoService= photoService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRole()
        {

            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()

                }).ToListAsync();

            return Ok(users);
        }


        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditeRole(string username, [FromQuery] string roles)
        {
            var selectRoles = roles.Split(",").ToArray();
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("user not found");

            var userRoles = await _userManager.GetRolesAsync(user);

            var res = await _userManager.AddToRolesAsync(user, selectRoles.Except(userRoles));
            if (!res.Succeeded) return BadRequest("faild to add roles");

            var removeRole = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectRoles));
            if (!removeRole.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photo-moderate")]
        public async Task<ActionResult> GetPhotoForModeration()
        {
           var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
           return Ok(photos);
        }




        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approuve-photo/{idPhoto}")]
        public async Task<ActionResult> ApprovePhoto(int idPhoto)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(idPhoto);
            if(photo==null) return NotFound();
            
            photo.isApproved=true;
            var user= await _unitOfWork.UserRepository.GetUserByPhotoId(idPhoto);
            
            if(!user.Photos.Any(x =>x.isMain)) photo.isMain=true;
           
            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to approve this photo");
        }


        [Authorize(Policy="ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo.PublicId!=null)
            {
                var result= await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Result=="ok")
                {
                    _unitOfWork.PhotoRepository.RemovePhoto(photo);
                }
            }else{
               _unitOfWork.PhotoRepository.RemovePhoto(photo);

            }
            if(await _unitOfWork.Complete()) return Ok();

            return BadRequest("failed to reject photo");
        }
    }
}