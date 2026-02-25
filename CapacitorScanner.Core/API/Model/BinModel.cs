using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.API.Model
{
    public class BinModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; } = decimal.Zero;
        public decimal PrevWeight { get; set; } = decimal.Zero;
        public decimal MaxWeight { get; set; } = decimal.Zero; 
        public string Instruction { get; set; } = string.Empty;


        public decimal GetWeightPercentage()
        {
            return Weight / (MaxWeight * 0.01M);
        }
    }
}
