using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CapacitorScanner.Services;
using CapacitorScanner.ViewModels;
using CapacitorScanner.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CapacitorScanner;
public static class ServiceCollectionExtension
{
    public static  void AddCommonServices(this IServiceCollection services)
    {
        Type[] viewmodels = [typeof(MainViewModel), typeof(WasteControlViewModel),typeof(BinControlViewModel),typeof(SettingsViewModel),typeof(LoginViewModel)];
        foreach (var viewmodel in viewmodels)
            services.AddSingleton(viewmodel);
        Type[] service = [typeof(PIDSGService),typeof(SQLiteService)];
        foreach (var viewmodel in service)
            services.AddScoped(viewmodel);
        ConfigService configService = new ConfigService();
         configService.LoadAsync();
        services.AddSingleton(configService);
        services.AddSingleton<AppState>();
    }
}

public partial class App : Application
{
    public static IServiceProvider Services { get;private set; } = null!;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public override  void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);
        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddCommonServices();
        Services = serviceCollection.BuildServiceProvider();
        _ = Task.Run(async ()=>await Services.GetRequiredService<SQLiteService>().Initialization());
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
