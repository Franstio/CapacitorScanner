using CapacitorScanner.API.Model;
using CapacitorScanner.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace CapacitorScanner.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<BinModel> Bins { get; set; } = [];
    [ObservableProperty]
    private string stationName  = "Station - Type";

    
    public MainViewModel()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await LoadBins();
                StationName = (await GetStationInfo())?.FirstOrDefault()?.description ?? stationName;
                await Task.Delay(500);
            }
        });
    }
    private HttpClient GetHttpClient()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new System.Uri( $"http://10.89.1.99/api/");
        return client;
    }
    private async Task LoadBins()
    {
        var data = await GetBins();
        if (data is null)
            return;
        Bins.Clear();
        foreach (var item in data.Select(x => new BinModel(x.name, x.scraptype_name, x.weightresult, x.capacity)))
            Bins.Add(item);
    }
    private async Task<List<ContainerBinModel>?> GetBins()
    {
        using ( var client = GetHttpClient() )
        {
            try
            {
                var builder = new UriBuilder(client.BaseAddress!);
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["object"] = "mwastebin";
                query["where"] = "wastestation_name=2-B2-001";
                query["select"] = "name,capacity,description,weightresult,scraptype_name,scrapitem_name";
                query["orderColumn"] = "name";
                query["orderSort"] = "asc";
                builder.Path += "pid/view";
                builder.Query = query.ToString();
                var test = await client.GetStringAsync(builder.ToString());
                var data = await client.GetFromJsonAsync<PayloadModel<ContainerBinModel>>(builder.ToString());
                return data?.data;
            }
            catch
            {
                return null;
            }
        }
    }
    private async Task<List<StationInfoModel>?> GetStationInfo()
    {
        using (var client = GetHttpClient())
        {
            try
            {
                var builder = new UriBuilder(client.BaseAddress!);
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["object"] = "mwastestation";
                query["where"] = "name=2-B2-001";
                query["select"] = "description";
                builder.Path += "pid/view";
                builder.Query = query.ToString();
                var test = await client.GetStringAsync(builder.ToString());
                var data = await client.GetFromJsonAsync<PayloadModel<StationInfoModel>>(builder.ToString());
                return data?.data;
            }
            catch
            {
                return null;
            }
        }
    }

}
