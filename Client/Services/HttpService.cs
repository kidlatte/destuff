using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public interface IHttpService
{
    Task<T?> GetAsync<T>(string uri) where T : class;
    Task<T?> PostAsync<T>(string uri, object value) where T : class;
    Task PostAsync(string uri, object value);
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

    public async Task<T?> GetAsync<T>(string uri) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await sendRequest<T>(request);
    }

    public async Task<T?> PostAsync<T>(string uri, object value) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await sendRequest<T>(request);
    }

    public async Task PostAsync(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

        // add jwt auth header if user is logged in and request is to the api url
        var user = await _storage.GetUserAsync();
        var isApiUrl = request.RequestUri?.OriginalString.StartsWith("/api") ?? default;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AuthToken);

        using var response = await _http.SendAsync(request);

        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            _navigation.NavigateTo("/login");

        // throw exception on error response
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error != null ? error["message"] : default);
        }
    }

    private async Task<T?> sendRequest<T>(HttpRequestMessage request) where T : class
    {
        // add jwt auth header if user is logged in and request is to the api url
        var user = await _storage.GetUserAsync();
        var isApiUrl = request.RequestUri?.OriginalString.StartsWith("/api") ?? default;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AuthToken);

        using var response = await _http.SendAsync(request);

        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigation.NavigateTo("/login");
            return default;
        }

        // throw exception on error response
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error != null ? error["message"] : default);
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}