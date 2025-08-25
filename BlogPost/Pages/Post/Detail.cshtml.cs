using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Post
{
    public class DetailModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public PostEntity Post {get;set;}

        public async Task OnGet(int id)
        {
            Post = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .FirstAsync(p => p.Id == id);
        }
    }
}
