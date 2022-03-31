namespace Destuff.Server.Services;

public static class UtilityHelper
{
    public static string GetStoragePath(this IConfiguration configuration)
    {
        string path;
        if (configuration["DOTNET_RUNNING_IN_CONTAINER"] == "true")
        {
            path = "/config";
        }
        else
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Join(folderPath, "Destuff");
        }

        Directory.CreateDirectory(path);
        return path;
    }
}