using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.FromBin
{
    public class UpdateBinRequestModel
    {
        public string BinName { get; set; } = string.Empty;
        public decimal Weight { get; set; } = decimal.Zero; 
        public string Hostname { get; set; } = string.Empty;    
    }
}
