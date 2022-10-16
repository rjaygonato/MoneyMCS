using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace MoneyMCS.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{

    public string? ReferralCode { get; set; }

    [MaxLength(100)]
    public string FirstName { get; set; }

    [MaxLength(100)]
    public string LastName { get; set; }

    [MaxLength(100)]
    public string? AgentType { get; set; }

    public string? ReferrerId { get; set; }
    public ApplicationUser Referrer { get; set; }
    public List<ApplicationUser> Referrals { get; set; }
    public List<Client> Clients { get; set; }
    public List<StripeTransaction> StripeTransactions { get; set; }
    public List<AppTransaction> Transactions { get; set; }
    public Wallet Wallet { get; set; }
    public DateTime CreationDate { get; set; }
    public UserType UserType { get; set; }
    public bool Subscribed { get; set; }


}

public class StripeTransaction
{
    public int StripeTransactionId { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string UserId { get; set; }
    public string SubscriptionId { get; set; }
}



public class Wallet
{
    public int WalletId { get; set; }
    public decimal Balance { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}

public class AppTransaction
{
    public int AppTransactionId { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }

    public decimal Amount { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public int? SubscriptionId { get; set; }
    public SubscriptionDetails Subscription { get; set; }
}

public class SubscriptionDetails
{
    public int SubscriptionDetailsId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public int AppTransactionId { get; set; }
    public AppTransaction AppTransaction { get; set; }
    public int PayerId { get; set; }
    public Payer Payer { get; set; }

}

public class Payer
{
    public int PayerId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    
    public string? Phone { get; set; }

    public int SubscriptionId { get; set; }
    public SubscriptionDetails Subscription { get; set; }
}

public enum TransactionType
{
    SUBSCRIPTION,
    COMMISSION,
    WITHDRAWAL
}

public enum UserType
{
    AGENT,
    ADMINISTRATOR,
    VIEWER
}



