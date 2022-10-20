using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Pages
{
    public class ClientInfoModel : PageModel
    {
        public ClientInfoModel(EntitiesContext context, ILogger<ClientInfoModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<ClientInfoModel> _logger;

        public Client Client { get; set; }
        public Business Business { get; set; }
        public Address Address { get; set; }
        public BusinessPhone Phone { get; set; }
        public async Task<IActionResult> OnGet(int? clientId)
        {
            if (clientId == null)
            {
                return BadRequest();
            }
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            Client = await _context.Clients
                .Where(c => c.ReferrerId == userId && c.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (Client == null)
            {
                return NotFound();
            }
            Business = await _context.Businesses.Where(c => c.ClientId == clientId).FirstOrDefaultAsync();
            Address = await _context.Addresses.Where(b => b.BusinessId == Business.BusinessId).FirstOrDefaultAsync();
            Phone = await _context.BusinessPhones.Where(b => b.BusinessId == Business.BusinessId).FirstOrDefaultAsync();


            return Page();
        }
    }
}
