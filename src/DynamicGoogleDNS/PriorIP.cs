using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace DynamicGoogleDNS
{
    public class PriorIP
    {
        public string ip { get; set; }

        public PriorIP()
        {
            if(File.Exists(AppContext.BaseDirectory + "\\myip"))
            {
                ip = File.ReadAllText(AppContext.BaseDirectory + "\\myip");
            }
        }

        public void writeIP(string newip)
        {
            ip = newip;
            File.WriteAllText(AppContext.BaseDirectory + "\\mpip", ip);
        }
    }
}
