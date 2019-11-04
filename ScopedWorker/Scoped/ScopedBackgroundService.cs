using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ScopedWorker.Scoped
{
    public class ScopedBackgroundService : ScheduledBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopedBackgroundService(ILogger<ScheduledBackgroundService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task Process(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                IScopedWorker scopedService = scope.ServiceProvider.GetRequiredService<IScopedWorker>();
                await scopedService.DoWork(stoppingToken);
            }
        }
    }
}
