using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.LocalDb
{
    public class BinLocalModel
    {
        public string bin { get; set; } = string.Empty;
        public decimal weight { get; set; } = decimal.Zero;
        public decimal maxweight { get; set; } = decimal.Zero;
        public string hostname { get; set; } = string.Empty;
        public decimal binweight { get; set; } = decimal.Zero;
        public decimal weightsystem { get; set; } = decimal.Zero;
        public string lastfrombinname { get; set; } = string.Empty;
        public string lastbadgeno { get; set; } = string.Empty;
        public string wastetype { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;  
    }
}
