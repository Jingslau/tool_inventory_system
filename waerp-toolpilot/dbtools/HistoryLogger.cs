using System;
using System.Data;
using MySqlConnector;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.dbtools
{
    internal class HistoryLogger
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public static DateTime Now { get; }


        public static bool CreateHistory(
            string item_id,
            string item_quantity,
            string item_location_old,
            string item_location_new,
            string zone_old,
            string zone_new,
            string action_id)
        {
            DateTime rentDateTime = DateTime.Now;
            string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string item_ident_str = RunSql($"SELECT * FROM item_objects WHERE item_id = {item_id}").Tables[0].Rows[0]["item_ident"].ToString();
            string location_old = "";
            string location_new = "";

            if (item_location_old != "0")
            {
                location_old = TempLocationsQueries.GetLocationNameById(item_location_old);

            }


            if (item_location_new != "0")
            {
                location_new = TempLocationsQueries.GetLocationNameById(item_location_new);

            }

            string username = RunSql($"SELECT * FROM users WHERE user_id = {MainWindowViewModel.UserID}").Tables[0].Rows[0]["username"].ToString();

            return RunSqlExec($"INSERT INTO history_log (history_id, item_id, item_quantity,item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES ({GetMaxId(RunSql($"SELECT * FROM history_log"), "history_id")}, '{item_ident_str}', {item_quantity}, '{location_old}', '{location_new}', {zone_old}, {zone_new}, {action_id}, '{username}', '{sqlFormattedDate}','{sqlFormattedDate}',{1})");

        }

        public static void CreateRentHistory(string item_id, string item_quantity, string item_location_old)
        {
            MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());

            DateTime rentDateTime = DateTime.Now;
            string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            conn.Open();
            string maxID = MaxIdStr;
            MySqlCommand cmd = new MySqlCommand($"INSERT INTO history_log VALUES (history_id = {maxID}, item_id = {item_id}, item_quantity={item_quantity}, item_location_old = '{item_location_old}', item_location_new='', action_id = 1, user_id = {MainWindowViewModel.UserID}, createdAt = '{sqlFormattedDate}', updatedAt = '{sqlFormattedDate}', show_trigger = 1)", conn);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand($"UPDATE history_log SET item_id = {item_id}, item_quantity={item_quantity}, item_location_old = '{item_location_old}', item_location_new='', action_id = 1, user_id = {MainWindowViewModel.UserID}, createdAt = '{sqlFormattedDate}', updatedAt = '{sqlFormattedDate}', show_trigger = 1 WHERE history_id = {maxID}", conn);
            cmd.ExecuteNonQuery();
            conn.Close();

        }


        private static string MaxIdStr
        {
            get
            {

                MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history_log", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                int maxID = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (int.Parse(row["history_id"].ToString()) > maxID)
                    {
                        maxID = int.Parse(row["history_id"].ToString());
                    }
                }
                maxID++;
                return maxID.ToString();
            }
        }
        public static string GetMaxId(DataSet ds, string Prompt)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                return "0";
            }
            else
            {
                int maxID = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (int.Parse(row[Prompt].ToString()) > maxID)
                    {
                        maxID = int.Parse(row[Prompt].ToString());
                    }
                }
                maxID++;
                return maxID.ToString();
            }

        }
        private static DataSet RunSql(string query)
        {
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
                ErrorHandlerModel.SQLQuery = query;
                ErrorLogger.LogSqlError(ex);
                return null;

            }
            finally { conn.Close(); }
        }
        private static bool RunSqlExec(string query)
        {
            try
            {
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();
                return true;

            }
            catch (MySqlException ex)
            {
                ErrorHandlerModel.SQLQuery = query;
                ErrorLogger.LogSqlError(ex);
                return false;

            }
            finally { conn.Close(); }
        }
    }
}
