using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public class AuthenticationService : AuthenticationStateProvider
{
    IHttpService Http { get; }
    IStorageService Storage { get; }

    public AuthenticationService(IHttpService http, IStorageService storage)
    {
        Http = http;
        Storage = storage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var auth = await Storage.GetAuth();

        if (auth?.UserName == null || auth.Expires < DateTime.UtcNow)
            return new AuthenticationState(new ClaimsPrincipal());

        var claims = new[] { new Claim(ClaimTypes.Name, auth.UserName) };
        var identity = new ClaimsIdentity(claims, "jwt-auth");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(LoginRequest login)
    {
        var model = await Http.PostAsync<AuthModel>(ApiRoutes.AuthLogin, login) 
            ?? throw new Exception("No auth token.");
        await Storage.SetAuth(model);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await Storage.ClearAuth();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}