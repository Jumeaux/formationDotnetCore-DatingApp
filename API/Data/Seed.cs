using System.Linq;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Entites;
using Microsoft.AspNetCore.Identity;
using API.Entites;

namespace API.Data
{
    public class Seed
    {

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager )
        {
            if (await userManager.Users.AnyAsync()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>{

                new AppRole{Name= "Admin"},
                new AppRole{Name= "Moderator"},
                new AppRole{Name= "Member"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.Photos.First().isApproved=true;
                await userManager.CreateAsync(user,"Pa$$w@rd1");
                await userManager.AddToRoleAsync(user, "Member");
                
            }

            var admin= new AppUser{ UserName="admin"};
            await userManager.CreateAsync(admin,"Pa$$w@rd1");
            await userManager.AddToRolesAsync(admin, new [] {"Moderator", "Admin"});
        }
    }
}