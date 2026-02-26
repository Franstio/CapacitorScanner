using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.LocalDb
{
    public class Step2WeightModel
    {
        public string BinName { get; set; } = string.Empty; 
        public decimal Weight { get; set; } = decimal.Zero;
        public string WeightDate { get; set; } = string.Empty;
        public string Result { get; set; }  = string.Empty;
    }
}
