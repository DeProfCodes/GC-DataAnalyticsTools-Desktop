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
        public static Dictionary<string, string> ReadFileToCompletetion()
        {
            Dictionary<string, string> scripts = new Dictionary<string, string>();

            var files = new List<string>()
            {
                "5G script", "Custom  Ping CDR", "Custom Data CDR",
                "Custom Streaming CDR", "Customized Voice CDR", "MO MT Voice CDR",
                "MOS samples perfected", "VDC Custom Data CDR", "VDC Custom Streaming CDR"
            };

            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            foreach (var file in files) 
            {
                var path = $"{projectDirectory}\\Folders\\SqlScripts\\{file}.sql";
                var script = File.ReadAllText(path);
                scripts.Add($"{file}.xlsx", script);
            }
            return scripts;
        }
    }
}
