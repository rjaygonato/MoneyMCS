using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMCS.Pages.Member.Accounts;

[Authorize(Policy = "MemberAccessPolicy")]
public class IndexModel : PageModel
{

    public IndexModel(UserManager<ApplicationUser> userManager, EntitiesContext context ,ILogger<IndexModel> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly EntitiesContext _context;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty(SupportsGet = true)]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string? Username { get; set; } = string.Empty;
        [Display(Name = "First Name")]
        public string? FirstName { get; set; } = string.Empty;
        [Display(Name = "Last Name")]
        public string? LastName { get; set; } = string.Empty;
        [Display(Name = "Email")]
        public string? Email { get; set; } = string.Empty;
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; } = string.Empty;
        [Display(Name = "User Type")]
        public string? AccountType { get; set; } = string.Empty;
    }

    public List<ApplicationUser> Accounts { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {

        cleanModel();

        Accounts = await _userManager.Users.Where(u => 
        u.UserName.Contains(Input.Username) &&
        u.FirstName.Contains(Input.FirstName) &&
        u.LastName.Contains(Input.LastName) &&
        u.Email.Contains(Input.Email) &&
        u.PhoneNumber.Contains(Input.PhoneNumber) &&
        Input.AccountType == string.Empty ? (u.UserType.Equals("Administrator") || u.UserType.Equals("Viewer")) : u.UserType.Equals(Input.AccountType)
        ).ToListAsync();

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

        List<Client> clients = await _context.Clients.Where(c => c.ReferrerId == user.Id).ToListAsync();
        if (clients != null)
        {
            foreach (var client in clients)
            {
                client.ReferrerId = null;
                _context.Entry(client).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        await _userManager.DeleteAsync(user);
        return RedirectToPage("/Member/Accounts/Index");
    }

    private void cleanModel()
    {
        Input.Username ??= string.Empty;
        Input.FirstName ??= string.Empty;
        Input.LastName ??= string.Empty;
        Input.Email ??= string.Empty;
        Input.PhoneNumber ??= string.Empty;
        Input.AccountType ??= string.Empty;
        if (Input.AccountType != "Administrator" && Input.AccountType != "Viewer")
        {
            Input.AccountType = string.Empty;
        }

    }
}
