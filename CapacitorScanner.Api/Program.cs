
using CapacitorScanner.Api.BackgroundServices;
using CapacitorScanner.Core.Services;

namespace CapacitorScanner.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddApplicationPart(typeof(Api.Program).Assembly);
            var configService = new ConfigService();
            configService.LoadAsync();
            builder.Services.AddSingleton<ConfigService>(configService);
            builder.Services.AddScoped<PIDSGService>();
            builder.Services.AddScoped<BinLocalDbService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            Func<HttpClientHandler> f = () =>
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
                return handler;
            };
            builder.Services.AddHttpClient().ConfigureHttpClientDefaults(cfg =>
            {
                cfg.ConfigurePrimaryHttpMessageHandler(f);
            });
            builder.Services.AddHostedService<BinBackgroundService>();
            builder.Services.AddHostedService<TransactionBackgroundService>();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || true)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            var sqliteservice = app.Services.CreateScope().ServiceProvider.GetRequiredService<BinLocalDbService>();
            _ = Task.Run(sqliteservice.Initialization);
            app.RunAsync("http://*:5000");
        }
    }
}
