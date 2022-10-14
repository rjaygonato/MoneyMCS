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
    public DbSet<StripeTransaction> StripeTransactions { get; set; }
    public DbSet<AppTransaction> AppTransactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

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
            .HasMany(au => au.StripeTransactions)
            .WithOne(st => st.ApplicationUser)
            .HasForeignKey(st => st.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);



        builder.Entity<ApplicationUser>()
            .HasMany(au => au.Transactions)
            .WithOne(t => t.ApplicationUser)
            .HasForeignKey(t => t.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<AppTransaction>()
            .Property(at => at.Type)
            .HasConversion(
                v => v.ToString(),
                v => (TransactionType)Enum.Parse(typeof(TransactionType), v)
            );

        builder.Entity<ApplicationUser>()
            .HasOne(au => au.Wallet)
            .WithOne(w => w.ApplicationUser)
            .HasForeignKey<Wallet>(w => w.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<ApplicationUser>()
            .HasIndex(au => au.ReferralCode)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .Property(au => au.Subscribed)
            .HasDefaultValue(false);

        builder.Entity<StripeTransaction>()
            .HasIndex(st => st.UserId)
            .IsUnique();

        builder.Entity<StripeTransaction>()
            .HasIndex(st => st.SubscriptionId)
            .IsUnique();


    }
}
