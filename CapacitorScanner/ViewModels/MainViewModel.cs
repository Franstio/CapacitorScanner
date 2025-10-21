using Avalonia.Threading;
using CapacitorScanner.API.Model;
using CapacitorScanner.Model;
using CapacitorScanner.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace CapacitorScanner.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string stationName  = "Station - Type";
    [ObservableProperty]
    private string machineName = "NAME";

    private readonly PIDSGService PIDSGService;
    private readonly ConfigService ConfigService;
    private readonly DialogService dialogService;
    [ObservableProperty]
    private string time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    public MainViewModel(PIDSGService service,ConfigService configService,DialogService dialogService)
    {
        PIDSGService = service;
        ConfigService = configService;
        this.dialogService = dialogService;

    }
    [RelayCommand]
    public async Task Exit()
    {
        var res = await dialogService.ShowConfirmAsync("Konfirmasi Keluar", "Apakah anda yakin untuk keluar dan melakukan proses rebooting?");
        if (res)   
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "reboot" });
    }
    [RelayCommand]
    public async Task LoadStation()
    {
        StationName = (await PIDSGService.GetStationInfo())?.FirstOrDefault()?.description ?? StationName;
        MachineName = ConfigService.Config.hostname;
        DispatcherTimer timer = new DispatcherTimer(),timer2 = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (_, _) => {

            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            };
        timer.Start();
        timer2.Interval = TimeSpan.FromSeconds(5);
        timer2.Tick += async (_, _) => {
            StationName = (await PIDSGService.GetStationInfo())?.FirstOrDefault()?.description ?? StationName;
            MachineName = ConfigService.Config.hostname;
        };
        timer2.Start();
    }


}
