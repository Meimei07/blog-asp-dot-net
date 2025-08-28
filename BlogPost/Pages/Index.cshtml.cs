using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _db;

        public IEnumerable<PostEntity> Posts {get;set;}

        [BindProperty]
        public IEnumerable<TagEntity> Tags {get;set;}

        [BindProperty]
        public string SearchText {get;set;} = "";

        public IndexModel(ILogger<IndexModel> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task OnGet()
        {
            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.User)
                .ToListAsync();

            Tags = await _db.Tags.ToListAsync();
        }

        public async Task<IActionResult> OnPostSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return RedirectToPage();
            }

            Posts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.User)
                .Where(p => p.Title.Contains(SearchText) || // search by post title
                        p.Category.Name.Contains(SearchText) || // by category name
                        p.Tags.Any(t => t.Name.Contains(SearchText))) // by tag name
                .ToListAsync();

            Tags = await _db.Tags.ToListAsync();
            return Page();
        }
    }
}
