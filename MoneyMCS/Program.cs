using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AffiliateEntitiesConnection");

builder.Services.AddDbContext<EntitiesContext>(options =>
    options.UseSqlServer(connectionString));



builder.Services.AddDefaultIdentity<MemberUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EntitiesContext>();
builder.Services.AddIdentityCore<AgentUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EntitiesContext>();

// Add services to the container.
builder.Services.AddRazorPages();

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

app.MapRazorPages();

app.Run();
