using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    IHttpService _http { get; }
    IStorageService _storage { get; }

    public CustomAuthenticationStateProvider(IHttpService http, IStorageService storage)
    {
        _http = http;
        _storage = storage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var currentUser = await _storage.GetUserAsync();
        if (currentUser?.UserName == null)
            return new AuthenticationState(new ClaimsPrincipal());

        var claims = new[] { new Claim(ClaimTypes.Name, currentUser.UserName) };
        var identity = new ClaimsIdentity(claims, "jwt-auth");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(LoginModel login)
    {
        var model = await _http.PostAsync<AuthTokenModel>(ApiRoutes.AuthLogin, login);
        if (model == null)
            throw new Exception("No auth token.");
        
        await _storage.SetUserAsync(model);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await _storage.ClearUserAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}