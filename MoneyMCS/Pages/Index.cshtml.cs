using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoneyMCS.Pages
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}
