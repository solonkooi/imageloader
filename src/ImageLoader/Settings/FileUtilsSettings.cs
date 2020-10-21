using ImageLoader.Contract.Settings;
using Microsoft.Extensions.Configuration;

namespace ImageLoader.Settings
{
    public class FileUtilsSettings : IFileUtilsSettings
    {
        private readonly IConfiguration _configuration;
        
        public FileUtilsSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FilePath => _configuration.GetValue<string>(nameof(FilePath));
    }
}