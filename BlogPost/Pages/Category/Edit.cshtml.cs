using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPost.Pages.Category
{
    public class EditModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public CategoryEntity Category {get;set;}

        public async Task OnGet(int id)
        {
            Category = await _db.Categories.FindAsync(id);
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Category.Posts");

            if (ModelState.IsValid)
            {
                var matchingCategory = await _db.Categories.FindAsync(Category.Id);
                matchingCategory.Name = Category.Name;
                await _db.SaveChangesAsync();

                return RedirectToPage("/Category/Index");
            }

            return Page();
        }
    }
}
