using CapacitorScanner.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CapacitorScanner.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<BinModel> Bins { get; set; } = [];

    public MainViewModel()
    {
        Bins = new ObservableCollection<BinModel>(new List<BinModel>()
        {
            new BinModel("2-B2-002-01","Non Copper",1.7m,30),
            new BinModel("2-B2-002-02","Non Copper",2.83m,30),
            new BinModel("2-B2-002-03","Non Copper",8.62m,30),
            new BinModel("2-B2-002-04","Non Copper",16.26m,30),
        });
    }

}
