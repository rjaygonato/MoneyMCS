using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace MoneyMCS.Pages.CA
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class IndexModel : PageModel
    {
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<IndexModel> _logger;

        public void OnGet()
        {
        }
    }
}
