using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Tag
{
    [Authorize]
    public class EditModel(AppDbContext _db, UserManager<AppUser> _userManager) : PageModel
    {
        [BindProperty]
        public TagEntity Tag {get;set;}

        public async Task<IActionResult> OnGet(int id)
        {
            Tag = await _db.Tags.FirstAsync(t => t.Id == id);

            var user = await GetAuthUser();

            // make sure user can only edit their own tags
            // if try to edit other tags, redirect to access denied page
            if (user.Id != Tag.UserId)
                return RedirectToPage("/AccessDenied");

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Tag.Posts");
            ModelState.Remove("Tag.UserId");
            ModelState.Remove("Tag.User");

            if (ModelState.IsValid)
            {
                var matchingTag = await _db.Tags.FindAsync(Tag.Id);

                matchingTag.Name = Tag.Name;
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
