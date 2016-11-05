using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DynamicGoogleDNS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DynamicDNS dyndns = new DynamicDNS();
            dyndns.update();
        }
    }
}
