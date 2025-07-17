using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.API.Model
{
    public class BinActivityModel
    {
        public record BinActivityPayloadResult(int activity, string status);
        public record BinActivityPayloadData(string openbinname);
        public record BinActivityPayload(bool success, BinActivityPayloadResult[] result,BinActivityPayloadData[] data);
        public BinActivityModel() { }
        public int activity { get; set; } 
        public string status { get; set; } = string.Empty;
        public string openbinname { get;set; } = string.Empty;
        public BinActivityModel(BinActivityPayload payload)
        {
            if (payload.result.Length > 0)
            {
                activity = payload.result[0].activity;
                status = payload.result[0].status;
            }
            if (payload.data.Length > 0)
                openbinname= payload.data[0].openbinname;
        }
    }
}
