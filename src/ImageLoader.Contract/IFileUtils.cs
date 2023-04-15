using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageLoader.Contract
{
    public interface IFileUtils
    {
        Task<ICollection<string>> GetDataListAsync();
    }
}
