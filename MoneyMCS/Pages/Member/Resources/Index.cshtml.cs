using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Models;
using MoneyMCS.Services;

namespace MoneyMCS.Pages.Member.Resources
{
    public class IndexModel : PageModel
    {
        public IndexModel(ResourceContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ResourceContext _context;
        private readonly ILogger<IndexModel> _logger;

        public List<Resource> Resources { get; set; } = new();

        public async Task OnGet(bool? all)
        {
            if (all != null && all == true)
            {
                Resources = await _context.Resources.ToListAsync();
            }
        }

        public async Task<IActionResult> OnGetResourcesPartial(List<string>? category, string? search)
        {

            

            if (category == null && search == null)
            {
                return BadRequest();
            }
            search ??= "";

            if (category != null && category.Count > 0)
            {
                Resources = await _context.Resources.Where(r => category.Contains(r.Category) && r.ResourceName.Contains(search)).ToListAsync();
            }


            return Partial("_ResourcesResultPartial", Resources);
        }
    }
}
