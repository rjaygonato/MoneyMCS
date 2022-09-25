
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MoneyMCS.Services;
using MoneyMCS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoneyMCS.Pages.Member
{
    public class AddResourceModel : PageModel
    {

        public AddResourceModel(ResourceContext context, ILogger<AddResourceModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ResourceContext _context;
        private readonly ILogger<AddResourceModel> _logger;

        public class InputModel
        {
            [Required]
            [Display(Name = "Resource Name")]
            public string ResourceName { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            public string Category { get; set; }
            [Required]
            [Display(Name = "Resource File")]
            public IFormFile ResourceFile { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        public List<SelectListItem> SelectResourceCategory = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Sales Resources", Value = "Sales Resources" },
            new SelectListItem() { Text = "Contracts and Agreements", Value = "Contracts and Agreements" },
            new SelectListItem() { Text = "Finance Resources", Value = "Finance Resources" },
            new SelectListItem() { Text = "Marketing Emails", Value = "Marketing Emails" },
            new SelectListItem() { Text = "Marketing Flyers", Value = "Marketing Flyers" },
            new SelectListItem() { Text = "Marketing Powerpoints", Value = "Marketing Powerpoints" },
            new SelectListItem() { Text = "Marketing Social Media", Value = "Marketing Social Media" },
        };

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Member/Resources");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string fileName = $"{Path.GetRandomFileName()}{Path.GetExtension(Input.ResourceFile.FileName)}";
            string filePath = Path.Combine("Resources", fileName);
            string urlPath = $"\\Resource\\{fileName}";
            var fs = new FileStream(filePath, FileMode.CreateNew);
            await Input.ResourceFile.CopyToAsync(fs);

            var newResource = new Resource()
            {
                ResourceName = Input.ResourceName,
                Description = Input.Description,
                Category = Input.Category,
                FilePath = urlPath
            };
            await _context.Resources.AddAsync(newResource);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Member/Resources");
            
        }
    }
}
