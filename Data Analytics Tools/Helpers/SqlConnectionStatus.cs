using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Helpers
{
    public enum SqlConnectionStatus
    {
        SUCCESS,
        INCORRECT_SERVER_NAME,
        DATABASE_NOT_FOUND,
        INCORRECT_CREDENTIALS,
        OTHER
    }
}
