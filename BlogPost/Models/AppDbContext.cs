using Microsoft.EntityFrameworkCore;

namespace BlogPost.Models
{
    public class AppDbContext(IConfiguration configuration) : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("conn"));
            optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
        }

        public DbSet<CategoryEntity> Categories {get;set;}
        public DbSet<TagEntity> Tags {get;set;}
    }
}
