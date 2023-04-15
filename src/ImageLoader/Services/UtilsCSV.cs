using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using ImageLoader.Contract;
using ImageLoader.Contract.Settings;

namespace ImageLoader.Services
{
    public class UtilsCsv: IFileUtils
    {
        private readonly string _filePath;
            
        public UtilsCsv(IFileUtilsSettings settings)
        {
            _filePath = settings.FilePath;
        }
        public async Task<ICollection<string>> GetDataListAsync()
        {
            var result = new HashSet<string>();
            
            using (var reader = new StreamReader(_filePath))
            {
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = false;
                while (await csv.ReadAsync())
                {
                    for (var i = 0; csv.TryGetField<string>(i, out var value); i++)
                    {
                        result.Add(value);
                    }
                }
            }

            return result;
        }
    }
}
