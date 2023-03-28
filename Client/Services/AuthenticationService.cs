using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public class AuthenticationService : AuthenticationStateProvider
{
    IHttpService _http { get; }
    IStorageService _storage { get; }

    public AuthenticationService(IHttpService http, IStorageService storage)
    {
        _http = http;
        _storage = storage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var auth = await _storage.GetAuth();

        if (auth?.UserName == null || auth.Expires < DateTime.UtcNow)
            return new AuthenticationState(new ClaimsPrincipal());

        var claims = new[] { new Claim(ClaimTypes.Name, auth.UserName) };
        var identity = new ClaimsIdentity(claims, "jwt-auth");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(LoginRequest login)
    {
        var model = await _http.PostAsync<AuthModel>(ApiRoutes.AuthLogin, login);
        if (model == null)
            throw new Exception("No auth token.");
        
        await _storage.SetAuth(model);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await _storage.ClearAuth();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}