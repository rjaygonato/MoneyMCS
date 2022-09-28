using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Models;
using MoneyMCS.Services;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Pages.Member.Clients
{
    public class AddModel : PageModel
    {
        public AddModel(ClientContext context, UserManager<AgentUser> userManager, ILogger<AddModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private readonly ClientContext _context;
        private readonly UserManager<AgentUser> _userManager;
        private readonly ILogger<AddModel> _logger;

        public List<SelectListItem> SelectAgents = new List<SelectListItem>();

        public string ReturnUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {


            [Required]
            [Display(Name = "Company")]
            public string Company { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }

            public string? Address { get; set; }
            
            public string? City { get; set; }
            public string? State { get; set; }

            public string? ZipCode { get; set; }

            [Display(Name = "Referer")]
            public string? ReferrerId { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            await LoadFormDefaultData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Member/Clients/Index");
            if (ModelState.IsValid)
            {
                Client newClient = new Client()
                {
                    Company = Input.Company,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    Address = Input.Address,
                    City = Input.City,
                    State = Input.State,
                    ZipCode = Input.ZipCode,
                    DateAdded = DateTime.UtcNow.Date
                };

               

                await _context.Clients.AddAsync(newClient);
                await _context.SaveChangesAsync();
                if (Input.ReferrerId != null)
                {
                    AgentUser referrer = await _userManager.FindByIdAsync(Input.ReferrerId);
                    if (referrer == null)
                    {
                        ModelState.AddModelError(string.Empty, $"There is no agent with the id: {Input.ReferrerId}");
                        return Page();
                    }

                    newClient.Referrer = referrer;
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage(returnUrl);
            }
            return Page();
        }

        private async Task LoadFormDefaultData()
        {
            await _userManager.Users.ForEachAsync(agent =>
            {
                SelectAgents.Add(new SelectListItem()
                {
                    Text = agent.UserName,
                    Value = agent.Id
                });
            });
        }

        
    }
}
