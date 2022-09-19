using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;

namespace MoneyMCS.Areas.Identity.Data;

public class EntitiesContext : IdentityDbContext<IdentityUser>
{
    public EntitiesContext(DbContextOptions<EntitiesContext> options)
        : base(options)
    {
    }

    public DbSet<AgentUser> AgentUsers { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Default
        //builder.Entity<MemberUser>()
        //    .HasOne(mu => mu.Referrer)
        //    .WithMany(mu => mu.Referrals)
        //    .HasForeignKey(mu => mu.ReferrerId)
        //    .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<AgentUser>()
            .HasOne(au => au.Referrer)
            .WithMany(au => au.Referrals)
            .HasForeignKey(au => au.ReferrerId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}
