using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Tag
{
    [Authorize]
    public class IndexModel(AppDbContext _db, UserManager<AppUser> _userManager) : PageModel
    {
        public IEnumerable<TagEntity> Tags {get;set;}

        [BindProperty]
        public string SearchText {get;set;}

        public async Task OnGet()
        {
            var user = await GetAuthUser();

            // get only the auth user's tags
            // so that user can only manage their own tags
            Tags = await _db.Tags
                .Where(t => t.UserId == user.Id)
                .ToListAsync();
        }

        // delete
        public async Task<IActionResult> OnPost(int id)
        {
            var user = await GetAuthUser();

            // make sure the tags belongs to the auth user, to delete it
            var matchingTag = await _db.Tags
                .FirstAsync(t => t.Id == id && t.UserId == user.Id);

            _db.Tags.Remove(matchingTag);
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

            var user = await GetAuthUser();

            // able to search through own tags only
            Tags = await _db.Tags
                .Where(t => t.Name.Contains(SearchText) && 
                            t.UserId == user.Id)
                .ToListAsync();

            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
