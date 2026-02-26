using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.API.Model
{
    public record ContainerBinModel(string name,string description,string scrapitem_name,string scraptype_name,decimal weight,decimal capacity,decimal weightresult,decimal weightsystem,
        string wastestation_name,string department_name,DateTime? logindate,int? doorstatus,string lastfrombinname,string url,string scrapgroup_name,string lastbadgeno);
}
