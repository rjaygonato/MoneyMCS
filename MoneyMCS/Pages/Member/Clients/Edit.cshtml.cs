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
    public class EditModel : PageModel
    {
        public EditModel(ClientContext context, UserManager<AgentUser> userManager, ILogger<EditModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private readonly ClientContext _context;
        private readonly UserManager<AgentUser> _userManager;
        private readonly ILogger<EditModel> _logger;

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

            public string? returnUrl { get; set; }
        }

        public Client ToEditClient { get; set; }

        public async Task<IActionResult> OnGet([FromRoute] int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            ToEditClient = await _context.Clients.FindAsync(id);

            if (ToEditClient == null)
            {
                return NotFound();
            }

            await LoadFormDefaultData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int? Id)
        {

            if (Id == null)
            {
                return BadRequest();
            }

            Client toEditClient = await _context.Clients.FindAsync(Id);
            if (toEditClient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                toEditClient.Company = Input.Company;
                toEditClient.FirstName = Input.FirstName;
                toEditClient.LastName = Input.LastName;
                toEditClient.Email = Input.Email;
                toEditClient.PhoneNumber = Input.PhoneNumber;
                toEditClient.Address = Input.Address;
                toEditClient.City = Input.City;
                toEditClient.State = Input.State;
                toEditClient.ZipCode = Input.ZipCode;
                _context.Clients.Update(toEditClient);
                await _context.SaveChangesAsync();
                if (Input.ReferrerId != null)
                {
                    AgentUser referrer = await _userManager.FindByIdAsync(Input.ReferrerId);
                    if (referrer == null)
                    {
                        ModelState.AddModelError(String.Empty, $"Agent with the id: {Input.ReferrerId} is not found.");
                        await LoadFormDefaultData();
                        return Page();
                    }

                    toEditClient.Referrer = referrer;
                    _context.Clients.Update(toEditClient);
                    await _context.SaveChangesAsync();
                }



                Input.returnUrl ??= Url.Content($"~/Member/Clients/{Id}");
                return RedirectToPage("/Member/Clients/Index");
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
