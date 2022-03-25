using Xunit;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Destuff.Server.Data;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Auth;

public class AuthRegisterRequestShould : IDisposable
{
    private readonly WebApplicationFactory<Program> app;
    private readonly HttpClient http;
    private readonly HttpRequestMessage request;

    public AuthRegisterRequestShould()
    {
        // Arrange
        var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Destuff");
        Directory.CreateDirectory(path);
        var dbpath = Path.Join(path, "destuff-test.db");
        File.Delete(dbpath);

        var connString = $"Data Source={dbpath}";

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => 
                {
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connString));   
                });
            });

        http = app.CreateClient();
        
        request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.AuthRegister);
    }

    [Fact]
    public async Task Create_New_User()
    {
        // Arrange
        var model = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        // Act
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RegisterResultModel>();

        // Assert
        Assert.True(result?.Succeeded);
    }

    [Fact]
    public async Task Fail_Existing_User()
    {
        var model = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        await http.SendAsync(request); // register once

        var request2 = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.AuthRegister);
        request2.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        // Act
        var response = await http.SendAsync(request2); // register twice
        var result = await response.Content.ReadFromJsonAsync<RegisterResultModel>();
        Console.WriteLine($"existing: {result?.Succeeded}");

        // Assert
        Assert.False(result?.Succeeded);
    }

    public void Dispose()
    {
        using (var scope = app.Services.CreateScope())
        {
            var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dataContext.Database.EnsureDeleted();
        }
    }
}