using Microsoft.Extensions.Configuration;
using System.IO;

namespace BandAPI.Common
{
    public class ConfigCollection
    {
        private readonly IConfigurationRoot configuration;
        public static ConfigCollection Instance { get; } = new ConfigCollection();

        protected ConfigCollection()
        {
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                                                             .AddJsonFile($"appsettings.Production.json",
                                                             optional: true, reloadOnChange: false)
                                                             .Build();
        }

        public IConfigurationRoot GetConfiguration()
        {
            return configuration;
        }
    }
}