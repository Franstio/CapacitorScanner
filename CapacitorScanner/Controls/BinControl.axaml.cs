using Avalonia;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CapacitorScanner.Model;
using CapacitorScanner.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reactive.Linq;

namespace CapacitorScanner.Controls;

public partial class BinControl : UserControl
{
    public static readonly StyledProperty<BinModel> BinProperty = AvaloniaProperty.Register<BinControl, BinModel>("Bin", defaultValue: new BinModel());

    public BinModel Bin
    {
        get => GetValue(BinProperty);
        set => SetValue(BinProperty, value);

    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is not BinControlViewModel)
        {
            BinControlViewModel vm = new BinControlViewModel();
            vm.Bin = Bin;
            DataContext = vm;
        }
    }
    public BinControl()
    {
        InitializeComponent();
    }
}