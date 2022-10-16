using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMCS.Pages.Member.Accounts
{
    //[Authorize(Policy = "MemberAccessPolicy")]
    public class AddModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<AddModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AddModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AddModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
            public UserType UserType { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Input.UserType != UserType.VIEWER && Input.UserType != UserType.ADMINISTRATOR)
                {
                    ModelState.AddModelError(string.Empty, "Please select a valid user type");
                    return Page();
                }

                ApplicationUser newUser = CreateUser();
                newUser.FirstName = Input.FirstName;
                newUser.LastName = Input.LastName;
                newUser.PhoneNumber = Input.PhoneNumber;
                newUser.CreationDate = DateTime.Now;
                newUser.UserType = Input.UserType;

                await _userStore.SetUserNameAsync(newUser, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(newUser, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(newUser, Input.Password);
                
                if (result.Succeeded)
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, Input.UserType.ToString()),
                        new Claim("MemberId", newUser.Id),
                        new Claim("FullName", $"{newUser.FirstName} {newUser.LastName}"),
                        new Claim(ClaimTypes.Email, newUser.Email),
                        new Claim("Username", newUser.UserName)
                    };

                    await _userManager.AddClaimsAsync(newUser, claims);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    await _userManager.ConfirmEmailAsync(newUser, token);
                    return RedirectToPage("/Member/Accounts/Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();

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
