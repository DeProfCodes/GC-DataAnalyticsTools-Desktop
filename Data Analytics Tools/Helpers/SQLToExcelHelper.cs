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

namespace Data_Analytics_Tools.Helpers
{
    public class SQLToExcelHelper
    {
        private static string connection = "Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database=C1_2023;User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;MultipleActiveResultSets=true";
        private static string destinationFolder = "C:\\Users\\TelkomTemp1\\Source\\Repos\\GC-DataAnalyticsTools-Desktop\\Data Analytics Tools\\Excel Files Output\\";

        private static string GetExcelColumnName(int columnNumber)
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

        public static void SQLToCSV(string query, string filename)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connection);
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
                string filepath = $"{projectDirectory}\\Excel Files Output\\{filename}";

                Application oXL;
                _Workbook oWB;
                _Worksheet oSheet;
                Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;

                //Start Excel and get Application object.
                oXL = new Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (_Workbook)(oXL.Workbooks.Add(""));
                oSheet = (_Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.


                //Format A1:D1 as bold, vertical alignment = center.
                
                StreamWriter fs = new StreamWriter(filepath);

                // Loop through the fields and add headers
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i);
                    if (name.Contains(","))
                        name = "\"" + name + "\"";

                    oSheet.Cells[1, (i + 1)] = name;
                }

                var lastHeaderCol = $"{GetExcelColumnName(dr.FieldCount)}1";
                oSheet.get_Range("A1", lastHeaderCol).Font.Bold = true;
                oSheet.get_Range("A1", lastHeaderCol).VerticalAlignment = XlVAlign.xlVAlignCenter;
                //fs.WriteLine();

                // Loop through the rows and output the data
                int row = 2;
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string value = dr[i].ToString();
                        oSheet.Cells[row, (i + 1)] = value;
                    }
                    row++;
                }
                oXL.Visible = false;
                oXL.UserControl = false;

                oWB.SaveAs($"{destinationFolder}\\{filename}", XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                            false, false, XlSaveAsAccessMode.xlNoChange,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                           );

                oWB.Close();
                oXL.Quit();
                fs.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public static void SQLToCSV2(string query, string filename)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connection);
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandTimeout = 3600;
                SqlDataReader dr = cmd.ExecuteReader();

                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
                string filepath = $"{projectDirectory}\\Excel Files Output\\{filename}";

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("Sheet1");

                // Loop through the fields and add headers
                var fontBold = workbook.CreateFont();
                fontBold.IsBold = true;
                fontBold.Boldweight = (short)FontBoldWeight.Bold;

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

                // Loop through the rows and output the data
                int rowCount = 1;
                while (dr.Read())
                {
                    IRow rowD = sheet1.CreateRow(rowCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string value = dr[i].ToString();
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
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ToExcel2()
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("Name");
            row1.CreateCell(1).SetCellValue("Age");
            row1.CreateCell(2).SetCellValue("City");

            IRow row2 = sheet1.CreateRow(1);
            row2.CreateCell(0).SetCellValue("Ben");
            row2.CreateCell(1).SetCellValue("20");
            row2.CreateCell(2).SetCellValue("xyz");

            IRow row3 = sheet1.CreateRow(2);
            row3.CreateCell(0).SetCellValue("Jack");
            row3.CreateCell(1).SetCellValue("25");
            row3.CreateCell(2).SetCellValue("xyz");

            IRow row4 = sheet1.CreateRow(3);
            row4.CreateCell(0).SetCellValue("Mike");
            row4.CreateCell(1).SetCellValue("45");
            row4.CreateCell(2).SetCellValue("zyx");

            FileStream sw = File.Create("work1_4.13.14.xlsx");
            workbook.Write(sw);
            sw.Close();
        }
    }
}
