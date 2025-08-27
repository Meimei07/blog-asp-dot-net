using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Category
{
    [Authorize]
    public class EditModel(AppDbContext _db, UserManager<AppUser> _userManager) : PageModel
    {
        [BindProperty]
        public CategoryEntity Category {get;set;}

        public async Task<IActionResult> OnGet(int id)
        {
            Category = await _db.Categories.FirstAsync(c => c.Id == id);

            var user = await GetAuthUser();

            // make sure user can only edit their own categories
            // if try to edit other categories, redirect to access denied page
            if (user.Id != Category.UserId) 
                return RedirectToPage("/AccessDenied");

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Category.Posts");
            ModelState.Remove("Category.UserId");
            ModelState.Remove("Category.User");

            if (ModelState.IsValid)
            {
                var matchingCategory = await _db.Categories.FindAsync(Category.Id);

                matchingCategory.Name = Category.Name;
                await _db.SaveChangesAsync();

                return RedirectToPage("/Category/Index");
            }

            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
