using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.Member.Agents
{
    public class DownlineModel : PageModel
    {
        public DownlineModel(UserManager<AgentUser> userManager, ILogger<DownlineModel> logger)
        {
            _userManager = userManager;
            _logger = logger;

        }

        public AgentUser Agent { get; set; }
        public List<AgentUser> DirectAgents { get; set; }

        public Dictionary<string, List<AgentUser>> LevelTwoAReferredAgents { get; set; } = new();
        public Dictionary<string, List<AgentUser>> LevelThreeReferredAgents { get; set; } = new();

        private readonly UserManager<AgentUser> _userManager;
        private readonly ILogger<DownlineModel> _logger;

        public async Task<IActionResult> OnGet([FromRoute] string? id)
        {
            if (id == null)
            {

                return BadRequest();
            }

            Agent = await _userManager.FindByIdAsync(id);

            if (Agent == null)
            {
                return NotFound();
            }

            DirectAgents = await _userManager.Users.Where(au => au.ReferrerId == Agent.Id).ToListAsync();
            foreach (var directAgent in DirectAgents)
            {
                LevelTwoAReferredAgents[directAgent.Id] = await _userManager.Users.Where(au => au.ReferrerId == directAgent.Id).ToListAsync();
                foreach (var levelTwoAgent in LevelTwoAReferredAgents[directAgent.Id])
                {
                    LevelThreeReferredAgents[levelTwoAgent.Id] = await _userManager.Users.Where(au => au.ReferrerId == levelTwoAgent.Id).ToListAsync();
                }

            }

            return Page();
        }
    }
}
