using System.Collections.Immutable;
using System;
using API.Entites;
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

        public DbSet<UserLike> Likes{ get; set;}
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<UserLike>().HasKey(k => new {k.SourceUSerId, k.LikedUserId});
        
            builder.Entity<UserLike>()
                .HasOne(s =>s.SourceUser)
                .WithMany(l =>l.LikedUsers)
                .HasForeignKey(s => s.SourceUSerId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<UserLike>()
                .HasOne(s =>s.LikedUser)
                .WithMany(l =>l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>()
                .HasOne(u =>u.Recipient)
                .WithMany(m => m.MessageReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u =>u.Sender)
                .WithMany(m => m.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}