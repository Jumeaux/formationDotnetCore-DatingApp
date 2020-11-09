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
    }
}