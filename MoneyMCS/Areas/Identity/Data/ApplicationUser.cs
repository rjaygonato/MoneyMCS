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
    public DateTime CreationDate { get; set; }
    public string UserType { get; set; }


}



