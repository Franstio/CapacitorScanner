using Avalonia.Controls;
using CapacitorScanner.Core.Model;
using CapacitorScanner.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;

namespace CapacitorScanner.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<MainWindow, LoginMessage>(this,  (w, m) =>
        {
            // Create an instance of MusicStoreWindow and set MusicStoreViewModel as its DataContext.
            if (m.IsClosing)
                return;
            var dialog = new LoginWindow();
            // Show dialog window and reply with returned AlbumViewModel or null when the dialog is closed.
            m.Reply(dialog.ShowDialog<LoginModel?>(w));
        });
    }
}
