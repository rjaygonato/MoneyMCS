using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MoneyMCS.Pages.Member.Accounts
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class EditPassword : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<EditPassword> _logger;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EditPassword(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<EditPassword> logger,
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
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

       
        public ApplicationUser ToEditAccount { get; set; }

        public async Task<IActionResult> OnGet([FromRoute] string? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Member/Accounts");
            }

            ToEditAccount = await _userManager.FindByIdAsync(id);
            if (ToEditAccount == null)
            {
                return RedirectToPage("/Member/Accounts");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromRoute] string? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Member/Accounts");
            }
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                if (Input.Password != Input.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Password and confirm password must be the same.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("/Member/Accounts/EditPassword", new { id = user.Id });
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return Page();

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
