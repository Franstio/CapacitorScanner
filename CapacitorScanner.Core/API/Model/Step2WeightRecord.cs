using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.API.Model
{
    public class Step2WeightRecord
    {
        public string BinName { get; set; } = string.Empty; 
        public decimal Weight { get; set; } = decimal.Zero;
        public string WeightDate { get; set; } = string.Empty;
        public string Result { get; set; }  = string.Empty;
    }
}
