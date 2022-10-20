using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
