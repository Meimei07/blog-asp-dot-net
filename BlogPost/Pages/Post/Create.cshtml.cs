using BlogPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Post
{
    [Authorize]
    public class CreateModel(AppDbContext _db, IWebHostEnvironment _environment, UserManager<AppUser> _userManager) : PageModel
    {
        [BindProperty]
        public PostEntity Post {get;set;}

        public SelectList CategoryItems {get;set;}
        [BindProperty]
        public int SelectedCategory {get;set;}

        public IEnumerable<TagEntity> Tags {get;set;}
        [BindProperty]
        public List<int> SelectedTags {get;set;} = new List<int>();

        public async Task OnGet()
        {
            var user = await GetAuthUser();

            // auth user can create post with their own categories
            var categories = await _db.Categories
                .Where(c => c.UserId == user.Id)
                .ToListAsync();
            CategoryItems = new SelectList(categories, "Id", "Name");

            Tags = await _db.Tags.ToListAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Post.Thumbnail");
            ModelState.Remove("Post.Category");
            ModelState.Remove("Post.Tags");
            ModelState.Remove("Post.ImgFile");
            ModelState.Remove("Post.UserId");
            ModelState.Remove("Post.User");

            if (ModelState.IsValid)
            {
                var user = await GetAuthUser();

                string imgName = "";

                // handle image upload
                if(Post.ImgFile != null && Post.ImgFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");

                    if(!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string fileName = Path.GetFileNameWithoutExtension(Post.ImgFile.FileName);
                    string extension = Path.GetExtension(Post.ImgFile.FileName);
                    string uploadFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                    string filePath = Path.Combine(uploadFolder, uploadFileName);

                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Post.ImgFile.CopyToAsync(fileStream);
                    }

                    imgName = uploadFileName;
                }

                Post.Thumbnail = imgName;

                // one to many : category < post
                var category = await _db.Categories.Include(c => c.Posts).FirstAsync(c => c.Id == SelectedCategory);
                category.Posts.Add(Post);

                // many to many : tag <> post
                Post.Tags = [];
                foreach(var selectedTag in SelectedTags)
                {
                    var tag = await _db.Tags.FirstAsync(t => t.Id == selectedTag);
                    Post.Tags.Add(tag);
                }

                // assign auth user id to current post
                Post.UserId = user.Id.ToString();

                _db.Posts.Add(Post);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Post/Index");
            }

            await OnGet();
            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
