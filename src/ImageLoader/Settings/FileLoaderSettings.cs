using ImageLoader.Contract.Settings;
using Microsoft.Extensions.Configuration;

namespace ImageLoader.Settings
{
    public class FileLoaderSettings : IFileLoaderSettings
    {
        private readonly IConfiguration _configuration;
        
        public FileLoaderSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DownloadDirectory => _configuration.GetValue<string>(nameof(DownloadDirectory));
        public int BulkSize => _configuration.GetValue<int>(nameof(BulkSize));
    }
}