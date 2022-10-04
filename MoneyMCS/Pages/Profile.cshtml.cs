using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Areas.Identity.Data;
using System.Security.Claims;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class ProfileModel : PageModel
    {

        public ProfileModel(UserManager<ApplicationUser> userManager, ILogger<ProfileModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileModel> _logger;

        public ApplicationUser UserInfo { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            UserInfo = await _userManager.FindByIdAsync(id);
            return Page();
        }
    }
}
