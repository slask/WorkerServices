using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WSOOBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}











//services.AddHostedService<Worker2>();
//services.AddHostedService<Worker3>();
//services.AddTransient<TransientService>();
//services.AddSingleton<MySingletonService>();





