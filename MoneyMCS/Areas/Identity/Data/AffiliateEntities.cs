using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Areas.Identity.Data;

public class AffiliateEntities : IdentityDbContext<AffiliateUser>
{
    public AffiliateEntities(DbContextOptions<AffiliateEntities> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
