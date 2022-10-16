using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using Stripe.Issuing;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace MoneyMCS.Pages.Member.Agents
{
    [Authorize(Policy = "MemberAccessPolicy")]
    public class IndexModel : PageModel
    {

        public IndexModel(UserManager<ApplicationUser> userManager, EntitiesContext context, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EntitiesContext _context;
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

        public async Task<IActionResult> OnGet()
        {
            cleanModel();
            IQueryable<ApplicationUser> query = _userManager.Users
                .Where(u => u.UserType == UserType.AGENT)
                .Where(u =>
                u.UserName.Contains(Input.Username) &&
                u.FirstName.Contains(Input.FirstName) &&
                u.LastName.Contains(Input.LastName) &&
                u.Email.Contains(Input.Email) &&
                u.PhoneNumber.Contains(Input.PhoneNumber) &&
                Input.AgentType == string.Empty ? (u.AgentType.Equals("BASIC") || u.AgentType.Equals("DIY") || u.AgentType.Equals("VIP")) : u.AgentType.Equals(Input.AgentType));
            if (!string.IsNullOrWhiteSpace(Input.ReferrerId))
            {
                query = query.Where(u => u.ReferrerId.Equals(Input.ReferrerId));
            }

            Agents = await query.ToListAsync();


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

            List<ApplicationUser> referredAgents = await _userManager.Users.Where(au => au.ReferrerId == user.Id).ToListAsync();
            List<Client> referredClients = await _context.Clients.Where(c => c.ReferrerId == user.Id).ToListAsync();
            List<AppTransaction> appTransactions = await _context.AppTransactions.Where(at => at.ApplicationUserId == user.Id).ToListAsync();
            List<StripeTransaction> stripeTransactions = await _context.StripeTransactions.Where(st => st.ApplicationUserId == user.Id).ToListAsync();
            Wallet wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.ApplicationUserId == user.Id);

            if (referredAgents != null)
            {
                foreach (var agent in referredAgents)
                {
                    agent.ReferrerId = null;
                    _context.Entry(agent).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (referredClients != null)
            {
                foreach (var client in referredClients)
                {
                    client.ReferrerId = null;
                    _context.Entry(client).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (appTransactions != null)
            {
                foreach (var appTransaction in appTransactions)
                {
                    appTransaction.ApplicationUserId = null;
                    _context.Entry(appTransaction).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (stripeTransactions != null)
            {
                foreach (var stripeTransaction in stripeTransactions)
                {
                    stripeTransaction.ApplicationUserId = null;
                    _context.Entry(stripeTransaction).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            if (wallet == null)
            {
                wallet.ApplicationUserId = null;
                _context.Entry(wallet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
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
