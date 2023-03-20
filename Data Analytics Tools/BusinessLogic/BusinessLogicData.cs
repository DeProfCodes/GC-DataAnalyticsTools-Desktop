using Data_Analytics_Tools.Data;
using Data_Analytics_Tools.Helpers;
using Data_Analytics_Tools.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.BusinessLogic
{
    public class BusinessLogicData : IBusinessLogicData
    {
        private SQL sql;
        public BusinessLogicData()
        {
            sql = new SQL();
            sql.SetDatabaseName("apacheLogsToMySqlMemory");
        }

        public async Task AddOrUpdateApacheLogFileImport(string filePath, string filename, bool importComplete, string error)
        {
            var allLogs = await GetProcessedApacheFiles();
            var existing = allLogs.FirstOrDefault(x=>x.FilePath.ToLower() == filePath.ToLower());
            var query = "";

            if (existing == null)
            {
                query = $"INSERT INTO ApacheFilesImportProgress Values('{filename}',{(importComplete?1:0)},'{error}','{filePath}','{DateTime.Now}')";
            }
            else
            {
                query = $"UPDATE ApacheFilesImportProgress" +
                        $" SET ImportComplete = {(importComplete?1:0)}, ProcessError = '{error}'" +
                        $" WHERE Id = {existing.Id}";
            }
            await sql.RunQueryOLD(query);    
        }

        public async Task DeleteApacheLogFileImport(string filePath)
        {
            
        }

        public async Task DeleteApacheLogFileImport(List<string>filePaths)
        {
            
        }

        public async Task<List<ProcessedApacheFile>> GetProcessedApacheFiles()
        {
            var query = "SELECT * FROM ApacheFilesImportProgress";
            var logs = await sql.GetProcessedApacheFiles(query);
            
            return logs;
        }

        public async Task AddOrUpdateBaseFolderDirectory(string newBaseFolder)
        {
            var query = "SELECT COUNT(*) FROM FolderMemory";
            var count = await sql.RunQueryOLD(query, SQL.SqlExecutionType.Scalar);

            if (count < 1)
            {
                query = $"INSERT INTO FolderMemory VALUES('{newBaseFolder}','{DateTime.Now}')";
            }
            else
            {
                query = $"UPDATE FolderMemory" +
                        $" SET BaseFolderPath='{newBaseFolder}', ModifyDate = '{DateTime.Now}'" +
                        $" WHERE Id = 1";
            }
            await sql.RunQueryOLD(query);  
        }

        public string GetBaseFolder()
        {
            var query = "SELECT * FROM FolderMemory WHERE Id = 1";
            
            var baseFolder = sql.GetBaseFolder(query);

            return baseFolder?.BaseFolderPath ?? "C:\\";
        }
    }
}
