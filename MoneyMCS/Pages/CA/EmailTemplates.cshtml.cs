using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class EmailTemplatesModel : PageModel
    {
        public EmailTemplatesModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser CurrentUser { get; set; }
        public string ReferrerUrl { get; set; }
        public async Task<IActionResult> OnGet()
        {
            string agentId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            CurrentUser = await _userManager.FindByIdAsync(agentId);
            ReferrerUrl = Url.Page("/Signup", new { referrerCode = CurrentUser.ReferralCode });
            return Page();

        }
    }
}
