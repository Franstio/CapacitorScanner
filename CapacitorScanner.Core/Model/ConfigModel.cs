using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model
{
    public class ConfigModel
    {
        public string API_URL { get; set; } = string.Empty;
        public string loginEndpoint { get; set; } = string.Empty;
        public string stationActivity { get; set; } = string.Empty;
        public string view { get; set; } = string.Empty;
        public string verifystep2 { get; set; } = string.Empty;
        public string hostname { get; set; } = null!;
       public string dbpath { get; set; } = string.Empty;

        public ConfigModel() 
        {
            hostname = Dns.GetHostName();
        }

        public ConfigModel(string aPI_URL, string loginEndpoint, string stationActivity, string view, string verifystep2, string hostname, string dbpath)
        {
            API_URL = aPI_URL;
            this.loginEndpoint = loginEndpoint;
            this.stationActivity = stationActivity;
            this.view = view;
            this.verifystep2 = verifystep2;
            this.hostname = string.IsNullOrEmpty(hostname) ? Dns.GetHostName() : hostname;
            this.dbpath = dbpath;
        }
    }
}
