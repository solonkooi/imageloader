using ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces;

namespace ImageLoader.TaskBasedAsyncPattern.Helper
{
    public sealed class ThreadsLimiterFactory: IThreadsLimiterFactory
    {
        public IThreadsLimiter Create(int initialCount)
        {
            return new ThreadsLimiter(initialCount);
        }

        public IThreadsLimiter Create(int initialCount, int maxCount)
        {
            return new ThreadsLimiter(initialCount, maxCount);
        }
    }
}
