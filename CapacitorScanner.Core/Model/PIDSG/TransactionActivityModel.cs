using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.PIDSG
{
    public class TransactionActivityModel
    {
        [JsonPropertyName("badgeno")]
        public string? BadgeNo { get; set; }

        [JsonPropertyName("logindate")]
        public string? LoginDate { get; set; }

        [JsonPropertyName("stationname")]
        public string? StationName { get; set; }

        [JsonPropertyName("frombinname")]
        public string? FromBinName { get; set; }

        [JsonPropertyName("tobinname")]
        public string? ToBinName { get; set; }

        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        [JsonPropertyName("activity")]
        public string? Activity { get; set; }
    }
}
