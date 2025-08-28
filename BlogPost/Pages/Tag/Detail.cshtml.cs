using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Tag
{
    public class DetailModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public IEnumerable<PostEntity> Posts { get; set; }
        public IEnumerable<TagEntity> Tags {get;set;}

        public async Task OnGet(int id) // category id
        {
            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.User)
                .Where(p => p.Tags.Select(t => t.Id).Contains(id))
                .ToListAsync();

            Tags = await _db.Tags.ToListAsync();
        }
    }
}
