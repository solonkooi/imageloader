using System;
using System.Threading;
using System.Threading.Tasks;
using ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces;

namespace ImageLoader.TaskBasedAsyncPattern.Helper
{
    internal class ThreadsLimiter : IThreadsLimiter
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public ThreadsLimiter(int initialCount, int maxCount)
        {
            _semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);
        }

        public ThreadsLimiter(int initialCount)
        {
            _semaphoreSlim = new SemaphoreSlim(initialCount);
        }


        public Task WaitAsync()
        {
            return _semaphoreSlim.WaitAsync();
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
        }

        public async Task PerformActionAndReleaseAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
