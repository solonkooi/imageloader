namespace ImageLoader.Contract.Settings
{
    public interface IFileLoaderSettings
    {
        string DownloadDirectory { get; }
        
        int BulkSize { get; }
    }
}