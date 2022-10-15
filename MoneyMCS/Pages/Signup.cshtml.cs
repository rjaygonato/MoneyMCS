using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace MoneyMCS.Pages
{
    [AllowAnonymous]
    public class SignupModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<SignupModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SignupModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<SignupModel> logger,
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

        public string ReferrerCode { get; set; }

        public string ReturnUrl { get; set; }
        public class InputModel
        {
            public string? ReferrerCode { get; set; }

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

        public async Task<IActionResult> OnGet([FromQuery] string? referrerCode)
        {
            if (referrerCode != null)
            {
                ReferrerCode = referrerCode;
            }
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.CreationDate = DateTime.Now;
                user.UserType = "Agent";
                user.AgentType = "BASIC";

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                if (Input.ReferrerCode != null)
                {
                    var referrer = await _userManager.Users.FirstOrDefaultAsync(au => au.ReferralCode == Input.ReferrerCode);
                    if (referrer != null)
                    {
                        user.Referrer = referrer;
                    }
                }

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "Agent"),
                    new Claim("AgentId", user.Id),
                    new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Username", user.UserName)
                };
                IdentityResult result;

                while(true)
                {
                    user.ReferralCode = GenerateReferralCode(6);
                    try
                    {
                        user.Wallet = new Wallet();
                        result = await _userManager.CreateAsync(user, Input.Password);
                        break;
                    }
                    catch(DbUpdateException ex)
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
                    await _userManager.AddClaimsAsync(user, claims);

                    _logger.LogInformation("User created a new account with password.");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = userId, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    HttpContext.Session.SetString("newuser", "true");
                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        HttpContext.Session.SetString("EmailConfirmationMessage", $"We have sent an email to {Input.Email}. Please check your email to verify your account.");
                        return RedirectToPage("/Login", new { email = Input.Email});
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToPage("/Index");
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }


            }

            // If we got this far, something failed, redisplay form
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
    }
}
