
using Blazored.LocalStorage;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public interface IStorageService 
{
    ValueTask<AuthTokenModel> GetUserAsync();
    ValueTask SetUserAsync(AuthTokenModel model);
    ValueTask ClearUserAsync();
}

public class StorageService : IStorageService
{
    public ILocalStorageService _localStorage { get; }

    public StorageService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    private readonly string currentUserKey = "current-user";
    public ValueTask ClearUserAsync() => _localStorage.RemoveItemAsync(currentUserKey);
    public ValueTask<AuthTokenModel> GetUserAsync() => _localStorage.GetItemAsync<AuthTokenModel>(currentUserKey);
    public ValueTask SetUserAsync(AuthTokenModel model) => _localStorage.SetItemAsync(currentUserKey, model);
}