using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class DownlinesModel : PageModel
    {
        public DownlinesModel(UserManager<ApplicationUser> userManager, ILogger<DownlinesModel> logger)
        {
            _userManager = userManager;
            _logger = logger;

        }

        public ApplicationUser Agent { get; set; }
        public List<ApplicationUser> DirectAgents { get; set; }

        public Dictionary<string, List<ApplicationUser>> LevelTwoAReferredAgents { get; set; } = new();
        public Dictionary<string, List<ApplicationUser>> LevelThreeReferredAgents { get; set; } = new();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownlinesModel> _logger;

        public async Task<IActionResult> OnGet()
        {
            string id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;

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
