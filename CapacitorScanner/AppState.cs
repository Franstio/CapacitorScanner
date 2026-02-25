using CapacitorScanner.Core.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner
{
    public partial class AppState : ObservableObject
    {
        [ObservableProperty]
        private LoginModel? login = null;
    }
}
