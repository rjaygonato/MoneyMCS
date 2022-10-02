using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.Member.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<MemberUser> _userManager;
        private readonly EntitiesContext _context;
        private readonly ILogger<IndexModel> _logger;
        public List<MemberUser> Accounts { get; set; } = new();


        public IndexModel(UserManager<MemberUser> userManager, EntitiesContext context, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            Accounts = await GetAccounts();
            return Page();
        }


        public async Task<IActionResult> OnPostDelete([FromForm] string? toDeleteAccountId)
        {
            if (toDeleteAccountId == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(toDeleteAccountId);
            if (user == null)
            {
                return BadRequest();
            }
            await _userManager.DeleteAsync(user);
            return RedirectToPage("/Member/Accounts/Index");
        }


        private async Task<List<MemberUser>> GetAccounts()
        {
            return await _userManager.Users.ToListAsync();
        }

    }
}
