using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Models;
using MoneyMCS.Services;

namespace MoneyMCS.Pages.Member
{
    public class ResourcesModel : PageModel
    {
        public ResourcesModel(ResourceContext context, ILogger<ResourceModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ResourceContext _context;
        private readonly ILogger<ResourceModel> _logger;

        public async Task OnGet()
        {
        }

        public async Task<IActionResult> OnGetResourcesPartial(List<string>? category)
        {
            if (category == null)
            {
                return BadRequest();
            }
            List<Resource> Resources = await _context.Resources.Where(r => category.Contains(r.Category)).ToListAsync();
            return Partial("_ResourcesResultPartial", Resources);
        }
    }
}
