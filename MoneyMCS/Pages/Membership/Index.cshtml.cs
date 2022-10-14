using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;

namespace MoneyMCS.Pages.Membership
{
    public class IndexModel : PageModel
    {
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            string domain = _configuration.GetSection("Domain").GetValue<string>("Name");
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = "price_1Ls4t6KeGRWG6LRGFBnN84SC",
                    Quantity = 1,
                  },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/Membership/Success?session_id={CHECKOUT_SESSION_ID}&user_id=" + userId,
                CancelUrl = domain + "/Membership/Failed",
            };
            var service = new SessionService();
            Session session = service.Create(options);


            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

    }
}
