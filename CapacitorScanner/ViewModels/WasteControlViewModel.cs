using Avalonia.Data;
using CapacitorScanner.API.Model;
using CapacitorScanner.Messages;
using CapacitorScanner.Model;
using CapacitorScanner.Services;
using CapacitorScanner.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
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
        private enum TransactionType
        {
            Dispose,
            Collection,
            Manual
        }

        private TransactionType? transactionType = null;

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
        
        private async Task ShowMessage(string message)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
    {
        ContentTitle = "Info",
        ContentMessage = "This works cross-platform!",
        ButtonDefinitions = ButtonEnum.OkCancel,
        Icon = Icon.Info
    });

            await messageBoxStandardWindow.ShowAsync();
        }

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
        void ResetStateInput(string message = "Scan Badge ID")
        {
            Message = message;
            User = null;
            Container = null;
            OpenBin = null;
            transactionType = null;
        }

        public async Task<ContainerBinModel?> LoadContainerBin(string binName)
        {
            var data = await Service.GetBins(binName);
            return data?.FirstOrDefault();
        }
        async Task HandleLogin()
        {
            User = await Service.LoginUser(Scan);
            loginDate = DateTime.Now;
            if (User is not null)
                Message = "Scan QR Code Sampah";
        }
        async Task ContainerScan()
        {
            Container = await LoadContainerBin(Scan);
            if (Container is null)
            {
                await MessageBoxManager.GetMessageBoxStandard("Scan Failed","Container Not Found").ShowAsync();
                return;
            }
            if (IsAuto)
                await ContainerAuto();
            else
                await ContainerManual();
        }
        async Task ContainerAuto()
        {
            if (User is null)
                throw new Exception("User haven't login yet");
            int[] activity = [1, 2];

            OpenBin = await Service.AutoProcessBinActivity(User.badgeno, Scan);
            if (OpenBin is null) return;

            if (activity.Contains(OpenBin.activity))
            {
                transactionType = OpenBin.activity == 1 ? TransactionType.Dispose : TransactionType.Collection;
                Message = $"Verification {transactionType.ToString()}\nScan QR Code Container bin";
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandard("Bin Error", OpenBin?.activity == 0 ? $"Bin Overload" : OpenBin?.status).ShowAsync();
                ResetStateInput();
            }

        }
        Task ContainerManual()
        {

            Message = "Waste process\n Pilih Waste bin";
            transactionType = TransactionType.Manual;
            return Task.CompletedTask;
        }
        async Task Verification()
        {
            if (User is null || OpenBin is null)
                throw new Exception("Invalid Input");
            if (OpenBin.openbinname == Scan && transactionType.HasValue && transactionType.Value != TransactionType.Manual)
            {
                
                var res = await Service.AutoProcessBinActivity(User.badgeno, Scan);
                if (res?.status != "PASS")
                    await MessageBoxManager.GetMessageBoxStandard("Verification", "ACCESS DENY").ShowAsync();
                else
                    await MessageBoxManager.GetMessageBoxStandard("Verification", OpenBin.activity == 1 ? "Verification Waste process" : "Verification Dispose process").ShowAsync();
                ResetStateInput();
            }
            else if (transactionType.HasValue && transactionType.Value != TransactionType.Manual)
                await MessageBoxManager.GetMessageBoxStandard("Verification Failed", "Wrong Container Bin").ShowAsync();
            await LoadBins();
        }
        [RelayCommand]
        public async Task WasteProcess()
        {
            await ShowMessage("test");
            if (string.IsNullOrEmpty(Scan))
                return;
            if (User is null)
                await HandleLogin();
            else if (Container is null)
                await ContainerScan();
            else if (OpenBin is not null)
                await Verification();
            Scan = string.Empty;
        }

        [RelayCommand]
        public async Task TriggerTransaction(BinModel? Bin =null)
        {
            if (Bin is null || Container is null || User is null)
                return;
            var res = await Service.VerifyStep2(User.badgeno, Container.name, Bin.Name);
            ScrapTransaction transaction = new ScrapTransaction(-1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loginDate.ToString("yyyy-MM-dd HH:mm:ss"),
                User.badgeno, Container.name, Bin.Name, "ONLINE", ConfigService.Config.hostname, (double)Container.weightresult, 0, res?.data[0].activity ?? "None", Container.scrapitem_name, Container.scraptype_name, Container.scrapgroup_name, User.badgeno);
            await DbService.CreateTransaction(transaction);
            string message = res?.data[0].status ?? "Error";
            await MessageBoxManager.GetMessageBoxStandard("Result", message).ShowAsync();
            ResetStateInput();
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