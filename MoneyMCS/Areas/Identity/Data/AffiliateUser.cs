using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MoneyMCS.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AffiliateUser class
public class AffiliateUser : IdentityUser
{
    
    public int Referrer
    public AffiliateUser Referrer { get; set; }
    public void test()
    {
    }

}

