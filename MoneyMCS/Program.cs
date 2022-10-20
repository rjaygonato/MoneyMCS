using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Policies;
using MoneyMCS.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AffiliateEntitiesConnection");

builder.Services.AddDbContext<EntitiesContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.MaxFailedAccessAttempts = 4;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
    options.Password.RequireUppercase = false;

})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<EntitiesContext>();

builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"Keys"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AgentAccessPolicy", policyBuilder =>
    {
        policyBuilder.AddRequirements(new AgentTypeRequirement(UserType.AGENT));
    });
    options.AddPolicy("MemberAccessPolicy", policyBuilder =>
    {
        policyBuilder.AddRequirements(new MemberTypeRequirement(new List<UserType> { UserType.VIEWER, UserType.ADMINISTRATOR }));
    });

});

//Policy
builder.Services.AddSingleton<IAuthorizationHandler, AgentTypeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, MemberTypeHandler>();

//Email
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddRazorPages();


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe").GetValue<string>("ApiKey");

if (!Directory.Exists(Path.Combine(builder.Environment.ContentRootPath, "Resources")))
{
    Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "Resources"));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Resources")),
    RequestPath = "/Resource"
});

app.MapRazorPages();

app.Run();