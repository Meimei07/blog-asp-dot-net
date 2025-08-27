using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPost.Pages.Category
{
    [Authorize]
    public class CreateModel(AppDbContext _db, UserManager<AppUser> _userManager) : PageModel
    {
        [BindProperty]
        public CategoryEntity Category {get;set;}

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Category.Posts");
            ModelState.Remove("Category.UserId");
            ModelState.Remove("Category.User");

            if(ModelState.IsValid)
            {
                var user = await GetAuthUser();

                // assign auth user id to current category
                Category.UserId = user.Id;

                _db.Categories.Add(Category);
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
