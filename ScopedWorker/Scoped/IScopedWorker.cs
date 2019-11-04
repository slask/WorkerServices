using System.Threading;
using System.Threading.Tasks;

namespace ScopedWorker.Scoped
{
    interface IScopedWorker
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
