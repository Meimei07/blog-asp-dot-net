using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Category
{
    [Authorize]
    public class DetailModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public IEnumerable<PostEntity> Posts {get;set;}

        public async Task OnGet(int id) // category id
        {
            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.CategoryEntityId == id)
                .ToListAsync();
        }
    }
}
