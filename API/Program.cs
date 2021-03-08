
using System;

using System.Threading.Tasks;
using API.Data;
using Data;
using Entites;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using API.Entites;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host= CreateHostBuilder(args).Build();
            using var scope= host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                Console.WriteLine("----------------------------------");
                var context = services.GetRequiredService<DataContext>();
                var userManager= services.GetRequiredService<UserManager<AppUser>>();
                var roleManager= services.GetRequiredService<RoleManager<AppRole>> ();
                await context.Database.MigrateAsync();
                await Seed.SeedUsers(userManager, roleManager);
            }
             catch (Exception ex)
            {
                
               var logger = services.GetRequiredService<ILogger<Program>>();
               logger.LogError(ex,"an error occured during migration");
            }
            await host.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
