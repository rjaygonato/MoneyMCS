using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMCS.Pages.Member.Agents
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class IndexModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EntitiesContext _context;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            EntitiesContext context,
            IUserStore<ApplicationUser> userStore,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _userStore = userStore;
            _logger = logger;
            _emailStore = GetEmailStore();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            public string? Id { get; set; }
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; } = string.Empty;

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string ActionType { get; set; }

        }
        public List<ApplicationUser> Agents { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            Agents = await _userManager.Users
                .Include(au => au.Clients)
                .Where(au => au.UserType == UserType.AGENT)
                .ToListAsync();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAdd()
        {
            if (Input.ActionType == null || Input.ActionType != "add")
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }


            var newAgent = CreateUser();
            newAgent.FirstName = Input.FirstName;
            newAgent.LastName = Input.LastName;
            newAgent.Email = Input.Email;
            newAgent.PhoneNumber = Input.PhoneNumber;
            newAgent.CreationDate = DateTime.Now;

            await _userStore.SetUserNameAsync(newAgent, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(newAgent, Input.Email, CancellationToken.None);

            IdentityResult result;

            while (true)

            {
                newAgent.ReferralCode = GenerateReferralCode(6);
                try
                {
                    newAgent.Wallet = new Wallet();
                    result = await _userManager.CreateAsync(newAgent, Input.Password);
                    break;
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null)
                    {
                        var sqlException = (SqlException)ex.InnerException;
                        if (sqlException.Number == 2601)
                        {
                            continue;
                        }
                        return BadRequest("There was a problem in registring your info");
                    }
                }

            }

            if (result.Succeeded)
            {
                List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "Agent"),
                        new Claim("AgentId", newAgent.Id),
                        new Claim("FullName", $"{newAgent.FirstName} {newAgent.LastName}"),
                        new Claim(ClaimTypes.Email, newAgent.Email),
                        new Claim("Username", newAgent.UserName)
                    };
                await _userManager.AddClaimsAsync(newAgent, claims);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToPage("/Member/Agents/Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();


        }

        public async Task<IActionResult> OnPostEdit()
        {
            if (Input.ActionType == null || Input.ActionType != "edit" || Input.Id == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }


            ApplicationUser toEditAgent = await _userManager.FindByIdAsync(Input.Id);
            if (toEditAgent == null)
            {
                return NotFound();
            }
            if (toEditAgent.Email != Input.Email)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(toEditAgent, Input.Email);
                await _userManager.ChangeEmailAsync(toEditAgent, Input.Email, token);
                await _userStore.SetUserNameAsync(toEditAgent, Input.Email, CancellationToken.None);

            }

            toEditAgent.FirstName = Input.FirstName;
            toEditAgent.LastName = Input.LastName;
            
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

            List<ApplicationUser> referredAgents = await _userManager.Users.Where(au => au.ReferrerId == user.Id).ToListAsync();
            List<Client> referredClients = await _context.Clients.Where(c => c.ReferrerId == user.Id).ToListAsync();
            List<AppTransaction> appTransactions = await _context.AppTransactions.Where(at => at.ApplicationUserId == user.Id).ToListAsync();
            List<StripeTransaction> stripeTransactions = await _context.StripeTransactions.Where(st => st.ApplicationUserId == user.Id).ToListAsync();
            Wallet wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.ApplicationUserId == user.Id);

            if (referredAgents != null)
            {
                foreach (var agent in referredAgents)
                {
                    agent.ReferrerId = null;
                    _context.Entry(agent).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (referredClients != null)
            {
                foreach (var client in referredClients)
                {
                    client.ReferrerId = null;
                    _context.Entry(client).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (appTransactions != null)
            {
                foreach (var appTransaction in appTransactions)
                {
                    appTransaction.ApplicationUserId = null;
                    _context.Entry(appTransaction).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (stripeTransactions != null)
            {
                foreach (var stripeTransaction in stripeTransactions)
                {
                    stripeTransaction.ApplicationUserId = null;
                    _context.Entry(stripeTransaction).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (wallet == null)
            {
                wallet.ApplicationUserId = null;
                _context.Entry(wallet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToPage("/Member/Agents/Index");
        }

        private string GenerateReferralCode(int sample)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, sample)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
