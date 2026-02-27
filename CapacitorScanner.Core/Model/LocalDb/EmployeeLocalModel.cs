using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.LocalDb
{
    public class EmployeeLocalModel
    {
        public int id { get; set; } 
        public string employeename { get; set; } = string.Empty;
        public string badgeno { get; set; } = string.Empty; 
        public string registerdate { get; set;}    = string.Empty;
    }
}
