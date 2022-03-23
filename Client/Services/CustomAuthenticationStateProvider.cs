using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    public HttpClient _http { get; }
    public ILocalStorageService _localStorage { get; }

    private readonly string currentUserKey = "current-user";

    public CustomAuthenticationStateProvider(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var currentUser = await _localStorage.GetItemAsync<AuthTokenModel>(currentUserKey);
        Console.WriteLine($">>> AuthenticationState: {currentUser?.UserName}");

        if (currentUser?.UserName == null)
            return new AuthenticationState(new ClaimsPrincipal());

        var claims = new[] { new Claim(ClaimTypes.Name, currentUser.UserName) };
        var identity = new ClaimsIdentity(claims, "jwt-auth");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(LoginModel login)
    {
        var result = await _http.PostAsJsonAsync(ApiRoutes.AuthLogin, login);
        if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            throw new Exception(await result.Content.ReadAsStringAsync());

        result.EnsureSuccessStatusCode();

        var model = await result.Content.ReadFromJsonAsync<AuthTokenModel>();
        await _localStorage.SetItemAsync(currentUserKey, model);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(currentUserKey);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}