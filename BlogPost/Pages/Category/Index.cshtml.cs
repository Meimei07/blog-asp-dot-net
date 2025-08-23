using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Category
{
    public class IndexModel(AppDbContext _db) : PageModel
    {
        public IEnumerable<CategoryEntity> Categories { get; set; }

        [BindProperty]
        public string SearchText { get; set; } = "";

        public async Task OnGet()
        {
            Categories = await _db.Categories.ToListAsync();
        }

        // delete
        public async Task<IActionResult> OnPost(int id)
        {
            var matchingCategory = await _db.Categories.FindAsync(id);
            _db.Categories.Remove(matchingCategory);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                // RedirectToPage() will refresh page and call OnGet() again
                return RedirectToPage();
            }

            Categories = await _db.Categories.Where(c => c.Name.Contains(SearchText)).ToListAsync();

            // Page() will return to current page without page refresh
            return Page();
        }
    }
}
