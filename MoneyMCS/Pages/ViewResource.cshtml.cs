using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Models;
using MoneyMCS.Services;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class ViewResourceModel : PageModel
    {
        public ViewResourceModel(ResourceContext context, ILogger<ViewResourceModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ResourceContext _context;
        private readonly ILogger<ViewResourceModel> _logger;

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

    }
}
