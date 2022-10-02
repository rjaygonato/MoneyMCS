using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoneyMCS.Pages
{
    [Authorize(Roles = "Agents")]
    public class AgentHomeModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
