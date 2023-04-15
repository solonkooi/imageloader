
namespace ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces
{
    public interface IThreadsLimiterFactory
    {
        IThreadsLimiter Create(int initialCount);
        IThreadsLimiter Create(int initialCount, int maxCount);
    }
}
