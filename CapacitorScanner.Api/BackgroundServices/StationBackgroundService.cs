
using CapacitorScanner.Core.Model.LocalDb;
using CapacitorScanner.Core.Services;

namespace CapacitorScanner.Api.BackgroundServices
{
    public class StationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider Services;
        public StationBackgroundService(IServiceProvider services)
        {
            Services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scoped = Services.CreateScope())
                {
                    try
                    {
                        var binLocalDbService = scoped.ServiceProvider.GetRequiredService<BinLocalDbService>();
                        var pidsgService = scoped.ServiceProvider.GetRequiredService<PIDSGService>();
                        var stationInfo = pidsgService.GetStationInfo().Result;
                        if (stationInfo is not null && stationInfo.Any())
                        {
                            var info = stationInfo.First();
                            var infoStation = new StationInfoLocalModel() { datavalue = info.description, property = "description" };
                            await binLocalDbService.AddOrUpdateStationInfoLocal(infoStation);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error in StationBackgroundService: {e.Message}");
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
