using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPost.Pages.Tag
{
    public class EditModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public TagEntity Tag {get;set;}

        public async Task OnGet(int id)
        {
            Tag = await _db.Tags.FindAsync(id);
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Remove("Tag.Posts");

            if (ModelState.IsValid)
            {
                var matchingTag = await _db.Tags.FindAsync(Tag.Id);
                matchingTag.Name = Tag.Name;
                await _db.SaveChangesAsync();

                return RedirectToPage("/Tag/Index");
            }

            return Page();
        }
    }
}
