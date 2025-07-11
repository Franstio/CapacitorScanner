using Avalonia;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CapacitorScanner.Model;
using CapacitorScanner.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CapacitorScanner.Controls;

public partial class BinControl : UserControl
{
    public static readonly StyledProperty<BinModel> BinProperty = AvaloniaProperty.Register<BinControl, BinModel>("Bin", defaultValue: new BinModel());


    public BinModel Bin
    {
        get => GetValue(BinProperty);
        set => SetValue(BinProperty, value);
        
    }

    public BinControl()
    {
        InitializeComponent();
        DataContext = this;
    }
}