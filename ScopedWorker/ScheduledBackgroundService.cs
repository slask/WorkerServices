using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace ScopedWorker
{
    public class ScheduledBackgroundService : BackgroundService
    {
        protected readonly ILogger<ScheduledBackgroundService> Logger;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        public ScheduledBackgroundService(ILogger<ScheduledBackgroundService> logger)
        {
            Logger = logger;
            _schedule = CrontabSchedule.Parse("*/10 * * * * *", new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected virtual async Task Process(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var nextRun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    await Process(stoppingToken);
                    _nextRun = nextRun;
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}
