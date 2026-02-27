using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.LocalDb
{

    public class ContainerBinLocalModel
    {
        public string name { get; set; } = default!;
        public string description { get; set; } = default!;
        public string scrapitem_name { get; set; } = default!;
        public string scraptype_name { get; set; } = default!;
        public decimal weight { get; set; }
        public decimal capacity { get; set; }
        public decimal weightresult { get; set; }
        public decimal weightsystem { get; set; }
        public string wastestation_name { get; set; } = default!;
        public string department_name { get; set; } = default!;
        public DateTime? logindate { get; set; }
        public int? doorstatus { get; set; }
        public string lastfrombinname { get; set; } = default!;
        public string url { get; set; } = default!;
        public string scrapgroup_name { get; set; } = default!;
        public string lastbadgeno { get; set; } = default!;
        public string activity { get; set; } = string.Empty;

        public ContainerBinLocalModel()
        {
        }

        public ContainerBinLocalModel(
            string name,
            string description,
            string scrapitem_name,
            string scraptype_name,
            decimal weight,
            decimal capacity,
            decimal weightresult,
            decimal weightsystem,
            string wastestation_name,
            string department_name,
            DateTime? logindate,
            int? doorstatus,
            string lastfrombinname,
            string url,
            string scrapgroup_name,
            string lastbadgeno)
        {
            this.name = name;
            this.description = description;
            this.scrapitem_name = scrapitem_name;
            this.scraptype_name = scraptype_name;
            this.weight = weight;
            this.capacity = capacity;
            this.weightresult = weightresult;
            this.weightsystem = weightsystem;
            this.wastestation_name = wastestation_name;
            this.department_name = department_name;
            this.logindate = logindate;
            this.doorstatus = doorstatus;
            this.lastfrombinname = lastfrombinname;
            this.url = url;
            this.scrapgroup_name = scrapgroup_name;
            this.lastbadgeno = lastbadgeno;
        }
    }
}
