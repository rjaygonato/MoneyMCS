using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;


namespace MoneyMCS.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {

        public LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager ,ILogger<LoginModel> logger, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;

        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

        }

        public IActionResult OnGet()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        [BindProperty(SupportsGet = true)]
        public string? email { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, false, true);
                if (result.Succeeded)
                {
                    return RedirectToPage("/Index");
                }
                var user = await _userManager.FindByNameAsync(Input.UserName);

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, $"Your account has been locked. You can login again at {user.LockoutEnd.ToString()}");
                }
                if (result.IsNotAllowed)
                {

                    HttpContext.Session.SetString("EmailConfirmationMessage", $"We have sent an email to {user.Email}. Please check your email to verify your account.");
                    return RedirectToPage("/Login");

                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Invalid username or password");

                }

            }

            
            return Page();
        }
    }
}
