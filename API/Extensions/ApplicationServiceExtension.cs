using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Interfaces;
using API.Services;
using Data;
using Microsoft.EntityFrameworkCore;
using API.Data;
using AutoMapper;
using API.Helpers;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices( this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService,TokenService>(); 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(opt=>{
                opt.UseMySql(config.GetConnectionString("DefaultConnection"));
            });
            
            return services;
        }
    }
}