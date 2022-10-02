using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Models;
using MoneyMCS.Services;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class ResourcesModel : PageModel
    {
        public ResourcesModel(ResourceContext context, ILogger<ResourcesModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ResourceContext _context;
        private readonly ILogger<ResourcesModel> _logger;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetResourcesPartial(List<string>? category, string? search)
        {
            if (category == null && search == null)
            {
                return BadRequest();
            }

            var Resources = new List<Resource>();
            if (category != null && category.Count > 0)
            {
                Resources = await _context.Resources.Where(r => category.Contains(r.Category)).ToListAsync();
            }

            else if (!string.IsNullOrWhiteSpace(search))
            {
                Resources = await _context.Resources.Where(r => r.ResourceName.Contains(search)).ToListAsync();
            }

            return Partial("_ResourcesResultPartial2", Resources);
        }
    }
}
