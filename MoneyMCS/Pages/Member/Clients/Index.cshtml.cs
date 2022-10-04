using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Services;

namespace MoneyMCS.Pages.Member.Clients
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class IndexModel : PageModel
    {


        public IndexModel(EntitiesContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<IndexModel> _logger;

        public List<Client> Clients { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            Clients = await _context.Clients
                .Include(c => c.Referrer)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDelete([FromForm] int? toDeleteClientId)
        {
            if (toDeleteClientId == null)
            {
                return BadRequest();
            }

            var client = await _context.Clients.FindAsync(toDeleteClientId);
            if (client == null)
            {
                return BadRequest();
            }
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Member/Clients/Index");
        }


    }
}
