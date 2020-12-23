using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
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

        public async Task<bool> SaveAllAsync()
        {
                return await _context.SaveChangesAsync() > 0;
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

        public async  Task<MemberDto> GetMemberAsync(string username)
        {

            return await _context.Users
            .Where(u => u.UserName== username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();

        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}