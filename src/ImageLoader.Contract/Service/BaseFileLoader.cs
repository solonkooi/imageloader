using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ImageLoader.Contract.Service
{
    public abstract class BaseFileLoader
    {
        public ILogger Logger { get; set; }
        public HttpClient HttpClient { get; set; }

        protected BaseFileLoader(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            HttpClient = httpClientFactory.CreateClient();
            Logger = logger;
        }

        public async Task DownloadUrlAsync(string url, string filePath)
        {
            try
            {
                using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                Logger.LogInformation(
                    $"started: {url} - {response.Content.Headers.ContentLength} bytes -  startThreadId:{Thread.CurrentThread.ManagedThreadId}");

                var watch = System.Diagnostics.Stopwatch.StartNew();

                await using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    await contentStream.CopyToAsync(fileStream);
                }

                Logger.LogInformation(
                    $"finished:  endThreadId:{Thread.CurrentThread.ManagedThreadId} - {watch.ElapsedMilliseconds}ms");
                watch.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error download file - url:{url}");
                Logger.LogTrace(ex, string.Empty);
                throw;
            }
        }
    }
}