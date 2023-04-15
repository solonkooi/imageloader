using System;
using System.Threading.Tasks;

namespace ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces
{
    public interface IRetry
    {
        Task ExecuteAsync(Func<Task> action);
    }
}
