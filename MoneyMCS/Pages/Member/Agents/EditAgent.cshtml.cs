using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMCS.Pages.Member.Agents
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class EditAgentModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<EditAgentModel> _logger;

        public EditAgentModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<EditAgentModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
            _emailStore = GetEmailStore();
        }

        public List<SelectListItem> SelectAgentType = new List<SelectListItem>() {
            new SelectListItem() { Value = "BASIC", Text = "BASIC" },
            new SelectListItem() { Value = "VIP", Text = "VIP"},
            new SelectListItem() { Value = "DIY", Text = "DIY" },
        };

        public List<SelectListItem> SelectAgents = new List<SelectListItem>();

        public string ReturnUrl { get; set; }

        public ApplicationUser ToEditAgent { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {

            [Required]
            [Display(Name = "Agent Type")]
            public string AgentType { get; set; }

            [Display(Name = "Referrer")]
            public string ReferrerId { get; set; }


            

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

            IList<ApplicationUser> Agents = await _userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Agent"));
            foreach (var agent in Agents)
            {
                if (agent.Id != ToEditAgent.Id)
                {
                    SelectAgents.Add(new SelectListItem()
                    {
                        Text = agent.UserName,
                        Value = agent.Id,
                        Selected = ToEditAgent.ReferrerId != null && ToEditAgent.ReferrerId != null && ToEditAgent.ReferrerId == agent.Id
                    });
                }
            }

            SelectAgentType.ForEach(at =>
            {
                if (ToEditAgent.AgentType == at.Value)
                {
                    at.Selected = true;
                }
            });
        
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

            List<string> agentTypes = new List<string>
            {
                "BASIC",
                "DIY",
                "VIP"
            };


            if (ModelState.IsValid)
            {
                if (!agentTypes.Contains(Input.AgentType))
                {
                    ModelState.AddModelError(string.Empty, "Please select a valid agent type.");
                    return Page();
                }

                ApplicationUser referrer = null;
                if (Input.ReferrerId != null)
                {
                    referrer = await _userManager.FindByIdAsync(Input.ReferrerId);
                    if (referrer == null)
                    {
                        ModelState.AddModelError(string.Empty, $"User with the id: {Input.ReferrerId} was not found.");
                        return Page();
                    }
                    
                }

                ToEditAgent.AgentType = Input.AgentType;
                if (referrer != null)
                {
                    ToEditAgent.Referrer = referrer;
                }

                var result = await _userManager.UpdateAsync(ToEditAgent);
                if (result.Succeeded)
                {
                    return RedirectToPage("/Member/Agents/EditAgent", new { id = ToEditAgent.Id });

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
