using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyMCS.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Policy;

namespace MoneyMCS.Pages.Member
{
    public class EditAgentModel : PageModel
    {
        private readonly UserManager<AgentUser> _userManager;
        private readonly IUserStore<AgentUser> _userStore;
        private readonly IUserEmailStore<AgentUser> _emailStore;
        private readonly ILogger<EditAgentModel> _logger;

        public EditAgentModel(
            UserManager<AgentUser> userManager,
            IUserStore<AgentUser> userStore,
            ILogger<EditAgentModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
            _emailStore = GetEmailStore();
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> SelectAgentType = new List<SelectListItem>() {
            new SelectListItem() { Value = "BASIC", Text = "BASIC" , Selected = true},
            new SelectListItem() { Value = "VIP", Text = "VIP"},
            new SelectListItem() { Value = "DIY", Text = "DIY" },
        };

        public List<SelectListItem> SelectAgents = new List<SelectListItem>();

        public string ReturnUrl { get; set; }

        public AgentUser ToEditAgent { get; set; }

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

            [Display(Name = "Referer")]
            public string? ReferrerId { get; set; }

            [Required]
            [Display(Name = "Agent Type")]
            public string AgentType { get; set; }



            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }


            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


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

            await _userManager.Users.ForEachAsync(agent =>
            {
                if (agent.Id != ToEditAgent.Id)
                {
                    SelectAgents.Add(new SelectListItem()
                    {
                        Text = agent.UserName,
                        Value = agent.Id,
                        Selected = (ToEditAgent.ReferrerId != null && ToEditAgent.ReferrerId != null) && ToEditAgent.ReferrerId == agent.Id
                    });
                }
                
            });
            return Page();
        }
        //Continue edit
        //public async Task<IActionResult> OnPostProfile(string Id, string? FirstName, string? LastName, string? returnUrl = null)
        //{
        //    returnUrl ??= Url.Content($"~/Member/EditAgent/{}");
        //}

        private IUserEmailStore<AgentUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AgentUser>)_userStore;
        }

        private void MapAgentUserToInputModel(AgentUser agentUser)
        {
            Input.UserName = agentUser.UserName;
            Input.FirstName = agentUser.FirstName;
            Input.LastName = agentUser.LastName;
            Input.Email = agentUser.Email;
            Input.PhoneNumber = agentUser.PhoneNumber;
            Input.AgentType = agentUser.AgentType;
        }
    }
}
