using Microsoft.Extensions.Logging;

namespace ScopedWorker.Scoped
{
    public class HelperService
    {

        private int _count;
        public HelperService(ILogger<HelperService> logger)
        {
            _count = 0;
        }

        public int GetCount()
        {
            _count++;
            return _count;
        }

    }
}