using System;
using ImageLoader.Contract;
using ImageLoader.Contract.Helpers;
using ImageLoader.TaskBasedAsyncPattern.Helper;
using ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces;
using ImageLoader.TaskBasedAsyncPattern.Models;
using ImageLoader.TaskBasedAsyncPattern.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace ImageLoader.TaskBasedAsyncPattern.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureTaskBasedAsyncServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileLoader, FileLoader>()
                .AddSingleton<IThreadsLimiterFactory, ThreadsLimiterFactory>();
            services.AddHttpClient<IFileLoader, FileLoader>();
            services = (ServiceCollection)ConfigureReTryHelper(services, EnumSelectHelper.GetSelectValueEnum<ImplementReTry>());
            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
        }
        
        private static IServiceCollection ConfigureReTryHelper(IServiceCollection services, ImplementReTry implementReTry)
        {
            switch (implementReTry)
            {
                case ImplementReTry.Polly:
                    services.AddTransient<IRetry, PollyRetry>();
                    break;
                case ImplementReTry.Custom:
                    services.AddTransient<IRetry, CustomRetry>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(implementReTry), implementReTry, null);
            }
            return services;
        }
    }
}