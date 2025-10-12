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
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CapacitorScanner.Controls;

public partial class WasteControl : UserControl
{
    private async Task ShowMessage(string message)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        {
            ContentTitle = "Info",
            ContentMessage = "This works cross-platform!",
            ButtonDefinitions = ButtonEnum.OkCancel,
            Icon = Icon.Info
        });


        await messageBoxStandardWindow.ShowWindowAsync();
    }

    public WasteControl()
    {
        InitializeComponent();
        ShowMessage("Test1").RunSynchronously();
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