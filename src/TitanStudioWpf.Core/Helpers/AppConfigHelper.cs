using Microsoft.Extensions.Configuration;
using System.IO;

namespace TitanStudioWpf.Core.Helpers;

public static class AppConfigHelper
{
    public static IConfigurationRoot ReadConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string GetAppNameFromConfig()
    {
        var config = ReadConfig();
        return config["AppSettings:AppName"] ?? "Unknown App";
    }

    public static string GetAppVersionFromConfig()
    {
        var config = ReadConfig();
        return config["AppSettings:AppVersion"] ?? "Unknown Version";
    }
}
