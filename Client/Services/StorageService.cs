
using Blazored.LocalStorage;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public interface IStorageService 
{
    ValueTask<AuthModel> GetUserAsync();
    ValueTask SetUserAsync(AuthModel model);
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
    public ValueTask<AuthModel> GetUserAsync() => _localStorage.GetItemAsync<AuthModel>(currentUserKey);
    public ValueTask SetUserAsync(AuthModel model) => _localStorage.SetItemAsync(currentUserKey, model);
}