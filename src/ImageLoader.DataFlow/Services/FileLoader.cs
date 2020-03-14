using System.Threading.Tasks;
using ImageLoader.Contract;
using ImageLoader.Contract.Settings;

namespace ImageLoader.DataFlow.Services
{
    public class FileLoader : IFileLoader
    {
        private readonly IFileLoaderSettings _settings;

        public FileLoader(IFileLoaderSettings settings)
        {
            _settings = settings;
        }

        public Task DownloadRandomUrlsAsync(int countRequest, int? maxDegreeOfParallelism = null)
        {
            maxDegreeOfParallelism ??= _settings.BulkSize;
            return Task.CompletedTask;
        }
    }
}