using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.CA
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class LinkGeneratorModel : PageModel
    {


        public LinkGeneratorModel(UserManager<ApplicationUser> userManager, ILogger<LinkGeneratorModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LinkGeneratorModel> _logger;

        public string ReferralCode { get; set; } = string.Empty;
        public string ReferralLink { get; set; } = string.Empty;


        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value);
            ReferralCode = user.ReferralCode;
            ReferralLink = @Url.Page("/Signup", new { referrerCode = user.ReferralCode });
            return Page();
        }
    }
}
