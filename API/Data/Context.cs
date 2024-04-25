using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Context : IdentityDbContext<User, Role, int>
    {
        public Context(DbContextOptions options) : base(options)
        {

        }
        public DbSet<NewsArticle> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Role>()
                .HasData(
                    new Role { Id = 1, Name = "Member", NormalizedName = "MEMBER" },
                    new Role { Id = 2, Name = "Admin", NormalizedName = "ADMIN" }
                );
        }
    }
}