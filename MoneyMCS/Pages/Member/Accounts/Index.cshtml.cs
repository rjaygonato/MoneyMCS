using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages.Member.Accounts;

public class IndexModel : PageModel
{

    public IndexModel(UserManager<MemberUser> userManager, ILogger<IndexModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    private readonly UserManager<MemberUser> _userManager;
    private readonly ILogger<IndexModel> _logger;

    public List<MemberUser> Accounts { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        Accounts = await _userManager.Users.ToListAsync();
        return Page();
    }
}
