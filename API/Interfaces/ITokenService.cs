
using System.Threading.Tasks;
using Entites;

namespace API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser appUser);
    }
}