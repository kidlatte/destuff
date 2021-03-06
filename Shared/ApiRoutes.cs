namespace Destuff.Shared;

public class ApiRoutes
{
    public const string Users = $"/api/users";
    public const string Auth = "/api/auth";
    public const string AuthLogin = $"{Auth}/login";
    public const string AuthRegister = $"{Auth}/register";
    public const string AuthChangePassword = $"{Auth}/change-password";

    public const string Locations = $"/api/locations";
    public const string LocationSlug = $"{Locations}/s";
    public const string LocationTree = $"{Locations}/t";
}