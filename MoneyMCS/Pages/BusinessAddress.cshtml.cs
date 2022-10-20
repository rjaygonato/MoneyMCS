using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Constants;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Pages
{
    public class BusinessAddressModel : PageModel
    {
        public BusinessAddressModel(EntitiesContext context, ILogger<BusinessAddressModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly EntitiesContext _context;
        private readonly ILogger<BusinessAddressModel> _logger;

        [BindProperty]
        public InputModel Input { get; set; }
        public Client Client { get; set; }

        public List<SelectListItem> StatesSelect { get; set; } = new();
        public List<SelectListItem> AddressTypeSelect { get; set; } = new();

        public class InputModel
        {
            [Required]
            public int clientId { get; set; }
            [Display(Name = "Company Address")]
            public string CompanyAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }
            public AddressType Type { get; set; }
        }

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
                .ThenInclude(b => b.Address)
                .FirstOrDefaultAsync();

            if (Client == null)
            {
                return NotFound();
            }

            if (Client.Business == null)
            {
                Client.Business = new Business();
                Client.Business.Address = new Address();
                Client.Business.BusinessPhone = new BusinessPhone();


                _context.Update(Client);
            }

            LoadSelects();

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
                .ThenInclude(b => b.Address)
                .FirstOrDefaultAsync();

            if (Client == null)
            {
                return NotFound();
            }

            if (Client.Business == null)
            {
                Business newBusiness = new Business()
                {
                    ClientId = Client.ClientId,
                    Address = new Address(),
                    BusinessPhone = new BusinessPhone()
                };

                await _context.Businesses.AddAsync(newBusiness);
                await _context.SaveChangesAsync();
            }

            Client.Business.Address.CompanyAddress = Input.CompanyAddress;
            Client.Business.Address.City = Input.City;
            Client.Business.Address.State = Input.State;
            Client.Business.Address.ZipCode = Input.ZipCode;
            Client.Business.Address.Type = Input.Type;

            _context.Clients.Update(Client);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ClientInfo", new { clientId = Input.clientId });

        }

        private void LoadSelects()
        {
            AppConstants.USAStates.ForEach(s =>
            {
                StatesSelect.Add(new SelectListItem()
                {
                    Text = s,
                    Value = s,
                    Selected = Client.Business.Address.State == s
                });
            });

            string businessAddressTypeStr = Client.Business.Address.Type == null ? string.Empty : Client.Business.Address.Type.ToString();

            var addressTypes = Enum.GetNames<AddressType>();
            foreach (var type in addressTypes)
            {
                AddressTypeSelect.Add(new SelectListItem()
                {
                    Text = type,
                    Value = type,
                    Selected = type == businessAddressTypeStr
                });
            };



        }
    }
}
