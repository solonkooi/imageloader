using System;
using System.Threading.Tasks;

namespace ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces
{
    public interface IThreadsLimiter : IDisposable
    {
        Task WaitAsync();
        Task PerformActionAndReleaseAsync(Func<Task> action);
    }
}
