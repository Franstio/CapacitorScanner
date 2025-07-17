using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CapacitorScanner.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CapacitorScanner.Controls;

public partial class LoginControl : UserControl
{
    public LoginControl()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<LoginViewModel>();
    }
}