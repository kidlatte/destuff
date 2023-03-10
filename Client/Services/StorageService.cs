
using Blazored.LocalStorage;
using Destuff.Shared.Models;

namespace Destuff.Client.Services;

public interface IStorageService 
{
    ValueTask<AuthModel> GetAuth();
    ValueTask SetAuth(AuthModel model);
    ValueTask ClearAuth();
    ValueTask SetDarkTheme(bool value);
    ValueTask<bool> GetDarkTheme();
}

public class StorageService : IStorageService
{
    public ILocalStorageService LocalStorage { get; }

    public StorageService(ILocalStorageService localStorage)
    {
        LocalStorage = localStorage;
    }

    private readonly string currentUserKey = "current-user";
    public ValueTask ClearAuth() => LocalStorage.RemoveItemAsync(currentUserKey);
    public ValueTask<AuthModel> GetAuth() => LocalStorage.GetItemAsync<AuthModel>(currentUserKey);
    public ValueTask SetAuth(AuthModel model) => LocalStorage.SetItemAsync(currentUserKey, model);

    private readonly string darkThemeKey = "dark-theme";
    public ValueTask<bool> GetDarkTheme() => LocalStorage.GetItemAsync<bool>(darkThemeKey);
    public ValueTask SetDarkTheme(bool value) => LocalStorage.SetItemAsync(darkThemeKey, value);

}