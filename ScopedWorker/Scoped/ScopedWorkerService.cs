using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ScopedWorker.Scoped
{
    class ScopedWorkerService : IScopedWorker
    {
        private readonly ILogger<ScopedWorkerService> _logger;
        private readonly HelperService _helperService;

        public ScopedWorkerService(ILogger<ScopedWorkerService> logger, HelperService helperService)
        {
            _helperService = helperService;
            _logger = logger;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScopedWorkerService running at: {time}", DateTimeOffset.Now);
            _logger.LogWarning($"Count from helper={_helperService.GetCount()}");

            await Task.CompletedTask;
        }
    }
}