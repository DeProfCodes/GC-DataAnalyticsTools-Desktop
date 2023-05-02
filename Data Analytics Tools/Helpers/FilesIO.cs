using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Helpers
{
    public class FilesIO
    {
        private static string apacheLogsDirectory;
        private static string apacheLogsDirectory_LogHashes;
        private static string apacheLogsDirectory_Downloads;
        private static string apacheLogsDirectory_Schema;
        private static string apacheLogsDirectory_server;

        private static string GetMemoryFile()
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            var path = $"{projectDirectory}\\Folders\\memory\\LastKnownDirectories.txt";

            if (!File.Exists(path))
            {
                File.Create(path) ;
            }
            return path;
        }

        public static void SaveDirectories(string source, string destination)
        {
            try
            {
                var path = GetMemoryFile();
                File.WriteAllText(path, $"{source},{destination}");
            }
            catch
            {
                
            }
        }

        public static string GetSavedSourceFolder()
        {
            try
            {
                var path = GetMemoryFile();
                var directories = File.ReadAllText(path);

                var data = directories.Split(",");

                if (data.Length > 1)
                    return data[0];
            }
            catch
            {
                
            }
            return "C:\\";
        }

        public static string GetSavedDestinationFolder()
        {
            try
            {
                var path = GetMemoryFile();
                var directories = File.ReadAllText(path);

                var data = directories.Split(",");

                if (data.Length > 1)
                    return data[1];
            }
            catch
            {
                
            }
            return "C:\\";
        }

        private static string GetFileName(string filepath)
        {
            var data = filepath.Split('\\');
            var filename = data[data.Length - 1].Split(".")[0];

            return filename;
        }
        
        public static Dictionary<string, string> ReadFileToCompletetion(string sourceFolder)
        {
            Dictionary<string, string> scripts = new Dictionary<string, string>();

            var scriptFiles = Directory.GetFiles(sourceFolder);

            foreach (var script in scriptFiles)
            {
                var filename = GetFileName(script);   
                var sql = File.ReadAllText(script);
                scripts.Add($"{filename}.xlsx", sql);
            }

            return scripts;
        }

        public static void SetApacheLogsDirectory(string directory)
        {
            apacheLogsDirectory = directory;
            apacheLogsDirectory_LogHashes = apacheLogsDirectory + @"\Log hashes\";
            apacheLogsDirectory_Downloads = apacheLogsDirectory + @"\Downloads\";
            apacheLogsDirectory_Schema = apacheLogsDirectory + @"\Schema\";
            apacheLogsDirectory_server = apacheLogsDirectory + @"\Other\";

            Directory.CreateDirectory(apacheLogsDirectory_LogHashes);
            Directory.CreateDirectory(apacheLogsDirectory_Downloads);
            Directory.CreateDirectory(apacheLogsDirectory_Schema);
            Directory.CreateDirectory(apacheLogsDirectory_server);
        }

        static void CreateFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var myFile = File.Create(filePath);
                myFile.Close();
            }
        }

        public static string GetBaseFolder()
        {
            SetApacheLogsDirectory("C:\\ApachacheToMySQL");
            string baseFld = apacheLogsDirectory_server + "BaseFolder.txt";
            StreamReader baseFolder = new StreamReader(baseFld); 
            
            string folderPath = baseFolder.ReadToEnd();    

            baseFolder.Close();
            
            return folderPath;
        }

        public static string GetServerName()
        {
            try
            {
                string serv = apacheLogsDirectory_server + "Server.txt";
                StreamReader server = new StreamReader(serv);

                string serverName = server.ReadToEnd();

                server.Close();

                return serverName;
            }
            catch 
            {
                return "";
            }
        }

        public static void UpdateBaseFolder(string baseFolder)
        {
            string baseFld = apacheLogsDirectory_server + "BaseFolder.txt";
            StreamWriter writer = new StreamWriter(baseFld);

            writer.Flush();
            writer.Write(baseFolder); 
            writer.Close();
        }

        public static void UpdateServerName(string serverName)
        {
            SetApacheLogsDirectory(GetBaseFolder());
            string serv = apacheLogsDirectory_server + "Server.txt";
            StreamWriter writer = new StreamWriter(serv);

            writer.Flush();
            writer.Write(serverName);
            writer.Close();
        }

        public static void CreateSchemaTextAndOtherTexts()
        {
            string baseFolder = "";
            try
            {
                baseFolder = GetBaseFolder();
            }
            catch { }
            baseFolder = !string.IsNullOrEmpty(baseFolder) ? baseFolder : "C:\\ApachacheToMySQL";

            Directory.CreateDirectory(baseFolder);
            SetApacheLogsDirectory(baseFolder);

            string schema = apacheLogsDirectory_Schema + "Schema.txt";
            string server = apacheLogsDirectory_server + "Server.txt";
            //string baseFld = apacheLogsDirectory_server + "BaseFolder.txt";

            Directory.CreateDirectory(apacheLogsDirectory_Schema);
            Directory.CreateDirectory(apacheLogsDirectory_server);

            CreateFile(schema);
            CreateFile(server);

            StreamWriter schemaFile = new StreamWriter(schema);
            
            schemaFile.Write(SchemaHelper.Schema());
            UpdateBaseFolder(baseFolder);

            schemaFile.Close();
        }
    }
}
