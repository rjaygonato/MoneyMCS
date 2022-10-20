using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class IndexModel : PageModel
    {
        public IndexModel(IWebHostEnvironment webHostEnvironment, ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public void OnGet()
        {

        }
    }
}
