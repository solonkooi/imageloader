using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImageLoader.Contract;
using ImageLoader.Contract.Service;
using ImageLoader.Contract.Settings;
using Microsoft.Extensions.Logging;

namespace ImageLoader.DataFlow.Services
{
    public class FileLoader : BaseFileLoader, IFileLoader
    {
        private readonly IFileLoaderSettings _settings;
        private readonly string _pathDirectory;
        private readonly IFileUtils _fileUtils;
        private readonly Random _random;
        private ICollection<string> _urls;
        
        public FileLoader(IFileLoaderSettings settings,
            HttpClient httpClient,
            ILogger<FileLoader> logger, 
            IFileUtils fileUtils) : base(httpClient, logger)
        {
            _settings = settings;
            _fileUtils = fileUtils;
            _random = new Random();
            _pathDirectory = $"{Directory.GetCurrentDirectory()}{_settings.DownloadDirectory}";
        }

        public async Task DownloadRandomUrlsAsync(int countRequest, int? maxDegreeOfParallelism = null)
        {
            _urls = await _fileUtils.GetDataListAsync();
            maxDegreeOfParallelism ??= _settings.BulkSize;

            var downloadActionBlock = new ActionBlock<string>(
                async x =>
                {
                    var downloadPath = await GetDownloadPathAsync();
                    await DownloadUrlAsync(x,  downloadPath);
                }, 
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism.Value,
                });

            for (var i = 0; i < countRequest; i++)
            {
                downloadActionBlock.Post(await GetRandomUrlAsync());
            }

            downloadActionBlock.Complete();
            downloadActionBlock.Completion.Wait();
        }

        private async Task<string> GetRandomUrlAsync() => await Task.FromResult(_urls.ElementAt(_random.Next(_urls.Count)));
        private async Task<string> GetDownloadPathAsync() => await Task.FromResult($"{_pathDirectory}{Guid.NewGuid()}.png");
    }
}