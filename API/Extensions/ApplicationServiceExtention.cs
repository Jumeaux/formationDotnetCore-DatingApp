using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Interfaces;
using API.Services;
using Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtention
    {
        public static IServiceCollection AddAplicationServices( this IServiceCollection services, IConfiguration config)
        {
              services.AddScoped<ITokenService,TokenService>(); 
            services.AddDbContext<DataContext>(opt=>{
                opt.UseMySql(config.GetConnectionString("DefaultConnection"));
            });
            
            return services;
        }
    }
}