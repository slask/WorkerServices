using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ScopedWorker
{
    public class PingWorker: ScheduledBackgroundService
    {
        public PingWorker(ILogger<ScheduledBackgroundService> logger) : base(logger)
        {
        }

        protected override Task Process(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Pinging somebody :)");
            return Task.CompletedTask;
        }
    }
}