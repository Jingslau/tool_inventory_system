using System.Data;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store;

namespace waerp_toolpilot.sql
{
    internal class BookItemQueries
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());

        public static void BookItemNewLocation()
        {
            string maxIdStr = GetMaxId(RunSql("SELECT * FROM item_location_relations"), "id");

            RunSqlExec($"INSERT INTO item_location_relations (id, item_id, location_id, location_item_quantity) VALUES ({maxIdStr}, {CurrentRentModel.ItemIdent}, {CurrentReturnModel.ReturnLocationID}, {CurrentReturnModel.ReturnQuantity})");
            RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentRentModel.ItemIdent}");
            RunSqlExec($"UPDATE location_objects SET location_quantity = location_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");
            HistoryLogger.CreateHistory(CurrentRentModel.ItemIdent, CurrentReturnModel.ReturnQuantity, "0", CurrentReturnModel.ReturnLocationID, "0", "0", "4");
        }


        public static void BookItemLocation()
        {
            RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentRentModel.ItemIdent}");

            RunSqlExec($"UPDATE item_location_relations SET location_item_quantity = location_item_quantity + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent} AND location_id = {CurrentReturnModel.ReturnLocationID}");
            RunSqlExec($"UPDATE location_objects SET location_quantity = location_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");
            HistoryLogger.CreateHistory(CurrentRentModel.ItemIdent, CurrentReturnModel.ReturnQuantity, "0", CurrentReturnModel.ReturnLocationID, "0", "0", "4");
        }

        public static bool BookNewItemInput()
        {
            string MaxIDFloorObj = GetMaxId(RunSql("SELECT * FROM floor_group_objects"), "id");
            string MaxIDRelations = GetMaxId(RunSql("SELECT * FROM floor_group_item_relations"), "id");
            DataSet EmptyExistingGroups = RunSql("SELECT * FROM group_objects WHERE group_quantity = 0");
            int NewGroupID = -1;
            if (EmptyExistingGroups.Tables.Count > 0 && EmptyExistingGroups.Tables[0].Rows.Count > 0)
            {
                NewGroupID = int.Parse(EmptyExistingGroups.Tables[0].Rows[0]["group_id"].ToString());
                RunSqlExec($"UPDATE group_objects SET group_quantity = 1 WHERE group_id = {NewGroupID}");
            }
            else
            {
                string NewGroupIDStr = GetMaxId(RunSql("SELECT * FROM group_objects"), "group_id");
                RunSqlExec($"INSERT INTO group_objects (group_id, group_name, group_quantity) VALUES ({NewGroupIDStr}, '{"P" + NewGroupIDStr}', 1)");
                NewGroupID = int.Parse(NewGroupIDStr);
            }


            RunSqlExec($"UPDATE floor_objects SET floor_quantity = floor_quantity + 1 WHERE floor_id = 1");
            RunSqlExec($"INSERT INTO floor_group_objects (id, floor_id, group_id) VALUES ({MaxIDFloorObj},1, {NewGroupID})");
            RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentRentModel.RentQuantity} WHERE item_id = {CurrentRentModel.ItemIdent}");


            if (RunSql($"SELECT * FROM floor_group_item_relations WHERE item_id = {CurrentRentModel.ItemIdent} AND group_id = {NewGroupID}").Tables[0].Rows.Count > 0)
            {
                RunSqlExec($"UPDATE floor_group_item_relations SET item_quantity = item_quantity + {CurrentRentModel.RentQuantity}");
            }
            else
            {
                RunSqlExec($"INSERT INTO floor_group_item_relations (id, item_id, group_id, item_quantity) VALUES ({GetMaxId(RunSql("SELECT * FROM floor_group_item_relations"), "id")}, {CurrentRentModel.ItemIdent}, {NewGroupID}, {CurrentRentModel.RentQuantity})");
            }

            HistoryLogger.CreateHistory(CurrentRentModel.ItemIdent, CurrentRentModel.RentQuantity, "0", "0", "0", "1", "9");
            ErrorHandlerModel.ErrorText = "Die Palette wurde erfolgreich im Wareneingang gespeichert!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            return true;
        }

        public static string GetMaxId(DataSet ds, string Prompt)
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
        private static void RunSqlExec(string query)
        {
            try
            {
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();

            }
            catch (MySqlException ex)
            {
                ErrorHandlerModel.SQLQuery = query;
                ErrorLogger.LogSqlError(ex);

            }
            finally { conn.Close(); }
        }
    }
}
