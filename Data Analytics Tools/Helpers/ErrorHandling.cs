using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Helpers
{
    public static class ErrorHandling
    {
        public static string GetAzenqosConnectionError(string error)
        {
            string message = "";
            if (error.Contains("401"))
            {
                message = "Incorrect credentials";
            }
            return message;
        }
    }
}
