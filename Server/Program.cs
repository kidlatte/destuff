using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Server.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Generate connection string
var path = configuration.GetStoragePath();

var dbfile = Path.Join(path, "destuff.db");
var connString = $"Data Source={dbfile}";

// Add services to the container.
builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connString));

builder.Services
    .AddIdentityCore<ApplicationUser>(options => options.User.RequireUniqueEmail = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var settings = builder.GetAppSettings();
builder.Services.AddJwtAuthentication(settings.Secret);

builder.Services.AddSingleton<ILocationIdService, LocationIdService>();
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    var locationId = provider.GetService<ILocationIdService>();
    if (locationId != null)
        cfg.AddProfile(new MapperProfile(locationId));
}).CreateMapper());

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Migrate datacontext changes
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dataContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
