using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageLoader.Contract;
using ImageLoader.Contract.Settings;
using ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces;
using Microsoft.Extensions.Logging;

namespace ImageLoader.TaskBasedAsyncPattern.Services
{
    public class FileLoader : IFileLoader
    {
        private ICollection<string> _urls;
        private readonly string _pathDirectory;
        private readonly Random _random;
        private readonly ILogger _logger;
        private readonly IFileLoaderSettings _settings;
        private readonly ConcurrentBag<int> _startThreadIds = new ConcurrentBag<int>();
        private readonly ConcurrentBag<int> _endThreadIds = new ConcurrentBag<int>();
        private readonly HttpClient _httpClient;
        private readonly IRetry _retry;
        private readonly IFileUtils _fileUtils;
        private readonly IThreadsLimiterFactory _threadsLimiterFactory;

        public FileLoader(IFileUtils fileUtils,
            ILogger<FileLoader> logger,
            IFileLoaderSettings settings,
            HttpClient httpClient,
            IRetry retry,
            IThreadsLimiterFactory threadsLimiterFactory)
        {
            _settings = settings;
            _fileUtils = fileUtils;
            _logger = logger;
            _random = new Random();
            _httpClient = httpClient;
            _retry = retry;
            _threadsLimiterFactory = threadsLimiterFactory;
            _pathDirectory = $"{Directory.GetCurrentDirectory()}{_settings.DownloadDirectory}";
        }

        public async Task DownloadRandomUrlsAsync(int countRequest, int? maxDegreeOfParallelism = null)
        {
            _urls = await _fileUtils.GetDataListAsync();
            maxDegreeOfParallelism ??= _settings.BulkSize;
            using (var threadsLimiter = 
                _threadsLimiterFactory.Create(maxDegreeOfParallelism.Value, maxDegreeOfParallelism.Value))
            {
                var tasks = new List<Task>();
                for (var i = 0; i < countRequest; i++)
                {
                    await threadsLimiter.WaitAsync();
                    var downloadPath = await GetDownloadPathAsync();
                    tasks.Add(threadsLimiter.PerformActionAndReleaseAsync(() =>
                        _retry.ExecuteAsync(async () =>
                            await DownloadFileAsync(await GetRandomUrlAsync(), downloadPath))
                    )
                );
                }
                await Task.WhenAll(tasks);
            }
            _logger.LogInformation($"StartThreadIds: {string.Join(",", _startThreadIds)}");
            _logger.LogInformation($"EndThreadIds: {string.Join(",", _endThreadIds)}");
        }

        private async Task DownloadFileAsync(string url, string filePath)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                if (!_startThreadIds.Contains(Thread.CurrentThread.ManagedThreadId))
                {
                    _startThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
                }

                _logger.LogInformation(
                    $"started: {url} - {response.Content.Headers.ContentLength} bytes -  startThreadId:{Thread.CurrentThread.ManagedThreadId}");

                var watch = System.Diagnostics.Stopwatch.StartNew();

                await using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    await contentStream.CopyToAsync(fileStream);
                }

                if (!_endThreadIds.Contains(Thread.CurrentThread.ManagedThreadId))
                {
                    _endThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
                }

                _logger.LogInformation(
                    $"finished:  endThreadId:{Thread.CurrentThread.ManagedThreadId} - {watch.ElapsedMilliseconds}ms");
                watch.Stop();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error download file - url:{url}");
                _logger.LogTrace(ex,string.Empty);
                throw;
            }
        }

        private async Task<string> GetRandomUrlAsync() => await Task.FromResult(_urls.ElementAt(_random.Next(_urls.Count)));

        private async Task<string> GetDownloadPathAsync() => await Task.FromResult($"{_pathDirectory}{Guid.NewGuid()}.png");
    }
}
