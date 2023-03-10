using System.Linq;

namespace Data_Analytics_Tools.Helpers
{
    public class NameExtractionHelpers
    {
        public static string GetTableNameFromApacheFile(string parquetFileName)
        {
            var data = parquetFileName.Split("-");
            string tableName = data[data.Length - 1].Split(".")[0];

            return tableName;
        }
    }
}
