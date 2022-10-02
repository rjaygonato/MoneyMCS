using Microsoft.AspNetCore.Identity;
using MoneyMCS.Models;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Areas.Identity.Data;

public class AgentUser : IdentityUser
{

    public string ReferralCode { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AgentType { get; set; } = string.Empty;

    public string? ReferrerId { get; set; }
    public virtual AgentUser Referrer { get; set; }
    public virtual List<AgentUser> Referrals { get; set; }
    public virtual List<Client> Clients { get; set; }



}



