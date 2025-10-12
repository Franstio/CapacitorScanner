using CapacitorScanner.API.Model;
using CapacitorScanner.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace CapacitorScanner.Services
{
    public class PIDSGService
    {
        private readonly ConfigService _configService;
        public PIDSGService(ConfigService config)
        {
            _configService = config;
        }
        private HttpClient BuildHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{_configService.Config.API_URL}");
            return client;
        }
        public async Task<List<ContainerBinModel>?> GetBins(string? name=null)
        {
            using (var client = BuildHttpClient())
            {
                try
                {
                    var builder = new UriBuilder(client.BaseAddress!);
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["object"] = "mwastebin";
                    if (name is null)
                    {
                        query["where"] = $"wastestation_name={_configService.Config.hostname}";
                        query["select"] = "name,capacity,description,weightresult,scraptype_name,scrapitem_name";
                        query["orderColumn"] = "name";
                        query["orderSort"] = "asc";
                    }
                    else
                        query["where"] = $"name={name}";
                        builder.Path += _configService.Config.view;
                    builder.Query = query.ToString();
                    Console.WriteLine(builder.ToString());
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


        public async Task<List<StationInfoModel>?> GetStationInfo()
        {
            using (var client = BuildHttpClient())
            {
                try
                {
                    var builder = new UriBuilder(client.BaseAddress!);
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["object"] = "mwastestation";
                    query["where"] = $"name={_configService.Config.hostname}";
                    query["select"] = "description";
                    builder.Path += _configService.Config.view;
                    builder.Query = query.ToString();
                    var data = await client.GetFromJsonAsync<PayloadModel<StationInfoModel>>(builder.ToString());
                    return data?.data;
                }
                catch
                {
                    return null;
                }
            }
        }
        public async Task<UserModel?> LoginUser(string badgeno)
        {
            try
            {
                using (var client = BuildHttpClient())
                {
                    var builder = new UriBuilder(client.BaseAddress!);
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["f1"] = _configService.Config.hostname;
                    query["f2"] = badgeno;
                    builder.Path += _configService.Config.loginEndpoint;
                    builder.Query = query.ToString();

                    var res = await client.GetFromJsonAsync<PayloadModel<UserModel>>(builder.ToString());
                    var usr = res?.data.FirstOrDefault();
                    if (usr is not null)
                        usr = new UserModel(usr.employeename,usr.logindate, badgeno);
                    return usr;
                }
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message + " | " + ex.StackTrace);
                return null;
            }
        }
        public async Task<BinActivityModel?> AutoProcessBinActivity(string badgeno,string scan)
        {
            try
            {
                using (var client = BuildHttpClient())
                {
                    var builder = new UriBuilder(client.BaseAddress!);
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["f1"] = _configService.Config.hostname;
                    query["f2"] = badgeno;
                    query["f3"] = scan;
                    builder.Path += _configService.Config.stationActivity;
                    builder.Query = query.ToString();

                    var test = await client.GetStringAsync(builder.ToString());
                    var res = await client.GetFromJsonAsync<BinActivityModel.BinActivityPayload>(builder.ToString());
                    if (res is null)
                        return null;
                    BinActivityModel binActivityModel = new BinActivityModel(res);
                    return binActivityModel;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + " | " + ex.StackTrace);
                return null;
            }
        }
        public async Task<VerifyStep2Model?> VerifyStep2(string badgeno,string container,string bin)
        {
            try
            {
                using (var client = BuildHttpClient())
                {
                    var builder = new UriBuilder(client.BaseAddress!);
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["f1"] = _configService.Config.hostname;
                    query["f2"] = badgeno;
                    query["f3"] = container;
                    query["f4"] = bin;
                    builder.Path += _configService.Config.verifystep2;
                    builder.Query = query.ToString();

                    var test = await client.GetStringAsync(builder.ToString());
                    var json = await client.GetFromJsonAsync<VerifyStep2Model>(builder.ToString());
                    return json;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + " | " + ex.StackTrace);
                return null;
            }
        }
    }
}
