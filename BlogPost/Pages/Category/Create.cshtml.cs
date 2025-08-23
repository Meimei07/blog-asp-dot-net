using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPost.Pages.Category
{
    public class CreateModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public CategoryEntity Category {get;set;}

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Category.Posts");

            if(ModelState.IsValid)
            {
                _db.Categories.Add(Category);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Category/Index");
            }

            return Page();
        }
    }
}
