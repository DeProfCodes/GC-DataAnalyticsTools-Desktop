using Data_Analytics_Tools.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.BusinessLogic
{
    public interface IBusinessLogicData
    {
        public Task AddOrUpdateApacheLogFileImport(string filePath, string fileName, bool importComplete, string error);

        public Task DeleteApacheLogFileImport(string filePath);

        public Task DeleteApacheLogFileImport(List<string> filenames);

        public Task<List<ProcessedApacheFile>> GetProcessedApacheFiles();

        public Task AddOrUpdateBaseFolderDirectory(string newBaseFolder);

        public string GetBaseFolder();
    }
}
