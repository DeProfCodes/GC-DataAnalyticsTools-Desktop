using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Helpers
{
    public  class SQL
    {

        SqlConnection Connection;
        SqlCommand command;

        public enum SqlExecutionType
        {
            Scalar,
            NonQuery
        };

        public SQL()
        {
            //Server=<server_IP>\\MSSQLSERVER, port; Uid=root; Pwd=password; Database=database_name
            //string connectionString = "Server=(localdb)\\mssqllocaldb;Database=C1_2023;Trusted_Connection=True;MultipleActiveResultSets=true";
            string connectionString = "Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database=C1_2023;User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;MultipleActiveResultSets=true";
            Connection = new SqlConnection(connectionString);
            command = new SqlCommand() { Connection = Connection };
        }

        public void RunSelectQuery(string query)
        {
            try
            {
                command.CommandText = query;
                command.CommandTimeout = 3600;

                Connection.Close();
                Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(String.Format("{0}", reader["id"]));
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public async Task<int> RunQuery(string query, SqlExecutionType executionType = SqlExecutionType.NonQuery)
        {
            int rows = 0;
            object rowsObj = null;

            try
            {
                command = new SqlCommand(query, Connection);
                command.CommandTimeout = 3600;

                Connection.Close();
                Connection.Open();

                if (executionType == SqlExecutionType.NonQuery)
                {
                    rows = command.ExecuteNonQuery();
                }
                else if (executionType == SqlExecutionType.Scalar)
                {
                    rowsObj = command.ExecuteScalar();
                }
                Connection.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("There is already an object named"))
                {
                    //all good: DROP TABLE and CREATE TABLE
                }
                else if (ex.Message.Contains("Database") && ex.Message.Contains("already exists"))
                {
                    //all good
                }
                else
                {
                    string msg = ex.Message;
                }
            }

            if (executionType == SqlExecutionType.NonQuery)
            {
                return rows;
            }
            else if (executionType == SqlExecutionType.Scalar)
            {
                if (rowsObj != null && rowsObj is int)
                {
                    return (int)rowsObj;
                }
            }
            return -1;
        }
    }
}
