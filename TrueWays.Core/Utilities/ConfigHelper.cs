using System.Configuration;

namespace TrueWays.Core.Utilities
{
    public static class ConfigHelper
    {
        public static string GetAppSettingString(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }

        public static string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString ?? string.Empty;
        }
    }
}
