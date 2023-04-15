using System;

namespace ImageLoader.Contract.Settings
{
    public interface IRetrySettings
    {
        int MaxRetryCount { get; }
        
        TimeSpan TimeSpanDelay { get; }
    }
}