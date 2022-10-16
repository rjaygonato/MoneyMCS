using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.Member
{

    [Authorize(Policy = "MemberAccessPolicy")]
    public class IndexModel : PageModel
    {
        public IndexModel(UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public List<ApplicationUser> NotSubscribedAgents { get; set; }

        public async Task<IActionResult> OnGet()
        {
            NotSubscribedAgents = await _userManager.Users
                .Where(au => au.UserType == UserType.AGENT && !au.Subscribed)
                .ToListAsync();

            return Page();
        }
    }
}
