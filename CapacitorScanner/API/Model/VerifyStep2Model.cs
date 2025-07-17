using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.API.Model
{
    //{"success":true,"data":[{"activity":null,"status":"fail, Out Process"}]}
    public class VerifyStep2Model
    {
        public bool success { get; set; } = false;
        public VerifyStep2Data[] data { get; set; } = [];
        public record VerifyStep2Data(string? activity,string status);
        public VerifyStep2Model() { }
        public VerifyStep2Model(bool success,VerifyStep2Data[] data)
        {
            this.success = success;
            this.data = data;
        }
    }
}
