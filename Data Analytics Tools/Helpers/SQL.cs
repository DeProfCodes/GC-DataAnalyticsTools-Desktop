using Data_Analytics_Tools.Constants;
using Data_Analytics_Tools.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Data_Analytics_Tools.Helpers
{
    public class SQL
    {
        SqlConnection Connection;
        SqlCommand command;

        string connectionString;

        public enum SqlExecutionType
        {
            Scalar,
            NonQuery
        };

        struct TempData
        {
            public string databaseName;
            public string tableName;
            public int totalInsertRows;

            public void Set(string tableName, int totalInsertRows)
            {
                this.tableName = tableName;
                this.totalInsertRows = totalInsertRows;
            }
        };

        TempData tempData;

        bool testMode = true;

        const string INSUFFICIENT_MEMORY = "Insufficient Memory";
        const string INSPECT_SCHEMA = "Insepct Schema";

        public SQL()
        {
            connectionString = "Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database=apacheLogsToMySqlMemory;User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";
            Connection = new SqlConnection(connectionString);
        }

        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            Connection = new SqlConnection(connectionString);
        }

        public void SetDatabaseName(string databaseName)
        {
            connectionString = $"Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database={databaseName};User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";
            SetConnectionString(connectionString);  
        }

        public async Task CreateDatabase(string databaseName)
        {
            if (testMode)
                databaseName += "_TEST";

            string query = $"CREATE DATABASE {databaseName}";
            await RunQuery(query);

            string connectionString = $"Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database={databaseName};User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;Encrypt=False";
            Connection = new SqlConnection(connectionString);

            tempData.databaseName = databaseName;
        }

        public async Task<int> RunBulkQueries(List<string> queries)
        {
            int rows = 0;
            foreach (string query in queries)
            {
                rows += await RunQuery(query);
            }
            return rows;
        }

        public async Task<int> ClearTable(string tableName)
        {
            string query = $"DELETE FROM {tableName}";
            int rows = await RunQuery(query);

            return rows;
        }

        public int InsertIntoTable(string tableName, string query, List<string> insertQueries)
        {
            int rows = 0;
            try
            {

            }
            catch (Exception ex)
            {
                if (ex.Message == INSUFFICIENT_MEMORY)
                {

                }
            }
            return rows;
        }

        public async Task<bool> RowExist(string tableName, string insertQuery)
        {
            if (!insertQuery.ToLower().Contains("insert"))
                return true;

            string[] values = insertQuery.Split("(");
            string[] values2 = values[1].Split(",");
            string log_hash = values2[0];
            string time = values2[1];

            string count = $"SELECT COUNT(*) FROM {tableName} WHERE log_hash = {log_hash} AND time={time}";
            int rows = await RunQuery(count, "",SqlExecutionType.Scalar);

            return rows > 0;
        }

        public async Task<int> RunQuery(string query, string tableName="", SqlExecutionType executionType = SqlExecutionType.NonQuery)
        {
            int rows = 0;
            object rowsObj = null;
            
            try
            {
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 3600;
                
                Connection.Close();
                Connection.Open();

                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(Connection.ConnectionString))
                {
                    bulkcopy.DestinationTableName = tableName;
                    await bulkcopy.WriteToServerAsync(reader);
                    bulkcopy.Close();
                    Connection.Close();
                    return 100;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("There is already an object named"))
                {
                    //all good: DROP TABLE and CREATE TABLE
                }
                else if (ex.Message.Contains("Database") && ex.Message.Contains("already exists"))
                {
                    //all good
                }
                else if (ex.Message.Contains("There is insufficient system memory in resource pool"))
                {
                    throw new Exception(INSUFFICIENT_MEMORY);
                }
                else if (ex.Message.Contains("Operand type clash: numeric is incompatible with text"))
                {
                    throw new Exception(INSPECT_SCHEMA);
                }
                else
                {
                    string msg = ex.Message;
                    throw ex;
                }
            }

            if (executionType == SqlExecutionType.NonQuery)
            {
                return rows;
            }
            else if (executionType == SqlExecutionType.Scalar)
            {
                if (rowsObj != null && rowsObj is int)
                {
                    return (int)rowsObj;
                }
            }
            return -1;
        }

        public async Task<int> RunQueryOLD(string query, SqlExecutionType executionType = SqlExecutionType.NonQuery)
        {
            int rows = 0;
            object rowsObj = null;

            try
            {
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 3600;

                Connection.Close();
                Connection.Open();

                if (executionType == SqlExecutionType.NonQuery)
                {
                    rows = command.ExecuteNonQuery();
                }
                else if (executionType == SqlExecutionType.Scalar)
                {
                    rowsObj = command.ExecuteScalar();
                }
                Connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (executionType == SqlExecutionType.NonQuery)
            {
                return rows;
            }
            else if (executionType == SqlExecutionType.Scalar)
            {
                if (rowsObj != null && rowsObj is int)
                {
                    return (int)rowsObj;
                }
            }
            return -1;
        }

        public async Task<List<ProcessedApacheFile>> GetProcessedApacheFiles(string query)
        {
            try
            {
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 3600;

                Connection.Close();
                Connection.Open();

                command.CommandText = query;

                var reader = command.ExecuteReader();

                var processed = new List<ProcessedApacheFile>();

                while (reader.Read())
                {
                    var log = new ProcessedApacheFile
                    {
                        Id = (int)reader[0],
                        Filename = reader.GetString(1),
                        ImportComplete = (bool)reader[2],
                        ProcessError = reader.GetString(3),
                        FilePath = reader.GetString(4),
                    };
                    processed.Add(log);
                }
                return processed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FolderMemory GetBaseFolder(string query)
        {
            try
            {
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 3600;

                Connection.Close();
                Connection.Open();

                command.CommandText = query;

                var reader = command.ExecuteReader();

                var folder = new FolderMemory();

                while (reader.Read())
                {
                    folder = new FolderMemory
                    {
                        Id = (int)reader[0],
                        BaseFolderPath = reader.GetString(1),
                        ModifyDate = (DateTime)reader[2]
                    };
                    break;
                }
                return folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SqlConnectionStatus> CheckSQLConnection(string server, string database, string username, string password)
        {
            connectionString = $"Server={server};Database={database};User Id={username};Password={password};Integrated Security=false;Encrypt=False;";
            Connection = new SqlConnection(connectionString);

            try
            {
                Connection.Close();
                await Connection.OpenAsync();

                return SqlConnectionStatus.SUCCESS;             
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The server was not found or was not accessible"))
                    return SqlConnectionStatus.INCORRECT_SERVER_NAME;

                if (ex.Message.Contains("Login failed for user") && !ex.Message.Contains("Cannot open database"))
                    return SqlConnectionStatus.INCORRECT_CREDENTIALS;

                if (ex.Message.Contains("Cannot open database"))
                    return SqlConnectionStatus.DATABASE_NOT_FOUND;

                return SqlConnectionStatus.OTHER;
            }
        }

        public AppCredentials GetSavedCredentials()
        {
            try
            {
                string query = "SELECT * FROM AppCredentials";
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 3600;

                Connection.Close();
                Connection.Open();

                command.CommandText = query;

                var reader = command.ExecuteReader();

                var credentials = new AppCredentials();

                while (reader.Read())
                {
                    credentials = new AppCredentials
                    {
                        AzenqosUsername = reader.GetString(0),
                        AzenqosPassword = reader.GetString(1),
                        SqlUsername = reader.GetString(2),
                        SqlPassword = reader.GetString(3),
                        SqlDatabase = reader.GetString(4),
                        SqlServer = reader.GetString(5)
                    };
                    break;
                }
                return credentials;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateSavedCredentials(AppCredentials creds)
        {
            string query = $"UPDATE AppCredentials SET AzenqosUsername='{creds.AzenqosUsername}', AzenqosPassword='{creds.AzenqosPassword}', " +
                           $"SqlUsername='{creds.SqlUsername}', SqlPassword='{creds.SqlPassword}', SqlDatabase='{creds.SqlDatabase}', SqlServer='{creds.SqlServer}'";

            connectionString = $"Server={creds.SqlServer};Database={ApacheConstants.MemoryDatabase};User Id={creds.SqlUsername};Password={creds.SqlPassword};Integrated Security=false;Encrypt=False;";
            SetConnectionString(connectionString);

            try
            {
                await RunQueryOLD(query);
            }
            catch (Exception e)
            {
                var checkDb = await CheckSQLConnection(creds.SqlServer, ApacheConstants.MemoryDatabase, credentials.SqlUsername, creds.SqlPassword);
                if (checkDb == SqlConnectionStatus.DATABASE_NOT_FOUND)
                {
                    connectionString = $"Server={creds.SqlServer};User Id={creds.SqlUsername};Password={creds.SqlPassword};Integrated Security=false;Encrypt=False;";
                    SetConnectionString(connectionString);

                    await RunQueryOLD($"CREATE DATABASE {ApacheConstants.MemoryDatabase}");

                    connectionString = $"Server={creds.SqlServer};Database={ApacheConstants.MemoryDatabase};User Id={creds.SqlUsername};Password={creds.SqlPassword};Integrated Security=false;Encrypt=False;";
                    SetConnectionString(connectionString);

                    var createTables = $"CREATE TABLE \"{ApacheConstants.MemoryTable}\"(" +
                                        $"Id INT, Filename TEXT, ImportComplete BIT, ProcessError TEXT, FilePath TEXT, CreateDate DATETIME);" +
                                       $"CREATE TABLE \"{ApacheConstants.FolderMemoryTable}\"(" +
                                        $"Id INT, BaseFolderPath TEXT, ModifyDate DATETIME);" +
                                       $"CREATE TABLE \"{ApacheConstants.CredentialsMemory}\"(" +
                                        $"AzenqosUsername TEXT, AzenqosPassword TEXT, SqlUsername TEXT, SqlPassword TEXT, SqlDatabase TEXT, SqlServer TEXT);";

                    await RunQueryOLD(createTables);

                    query = $"INSERT INTO \"{ApacheConstants.CredentialsMemory}\" VALUES('{creds.AzenqosUsername}','{creds.AzenqosPassword}','{creds.SqlUsername}','{creds.SqlPassword}','{creds.SqlDatabase}','{creds.SqlServer}');";
                    await RunQueryOLD(query);

                    if (!e.Message.Contains("Cannot open database"))
                        throw e;
                }
            }
        }
    }
}
