global using Xunit;
global using System;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Threading.Tasks;
global using Destuff.Shared;
global using Destuff.Shared.Models;

using System.Data.Common;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Destuff.Server.Data;
using Destuff.Server.Services;

namespace Destuff.Tests;

public abstract class IntegrationTestBase: IDisposable
{
    protected string? AuthToken { get; set; }

    protected readonly HttpMethod Method;
    protected readonly string Route;
    protected readonly HttpClient Http;
    readonly WebApplicationFactory<Program> app;
    readonly DbConnection _connection;

    public IntegrationTestBase(HttpMethod method, string route)
    {
        Method = method;
        Route = route;

        // Arrange
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => 
                {
                    var types = new [] { typeof(DbContextOptions<ApplicationDbContext>), typeof(IFileService) };
                    var descriptors = services.Where(d => types.Contains(d.ServiceType)).ToList();
                    descriptors.ForEach(x => services.Remove(x));

                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
                    services.AddScoped<IFileService>(_ => new FileService(Directory.GetCurrentDirectory()));
                    services.BuildServiceProvider();
                });
            });

        Http = app.CreateClient();
    }

    protected async Task<HttpResponseMessage> SendAsync(object? model, HttpMethod? method = null, string? route = null)
    {
        var request = new HttpRequestMessage(method ?? Method, route ?? Route);
        if (model != null)
            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        return await Http.SendAsync(request);
    }

    protected async Task<T?> SendAsync<T>(object model, HttpMethod? method = null, string? route = null) where T : class
    {
        var response = await SendAsync(model, method, route);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<HttpResponseMessage> AuthorizedSendAsync(HttpRequestMessage request)
    {
        if (AuthToken == null)
        {
            var user = new RegisterRequest { UserName = "TokenUser", Password = "Qwer1234!" };
            await SendAsync(user, HttpMethod.Post, ApiRoutes.AuthRegister);
            var token = await SendAsync<AuthModel>(user, HttpMethod.Post, ApiRoutes.AuthLogin);
            AuthToken = token?.Token;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
        return await Http.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> AuthorizedSendAsync(object? model = null, HttpMethod? method = null, string? route = null)
    {
        var request = new HttpRequestMessage(method ?? Method, route ?? Route);
        if (model != null)
            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            
        return await AuthorizedSendAsync(request);
    }

    protected async Task<T?> AuthorizedSendAsync<T>(object? model = null, HttpMethod? method = null, string? route = null) where T : class
    {
        var response = await AuthorizedSendAsync(model, method, route);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected Task<HttpResponseMessage> AuthorizedGetAsync(string? route = null) => AuthorizedSendAsync(null, HttpMethod.Get, route);
    protected Task<T?> AuthorizedGetAsync<T>(string? route = null) where T : class => AuthorizedSendAsync<T>(null, HttpMethod.Get, route);

    protected Task<HttpResponseMessage> AuthorizedPostAsync(object model, string? route = null) => AuthorizedSendAsync(model, HttpMethod.Post, route);
    protected Task<T?> AuthorizedPostAsync<T>(object model, string? route = null) where T : class => AuthorizedSendAsync<T>(model, HttpMethod.Post, route);

    protected Task<HttpResponseMessage> AuthorizedPutAsync(string id, object? model = null) => AuthorizedSendAsync(model, HttpMethod.Put, $"{Route}/{id}");
    protected Task<T?> AuthorizedPutAsync<T>(string id, object? model = null) where T : class => AuthorizedSendAsync<T>(model, HttpMethod.Put, $"{Route}/{id}");

    protected Task<HttpResponseMessage> AuthorizedDeleteAsync(string id) => AuthorizedSendAsync(null, HttpMethod.Delete, $"{Route}/{id}");

    public void Dispose()
    {
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dataContext.Database.EnsureDeleted();
        dataContext.Database.CloseConnection();
        _connection.Dispose();
    }
}