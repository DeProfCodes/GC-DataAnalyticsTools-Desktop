using Data_Analytics_Tools.Constants;
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
            sql.SetDatabaseName(ApacheConstants.MemoryDatabase);
        }

        public async Task AddOrUpdateApacheLogFileImport(string filePath, string filename, bool importComplete, string error)
        {
            var allLogs = await GetProcessedApacheFiles();
            var existing = allLogs.FirstOrDefault(x=>x.FilePath.ToLower() == filePath.ToLower());
            var query = "";

            if (existing == null)
            {
                query = $"INSERT INTO {ApacheConstants.MemoryTable} Values('{filename}',{(importComplete?1:0)},'{error}','{filePath}','{DateTime.Now}')";
            }
            else
            {
                query = $"UPDATE {ApacheConstants.MemoryTable}" +
                        $" SET ImportComplete = {(importComplete?1:0)}, ProcessError = '{error}'" +
                        $" WHERE Id = {existing.Id}";
            }
            sql.RunQueryOLD(query);    
        }

        public async Task DeleteApacheLogFileImport(string filePath)
        {
            
        }

        public async Task DeleteApacheLogFileImport(List<string>filePaths)
        {
            
        }

        public async Task<List<ProcessedApacheFile>> GetProcessedApacheFiles()
        {
            var query = $"SELECT * FROM {ApacheConstants.MemoryTable}";
            var logs = await sql.GetProcessedApacheFiles(query);
            
            return logs;
        }

        public void AddOrUpdateBaseFolderDirectory(string newBaseFolder)
        {
            sql.SetDatabaseName(ApacheConstants.MemoryDatabase);

            var query = $"SELECT COUNT(*) FROM {ApacheConstants.FolderMemoryTable}";
            var count = sql.RunQueryOLD(query, SQL.SqlExecutionType.Scalar);

            if (count < 1)
            {
                query = $"INSERT INTO {ApacheConstants.FolderMemoryTable} VALUES(1,'{newBaseFolder}','{DateTime.Now}')";
            }
            else
            {
                query = $"UPDATE {ApacheConstants.FolderMemoryTable}" +
                        $" SET BaseFolderPath='{newBaseFolder}', ModifyDate = '{DateTime.Now}'" +
                        $" WHERE Id = 1";
            }
            try
            {
                sql.RunQueryOLD(query);  
            }
            catch{}
        }

        public string GetBaseFolder()
        {
            var query = $"SELECT * FROM {ApacheConstants.FolderMemoryTable}";
            
            var baseFolder = sql.GetBaseFolder(query);

            if (baseFolder != null && baseFolder.BaseFolderPath == null)
            {
                AddOrUpdateBaseFolderDirectory("C:\\");
            }

            return baseFolder?.BaseFolderPath ?? "C:\\";
        }
    }
}
