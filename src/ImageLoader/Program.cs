using System;
using System.IO;
using ImageLoader.Contract;
using ImageLoader.Contract.Helpers;
using ImageLoader.Contract.Settings;
using ImageLoader.Models;
using ImageLoader.Services;
using ImageLoader.Settings;
using ImageLoader.TaskBasedAsyncPattern.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace ImageLoader
{
    internal static class Program
    {
        private static IServiceProvider _serviceProvider;

        private static void Main()
        {
            _serviceProvider = ConfigureServices();
            var imageLoader = _serviceProvider.GetService<IFileLoader>();
            AsyncContext.Run(() => imageLoader.DownloadRandomUrlsAsync(GetCountRequest()));
        }

        private static int GetCountRequest()
        {
            int countRequest;
            Console.Write("Number of requests to download data: ");
            var consoleCount = Console.ReadLine();
            while (!int.TryParse(consoleCount, out countRequest))
            {
                Console.Write("Not a valid number, try again: ");
                consoleCount = Console.ReadLine();
            }
            return countRequest;
        }
        
        private static IServiceProvider ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("resources/app-settings.json", false)
              .Build();
            var services = new ServiceCollection();
            services.AddOptions()
                .AddLogging(configure =>
                {
                    configure.AddConfiguration(configuration.GetSection("Logging"));
                    configure.AddConsole();
                })
                .AddSingleton<IConfiguration>(x => configuration.GetSection("Configuration"))
                .AddSingleton<IFileUtils, UtilsCsv>()
                .AddSingleton<IFileLoaderSettings, FileLoaderSettings>()
                .AddSingleton<IRetrySettings, RetrySettings>()
                .AddSingleton<IFileUtilsSettings, FileUtilsSettings>();
            services.ConfigureAsyncPattern(EnumSelectHelper.GetSelectValueEnum<ImplementAsyncPattern>());
            return services.BuildServiceProvider();
        }
        
        private static void ConfigureAsyncPattern(this IServiceCollection serviceCollection, ImplementAsyncPattern implementReTry)
        {
            switch (implementReTry)
            {
                case ImplementAsyncPattern.TaskBasedAsync:
                    serviceCollection.ConfigureTaskBasedAsyncServices();
                    break;
                case ImplementAsyncPattern.DataFlow:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(implementReTry), implementReTry, null);
            }
        }
    }
}
