using System;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Destuff.Server.Data;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests;

public abstract class IntegrationTestBase: IDisposable
{
    protected string? AuthToken { get; set; }

    readonly HttpMethod _method;
    protected readonly string Route;
    readonly HttpClient Http;
    readonly WebApplicationFactory<Program> app;
    readonly DbConnection _connection;

    public IntegrationTestBase(HttpMethod method, string route)
    {
        _method = method;
        Route = route;

        // Arrange
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => 
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
                    services.BuildServiceProvider();
                });
            });

        Http = app.CreateClient();
    }


    protected async Task<HttpResponseMessage> SendAsync(object? model, HttpMethod? method = null, string? route = null)
    {
        var request = new HttpRequestMessage(method ?? _method, route ?? Route);
        if (model != null)
            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        return await Http.SendAsync(request);
    }

    protected async Task<T?> SendAsync<T>(object model, HttpMethod? method = null, string? route = null) where T : class
    {
        var response = await SendAsync(model, method, route);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<HttpResponseMessage> AuthorizedSendAsync(object? model = null, HttpMethod? method = null, string? route = null)
    {
        if (AuthToken == null)
        {
            var user = new RegisterModel { UserName = "TokenUser", Password = "Qwer1234!" };
            await SendAsync(user, HttpMethod.Post, ApiRoutes.AuthRegister);
            var token = await SendAsync<AuthTokenModel>(user, HttpMethod.Post, ApiRoutes.AuthLogin);
            AuthToken = token?.AuthToken;
        }

        var request = new HttpRequestMessage(method ?? _method, route ?? Route);
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

    protected Task<T?> AuthorizedGetAsync<T>(string? route = null) where T : class => AuthorizedSendAsync<T>(null, HttpMethod.Get, route);

    protected Task<HttpResponseMessage> AuthorizedGetAsync(string? route = null) => AuthorizedSendAsync(null, HttpMethod.Get, route);
    
    protected Task<T?> AuthorizedPutAsync<T>(string id, object model) where T : class => AuthorizedSendAsync<T>(model, HttpMethod.Put, $"{Route}/{id}");

    protected Task<HttpResponseMessage> AuthorizedPutAsync(string id, object model) => AuthorizedSendAsync(model, HttpMethod.Put, $"{Route}/{id}");

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