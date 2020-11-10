using System;
using Entites;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions opt) :base(opt)
        {

        }

        public DbSet<AppUser> Users { get; set; }

        internal object SingleOrDefaultAsync(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}