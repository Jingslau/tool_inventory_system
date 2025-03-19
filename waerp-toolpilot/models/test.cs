using System.Data;
using MySqlConnector;
using waerp_toolpilot.errorHandling;

namespace waerp_toolpilot.models
{
    internal class test
    {


        public static void SyncSchmidtsDB()
        {
            DataSet locations = RunSql("SELECT * FROM locations");

            for (int i = 0; i < locations.Tables[0].Rows.Count; i++)
            {
                RunSqlExec($"UPDATE tools SET Total_Quantity = Total_Quantity + {locations.Tables[0].Rows[i]["Bestand"]} WHERE Art_Nr = '{locations.Tables[0].Rows[i]["Art_Nr"]}'");
                if (locations.Tables[0].Rows[i]["Nachgearbeitet_Kz"].ToString() == "FALSE")
                {
                    RunSqlExec($"UPDATE tools SET Total_Quantity_New = Total_Quantity_New + {locations.Tables[0].Rows[i]["Bestand"]} WHERE Art_Nr = '{locations.Tables[0].Rows[i]["Art_Nr"]}'");
                }
            }
        }


        private static DataSet RunSql(string query)
        {
            MySqlConnection conn = new MySqlConnection("Server= localhost;userid=root;password=050582496031dDp!;Database=schmidts_db;AllowZeroDateTime=True");
            try
            {
                conn.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter();
                DataSet ReturnDataSet = new DataSet();
                adp = new MySqlDataAdapter(new MySqlCommand(query, conn));
                adp.Fill(ReturnDataSet);
                conn.Close();
                return ReturnDataSet;
            }
            catch (MySqlException ex)
            {
                ErrorLogger.LogSqlError(ex);
                return null;

            }
            finally { conn.Close(); }
        }
        private static void RunSqlExec(string query)
        {
            MySqlConnection conn = new MySqlConnection("Server= localhost;userid=root;password=050582496031dDp!;Database=schmidts_db;AllowZeroDateTime=True");
            try
            {
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();

            }
            catch (MySqlException)
            {
                //MessageBox.Show("ASSDJSJ " + ex);
                //  ErrorLogger.LogSqlError(ex);

            }
            finally { conn.Close(); }
        }
    }
}
