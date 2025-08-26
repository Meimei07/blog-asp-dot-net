using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Models
{
    public class AppDbContext(IConfiguration configuration) : IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("conn"));
            optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
        }

        public DbSet<CategoryEntity> Categories {get;set;}
        public DbSet<TagEntity> Tags {get;set;}
        public DbSet<PostEntity> Posts {get;set;}
    }
}
