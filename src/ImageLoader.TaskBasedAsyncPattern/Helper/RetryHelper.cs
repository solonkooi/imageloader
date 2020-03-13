using System;
using System.Threading.Tasks;
using ImageLoader.Contract.Settings;
using ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces;

namespace ImageLoader.TaskBasedAsyncPattern.Helper
{
    public class RetryHelper: IRetryHelper
    {
        private readonly IRetrySettings _settings;

        public RetryHelper(IRetrySettings settings)
        {
            _settings = settings;
        }
        
        public async Task ExecuteAsync(Func<Task> action)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    retryCount++;
                    await action();
                    break;
                }
                catch
                {
                    if (retryCount > _settings.MaxRetryCount)
                        throw;
                }
                await Task.Delay(_settings.TimeSpanDelay);
            }
        }
    }
}
