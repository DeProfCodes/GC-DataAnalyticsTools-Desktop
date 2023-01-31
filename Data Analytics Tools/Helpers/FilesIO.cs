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
        private static string GetMemoryFile()
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            var path = $"{projectDirectory}\\Folders\\memory\\LastKnownDirectories.txt";

            return path;
        }

        public static void SaveDirectories(string source, string destination)
        {
            var path = GetMemoryFile();
            File.WriteAllText(path, $"{source},{destination}"); 
        }

        public static string GetSavedSourceFolder()
        {
            var path = GetMemoryFile();
            var directories = File.ReadAllText(path);

            var data = directories.Split(",");
            
            if(data.Length > 1)
                return data[0];

            return "C:\\";
        }

        public static string GetSavedDestinationFolder()
        {
            var path = GetMemoryFile();
            var directories = File.ReadAllText(path);

            var data = directories.Split(",");

            if (data.Length > 1)
                return data[1];

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
    }
}
