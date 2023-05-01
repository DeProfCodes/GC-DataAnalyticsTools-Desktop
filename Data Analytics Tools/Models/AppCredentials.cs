using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Models
{
    public class AppCredentials
    {
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }
        public string SqlServer { get; set; }
        public string SqlDatabase { get; set; }
        public string AzenqosUsername { get; set; }
        public string AzenqosPassword { get; set; }
    }
}
