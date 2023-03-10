using Data_Analytics_Tools.Data;
using Data_Analytics_Tools.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.BusinessLogic
{
    public class BusinessLogicData : IBusinessLogicData
    {
        private readonly ApplicationDbContext _dbContext;
        public BusinessLogicData()
        {
            
        }

        public async Task AddOrUpdateApacheLogFileImport(string filePath, string filename, bool importComplete, string error)
        {
            var existing = await _dbContext.ApacheFilesImportProgress.FirstOrDefaultAsync(x=>x.FilePath.ToLower() == filePath.ToLower());
            if (existing == null)
            {
                var newApacheImport = new ProcessedApacheFile
                {
                    FilePath = filePath,
                    Filename = filename,
                    ImportComplete = importComplete,
                    ProcessError = error
                };
                _dbContext.Add(newApacheImport);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                existing.ImportComplete = importComplete;
                existing.ProcessError = error;

                _dbContext.Update(existing);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteApacheLogFileImport(string filePath)
        {
            var existing = await _dbContext.ApacheFilesImportProgress.FirstOrDefaultAsync(x => x.FilePath.ToLower() == filePath.ToLower());

            _dbContext.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteApacheLogFileImport(List<string>filePaths)
        {
            var existingFiles = await _dbContext.ApacheFilesImportProgress.Where(x => filePaths.Contains(x.FilePath.ToLower())).ToListAsync();

            _dbContext.RemoveRange(existingFiles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<string>> GetProcessedApacheFiles()
        {
            var processed = await _dbContext.ApacheFilesImportProgress.Where(x => x.ImportComplete == true).ToListAsync();

            return processed.Select(x=>x.Filename).ToList();
        }

        public async Task<List<ProcessedApacheFile>> GetProcessedApacheFiles2()
        {
            //var processed = await _dbContext.ApacheFilesImportProgress.Where(x => x.ImportComplete == true).ToListAsync();

            return new List<ProcessedApacheFile>();
        }
    }
}
