using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CapacitorScanner.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;

namespace CapacitorScanner.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<LoginWindow,LoginMessage>(this, (window, message) =>
        {
            if (message.IsClosing)
                window.Close(message.LoginModel);
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}