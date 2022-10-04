using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Models;
using MoneyMCS.Services;

namespace MoneyMCS.Pages.Member.Resources
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class ViewModel : PageModel
    {
        public ViewModel(ResourceContext context, ILogger<ViewModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ResourceContext _context;
        private readonly ILogger<ViewModel> _logger;

        public Resource ResourceInfo { get; set; }
        public async Task<IActionResult> OnGet(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            ResourceInfo = await _context.Resources.FindAsync(id);
            if (ResourceInfo == null)
            {
                return NotFound();
            }
            return Page();

        }

        public async Task<IActionResult> OnPostDelete(int? toDeleteResourceId)
        {
            if (toDeleteResourceId == null)
            {
                return BadRequest();
            }
            var resource = await _context.Resources.FindAsync(toDeleteResourceId);
            if (resource == null)
            {
                return NotFound();
            }

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Member/Resources/Index");
        }
    }
}
