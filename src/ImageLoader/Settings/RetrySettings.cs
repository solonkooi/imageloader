using System;
using ImageLoader.Contract.Settings;
using Microsoft.Extensions.Configuration;

namespace ImageLoader.Settings
{
    public class RetrySettings : IRetrySettings
    {
        private readonly IConfiguration _configuration;
        
        public RetrySettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public int MaxRetryCount => _configuration.GetValue<int>(nameof(MaxRetryCount));
        
        public TimeSpan TimeSpanDelay => TimeSpan.FromSeconds(DelaySecond);
        
        private int DelaySecond => _configuration.GetValue<int>(nameof(DelaySecond));
    }
}