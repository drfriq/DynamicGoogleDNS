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
            string newip = grabCheckUrl(url);
            if(!string.IsNullOrEmpty(newip))
            {
                ip = newip;
            }
        }

        public string grabCheckUrl(string url)
        {
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            return "";
        }
    }
}
