using System;
using System.Collections.Generic;
using System.Linq;
using CapacitorScanner.Core.API.Model;
using CapacitorScanner.Core.Model.LocalDb;

namespace CapacitorScanner.Core.Services
{
    public static class ContainerBinMapper
    {
        public static ContainerBinModel ToApiModel(this ContainerBinLocalModel local)
        {
            if (local is null) throw new ArgumentNullException(nameof(local));

            return new ContainerBinModel(
                local.name,
                local.description,
                local.scrapitem_name,
                local.scraptype_name,
                local.weight,
                local.capacity,
                local.weightresult,
                local.weightsystem,
                local.wastestation_name,
                local.department_name,
                local.logindate,
                local.doorstatus,
                local.lastfrombinname,
                local.url,
                local.scrapgroup_name,
                local.lastbadgeno
            );
        }

        public static ContainerBinLocalModel ToLocalModel(this ContainerBinModel api)
        {
            if (api is null) throw new ArgumentNullException(nameof(api));

            return new ContainerBinLocalModel(
                api.name,
                api.description,
                api.scrapitem_name,
                api.scraptype_name,
                api.weight,
                api.capacity,
                api.weightresult,
                api.weightsystem,
                api.wastestation_name,
                api.department_name,
                api.logindate,
                api.doorstatus,
                api.lastfrombinname,
                api.url,
                api.scrapgroup_name,
                api.lastbadgeno
            );
        }

        public static IEnumerable<ContainerBinModel> ToApiModels(this IEnumerable<ContainerBinLocalModel> locals)
        {
            if (locals is null) throw new ArgumentNullException(nameof(locals));
            return locals.Select(x => x.ToApiModel());
        }

        public static IEnumerable<ContainerBinLocalModel> ToLocalModels(this IEnumerable<ContainerBinModel> apis)
        {
            if (apis is null) throw new ArgumentNullException(nameof(apis));
            return apis.Select(x => x.ToLocalModel());
        }
    }
}
