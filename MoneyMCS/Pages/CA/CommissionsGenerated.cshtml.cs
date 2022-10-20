using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.CA
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class CommissionsGeneratedModel : PageModel
    {

        public CommissionsGeneratedModel(EntitiesContext context, ILogger<CommissionsGeneratedModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<CommissionsGeneratedModel> _logger;

        public decimal TotalCommission { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            TotalCommission = await _context.AppTransactions.Where(at => at.ApplicationUserId == userId && at.Type == TransactionType.COMMISSION).SumAsync(at => at.Amount);


            return Page();

        }
    }
}
