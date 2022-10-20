using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class ClientDashboardModel : PageModel
    {
        public ClientDashboardModel(EntitiesContext context, ILogger<ClientDashboardModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<ClientDashboardModel> _logger;

        public List<Client> Clients { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            string agentId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            Clients = await _context.Clients.Where(c => c.ReferrerId == agentId).ToListAsync();
            return Page();
        }
    }
}
