using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Pages.Member.Clients
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class IndexModel : PageModel
    {


        public IndexModel(EntitiesContext context, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public List<SelectListItem> AgentsSelect = new List<SelectListItem>
        {
            new SelectListItem() { Text = "None", Value = "", Selected = true }
        };



        public class InputModel
        {
            public string? Email { get; set; } = string.Empty;
            [Display(Name = "Phone")]
            public string? Phone { get; set; } = string.Empty;
            [Display(Name = "First Name")]
            public string? FirstName { get; set; } = string.Empty;
            [Display(Name = "Last Name")]
            public string? LastName { get; set; } = string.Empty;
            [Display(Name = "Referral Agent")]
            public string? ReferrerId { get; set; }

        }

        public List<Client> Clients { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            cleanModel();

            IQueryable<Client> query = _context.Clients
                .Where(u =>
                u.Email.Contains(Input.Email) &&
                u.PhoneNumber.Contains(Input.Phone) &&
                u.FirstName.Contains(Input.FirstName) &&
                u.LastName.Contains(Input.LastName));

            if (!string.IsNullOrWhiteSpace(Input.ReferrerId))
            {
                query = query.Where(u => u.ReferrerId.Equals(Input.ReferrerId));
            }

            Clients = await query.ToListAsync();

            await _userManager.Users.Where(u => u.UserType == UserType.AGENT).ForEachAsync(a =>
            {
                AgentsSelect.Add(new SelectListItem
                {
                    Text = a.UserName,
                    Value = a.Id
                });
            });

            return Page();
        }

        public async Task<IActionResult> OnPostDelete([FromForm] int? toDeleteClientId)
        {
            if (toDeleteClientId == null)
            {
                return BadRequest();
            }

            var client = await _context.Clients.FindAsync(toDeleteClientId);
            if (client == null)
            {
                return BadRequest();
            }
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Member/Clients/Index");
        }


        private void cleanModel()
        {
            Input.Email ??= string.Empty;
            Input.Phone ??= string.Empty;
            Input.FirstName ??= string.Empty;
            Input.LastName ??= string.Empty;
            Input.ReferrerId ??= string.Empty;
        }


    }
}
