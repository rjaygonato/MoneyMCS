using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.Security.Claims;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class ProfileModel : PageModel
    {

        public ProfileModel(UserManager<ApplicationUser> userManager, EntitiesContext context, ILogger<ProfileModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EntitiesContext _context;
        private readonly ILogger<ProfileModel> _logger;
        

        public ApplicationUser UserInfo { get; set; }
        public List<AppTransaction> SubscriptionHistory { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            UserInfo = await _userManager.FindByIdAsync(id);
            SubscriptionHistory = await _context.AppTransactions
                .Where(at => at.Type == TransactionType.SUBSCRIPTION && at.ApplicationUserId == id)
                .Include(at => at.Subscription)
                .ThenInclude(s => s.Payer)
                .OrderByDescending(at => at.Subscription.EndDate)
                .ToListAsync();
            return Page();
        }
    }
}
