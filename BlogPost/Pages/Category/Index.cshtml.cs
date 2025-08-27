using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Category
{
    [Authorize]
    public class IndexModel(AppDbContext _db, UserManager<AppUser> _userManager) : PageModel
    {
        public IEnumerable<CategoryEntity> Categories { get; set; }

        [BindProperty]
        public string SearchText { get; set; } = "";

        public async Task OnGet()
        {
            var user = await GetAuthUser();

            // get only the auth user's categories
            // so that user can only manage their own categories
            Categories = await _db.Categories
                .Where(c => c.UserId == user.Id)
                .ToListAsync();
        }

        // delete
        public async Task<IActionResult> OnPost(int id)
        {
            var user = await GetAuthUser();

            // make sure the categories belongs to the auth user, to delete it
            var matchingCategory = await _db.Categories
                .FirstAsync(c => c.Id == id && c.UserId == user.Id);

            _db.Categories.Remove(matchingCategory);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        // search
        public async Task<IActionResult> OnPostSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                // RedirectToPage() will refresh page and call OnGet() again
                return RedirectToPage();
            }

            var user = await GetAuthUser();

            // able to search through own categories only
            Categories = await _db.Categories
                .Where(c => c.Name.Contains(SearchText) &&
                            c.UserId == user.Id)
                .ToListAsync();

            // Page() will return to current page without page refresh
            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
