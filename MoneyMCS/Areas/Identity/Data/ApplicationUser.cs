using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string UserType { get; set; }
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
    [Column(TypeName = "Money")]
    public decimal Balance { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}

public class AppTransaction
{
    public int AppTransactionId { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    [Column(TypeName = "Money")]
    public decimal Amount { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public DateTime? ExpirationDate { get; set; }

}

public enum TransactionType
{
    SUBSCRIPTION,
    COMMISSION,
    WITHDRAWAL
}



