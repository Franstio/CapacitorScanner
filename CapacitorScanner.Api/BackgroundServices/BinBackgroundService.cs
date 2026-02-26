
using CapacitorScanner.Core.Model;
using CapacitorScanner.Core.Model.LocalDb;
using CapacitorScanner.Core.Services;

namespace CapacitorScanner.Api.BackgroundServices
{
    public class BinBackgroundService : BackgroundService
    {
        private IServiceProvider serviceProvider;
        public BinBackgroundService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    try
                    {
                        var binLocalDbService = scope.ServiceProvider.GetRequiredService<BinLocalDbService>();
                        var binService = scope.ServiceProvider.GetRequiredService<PIDSGService>();
                        var data = await binService.GetBins();
                        if (data is null)
                            continue;
                        foreach (var item in data)
                        {
                            if ((await binLocalDbService.GetBin(item.name)) is null)
                            {
                                await binLocalDbService.InsertBinHost(new BinLocalModel()
                                {
                                    bin = item.name,
                                    maxweight = item.capacity,
                                    weight = item.weightresult,
                                    hostname = string.Empty,
                                    status = "",
                                    wastetype = item.scraptype_name,
                                    binweight = item.weight,
                                    lastfrombinname = item.lastfrombinname,
                                    weightsystem = item.weightsystem

                                });
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }
    }
}
