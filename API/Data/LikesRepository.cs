using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Data;
using Entites;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public  LikesRepository(DataContext dataContext){

           _context=dataContext;

        }
        public async Task<UserLike> GetUserLike(int sourceId, int likedUserId)
        {
           return await _context.Likes.FindAsync(sourceId,likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes= _context.Likes.AsQueryable();


            // liste des utilisateurs aimés par l'ulitisateur connecté
            if(likeParams.predicat=="liked"){
                likes= likes.Where(like => like.SourceUSerId==likeParams.userId);
                users= likes.Select(like => like.LikedUser);
            }

             // liste des utilisateurs qui ont aimés l'ulitisateur connecté
            if(likeParams.predicat=="likedBy"){
                likes= likes.Where(like => like.LikedUserId==likeParams.userId);
                users= likes.Select(like => like.SourceUser);
            }
            var likeUsers=  users.Select(user => new LikeDto
            {
                    Id=user.Id,
                    Username=user.UserName,
                    KnownAs=user.KnownAs,
                    Age= user.DateOfBirth.CalculateAge(),
                    City=user.City,
                    PhotoUrl= user.Photos.FirstOrDefault(p => p.isMain).Url

            }); 
            return await PagedList<LikeDto>.CreateAsync(likeUsers,likeParams.PageNumber, likeParams.PageSize);


        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(u => u.Id== userId);
        }
    }   
}