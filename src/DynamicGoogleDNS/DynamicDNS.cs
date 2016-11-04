using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DynamicGoogleDNS
{
    public class DynamicDNS
    {
        DynamicConfig config { get; set; }
        HttpClient client { get; set; }
        ILogger logger { get; set; }

        public DynamicDNS()
        {
            client = new HttpClient();

            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole();
            logger = loggerFactory.CreateLogger<DynamicDNS>();

            var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config = DynamicConfigFactory.buildConfig(builder.Build());
        }

        public void update()
        {
            PriorIP myip = new PriorIP();
            CheckIP check = new CheckIP("https://domains.google.com/checkip");
            if (check.ip != myip.ip)
            {
                // new ip -- lets update it
                myip.writeIP(check.ip);
                logger.LogInformation("New IP found: " + check.ip);

                var response = client.GetAsync("https://" + config.username + ":" + config.password + "@domains.google.com/nic/update?hostname=" + config.hostname).Result;
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError(response.ReasonPhrase);
                }
                else
                {
                    if (response.Content.ReadAsStringAsync().Result.Contains("good"))
                    {
                        logger.LogInformation("Address recorded with google");
                    }
                    else
                    {
                        logger.LogError(check.ip + " NOT recorded with google");
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
