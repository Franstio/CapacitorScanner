using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapacitorScanner.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
namespace CapacitorScanner.ViewModels
{
    public partial class BinControlViewModel : ViewModelBase
    {
        [ObservableProperty]
        BinModel bin = new BinModel();
        public BinControlViewModel()
        {
            
        }

        [RelayCommand]
        async void Click()
        {
            await MessageBoxManager.GetMessageBoxStandard("Test", "test").ShowAsync();
        }
        
    }
}
