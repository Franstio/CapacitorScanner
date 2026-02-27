using Avalonia.Data;
using CapacitorScanner.Core.API.Model;
using CapacitorScanner.Core.Model;
using CapacitorScanner.Core.Services;
using CapacitorScanner.Messages;
using CapacitorScanner.Core.Model.PIDSG;
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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using CapacitorScanner.Core.Model.LocalDb;

namespace CapacitorScanner.ViewModels
{
    public partial class WasteControlViewModel: ViewModelBase
    {
        private readonly PIDSGService Service;
        private readonly ConfigService ConfigService;   
        private readonly BinLocalDbService DbService;
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
        
        public WasteControlViewModel(PIDSGService service,BinLocalDbService _db,ConfigService config,DialogService dialogService,HttpClient client) 
        {
            Service = service;
            DbService = _db; 
            ConfigService = config;
            this.httpClient = client;
            this.dialogService = dialogService;
        }
        private System.Threading.Timer? timer = null;
        public ObservableCollection<BinModel> Bins { get; set; } = [new BinModel("test","test",1,23)];
        
        [RelayCommand]
        public void LoadBins()
        {
            if (timer is not null)
                timer.Dispose();
            timer = new System.Threading.Timer(async (obj) =>
            {
                try
                {
                    if (Container is not null)
                        return;
//                    bool resultServer = await LoadBinFromPidsg();
  //                  if (!resultServer) 
                        await LoadBinFromLocal();

                }
                catch { }
            },null,0,3000);

        }
        async Task<bool> LoadBinFromPidsg()
        {
            var data = await Service.GetBins();
            if (data is null)
                return false;
            Bins.Clear();
            var _data = data.Select(x => new BinModel(x.name, x.scraptype_name, x.weightresult, x.capacity)).OrderBy(x => x.Name);
            foreach (var item in _data)
            {
                Bins.Add(item);
            }
            return true;
        }
        async Task<bool> LoadBinFromLocal()
        {
            var data = await DbService.GetBin();
            if (!data.Any())
                return false;
            Bins.Clear();
            var _data = data.Select(x => new BinModel(x.bin, x.wastetype, x.weight, x.maxweight)).OrderBy(x => x.Name);
            foreach (var item in _data)
            {
                Bins.Add(item);
            }
            return true;
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
            {
                await dialogService.ShowMessageAsync("Scan Failed", "User Not Found");
                Message = "Scan QR Code Sampah";
            }
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
            await DbService.UpdateStatusBin(bin.activity == 1 ? "Dispose" : "Collection", bin.openbinname);
            if (activity.Contains(bin.activity) && bin.openbinname.ToLower() != "nothing")
            {
                transactionType = bin.activity == 1 ? TransactionType.Dispose : TransactionType.Collection;
                Message = $"Verification {transactionType.ToString()}\nScan QR Code Container bin";
            }
            else
            {
                Scan = string.Empty;
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
            var binData = await DbService.GetBin(OpenBin.openbinname);
            if (OpenBin.openbinname == Scan && transactionType.HasValue && transactionType.Value != TransactionType.Manual)
            {
                if (transactionType.HasValue && transactionType.Value == TransactionType.Collection)
                {
                    await Collection();
                }
                else 
                {
                    if (!await SendBinVerif(OpenBin.openbinname))
                    {

                        await dialogService.ShowMessageAsync("Transaction Not Finished", "Bin Not Finished");
                        return;
                    }
                }
                Scan = string.Empty;
                await dialogService.ShowMessageAsync("Verification", OpenBin.activity == 1 ? "Verification Waste process" : "Verification Dispose process");
                ResetStateInput();
            }
            else if (transactionType.HasValue && transactionType.Value != TransactionType.Manual)
                await dialogService.ShowMessageAsync("Verification Failed", "Wrong Container Bin");
//             LoadBins();
        }
        async Task Collection()
        {
            if (User is null || OpenBin is null)
                throw new Exception("Invalid input");
            var station = (await Service.GetStationInfo())!.First();
            var activity = new TransactionActivityModel()
            {
                BadgeNo = User!.badgeno,
                Activity = "Collection",
                StationName = ConfigService.Config.hostname,
                FromBinName = OpenBin!.openbinname,
                LoginDate = "",
                ToBinName = "",
                Weight = 0
            };
            var res = await Service.SendTransactionPIDSG(activity);

            ScrapTransactionModel transaction = new ScrapTransactionModel(-1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loginDate.ToString("yyyy-MM-dd HH:mm:ss"),
                User.badgeno, Container!.name, OpenBin.openbinname, "ONLINE", ConfigService.Config.hostname, (double)0, "Collection", User.badgeno);
            transaction.Status = res ? "SUCCESS" : "FAILED";
            await DbService.CreateTransaction(transaction);
            await DbService.UpdateStatusBin("", OpenBin.openbinname);
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
            ScrapTransactionModel transaction = new ScrapTransactionModel(-1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loginDate.ToString("yyyy-MM-dd HH:mm:ss"),
                User.badgeno, Container.name, Bin.Name, "ONLINE", ConfigService.Config.hostname, (double)Container.weightresult, res?.data[0].activity ?? "None", User.badgeno);
            await DbService.CreateTransaction(transaction);
            string message = res?.data[0].status ?? "Error";
            ResetStateInput();
            await dialogService.ShowMessageAsync("Result", message);
//            LoadBins();
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