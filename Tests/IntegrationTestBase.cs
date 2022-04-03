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
using Destuff.Shared.Models;
using Destuff.Shared;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Destuff.Tests;

public abstract class IntegrationTestBase: IDisposable
{
    readonly HttpMethod _method;
    readonly string _route;
    readonly HttpClient Http;
    readonly WebApplicationFactory<Program> app;

    public IntegrationTestBase(HttpMethod method, string route)
    {
        _method = method;
        _route = route;

        // Arrange
        var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Destuff");
        Directory.CreateDirectory(path);

        var dbpath = Path.Join(path, $"{route.Trim('/').Replace("/", "-")}-{method.ToString().ToLower()}.db");
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
    }


    protected async Task<HttpResponseMessage> SendAsync(object model, HttpMethod? method = null, string? route = null)
    {
        var request = new HttpRequestMessage(method ?? _method, route ?? _route);
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        return await Http.SendAsync(request);
    }

    protected async Task<T?> SendAsync<T>(object model, HttpMethod? method = null, string? route = null) where T : class
    {
        var response = await SendAsync(model, method, route);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    private string? AuthToken { get; set; }

    protected async Task<HttpResponseMessage> AuthorizedSendAsync(object? model = null, HttpMethod? method = null, string? route = null)
    {
        if (AuthToken == null)
        {
            var user = new RegisterModel { UserName = "TokenUser", Password = "Qwer1234!" };
            await SendAsync(user, HttpMethod.Post, ApiRoutes.AuthRegister);
            var token = await SendAsync<AuthTokenModel>(user, HttpMethod.Post, ApiRoutes.AuthLogin);
            AuthToken = token?.AuthToken;
        }

        var request = new HttpRequestMessage(method ?? _method, route ?? _route);
        if (model != null)
            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);

        return await Http.SendAsync(request);
    }

    protected async Task<T?> AuthorizedSendAsync<T>(object? model = null, HttpMethod? method = null, string? route = null) where T : class
    {
        var response = await AuthorizedSendAsync(model, method, route);
        return await response.Content.ReadFromJsonAsync<T>();
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