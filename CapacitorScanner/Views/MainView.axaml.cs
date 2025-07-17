using Avalonia.Controls;
using CapacitorScanner.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CapacitorScanner.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}
