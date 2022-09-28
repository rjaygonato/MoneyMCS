using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AffiliateEntitiesConnection");

builder.Services.AddDbContext<EntitiesContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ResourceContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ClientContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddDefaultIdentity<MemberUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<EntitiesContext>();
builder.Services.AddIdentityCore<AgentUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<EntitiesContext>();

// Add services to the container.
builder.Services.AddRazorPages();

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Resources")),
    RequestPath = "/Resource"
});

app.MapRazorPages();

app.Run();
