using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazored.LocalStorage;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public class CustomStateProvider : AuthenticationStateProvider
{
    public ILocalStorageService _localStorage { get; set; }

    public CustomStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var currentUser = await _localStorage.GetItemAsync<AuthTokenModel>("current-user");
        if (currentUser?.UserName == null)
            return new AuthenticationState(new ClaimsPrincipal());

        var claims = new[] { new Claim(ClaimTypes.Name, currentUser.UserName) };
        var identity = new ClaimsIdentity(claims, "jwt-auth");
        var state = new AuthenticationState(new ClaimsPrincipal(identity));

        Console.WriteLine($">>> GetAuthenticationState: {state.User.Identity!.IsAuthenticated} {state.User.Identity!.AuthenticationType} ");

        return state;
    }
}