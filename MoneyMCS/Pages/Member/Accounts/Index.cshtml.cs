using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.Security.Claims;

namespace MoneyMCS.Pages.Member.Accounts;

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

    public List<ApplicationUser> Accounts { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {

        Accounts = await _userManager.Users.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDelete([FromForm] string? toDeleteUserId)
    {
        if (toDeleteUserId == null)
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(toDeleteUserId);
        if (user == null)
        {
            return BadRequest();
        }
        await _userManager.DeleteAsync(user);
        return RedirectToPage("/Member/Accounts/Index");
    }
}
