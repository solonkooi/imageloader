using System.Threading.Tasks;

namespace ImageLoader.Contract
{
    public interface IFileLoader
    {
        Task DownloadRandomUrlsAsync(int countRequest, int? maxDegreeOfParallelism = null);
    }
}
