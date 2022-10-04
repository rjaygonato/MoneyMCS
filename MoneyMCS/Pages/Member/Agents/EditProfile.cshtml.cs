using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Pages.Member.Agents
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class EditProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<EditProfileModel> _logger;

        public EditProfileModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<EditProfileModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
            _emailStore = GetEmailStore();
        }

  


        public string ReturnUrl { get; set; }

        public ApplicationUser ToEditAgent { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

        }

       


        public async Task<IActionResult> OnGet([FromRoute] string? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Member/Agents");
            }

            ToEditAgent = await _userManager.FindByIdAsync(id);
            if (ToEditAgent == null)
            {
                return NotFound();
            }
           
            return Page();
        }
        public async Task<IActionResult> OnPostAsync([FromRoute] string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            ToEditAgent = await _userManager.FindByIdAsync(id);
            if (ToEditAgent == null)
            {
                return NotFound($"User with the id: {id} is not found.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ToEditAgent.UserName = Input.UserName;
            ToEditAgent.FirstName = Input.FirstName;
            ToEditAgent.LastName = Input.LastName;
            ToEditAgent.PhoneNumber = Input.PhoneNumber;
            ToEditAgent.Email = Input.Email;

            await _userManager.UpdateAsync(ToEditAgent);

            return RedirectToPage("/Member/Agents/EditProfile", new { id=ToEditAgent.Id });

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
