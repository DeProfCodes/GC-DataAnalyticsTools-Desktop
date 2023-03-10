using Microsoft.Office.Interop.Excel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Microsoft.Office.Interop.Excel.Application;
using Microsoft.Data.SqlClient;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace Data_Analytics_Tools.Helpers
{
    public class SQLToExcelHelper
    {
        private string connection = "Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database=C1_2023;User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";
        //private string connection = "Server=PSONA90ZATCWI\\TDAB;Database=C1_2023;User Id=VDC_user;Password=bmw%%2022Vdc;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";
        private volatile bool isCancelRequested;

        public SQLToExcelHelper()
        {
            isCancelRequested = false;
        }

        public void SetDatabaseName(string databaseName)
        {
            connection = $"Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database={databaseName};User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";
            //connection = $"Server=PSONA90ZATCWI\\TDAB;Database={databaseName};User Id=VDC_user;Password=bmw%%2022Vdc;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";
        }

        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
        public void CancelWork()
        {
            isCancelRequested = true;
        }

        public void ReadSQLResultToExcel(string destinationFolder, string query, string filename)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connection);
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandTimeout = 3600;
                SqlDataReader dr = cmd.ExecuteReader();

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("Sheet1");

                // Loop through the fields and add headers
                var fontBold = workbook.CreateFont();
                fontBold.IsBold = true;
                
                var style = workbook.CreateCellStyle();
                style.SetFont(fontBold);

                IRow row1 = sheet1.CreateRow(0);
                row1.RowStyle = style;

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i);
                    if (name.Contains(","))
                        name = "\"" + name + "\"";

                    var cell = row1.CreateCell(i);
                    cell.SetCellValue(name);
                    cell.CellStyle = style;
                }

                if (isCancelRequested)
                    return;

                // Loop through the rows and output the data
                int rowCount = 1;
                while (dr.Read())
                {
                    if (isCancelRequested)
                        return;

                    IRow rowD = sheet1.CreateRow(rowCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (isCancelRequested)
                            return;

                        var value = "NULL";
                        if (dr[i] is DateTime)
                        {
                            var date = (DateTime)dr[i];
                            value = date.ToString("dd-MM-yyyy HH:mm:ss.FFF");
                        }
                        else if(!string.IsNullOrEmpty(dr[i].ToString()))
                        {
                            value = dr[i].ToString();
                        }
                        rowD.CreateCell(i).SetCellValue(value);
                    }
                    rowCount++;
                }

                FileStream sw = File.Create($"{destinationFolder}\\{filename}");
                workbook.Write(sw);
                sw.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
