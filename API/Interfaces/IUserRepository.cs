using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using Entites;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void UpdateUSer(AppUser AppUser);

        Task<IEnumerable<AppUser>> GetUSersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<string> GetGender(string username);
        
        Task<MemberDto> GetMemberAsync(string username);
        
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParms);
    }
}