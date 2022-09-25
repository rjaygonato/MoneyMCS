using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoneyMCS.Pages
{
    public class TestModel : PageModel
    {

        [BindProperty]
        public string username { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("password", "Please enter password");
                return Page();
            }
            return RedirectToPage("Test");
        }

    }
}
