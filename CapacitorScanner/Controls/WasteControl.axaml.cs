using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CapacitorScanner.Messages;
using CapacitorScanner.ViewModels;
using CapacitorScanner.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CapacitorScanner.Controls;

public partial class WasteControl : UserControl
{

    public WasteControl()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<WasteControlViewModel>();
        Dispatcher.UIThread.Post(() =>
        {

            ScanBox.Focus();
        }, DispatcherPriority.Background);
        WeakReferenceMessenger.Default.Register<WasteControl, LoginMessage>(this, (w, m) =>
        {
            if (m.IsClosing)
                ScanBox.LostFocus += ScanBox_LostFocus;
            else
                ScanBox.LostFocus -= ScanBox_LostFocus;
        });
        ScanBox.LostFocus += ScanBox_LostFocus;
    }

    private async void ScanBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        await Task.Delay(500);
        Dispatcher.UIThread.Post(() =>
        {

            ScanBox.Focus();
        }, DispatcherPriority.Background);
    }
}