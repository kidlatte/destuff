using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Destuff.Client.Services;

public interface IHttpService
{
    Task<T?> GetAsync<T>(string uri) where T : class;
    Task<T?> PostAsync<T>(string uri, object value) where T : class;
    Task<T?> PutAsync<T>(string uri, object value) where T : class;
    Task<HttpResponseMessage> DeleteAsync(string uri);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}

public class HttpService : IHttpService
{
    private IStorageService _storage;
    private HttpClient _http;
    private NavigationManager _navigation;
    
    public HttpService(
        HttpClient httpClient,
        NavigationManager navigation,
        IStorageService storage
    )
    {
        _http = httpClient;
        _navigation = navigation;
        _storage = storage;
    }

    public Task<T?> GetAsync<T>(string uri) where T : class => sendRequest<T>(HttpMethod.Get, uri);

    public Task<T?> PostAsync<T>(string uri, object value) where T : class => sendRequest<T>(HttpMethod.Post, uri, value);

    public Task<T?> PutAsync<T>(string uri, object value) where T : class => sendRequest<T>(HttpMethod.Put, uri, value);

    public Task<HttpResponseMessage> DeleteAsync(string uri) => sendRequest(HttpMethod.Delete, uri);

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        // add jwt auth header if user is logged in and request is to the api url
        var user = await _storage.GetUserAsync();
        var isApiUrl = request.RequestUri?.OriginalString.StartsWith("/api") ?? default;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AuthToken);

        return await _http.SendAsync(request);
    }

    private Task<HttpResponseMessage> sendRequest(HttpMethod method, string uri, object? value = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (value != null)
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

        return SendAsync(request);
    }

    private async Task<T?> sendRequest<T>(HttpMethod method, string uri, object? value = null) where T : class
    {
        var response = await sendRequest(method, uri, value);

        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigation.NavigateTo("/login");
            return default;
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }
    
}