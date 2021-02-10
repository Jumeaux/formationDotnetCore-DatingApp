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

namespace API.Data
{
    public class Seed
    {

        public static async Task SeedUsers(DataContext context)
        {
            Console.WriteLine("---------------------------------------");
            if (await context.Users.AnyAsync()) return;
             Console.WriteLine("---------------------------------------");
            var userData = await System.IO.File.ReadAllTextAsync("Data/USerSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("passw@rd"));
                user.PasswordSalt =  hmac.Key;

                context.Users.Add(user);
            }
            context.SaveChanges();
        }
    }
}