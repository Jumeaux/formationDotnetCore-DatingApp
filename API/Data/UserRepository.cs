using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Entites;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository: IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper= mapper;
        }

        public void UpdateUSer(AppUser AppUser)
        {
            _context.Entry(AppUser).State= EntityState.Modified;
        }

        public async Task<IEnumerable<AppUser>> GetUSersAsync()
        {
                return await  _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }


        public async Task<AppUser> GetUserByIdAsync(int id)
        {
           return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(u => u.UserName== username);
        }

        public async  Task<MemberDto> GetMemberAsync(string username, bool currentUser)
        {
            var query= _context.Users.Where(u =>u.UserName==username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsQueryable();

            if(currentUser)  query=query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync();

        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            
            var query=  _context.Users.AsQueryable();
            query= query.Where(u => u.UserName != userParams.CurrentUSername);
            query= query.Where(x => x.Gender == userParams.Gender);
            var minDob= DateTime.Today.AddYears(-userParams.MaxAge -1);
            var maxDob= DateTime.Today.AddYears(-userParams.MinAge -1);
            query=query.Where(u => u.DateOfBirth>= minDob && u.DateOfBirth <= maxDob);
           
    
            switch (userParams.OrderBy )
            {
                case "created": query.OrderByDescending(u => u.Created);
                    break;
                default:  query.OrderByDescending(u => u.LastActive);
                break;
            };
         
            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper
            .ConfigurationProvider).AsNoTracking(), 
                userParams.PageNumber, 
                userParams.PageSize
            );
        }

        public  async Task<string> GetGender(string username)
        {
            return await _context.Users.Where(u =>u.UserName==username).Select(u =>u.Gender).FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await _context.Users
            .Include(u =>u.Photos)
            .IgnoreQueryFilters()
            .Where(p=>p.Photos.Any(p =>p.Id==photoId))
            .FirstOrDefaultAsync();
        }
    }
}