using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.CA
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class LedgerModel : PageModel
    {
        public LedgerModel(EntitiesContext context, ILogger<LedgerModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<LedgerModel> _logger;
        public decimal TotalCommission { get; set; }
        public async Task<IActionResult> OnGet()
        {
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            TotalCommission = await _context.AppTransactions.Where(at => at.ApplicationUserId == userId && at.Type == TransactionType.COMMISSION).SumAsync(at => at.Amount);


            return Page();

        }
    }
}
