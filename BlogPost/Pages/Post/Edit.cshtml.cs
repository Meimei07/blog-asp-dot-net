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
    public class EditModel(AppDbContext _db, IWebHostEnvironment _environment, UserManager<AppUser> _userManager) : PageModel
    {
        [BindProperty]
        public PostEntity Post {get;set;}

        public SelectList CategoryItems {get;set;}
        [BindProperty]
        public int SelectedCategory {get;set;}

        public IEnumerable<TagEntity> Tags {get;set;}
        [BindProperty]
        public List<int> SelectedTags { get;set;} = new List<int>();

        public string Thumbnail {get;set;}

        public async Task<IActionResult> OnGet(int id)
        {
            Post = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags).
                FirstAsync(p => p.Id == id);

            var user = await GetAuthUser();

            // make sure user can only edit their own posts
            // if try to edit other posts, redirect to access denied page
            if (user.Id.ToString() != Post.UserId)
                return RedirectToPage("/AccessDenied");

            SelectedCategory = Post.CategoryEntityId;
            SelectedTags = Post.Tags.Select(t => t.Id).ToList();

            if(!string.IsNullOrEmpty(Post.Thumbnail))
            {
                // getting filename.ext
                string[] filename = Post.Thumbnail.Split('_', ',');
                string extension = filename[1].Split('.', ',')[1];
                Thumbnail = $"{filename[0]}.{extension}";
            }

            // auth user can edit post with their own categories
            var categories = await _db.Categories
               .Where(c => c.UserId == user.Id)
               .ToListAsync();
            CategoryItems = new SelectList(categories, "Id", "Name");

            Tags = await _db.Tags.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Post.Category");
            ModelState.Remove("Post.Tags");
            ModelState.Remove("Post.Thumbnail");
            ModelState.Remove("Post.ImgFile");
            ModelState.Remove("Post.UserId");
            ModelState.Remove("Post.User");

            if (ModelState.IsValid)
            {
                var matchingPost = await _db.Posts
                    .Include(p => p.Category)
                    .Include(p => p.Tags)
                    .FirstAsync(p => p.Id == Post.Id);

                matchingPost.Title = Post.Title;
                matchingPost.Content = Post.Content;

                string imgName = "";

                // handle image upload
                if (Post.ImgFile != null && Post.ImgFile.Length > 0)
                {
                    var uploadFolder = Path.Combine(_environment.WebRootPath, ("uploads"));

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // if old image exists, delete it from wwwroot/uploads
                    if (!string.IsNullOrEmpty(matchingPost.Thumbnail))
                    {
                        string oldImgPath = Path.Combine(uploadFolder, matchingPost.Thumbnail);

                        if (System.IO.File.Exists(oldImgPath))
                        { 
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    var fileName = Path.GetFileNameWithoutExtension(Post.ImgFile.FileName);
                    var extension = Path.GetExtension(Post.ImgFile.FileName);
                    var uploadFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                    var filePath = Path.Combine(uploadFolder, uploadFileName);

                    // add new image
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Post.ImgFile.CopyToAsync(fileStream);
                    }

                    imgName = uploadFileName;
                }

                matchingPost.Thumbnail = !string.IsNullOrEmpty(imgName) ? imgName : matchingPost.Thumbnail; 

                var category = await _db.Categories.FirstAsync(c => c.Id == SelectedCategory);
                matchingPost.Category = category;

                // prevent duplicate tag
                matchingPost.Tags = [];
                if (SelectedTags != null)
                {
                    foreach (var selectedTag in SelectedTags)
                    {
                        var tag = await _db.Tags.FirstAsync(t => t.Id == selectedTag);
                        matchingPost.Tags.Add(tag);
                    }
                }

                await _db.SaveChangesAsync();

                return RedirectToPage("/Post/Index");
            }

            await OnGet(Post.Id);
            return Page();
        }

        private Task<AppUser> GetAuthUser()
        {
            return _userManager.GetUserAsync(User);
        }
    }
}
