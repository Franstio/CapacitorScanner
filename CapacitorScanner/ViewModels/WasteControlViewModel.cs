using Avalonia.Data;
using CapacitorScanner.API.Model;
using CapacitorScanner.Messages;
using CapacitorScanner.Model;
using CapacitorScanner.Model.PIDSG;
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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace CapacitorScanner.ViewModels
{
    public partial class WasteControlViewModel: ViewModelBase
    {
        private readonly PIDSGService Service;
        private readonly ConfigService ConfigService;   
        private readonly SQLiteService DbService;
        private readonly DialogService dialogService;
        private HttpClient httpClient;
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
        private bool isAuto = true;
        
        [ObservableProperty]
        private string message= "Scan Badge ID";
        
        private DateTime loginDate = DateTime.Now;
        
        public WasteControlViewModel(PIDSGService service,SQLiteService _db,ConfigService config,DialogService dialogService,HttpClient client) 
        {
            Service = service;
            DbService = _db; 
            ConfigService = config;
            this.httpClient = client;
            this.dialogService = dialogService;
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
            Console.WriteLine(JsonSerializer.Serialize( data));
            Container = data?.FirstOrDefault();
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
            var container = await LoadContainerBin(Scan);
            if (container is null)
            {
                await dialogService.ShowMessageAsync("Scan Failed","Container Not Found");
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

            var bin = await Service.AutoProcessBinActivity(User.badgeno, Scan);
            OpenBin = bin;
            if (bin is null) return;

            if (activity.Contains(bin.activity))
            {
                transactionType = bin.activity == 1 ? TransactionType.Dispose : TransactionType.Collection;
                Message = $"Verification {transactionType.ToString()}\nScan QR Code Container bin";
            }
            else
            {
                await dialogService.ShowMessageAsync("Bin Error", bin?.activity == 0 ? $"Bin Overload" : bin?.status ?? "");
                ResetStateInput();
            }

        }
        Task ContainerManual()
        {

            Message = "Waste process\n Pilih Waste bin";
            transactionType = TransactionType.Manual;
            return Task.CompletedTask;
        }
        async Task<bool> SendBinVerif(string bin)
        {
            string binhost = await DbService.GetHostname(bin);
            string token = $"root:00000000";
            string base64token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, $"https://{binhost}/verifikasi?verifikasi=1");
            req.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64token}");
            try
            {
                var res = await httpClient.SendAsync(req);
                res.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine   (ex.Message);
                    return false;
            }
        }
        async Task Verification()
        {
            if (User is null || OpenBin is null)
                throw new Exception("Invalid Input");
            if (OpenBin.openbinname == Scan && transactionType.HasValue && transactionType.Value != TransactionType.Manual)
            {
                if (!await SendBinVerif(OpenBin.openbinname))
                {

                    await dialogService.ShowMessageAsync("Transaction Not Finished", "Bin Offline");
                    return;
                }
                else
                {
                    if (transactionType.HasValue && transactionType.Value == TransactionType.Collection)
                        await Collection();
                    await dialogService.ShowMessageAsync("Verification", OpenBin.activity == 1 ? "Verification Waste process" : "Verification Dispose process");
                    ResetStateInput();
                }
            }
            else if (transactionType.HasValue && transactionType.Value != TransactionType.Manual)
                await dialogService.ShowMessageAsync("Verification Failed", "Wrong Container Bin");
            await LoadBins();
        }
        async Task Collection()
        {
            if (User is null || OpenBin is null)
                throw new Exception("Invalid input");
            var station = (await Service.GetStationInfo())!.First();
            var activity = new CollectionActivityModel()
            {
                BadgeNo = User!.badgeno,
                Activity = "Collection",
                StationName = OpenBin!.openbinname.Substring(0,OpenBin!.openbinname.Length-3),
                FromBinName = OpenBin!.openbinname,
                LoginDate = "",
                ToBinName = "",
                Weight = "0"
            };
            await Service.SendCollection(activity);
        }
        [RelayCommand]
        public async Task WasteProcess()
        {
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
            await dialogService.ShowMessageAsync("Result", message);
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