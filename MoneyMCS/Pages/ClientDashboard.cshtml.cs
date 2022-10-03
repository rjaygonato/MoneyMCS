using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Models;
using MoneyMCS.Services;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class ClientDashboardModel : PageModel
    {
        public ClientDashboardModel(ClientContext context, ILogger<ClientDashboardModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ClientContext _context;
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
