using System;
using System.Threading.Tasks;

namespace ImageLoader.TaskBasedAsyncPattern.Helper.Interfaces
{
    public interface IRetryHelper
    {
        Task ExecuteAsync(Func<Task> action);
    }
}
