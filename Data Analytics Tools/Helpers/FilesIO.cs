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
        public static string ReadFileToCompletetion()
        {
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = $"{projectDirectory}\\Folders\\SqlScripts\\5G script.txt";

            // Calling the ReadAllLines() function
            string script = File.ReadAllText(path);

            return script;
        }
    }
}
