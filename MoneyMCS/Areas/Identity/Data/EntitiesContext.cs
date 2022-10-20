using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
    public DbSet<SubscriptionDetails> Subscriptions { get; set; }
    public DbSet<Payer> Payers { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<BusinessPhone> BusinessPhones { get; set; }

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


        //ApplicationUser One to Many relationship with itself
        builder.Entity<ApplicationUser>()
            .HasOne(au => au.Referrer)
            .WithMany(au => au.Referrals)
            .HasForeignKey(au => au.ReferrerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        //ApplicationUser One to Many relationship with Client
        builder.Entity<ApplicationUser>()
            .HasMany(au => au.Clients)
            .WithOne(c => c.Referrer)
            .HasForeignKey(c => c.ReferrerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        //ApplicationUser One to Many relationship with StripeTransaction
        builder.Entity<ApplicationUser>()
            .HasMany(au => au.StripeTransactions)
            .WithOne(st => st.ApplicationUser)
            .HasForeignKey(st => st.ApplicationUserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        //ApplicationUser One to Many relationship with Transaction
        builder.Entity<ApplicationUser>()
            .HasMany(au => au.Transactions)
            .WithOne(t => t.ApplicationUser)
            .HasForeignKey(t => t.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //ApplicationUser One to One relationship with Wallet
        builder.Entity<ApplicationUser>()
            .HasOne(au => au.Wallet)
            .WithOne(w => w.ApplicationUser)
            .HasForeignKey<Wallet>(w => w.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //AppTransaction One to One relationship with SubscriptionDetail
        builder.Entity<AppTransaction>()
            .HasOne(at => at.Subscription)
            .WithOne(s => s.AppTransaction)
            .HasForeignKey<SubscriptionDetails>(s => s.AppTransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        //SubscriptionDetail One to One relationship with Payer
        builder.Entity<SubscriptionDetails>()
            .HasOne(s => s.Payer)
            .WithOne(p => p.Subscription)
            .HasForeignKey<Payer>(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Client>()
            .HasOne(c => c.Business)
            .WithOne(b => b.Client)
            .HasForeignKey<Business>(b => b.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Client>()
            .HasMany(c => c.Notes)
            .WithOne(n => n.Client)
            .HasForeignKey(n => n.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Business>()
            .HasOne(b => b.Address)
            .WithOne(a => a.Business)
            .HasForeignKey<Address>(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Business>()
            .HasOne(b => b.BusinessPhone)
            .WithOne(p => p.Business)
            .HasForeignKey<BusinessPhone>(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StripeTransaction>()
            .HasIndex(st => st.UserId)
            .IsUnique();

        builder.Entity<StripeTransaction>()
            .HasIndex(st => st.SubscriptionId)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .Property(au => au.UserType)
            .HasConversion(
                v => v.ToString(),
                v => (UserType)Enum.Parse(typeof(UserType), v)
            );

        builder.Entity<Client>()
            .Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ClientStatus)Enum.Parse(typeof(ClientStatus), v)
            );

        builder.Entity<Address>()
            .Property(a => a.Type)
            .HasConversion(
                v => v.ToString(),
                v => (AddressType)Enum.Parse(typeof(AddressType), v)
            );



        builder.Entity<Client>()
            .Property(c => c.Services)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));


        builder.Entity<AppTransaction>()
            .Property(at => at.Type)
            .HasConversion(
                v => v.ToString(),
                v => (TransactionType)Enum.Parse(typeof(TransactionType), v)
            );

        builder.Entity<ApplicationUser>()
            .HasIndex(au => au.ReferralCode)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .Property(au => au.Subscribed)
            .HasDefaultValue(false);


    }

    protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(7, 2);
    }
}
