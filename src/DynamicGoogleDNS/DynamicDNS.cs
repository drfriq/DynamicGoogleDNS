using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using Serilog;

namespace DynamicGoogleDNS
{
    public class DynamicDNS
    {
        DynamicConfig config { get; set; }
        HttpClient client { get; set; }
        ILogger logger { get; set; }

        public DynamicDNS()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json");
            config = DynamicConfigFactory.buildConfig(builder.Build());

            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.File(config.logdirectory + "\\DynamicGoogleDns.log")
                .CreateLogger();
            
            client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Chrome/41.0");
        }

        public void update()
        {
            PriorIP myip = new PriorIP();
            CheckIP check = new CheckIP("https://domains.google.com/checkip");
            if(string.IsNullOrEmpty(check.ip))
            {
                logger.Error("checkip failed");
                return;
            }

            if (check.ip != myip.ip)
            {
                // new ip -- lets update it
                myip.writeIP(check.ip);
                logger.Information("New IP found: " + check.ip);

                string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes(config.username + ":" + config.password));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + authInfo);

                var NicUri = new Uri("https://domains.google.com/nic/update?hostname=" + config.hostname);
                var response = client.GetAsync(NicUri).Result;
                if (!response.IsSuccessStatusCode)
                {
                    logger.Error(response.ReasonPhrase + " email provided probably wrong");
                }
                else
                {
                    var ResponseCode = response.Content.ReadAsStringAsync().Result.Split(' ')[0];
                    switch (ResponseCode)
                    {
                        case "good":
                            logger.Information(myip.ip + " recorded with google");
                            break;
                        case "nochg":
                            logger.Information("The supplied IP address is already set for this host. You should not attempt another update until your IP address changes.");
                            break;
                        case "nohost":
                            logger.Error("The hostname does not exist, or does not have Dynamic DNS enabled.");
                            break;
                        case "badauth":
                            logger.Error("The username / password combination is not valid for the specified host.");
                            break;
                        case "notfqdn":
                            logger.Error("The supplied hostname is not a valid fully-qualified domain name.");
                            break;
                        case "badagent":
                            logger.Error("Your Dynamic DNS client is making bad requests. Ensure the user agent is set in the request, and that you’re only attempting to set an IPv4 address. IPv6 is not supported.");
                            break;
                        case "abuse":
                            logger.Error("Dynamic DNS access for the hostname has been blocked due to failure to interpret previous responses correctly.");
                            break;
                        case "911":
                            logger.Error("An error happened on our end. Wait 5 minutes and retry.");
                            myip.clear();
                            break;
                        default:
                            logger.Error(ResponseCode);
                            break;
                    }
                }
            }
        }
    }
}
