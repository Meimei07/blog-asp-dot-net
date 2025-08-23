using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Post
{
    public class IndexModel(AppDbContext _db) : PageModel
    {
        public IEnumerable<PostEntity> Posts {get;set;}

        [BindProperty]
        public string SearchText {get;set;}

        public async Task OnGet()
        {
            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .ToListAsync();
        }

        // delete 
        public async Task<IActionResult> OnPost(int id)
        {
            var matchingPost = await _db.Posts.FindAsync(id);
            _db.Posts.Remove(matchingPost);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        // search
        public async Task<IActionResult> OnPostSearch()
        {
            if(string.IsNullOrEmpty(SearchText))
            {
                return RedirectToPage();
            }

            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Title.Contains(SearchText)).ToListAsync();

            return Page();
        }
    }
}
