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
    public class EditProfile : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<EditProfile> _logger;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EditProfile(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<EditProfile> logger,
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

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;

                
                if (user.UserName != Input.UserName)
                {
                    await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                }
                if (user.Email != Input.Email)
                {
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToPage("/Member/Accounts/EditProfile", new { id = user.Id });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
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
