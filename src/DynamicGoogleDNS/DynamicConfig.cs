using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicGoogleDNS
{
    public class DynamicConfig
    {
        public string username { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public string logdirectory { get; set; }
    }

    public class DynamicConfigFactory
    {
        public static DynamicConfig buildConfig(IConfigurationRoot Configuration)
        {
            DynamicConfig config = new DynamicConfig();
            var configurationSection = Configuration.GetSection("AppSettings");
            config.username = configurationSection.GetValue<string>("username");
            config.password = configurationSection.GetValue<string>("password");
            config.hostname = configurationSection.GetValue<string>("hostname");
            config.logdirectory = configurationSection.GetValue<string>("logdirectory");
            return config;
        }
    }
}
