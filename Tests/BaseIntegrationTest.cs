using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Destuff.Server.Data;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Destuff.Tests;

public abstract class BaseIntegrationTest: IDisposable
{
    protected readonly HttpClient Http;
    protected readonly HttpRequestMessage Request;
    readonly WebApplicationFactory<Program> app;
    readonly HttpMethod _method;
    readonly string _route;

    public BaseIntegrationTest(HttpMethod method, string route)
    {
        _method = method;
        _route = route;

        // Arrange
        var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Destuff");
        Directory.CreateDirectory(path);
        var dbpath = Path.Join(path, $"{route.Replace("/", "-")}.db");
        File.Delete(dbpath);

        var connString = $"Data Source={dbpath}";

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => 
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connString));
                    services.BuildServiceProvider();
                });
            });

        Http = app.CreateClient();

        Request = new HttpRequestMessage(method, route);
    }

    protected async Task<T?> SendAsync<T>(object model, HttpMethod? method = null, string? route = null) where T : class
    {
        var response = await SendAsync(model, method, route);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<HttpResponseMessage> SendAsync(object model, HttpMethod? method = null, string? route = null)
    {
        var request = new HttpRequestMessage(method ?? _method, route ?? _route);
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        return await Http.SendAsync(request);
    }

    public void Dispose()
    {
        using (var scope = app.Services.CreateScope())
        {
            var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dataContext.Database.EnsureDeleted();
            dataContext.Database.CloseConnection();
        }
    }
}