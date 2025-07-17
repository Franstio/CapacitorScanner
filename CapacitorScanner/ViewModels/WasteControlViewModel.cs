using CapacitorScanner.API.Model;
using CapacitorScanner.Messages;
using CapacitorScanner.Model;
using CapacitorScanner.Services;
using CapacitorScanner.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace CapacitorScanner.ViewModels
{
    public partial class WasteControlViewModel: ViewModelBase
    {
        private readonly PIDSGService Service;
        private readonly ConfigService ConfigService;   
        private readonly SQLiteService DbService;
        [ObservableProperty]
        private UserModel? user = null;

        [ObservableProperty]
        private string scan = string.Empty;

        [ObservableProperty]
        private BinActivityModel? openBin = null;

        [ObservableProperty]
        private ContainerBinModel? container = null;
        [ObservableProperty]
        private bool isAuto = false;
        [ObservableProperty]
        private string message= "Scan Badge ID";
        private DateTime loginDate = DateTime.Now;
        public WasteControlViewModel(PIDSGService service,SQLiteService _db,ConfigService config) 
        {
            Service = service;
            DbService = _db;
            ConfigService = config;
        }

        public ObservableCollection<BinModel> Bins { get; set; } = [new BinModel("test","test",1,23)];
        [RelayCommand]
        public async Task LoadBins()
        {
            var data = await Service.GetBins();
            if (data is null)
                return;
            Bins.Clear();
            var _data = data.Select(x => new BinModel(x.name, x.scraptype_name, x.weightresult, x.capacity)).OrderBy(x => x.Name);
            foreach (var item in _data)
                Bins.Add(item);
        }
        public void MarkBin()
        {
            if (Container is null)
                return;
            
        }

        [RelayCommand]
        public async Task WasteProcess()
        {
            if (string.IsNullOrEmpty(Scan))
                return;
            if (User is null)
            {
                User = await Service.LoginUser(Scan);
                loginDate = DateTime.Now;
                if (User is not null)
                    Message = "Scan QR Code Sampah";
            }
            else if (Container is null)
            {
                await LoadContainerBin(Scan);
                if (Container is not null)
                {
                    if (IsAuto)
                    {
                        OpenBin = await Service.AutoProcessBinActivity(User.badgeno, Scan);
                        if (OpenBin?.activity == 1)
                            Message = "Verification Dispose\nScan QR Code Container bin";
                        else if (OpenBin?.activity == 2)
                            Message = "Verifcation Collection\nScan QR Code Container bin";
                        else
                        {
                            if (OpenBin?.activity == 0)
                                await MessageBoxManager.GetMessageBoxStandard("Bin Error", $"Bin Overload").ShowAsync();
                            else
                                await MessageBoxManager.GetMessageBoxStandard("Bin Error", OpenBin?.status).ShowAsync();
                            Message = "Scan Badge ID";
                            User = null;
                            Container = null;
                            OpenBin = null;
                        }
                    }
                    else
                        Message = "Waste process\n Pilih Waste bin";
                }
            }
            else if (OpenBin is not null)
            {
                if (OpenBin.openbinname == Scan)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Verification", OpenBin.activity == 1 ? "Verification Waste process" : "Verification Dispose process").ShowAsync();
                    Message = "Scan Badge ID";
                    User = null;
                    Container = null;
                    OpenBin = null;
                    await LoadBins();
                }
                else if (Scan[0] == '1')
                {
                    var res=  await Service.AutoProcessBinActivity(User.badgeno, Scan);
                    if (res.status != "PASS")
                        await MessageBoxManager.GetMessageBoxStandard("Verification", "ACCESS DENY").ShowAsync();
                }
                else
                    await MessageBoxManager.GetMessageBoxStandard("Verification Failed", "Wrong Container Bin").ShowAsync();
            }
            Scan = string.Empty;
        }
        public async Task LoadContainerBin(string binName)
        {
            var data = await Service.GetBins(binName);
            Container = data?.FirstOrDefault();
        }

        [RelayCommand]
        public async Task TestClick(BinModel? Bin =null)
        {
            if (Bin is null || Container is null || User is null)
                return;
            var res = await Service.VerifyStep2(User.badgeno, Container.name, Bin.Name);
            ScrapTransaction transaction = new ScrapTransaction(-1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loginDate.ToString("yyyy-MM-dd HH:mm:ss"),
                User.badgeno, Container.name, Bin.Name, "ONLINE", ConfigService.Config.hostname, (double)Container.weightresult, 0, res?.data[0].activity ?? "None", Container.scrapitem_name, Container.scraptype_name, Container.scrapgroup_name, User.badgeno);
            await DbService.CreateTransaction(transaction);
            string message = res?.data[0].status ?? "Error";
            await MessageBoxManager.GetMessageBoxStandard("Result", message).ShowAsync();
            User = null;
            Container = null;
            OpenBin = null;
            Message = "Scan Badge ID";
            await LoadBins();
        }

        [RelayCommand]
        public  async Task ToggleMode()
        {
            var res=  await WeakReferenceMessenger.Default.Send(new LoginMessage());
            if (res is not null)
                IsAuto = !IsAuto;
        }
    }
}