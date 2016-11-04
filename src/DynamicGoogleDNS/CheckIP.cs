using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DynamicGoogleDNS
{
    public class CheckIP
    {
        public string ip { get; set; }
        public HttpClient client;

        public CheckIP(string url)
        {
            client = new HttpClient();
            ip = grabCheckUrl(url);
        }

        public string grabCheckUrl(string url)
        {
            var response = client.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
