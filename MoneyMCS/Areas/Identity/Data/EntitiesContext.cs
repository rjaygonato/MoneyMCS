using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace MoneyMCS.Areas.Identity.Data;

public class EntitiesContext : IdentityDbContext<ApplicationUser>
{
    public EntitiesContext(DbContextOptions<EntitiesContext> options)
        : base(options)
    {
        

    }

    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        //Default
        //builder.Entity<MemberUser>()
        //    .HasOne(mu => mu.Referrer)
        //    .WithMany(mu => mu.Referrals)
        //    .HasForeignKey(mu => mu.ReferrerId)
        //    .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ApplicationUser>()
            .HasOne(au => au.Referrer)
            .WithMany(au => au.Referrals)
            .HasForeignKey(au => au.ReferrerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<ApplicationUser>()
            .HasMany(au => au.Clients)
            .WithOne(c => c.Referrer)
            .HasForeignKey(c => c.ReferrerId)
            .OnDelete(DeleteBehavior.ClientSetNull);


        builder.Entity<ApplicationUser>()
            .HasIndex(au => au.ReferralCode)
            .IsUnique();
    }
}
