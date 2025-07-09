using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.API.Model
{
    public record PayloadModel<T>(bool success, List<T> data,string result);
}
