using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Destuff.Server.Models;

namespace Destuff.Server.Services;

public static class UtilityHelper
{
    public static AppSettings GetAppSettings(this WebApplicationBuilder builder)
    {
        // Configure strongly typed settings objects
        
        var configuration = builder.Configuration;
        var appSettingsSection = configuration.GetSection(nameof(AppSettings));
        builder.Services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        if (appSettings == null)
            throw new Exception("AppSettings is missing.");

        return appSettings;
    }

    public static string GetConfigPath(this IConfiguration configuration) => GetPath(configuration, "config");

    public static string GetDataPath(this IConfiguration configuration) => GetPath(configuration, "data");

    private static string GetPath(this IConfiguration configuration, string pathName)
    {
        string path;
        if (configuration["DOTNET_RUNNING_IN_CONTAINER"] == "true")
        {
            path = $"/{pathName}";
        }
        else
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Join(folderPath, "Destuff", pathName);
        }

        Directory.CreateDirectory(path);
        return path;
    }

    public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, string secret)
    {
        var key = Encoding.ASCII.GetBytes(secret);
        return services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }
}