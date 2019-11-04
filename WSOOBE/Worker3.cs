using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WSOOBE
{
    public class Worker3 : BackgroundService
    {
        private readonly ILogger<Worker3> _logger;
        private readonly MySingletonService _singleMySingletonService;
        private readonly TransientService _transientService;

        public Worker3(ILogger<Worker3> logger, MySingletonService singleMySingletonService, TransientService transientService)
        {
            _transientService = transientService;
            _singleMySingletonService = singleMySingletonService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker3 running at: {time}", DateTimeOffset.Now);
                _logger.LogWarning($"Count from singleton={_singleMySingletonService.GetCount()}");
                _logger.LogWarning($"Count from transient={_transientService.GetCount()}");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }

    public class MySingletonService
    {
        private readonly ILogger<MySingletonService> _logger;
        private int _count;
        private readonly TransientService _transientService;

        public MySingletonService(ILogger<MySingletonService> logger, TransientService transientService)
        {
            _transientService = transientService;
            _logger = logger;
            _count = 0;
            _logger.LogInformation("MySingletonService::ctor - Count Starting At {_count}");
        }

        public int GetCount()
        {
            _logger.LogCritical($"TransientInsideSingleton::GetCount()={_transientService.GetCount()}");
            _count++;
            return _count;
        }
    }

    public class TransientService
    {
        private int _count;
        public TransientService(ILogger<TransientService> logger)
        {
            _count = (new Random()).Next(0, 10);
            logger.LogCritical($"TransientService::ctor - Count Starting At {_count}");
        }

        public int GetCount()
        {
            _count++;
            return _count;
        }
    }
}
