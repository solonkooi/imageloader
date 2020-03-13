using System;
using System.Threading;
using System.Threading.Tasks;
using ImageLoader.Contract.Settings;
using ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;

namespace ImageLoader.TaskBasedAsyncPattern.Helper
{
    public class PollyRetryHelper: IRetryHelper
    {
        private readonly ILogger _logger;
        private readonly IRetrySettings _settings;

        public PollyRetryHelper(IRetrySettings settings, ILogger<PollyRetryHelper> logger)
        {
            _settings = settings;
            _logger = logger;
        }
        public async Task ExecuteAsync(Func<Task> action)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    _settings.MaxRetryCount,
                    i => _settings.TimeSpanDelay,
                    onRetry: OnReTryLogger);


            await retryPolicy.ExecuteAsync(action);
        }

        private void OnReTryLogger(Exception ex, TimeSpan timeSpan, int retryAttempt, Context context)
        {
            _logger.LogWarning($"Delay: {timeSpan.TotalSeconds}s, Retry №: {retryAttempt}, ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}