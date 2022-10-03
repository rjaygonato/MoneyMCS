using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMCS.Services;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using MoneyMCS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class AddClientModel : PageModel
    {
        public AddClientModel(ClientContext context, UserManager<AgentUser> userManager, ILogger<AddClientModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            LoadStatesSelect();
        }


        private readonly ClientContext _context;
        private readonly UserManager<AgentUser> _userManager;
        private readonly ILogger<AddClientModel> _logger;
        public List<SelectListItem> StateSelect { get; set; } = new();

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public string Company { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]

            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            public string Address { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string State { get; set; }

            [Required]
            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Client newClient = new Client();
                newClient.Company = Input.Company;
                newClient.FirstName = Input.FirstName;
                newClient.LastName = Input.LastName;
                newClient.Email = Input.Email;
                newClient.PhoneNumber = Input.PhoneNumber;
                newClient.Address = Input.Address;
                newClient.City = Input.City;
                newClient.State = Input.State;
                newClient.ZipCode = Input.ZipCode;
                newClient.DateAdded = DateTime.Now;

                string agentId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId")?.Value;
                if (agentId == null)
                {
                    return NotFound();
                }

                AgentUser referrer = await _userManager.FindByIdAsync(agentId);
                if (referrer != null)
                {
                    newClient.ReferrerId = referrer.Id;
                }
                await _context.Clients.AddAsync(newClient);
                await _context.SaveChangesAsync();

                return RedirectToPage("/ClientDashboard");
            }
            return Page();
        }

        private void LoadStatesSelect()
        {
            StateSelect.Add(new SelectListItem { Text = "Alaska", Value = "Alaska" });
            StateSelect.Add(new SelectListItem { Text = "Alabama", Value = "Alabama" });
            StateSelect.Add(new SelectListItem { Text = "Arkansas", Value = "Arkansas" });
            StateSelect.Add(new SelectListItem { Text = "California", Value = "California" });
            StateSelect.Add(new SelectListItem { Text = "Colorado", Value = "Colorado" });
            StateSelect.Add(new SelectListItem { Text = "Connecticut", Value = "Connecticut" });
            StateSelect.Add(new SelectListItem { Text = "Delaware", Value = "Delaware" });
            StateSelect.Add(new SelectListItem { Text = "District of Columbia", Value = "District of Columbia" });
            StateSelect.Add(new SelectListItem { Text = "Florida", Value = "Florida" });
            StateSelect.Add(new SelectListItem { Text = "Georgia", Value = "Georgia" });
            StateSelect.Add(new SelectListItem { Text = "Hawaii", Value = "Hawaii" });
            StateSelect.Add(new SelectListItem { Text = "Idaho", Value = "Idaho" });
            StateSelect.Add(new SelectListItem { Text = "Illinois", Value = "Illinois" });
            StateSelect.Add(new SelectListItem { Text = "Indiana", Value = "Indiana" });
            StateSelect.Add(new SelectListItem { Text = "Iowa", Value = "Iowa" });
            StateSelect.Add(new SelectListItem { Text = "Kansas", Value = "Kansas" });
            StateSelect.Add(new SelectListItem { Text = "Kentucky", Value = "Kentucky" });
            StateSelect.Add(new SelectListItem { Text = "Louisiana", Value = "Louisiana" });
            StateSelect.Add(new SelectListItem { Text = "Maine", Value = "Maine" });

        }
    }
}
