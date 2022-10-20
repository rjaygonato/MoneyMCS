using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using Stripe;
using Stripe.Checkout;

namespace MoneyMCS.Pages.Membership
{
    [Authorize(Policy = "AgentAccessPolicy")]
    public class SuccessModel : PageModel
    {

        public SuccessModel(UserManager<ApplicationUser> userManager, EntitiesContext context, ILogger<SuccessModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EntitiesContext _context;
        private readonly ILogger<SuccessModel> _logger;


        public async Task<IActionResult> OnGet([FromQuery] string? session_id)
        {
            if (session_id == null)
            {
                return BadRequest();
            }

            var sessionService = new SessionService();
            Session session;
            try
            {
                var options = new SessionGetOptions();
                options.AddExpand("customer");
                options.AddExpand("subscription");
                options.AddExpand("line_items");
                session = await sessionService.GetAsync(session_id, options);

            }
            catch (StripeException ex)
            {
                _logger.LogInformation($"Stripe session id not found: {session_id}");

                return BadRequest();
            }
            bool subscriptionExists = await _context.StripeTransactions.AnyAsync(st => st.SubscriptionId == session.SubscriptionId);
            if (subscriptionExists)
            {
                _logger.LogInformation($"Subscription id already exists: {session.SubscriptionId}");
                return BadRequest();
            }

            //Get User
            string agentId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentId").Value;
            ApplicationUser user = await _userManager.FindByIdAsync(agentId);


            //Check all subscrptions to check wether its their first to subscribe or not
            int subscriptions = await _context.StripeTransactions.CountAsync(st => st.ApplicationUserId == user.Id);

            //Change soon if there is new subscription plan START
            var subscriptionItem = session.LineItems.First();

            string priceId = subscriptionItem.Price.Id;


            PriceService priceService = new PriceService();
            var price = await priceService.GetAsync(priceId);

            //Change soon if there is new subscription plan END


            //Save Stripe transaction details
            StripeTransaction newStripeTransaction = new StripeTransaction()
            {
                ApplicationUserId = agentId,
                SubscriptionId = session.SubscriptionId,
                UserId = session.CustomerId,
            };
            await _context.StripeTransactions.AddAsync(newStripeTransaction);


            //Save app transaction
            AppTransaction newAppTransaction = new AppTransaction()
            {
                ApplicationUserId = agentId,
                Type = TransactionType.SUBSCRIPTION,
                Date = DateTime.Now,
                Amount = price.UnitAmountDecimal.Value
            };

            await _context.AppTransactions.AddAsync(newAppTransaction);
            await _context.SaveChangesAsync();


            //Save subscription details
            SubscriptionDetails subscriptionDetails = new SubscriptionDetails()
            {
                StartDate = session.Subscription.CurrentPeriodStart,
                EndDate = session.Subscription.CurrentPeriodEnd,
                Price = price.UnitAmountDecimal.Value,
                AppTransactionId = newAppTransaction.AppTransactionId,
                Payer = new Payer()
                {
                    FullName = session.Customer.Name,
                    Email = session.Customer.Email,
                    Phone = session.Customer.Phone
                }

            };

            await _context.Subscriptions.AddAsync(subscriptionDetails);
            await _context.SaveChangesAsync();

            user.Subscribed = true;

            await _userManager.UpdateAsync(user);

            if (subscriptions == 0)
            {
                if (user.ReferrerId != null)
                {
                    //Commission for level one referrer
                    var levelOneReferrer = await _userManager.Users.Where(au => au.Id == user.ReferrerId)
                            .Include(au => au.Wallet)
                            .FirstAsync();
                    if (!levelOneReferrer.Subscribed)
                    {
                        return Page();
                    }
                    levelOneReferrer.Wallet.Balance += 33.00M;
                    await _userManager.UpdateAsync(levelOneReferrer);
                    await _context.AppTransactions.AddAsync(new AppTransaction()
                    {
                        Type = TransactionType.COMMISSION,
                        Date = DateTime.Now,
                        Amount = 33.00M,
                        ApplicationUserId = levelOneReferrer.Id
                    });
                    await _context.SaveChangesAsync();

                    if (levelOneReferrer.ReferrerId != null)
                    {
                        //Commission for level two referrer
                        var levelTwoReferrer = await _userManager.Users.Where(au => au.Id == levelOneReferrer.ReferrerId)
                            .Include(au => au.Wallet)
                            .FirstAsync();

                        if (!levelTwoReferrer.Subscribed)
                        {
                            return Page();
                        }

                        levelTwoReferrer.Wallet.Balance += 9.00M;
                        await _userManager.UpdateAsync(levelTwoReferrer);
                        await _context.AppTransactions.AddAsync(new AppTransaction()
                        {
                            Type = TransactionType.COMMISSION,
                            Date = DateTime.Now,
                            Amount = 9.00M,
                            ApplicationUserId = levelTwoReferrer.Id
                        });
                        await _context.SaveChangesAsync();

                        if (levelTwoReferrer.ReferrerId != null)
                        {
                            //Commission for level three referrer
                            var levelThreeReferrer = await _userManager.Users.Where(au => au.Id == levelTwoReferrer.ReferrerId)
                            .Include(au => au.Wallet)
                            .FirstAsync();
                            if (!levelThreeReferrer.Subscribed)
                            {
                                return Page();
                            }
                            levelThreeReferrer.Wallet.Balance += 3.00M;
                            await _userManager.UpdateAsync(levelThreeReferrer);
                            await _context.AppTransactions.AddAsync(new AppTransaction()
                            {
                                Type = TransactionType.COMMISSION,
                                Date = DateTime.Now,
                                Amount = 3.00M,
                                ApplicationUserId = levelThreeReferrer.Id
                            });
                            await _context.SaveChangesAsync();
                        }

                    }
                }
            }

            return Page();

        }

    }
}
