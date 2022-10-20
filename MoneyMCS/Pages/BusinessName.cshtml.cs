using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Pages
{
    public class BusinessNameModel : PageModel
    {
        public BusinessNameModel(EntitiesContext context, ILogger<BusinessNameModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<BusinessNameModel> _logger;

        public class InputModel
        {
            [Required]
            public int clientId { get; set; }
            public string? BusinessName { get; set; }

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public Client Client { get; set; }



        public async Task<IActionResult> OnGet(int? clientId)
        {
            if (clientId == null)
            {
                return BadRequest();
            }

            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            Client = await _context.Clients
                .Where(c => c.ReferrerId == userId && c.ClientId == clientId)
                .Include(c => c.Business)
                .FirstAsync();

            if (Client == null)
            {
                return NotFound();
            }

            return Page();

        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            Client = await _context.Clients
                .Where(c => c.ReferrerId == userId && c.ClientId == Input.clientId)
                .Include(c => c.Business)
                .FirstOrDefaultAsync();

            if (Client == null)
            {
                return NotFound();
            }

            Client.Business.Name = Input.BusinessName ??= string.Empty;
            _context.Clients.Update(Client);
            await _context.SaveChangesAsync();
            return RedirectToPage("/ClientInfo", new { clientId = Input.clientId });

        }
    }
}
