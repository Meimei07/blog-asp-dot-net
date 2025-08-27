using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Post
{
    [Authorize]
    public class IndexModel(AppDbContext _db, IWebHostEnvironment _environment ,UserManager<AppUser> _userManager) : PageModel
    {
        public IEnumerable<PostEntity> Posts {get;set;}

        [BindProperty]
        public string SearchText {get;set;}

        public async Task OnGet()
        {
            var user = await GetAuthUser();

            // get only the auth user's posts
            // so that user can only manage their own posts
            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.UserId == user.Id.ToString())
                .ToListAsync();
        }

        // delete 
        public async Task<IActionResult> OnPost(int id)
        {
            var user = await GetAuthUser();

            // make sure the post belongs to the auth user, to delete it
            var matchingPost = await _db.Posts
                .FirstAsync(p => p.Id == id && 
                            p.UserId == user.Id.ToString());

            // if old image exists, delete it from wwwroot/uploads
            if (!string.IsNullOrEmpty(matchingPost.Thumbnail))
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, ("uploads"));
                string oldImgPath = Path.Combine(uploadFolder, matchingPost.Thumbnail);

                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }
            }

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

            var user = await GetAuthUser();
            
            // able to search through own posts only
            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Title.Contains(SearchText) && 
                            p.UserId == user.Id.ToString())
                .ToListAsync();

            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
