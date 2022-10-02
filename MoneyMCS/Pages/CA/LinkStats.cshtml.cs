using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace MoneyMCS.Pages.CA
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class LinkStatsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
