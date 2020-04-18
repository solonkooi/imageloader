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
            IHttpClientFactory httpClientFactory,
            ILogger<FileLoader> logger, 
            IFileUtils fileUtils) : base(httpClientFactory, logger)
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

            var dataPointBuffer = new BufferBlock<string>(new DataflowBlockOptions()
            {
                BoundedCapacity = DataflowBlockOptions.Unbounded
            });

            var options = new ExecutionDataflowBlockOptions()
            {
                BoundedCapacity = 1000,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var restBlock = new ActionBlock<string>(async (data) => {
                var success = false;
                var attempts = 0;
                while (!success && attempts < 5)
                {
                    await Task.Delay(1000);
                    attempts++;
                    success = true;
                }
            }, options);

            dataPointBuffer.LinkTo(restBlock, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });

            dataPointBuffer.Post("fdfdf");
            dataPointBuffer.Complete();
            dataPointBuffer.Completion.Wait();



            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var downloadString = new TransformBlock<string,string>(async (request) =>
            {
                Console.WriteLine("Downloading '{0}'...", request);

                return await new HttpClient().GetStringAsync(request);
            });

            var createWordList = new TransformBlock<string, string[]>(text =>
            {
                Console.WriteLine("Creating word list...");

                // Remove common punctuation by replacing all non-letter characters 
                // with a space character.
                char[] tokens = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
                text = new string(tokens);

                // Separate the text into an array of words.
                return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            });

            downloadString.LinkTo(createWordList, linkOptions);
            downloadString.Complete();
            createWordList.Completion.Wait();

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