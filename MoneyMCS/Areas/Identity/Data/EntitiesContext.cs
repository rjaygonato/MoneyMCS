using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MoneyMCS.Areas.Identity.Data;

public class EntitiesContext : IdentityDbContext<IdentityUser>
{
    public EntitiesContext(DbContextOptions<EntitiesContext> options)
        : base(options)
    {


    }

    public DbSet<MemberUser> MemberUsers { get; set; }
    public DbSet<AgentUser> AgentUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

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

        builder.Entity<AgentUser>()
            .HasIndex(au => au.ReferralCode)
            .IsUnique();
    }
}
