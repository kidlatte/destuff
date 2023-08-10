using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Destuff.Client.Services;

public interface IHttpService
{
    event EventHandler<bool>? LoadingStateChanged;
    event EventHandler<bool>? OnlineStateChanged;
    event EventHandler<HttpRequestError>? OnError;

    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    Task<T?> SendAsync<T>(HttpRequestMessage request) where T : class;
    Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object? value);
    Task<T?> SendAsync<T>(HttpMethod method, string uri, object? value = null) where T : class;
    Task<T?> GetAsync<T>(string uri) where T : class;
    Task<T?> PostAsync<T>(string uri, object value) where T : class;
    Task<T?> PutAsync<T>(string uri, object value) where T : class;
    Task DeleteAsync(string uri);
    void InvokeError(string message, HttpStatusCode? statusCode);
}

public class HttpService : IHttpService
{
    public event EventHandler<bool>? LoadingStateChanged;
    public event EventHandler<bool>? OnlineStateChanged;
    public event EventHandler<HttpRequestError>? OnError;

    readonly HttpClient Http;
    readonly IStorageService Storage;
    readonly JsonSerializerOptions JsonOptions;
    int LoadingCounter = 0;

    public HttpService(HttpClient http, IStorageService storage)
    {
        Http = http;
        Storage = storage;

        JsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        System.Diagnostics.Debug.WriteLine($"HttpService instantiated");
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        // add jwt auth header if user is logged in and request is to the api url
        var auth = await Storage.GetAuth();
        var isApiUrl = request.RequestUri?.OriginalString.StartsWith("/api") ?? default;
        if (auth != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        try {
            SetLoadingState(true);

            var result = await Http.SendAsync(request);
            SetOnlineState(true);
            return result;
        }
        catch (HttpRequestException ex) {
            InvokeError(ex.Message, ex.StatusCode);

            if (ex.InnerException != null && ex.InnerException is SocketException exception) {
                var inner = exception;
                if (inner.SocketErrorCode == SocketError.ConnectionRefused)
                    SetOnlineState(false);
            }

            if (ex.Message.Contains("NetworkError"))
                SetOnlineState(false);

            throw;
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally {
            SetLoadingState(false);
        }
    }

    public async Task<T?> SendAsync<T>(HttpRequestMessage request) where T : class
    {
        var response = await SendAsync(request);

        // invoke error on error response
        if (!response.IsSuccessStatusCode) {
            InvokeError(response.ReasonPhrase ?? response.StatusCode.ToString(), response.StatusCode);
            response.EnsureSuccessStatusCode();
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
            return null;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object? value = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (value != null)
            request.Content = new StringContent(JsonSerializer.Serialize(value, JsonOptions), Encoding.UTF8, "application/json");

        return SendAsync(request);
    }

    public Task<T?> SendAsync<T>(HttpMethod method, string uri, object? value = null) where T : class
    {
        var request = new HttpRequestMessage(method, uri);
        if (value != null)
            request.Content = new StringContent(JsonSerializer.Serialize(value, JsonOptions), Encoding.UTF8, "application/json");

        return SendAsync<T>(request);
    }

    public Task<T?> GetAsync<T>(string uri) where T : class => SendAsync<T>(HttpMethod.Get, uri);
    public Task<T?> PostAsync<T>(string uri, object value) where T : class => SendAsync<T>(HttpMethod.Post, uri, value);
    public Task<T?> PutAsync<T>(string uri, object value) where T : class => SendAsync<T>(HttpMethod.Put, uri, value);
    public Task DeleteAsync(string uri) => SendAsync<object>(HttpMethod.Delete, uri);

    void SetLoadingState(bool loading)
    {
        LoadingCounter += loading ? 1 : -1;
        LoadingCounter = Math.Max(LoadingCounter, 0);
        LoadingStateChanged?.Invoke(this, LoadingCounter > 0);
    }

    void SetOnlineState(bool online) => OnlineStateChanged?.Invoke(this, online);

    public void InvokeError(string message, HttpStatusCode? statusCode) => OnError?.Invoke(this, new HttpRequestError(message, statusCode));
}

public class HttpRequestError
{
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public HttpRequestError(string message, HttpStatusCode? statusCode = null)
    {
        Message = message;
        StatusCode = statusCode;
    }
}

public class ConflictException<T> : Exception where T : class
{
    public T Content { get; set; }

    public ConflictException(T content)
    {
        Content = content;
    }
}
