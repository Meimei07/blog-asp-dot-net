using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Tag
{
    public class IndexModel(AppDbContext _db) : PageModel
    {
        public IEnumerable<TagEntity> Tags {get;set;}

        [BindProperty]
        public string SearchText {get;set;}

        public async Task OnGet()
        {
            Tags = await _db.Tags.ToListAsync();
        }

        // delete
        public async Task<IActionResult> OnPost(int id)
        {
            var matchingTag = await _db.Tags.FindAsync(id);
            _db.Tags.Remove(matchingTag);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Tag/Index");
        }

        public async Task<IActionResult> OnPostSearch()
        {
            if(string.IsNullOrEmpty(SearchText))
            {
                return RedirectToPage();
            }

            Tags = await _db.Tags.Where(t => t.Name.Contains(SearchText)).ToListAsync();

            return Page();
        }
    }
}
