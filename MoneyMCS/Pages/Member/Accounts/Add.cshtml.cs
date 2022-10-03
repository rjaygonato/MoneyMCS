using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace MoneyMCS.Pages.Member.Accounts
{
    public class AddModel : PageModel
    {
        private readonly UserManager<MemberUser> _userManager;
        private readonly IUserStore<MemberUser> _userStore;
        private readonly IUserEmailStore<MemberUser> _emailStore;
        private readonly ILogger<AddModel> _logger;

        public AddModel(UserManager<MemberUser> userManager, IUserStore<MemberUser> userStore, IUserEmailStore<MemberUser> emailStore ,ILogger<AddModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = emailStore;
            _logger = logger;
        }

        

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]

            [Display(Name = "PhoneNumber")]
            public string? PhoneNumber { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [Display(Name = "User Type")]
            public string UserType { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Input.UserType != "Viewer" && Input.UserType != "Administrator")
                {
                    ModelState.AddModelError(string.Empty, "Please select a valid user type");
                    return Page();
                }

                MemberUser newUser = CreateUser();
                newUser.FirstName = Input.FirstName;
                newUser.LastName = Input.LastName;
                newUser.PhoneNumber = Input.PhoneNumber;

                await _userStore.SetUserNameAsync(newUser, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(newUser, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(newUser, Input.Password);

                if (result.Succeeded)
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, Input.UserType),
                        new Claim("MemberId", newUser.Id),
                        new Claim("FullName", $"{newUser.FirstName} {newUser.LastName}"),
                        new Claim(ClaimTypes.Email, newUser.Email),
                        new Claim("Username", newUser.UserName)
                    };

                    await _userManager.AddClaimsAsync(newUser, claims);
                    return RedirectToPage("/Member/Accounts/Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();

        }

        private MemberUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<MemberUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AgentUser)}'. " +
                    $"Ensure that '{nameof(AgentUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
