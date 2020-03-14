using ImageLoader.Contract;
using ImageLoader.DataFlow.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageLoader.DataFlow.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDataFlowServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileLoader, FileLoader>();
            services.AddHttpClient();
            return services;
        }
    }
}