using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace MoneyMCS.Pages.Member.Agents
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public List<SelectListItem> AgentsSelect = new List<SelectListItem>
        {
            new SelectListItem() { Text = "None", Value = "", Selected = true }
        };
        public List<SelectListItem> AgentTypeSelect = new List<SelectListItem> 
        { 

            new SelectListItem() { Text = "All", Value = "", Selected = true },
            new SelectListItem() { Text = "BASIC", Value = "BASIC" },
            new SelectListItem() { Text = "DIY", Value = "DIY" },
            new SelectListItem() { Text = "VIP", Value = "VIP" }

        };
        

        public class InputModel
        {
            public string? Username { get; set; } = string.Empty;
            [Display(Name = "First Name")]
            public string? FirstName { get; set; } = string.Empty;
            [Display(Name = "Last Name")]
            public string? LastName { get; set; } = string.Empty;
            [Display(Name = "Email")]
            public string? Email { get; set; } = string.Empty;
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; } = string.Empty;
            [Display(Name = "Agent Type")]
            public string? AgentType { get; set; } = string.Empty;
            [Display(Name = "Referral Agent")]
            public string? ReferrerId { get; set; }

        }
        public List<ApplicationUser> Agents { get; set; } = new();


        public IndexModel(UserManager<ApplicationUser> userManager, EntitiesContext context, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            cleanModel();
            Agents = await _userManager.Users
                .Where(u => u.UserType.Equals("Agent"))
                .Where(u =>
                u.UserName.Contains(Input.Username) &&
                u.FirstName.Contains(Input.FirstName) &&
                u.LastName.Contains(Input.LastName) &&
                u.Email.Contains(Input.Email) &&
                u.PhoneNumber.Contains(Input.PhoneNumber) &&
                Input.AgentType == string.Empty ? (u.AgentType.Equals("BASIC") || u.AgentType.Equals("DIY") || u.AgentType.Equals("VIP")) : u.AgentType.Equals(Input.AgentType) &&
                Input.ReferrerId == string.Empty ? u.Id.Contains(string.Empty) : u.Id.Contains(Input.ReferrerId)
                ).ToListAsync();
            await _userManager.Users.ForEachAsync(a =>
            {
                AgentsSelect.Add(new SelectListItem
                {
                    Text = a.UserName,
                    Value = a.Id
                });
            });




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
            await _userManager.DeleteAsync(user);
            return RedirectToPage("/Member/Agents/Index");
        }

        private void cleanModel()
        {
            Input.Username ??= string.Empty;
            Input.FirstName ??= string.Empty;
            Input.LastName ??= string.Empty;
            Input.Email ??= string.Empty;
            Input.PhoneNumber ??= string.Empty;
            Input.AgentType ??= string.Empty;
            Input.ReferrerId ??= string.Empty;
            if (Input.AgentType != "BASIC" && Input.AgentType != "DIY" && Input.AgentType != "VIP")
            {
                Input.AgentType = string.Empty;
            }


        }


    }
}
