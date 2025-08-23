using BlogPost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogPost.Pages.Tag
{
    public class CreateModel(AppDbContext _db) : PageModel
    {
        [BindProperty]
        public TagEntity Tag {get;set;}

        public async Task<IActionResult> OnPost()
        {
            if(ModelState.IsValid)
            {
                _db.Tags.Add(Tag);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Tag/Index");
            }

            return Page();
        }
    }
}
