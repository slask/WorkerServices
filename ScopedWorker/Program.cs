using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScopedWorker.Scoped;

namespace ScopedWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<PingWorker>();

                    services.AddHostedService<ScopedBackgroundService>();
                    services.AddScoped<IScopedWorker, ScopedWorkerService>();
                    services.AddScoped<HelperService>();
                });
    }
}
