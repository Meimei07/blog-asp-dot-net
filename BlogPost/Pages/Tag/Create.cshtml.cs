using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPost.Pages.Tag
{
    [Authorize]
    public class CreateModel(AppDbContext _db, UserManager<AppUser> _userManager) : PageModel
    {
        [BindProperty]
        public TagEntity Tag {get;set;}

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Tag.Posts");
            ModelState.Remove("Tag.UserId");
            ModelState.Remove("Tag.User");

            if (ModelState.IsValid)
            {
                var user = await GetAuthUser();

                // assign auth user id to current tag
                Tag.UserId = user.Id;

                _db.Tags.Add(Tag);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Tag/Index");
            }

            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
