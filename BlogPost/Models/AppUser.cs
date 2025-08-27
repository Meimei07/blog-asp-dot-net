using Microsoft.AspNetCore.Identity;

namespace BlogPost.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName {get;set;}
        public string? LastName {get;set;}

        // navigations
        public ICollection<PostEntity> Posts {get;set;}
        public ICollection<CategoryEntity> Categories {get;set;}
    }
}
