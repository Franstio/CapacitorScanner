
using CapacitorScanner.Core.Model;
using CapacitorScanner.Core.Model.LocalDb;
using CapacitorScanner.Core.Services;

namespace CapacitorScanner.Api.BackgroundServices
{
    public class TransactionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConfigService configService;
        public TransactionBackgroundService( IServiceProvider serviceProvider, ConfigService configService)
        {
            this.serviceProvider = serviceProvider;
            this.configService = configService;
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
                        var data = await binLocalDbService.GetFailedScrapTransaction();
                        foreach (var transaction in data)
                        {
                            var res = await binService.SendTransactionPIDSG(new Core.Model.PIDSG.TransactionActivityModel() { 
                                Activity = transaction.Activity,
                                BadgeNo = transaction.Badgeno,
                                FromBinName = transaction.Container,
                                LoginDate = transaction.LoginDate,
                                StationName = configService.Config.hostname,
                                ToBinName = transaction.Bin,
                                Weight = Convert.ToDecimal(transaction.WeightResult.ToString("0.00"))
                            });
                            await binLocalDbService.UpdateStatus(res ? "SUCCESS - OFFLINE" : "FAILED",transaction.Id);
                        }
                        
                    }
                    catch (Exception e)
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
