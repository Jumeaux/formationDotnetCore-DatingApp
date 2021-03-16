using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private DataContext _context;
        private IMapper _mapper;

        public PhotoRepository(DataContext dataContext, IMapper mapper){

            _context=dataContext;
            _mapper= mapper;
        }
        public async Task<Photo> GetPhotoById(int id)
        {
            return await _context.Photos
            .IgnoreQueryFilters().FirstOrDefaultAsync(p =>p.Id==id);        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _context.Photos
                .IgnoreQueryFilters()    
                .Where(p=>p.isApproved==false)
                .Select(u => new PhotoForApprovalDto{
                    Id= u.Id,
                    Username= u.AppUser.UserName,
                    Url=u.Url,
                    isApproved=u.isApproved
                }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}