using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FarmRecordManagementSystem.Utilities
{
    public static class ConfigurationExtensions
    {
        public static void SaveJsonToFile(this IConfiguration configuration, string filePath)
        {
            var json = configuration.ToJsonString();

            File.WriteAllText(filePath, json);
        }

        public static string ToJsonString(this IConfiguration configuration)
        {
            var json = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            return json;
        }
    }
}
