using System.IO;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

var path = configuration["DOTNET_RUNNING_IN_CONTAINER"] == "true" ?
    "/config" :
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

var folder = Path.Join(path, "Destuff");
Console.WriteLine($"folder: {folder}");
Directory.CreateDirectory(folder);

var dbPath = Path.Join(folder, "destuff.db");
var connString = $"Data Source={dbPath}";
Console.WriteLine($"connString: {connString}");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connString));

builder.Services
    .AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedEmail = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

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


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
