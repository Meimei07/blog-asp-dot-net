using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages.Post
{
    public class CreateModel(AppDbContext _db, IWebHostEnvironment _environment) : PageModel
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
            var categories = await _db.Categories.ToListAsync();
            CategoryItems = new SelectList(categories, "Id", "Name");

            Tags = await _db.Tags.ToListAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Post.Thumbnail");
            ModelState.Remove("Post.Category");
            ModelState.Remove("Post.Tags");
            ModelState.Remove("Post.ImgFile");

            if (ModelState.IsValid)
            {
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

                // one to many
                var category = await _db.Categories.Include(c => c.Posts).FirstAsync(c => c.Id == SelectedCategory);
                category.Posts.Add(Post);

                // many to many
                Post.Tags = [];
                foreach(var selectedTag in SelectedTags)
                {
                    var tag = await _db.Tags.FirstAsync(t => t.Id == selectedTag);
                    Post.Tags.Add(tag);
                }

                _db.Posts.Add(Post);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Post/Index");
            }

            await OnGet();
            return Page();
        }
    }
}
