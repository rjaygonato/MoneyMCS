using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.Member
{
    public class AgentsModel : PageModel
    {
        private readonly UserManager<AgentUser> _userManager;
        private readonly EntitiesContext _context;
        private readonly ILogger<AgentsModel> _logger;
        public List<AgentUser> RegisteredAgents { get; set; } = new();


        public AgentsModel(UserManager<AgentUser> userManager, EntitiesContext context ,ILogger<AgentsModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            RegisteredAgents = await GetAgents();
            return Page();
        }

        private async Task<List<AgentUser>> GetAgents()
        {
            RegisteredAgents = await _context.AgentUsers
                .ToListAsync();
            return RegisteredAgents;

        }
    }
}
