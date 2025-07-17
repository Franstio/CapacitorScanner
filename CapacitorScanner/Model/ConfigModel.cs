using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Model
{
    public record ConfigModel(string API_URL,string loginEndpoint,string stationActivity,string view,string verifystep2,string hostname,string dbpath);
}
