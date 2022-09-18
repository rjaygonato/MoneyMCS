using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMCS.Areas.Identity.Data;

public class AgentUser : IdentityUser
{
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



}



