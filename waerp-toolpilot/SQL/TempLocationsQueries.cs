using System.Data;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store;

namespace waerp_toolpilot.sql
{
    internal class TempLocationsQueries
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public static DataSet GetFloorZones()
        {
            return RunSql("SELECT * FROM floor_objects");
        }

        public static void RebookItem(string oldID, string itemIdent, string quantity, string newID, string oldIsUsed, string compartmentId, bool locationEmpty, bool isNewLocation)
        {
            if (locationEmpty)
            {
                RunSqlExec($"DELETE FROM compartment_item_relations WHERE location_id = {oldID}");
            }
            else
            {
                RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = item_quantity - {quantity} WHERE location_id = {oldID}");
            }

            if (isNewLocation)
            {
                RunSqlExec($"INSERT INTO compartment_item_relations (compartment_id, item_id, item_quantity, item_constructed, item_used) VALUES (" +
                    $"{compartmentId}, {itemIdent}, {quantity}, 0, {oldIsUsed}" +
                    $")");

                string locationId = RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {compartmentId}").Tables[0].Rows[0]["location_id"].ToString();
                HistoryLogger.CreateHistory(itemIdent, quantity, oldID, locationId, "0", "0", "3");
            }
            else
            {
                RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = item_quantity + {quantity} WHERE location_id = {newID}");
                HistoryLogger.CreateHistory(itemIdent, quantity, oldID, newID, "0", "0", "3");
            }

        }

        public static DataSet GetEmptyLocation(string itemIdent)
        {
            DataSet output = new DataSet();

            // Build optimized SQL query with JOINs to fetch all required data in one go
            string sqlQuery = $@"
        SELECT co.compartment_id, co.compartment_name, co.is_dynamic, co.onlyUsed, co.onlyNew,
               do.drawer_name,
               con.container_name_short, con.container_name_long
        FROM compartment_objects co
        LEFT JOIN compartment_item_relations cir ON co.compartment_id = cir.compartment_id
        LEFT JOIN drawer_objects do ON co.drawer_id = do.drawer_id
        LEFT JOIN container_objects con ON do.container_id = con.container_id
        WHERE cir.compartment_id IS NULL 
            OR (co.is_dynamic = 1 AND co.reserved_item_id = {itemIdent})
        ORDER BY co.is_dynamic;
    ";

            // Execute the optimized SQL query
            DataSet compartmentInfo = RunSql(sqlQuery);

            // Create the resulting DataTable
            DataTable dtTmp = new DataTable();
            dtTmp.Columns.Add("container_name", typeof(string));
            dtTmp.Columns.Add("compartment_id", typeof(string));
            dtTmp.Columns.Add("container_name_long", typeof(string));
            dtTmp.Columns.Add("drawer_name", typeof(string));
            dtTmp.Columns.Add("compartment_name", typeof(string));
            dtTmp.Columns.Add("item_quantity", typeof(string)); // Retained for future use
            dtTmp.Columns.Add("location_name", typeof(string));
            dtTmp.Columns.Add("item_used", typeof(string)); // Retained for future use
            dtTmp.Columns.Add("location_id", typeof(string)); // Retained for future use
            dtTmp.Columns.Add("location_name_long", typeof(string)); // Retained for future use
            dtTmp.Columns.Add("is_dynamic", typeof(string));
            dtTmp.Columns.Add("onlyUsed", typeof(string));
            dtTmp.Columns.Add("onlyNew", typeof(string));

            // Populate the DataTable with results
            foreach (DataRow row in compartmentInfo.Tables[0].Rows)
            {
                DataRow drTmp = dtTmp.NewRow();

                drTmp["container_name"] = row["container_name_short"];
                drTmp["container_name_long"] = row["container_name_long"];
                drTmp["drawer_name"] = row["drawer_name"];
                drTmp["compartment_name"] = row["compartment_name"];
                drTmp["compartment_id"] = row["compartment_id"];
                drTmp["location_name"] = $"{row["container_name_short"]}-{row["drawer_name"]}-{row["compartment_name"]}";
                drTmp["is_dynamic"] = row["is_dynamic"];
                drTmp["onlyUsed"] = row["onlyUsed"];
                drTmp["onlyNew"] = row["onlyNew"];

                dtTmp.Rows.Add(drTmp);
            }

            // Add the populated DataTable to the DataSet
            output.Tables.Add(dtTmp);

            return output;
        }


        public static string GetLocationNameById(string id)
        {

            DataSet compartment = RunSql($"SELECT * FROM compartment_objects WHERE compartment_id = '{id}'");
            string output = "";
            if (compartment.Tables[0].Rows.Count > 0)
            {
                DataSet drawerInfo = RunSql($"SELECT * FROM drawer_objects WHERE drawer_id = {compartment.Tables[0].Rows[0]["drawer_id"]}");
                DataSet containerInfo = RunSql($"SELECT * FROM container_objects WHERE container_id = {drawerInfo.Tables[0].Rows[0]["container_id"]}");


                output = containerInfo.Tables[0].Rows[0]["container_name_short"].ToString() + ":" + drawerInfo.Tables[0].Rows[0]["drawer_name"].ToString() + ":" + compartment.Tables[0].Rows[0]["compartment_name"].ToString();

            }

            return output;
        }

        public static DataSet GetOnlyNewLocations(string itemID)
        {
            DataSet output = new DataSet();

            // Build optimized SQL query with JOINs
            string sqlQuery = $@"
        SELECT cir.location_id, cir.item_quantity, cir.item_used, 
               co.compartment_id, co.compartment_name, co.is_dynamic, co.onlyUsed, co.onlyNew,
               do.drawer_name,
               con.container_name_short, con.container_name_long
        FROM compartment_item_relations cir
        INNER JOIN compartment_objects co ON cir.compartment_id = co.compartment_id
        INNER JOIN drawer_objects do ON co.drawer_id = do.drawer_id
        INNER JOIN container_objects con ON do.container_id = con.container_id
        WHERE cir.item_id = '{itemID}' AND cir.item_used = 0;
    ";

            // Execute the optimized SQL query
            DataSet itemLocations = RunSql(sqlQuery);

            if (itemLocations.Tables[0].Rows.Count > 0)
            {
                // Create the resulting DataTable
                DataTable dtTmp = new DataTable();
                dtTmp.Columns.Add("container_name", typeof(string));
                dtTmp.Columns.Add("container_name_long", typeof(string));
                dtTmp.Columns.Add("drawer_name", typeof(string));
                dtTmp.Columns.Add("compartment_name", typeof(string));
                dtTmp.Columns.Add("compartment_id", typeof(string));
                dtTmp.Columns.Add("item_quantity", typeof(string));
                dtTmp.Columns.Add("location_name", typeof(string));
                dtTmp.Columns.Add("item_used", typeof(string));
                dtTmp.Columns.Add("location_id", typeof(string));
                dtTmp.Columns.Add("location_name_long", typeof(string)); // Retained for future use
                dtTmp.Columns.Add("is_dynamic", typeof(string));
                dtTmp.Columns.Add("onlyUsed", typeof(string));
                dtTmp.Columns.Add("onlyNew", typeof(string));

                // Populate the DataTable with results
                foreach (DataRow row in itemLocations.Tables[0].Rows)
                {
                    DataRow drTmp = dtTmp.NewRow();
                    drTmp["location_id"] = row["location_id"];
                    drTmp["container_name"] = row["container_name_short"];
                    drTmp["container_name_long"] = row["container_name_long"];
                    drTmp["drawer_name"] = row["drawer_name"];
                    drTmp["compartment_name"] = row["compartment_name"];
                    drTmp["location_name"] = $"{row["container_name_short"]}-{row["drawer_name"]}-{row["compartment_name"]}";
                    drTmp["item_quantity"] = row["item_quantity"];
                    drTmp["compartment_id"] = row["compartment_id"];
                    drTmp["item_used"] = row["item_used"];
                    drTmp["is_dynamic"] = row["is_dynamic"];
                    drTmp["onlyUsed"] = row["onlyUsed"];
                    drTmp["onlyNew"] = row["onlyNew"];

                    dtTmp.Rows.Add(drTmp);
                }

                output.Tables.Add(dtTmp);
            }

            return output;
        }

        public static DataSet GetItemLocations(string itemID)
        {
            DataSet output = new DataSet();

            // Build optimized SQL query with JOINs
            string sqlQuery = $@"
        SELECT cir.location_id, cir.item_quantity, cir.item_used, 
               co.compartment_id, co.compartment_name, co.is_dynamic, co.onlyUsed, co.onlyNew,
               do.drawer_name,
               con.container_name_short, con.container_name_long
        FROM compartment_item_relations cir
        INNER JOIN compartment_objects co ON cir.compartment_id = co.compartment_id
        INNER JOIN drawer_objects do ON co.drawer_id = do.drawer_id
        INNER JOIN container_objects con ON do.container_id = con.container_id
        WHERE cir.item_id = '{itemID}';
    ";

            // Execute the optimized SQL query
            DataSet itemLocations = RunSql(sqlQuery);

            if (itemLocations.Tables[0].Rows.Count > 0)
            {
                // Create the resulting DataTable
                DataTable dtTmp = new DataTable();
                dtTmp.Columns.Add("container_name", typeof(string));
                dtTmp.Columns.Add("container_name_long", typeof(string));
                dtTmp.Columns.Add("drawer_name", typeof(string));
                dtTmp.Columns.Add("compartment_name", typeof(string));
                dtTmp.Columns.Add("compartment_id", typeof(string));
                dtTmp.Columns.Add("item_quantity", typeof(string));
                dtTmp.Columns.Add("location_name", typeof(string));
                dtTmp.Columns.Add("item_used", typeof(string));
                dtTmp.Columns.Add("location_id", typeof(string));
                dtTmp.Columns.Add("location_name_long", typeof(string)); // Retained for future use
                dtTmp.Columns.Add("is_dynamic", typeof(string));
                dtTmp.Columns.Add("onlyUsed", typeof(string));
                dtTmp.Columns.Add("onlyNew", typeof(string));

                // Populate the DataTable with results
                foreach (DataRow row in itemLocations.Tables[0].Rows)
                {
                    DataRow drTmp = dtTmp.NewRow();
                    drTmp["location_id"] = row["location_id"];
                    drTmp["container_name"] = row["container_name_short"];
                    drTmp["container_name_long"] = row["container_name_long"];
                    drTmp["drawer_name"] = row["drawer_name"];
                    drTmp["compartment_name"] = row["compartment_name"];
                    drTmp["location_name"] = $"{row["container_name_short"]}-{row["drawer_name"]}-{row["compartment_name"]}";
                    drTmp["item_quantity"] = row["item_quantity"];
                    drTmp["compartment_id"] = row["compartment_id"];
                    drTmp["item_used"] = row["item_used"];
                    drTmp["is_dynamic"] = row["is_dynamic"];
                    drTmp["onlyUsed"] = row["onlyUsed"];
                    drTmp["onlyNew"] = row["onlyNew"];

                    dtTmp.Rows.Add(drTmp);
                }

                output.Tables.Add(dtTmp);
            }

            return output;
        }


        public static DataSet GetZoneGroups()
        {
            DataSet ds = RunSql("SELECT * FROM group_objects");
            ds.Tables[0].Rows.Clear();
            DataSet tmp = RunSql($"SELECT * FROM floor_group_objects WHERE floor_id = {TempLocationsModel.FloorID}");
            for (int i = 0; i < tmp.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].ImportRow(RunSql($"SELECT * FROM group_objects WHERE group_id = {tmp.Tables[0].Rows[i]["group_id"].ToString()}").Tables[0].Rows[0]);
            }

            return ds;
        }

        public static void SyncLocations()
        {
            DataSet ds = RunSql("SELECT * FROM compartment_item_relations");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["item_used"].ToString() == "0")
                {
                    RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new + {ds.Tables[0].Rows[i]["item_quantity"]} WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}");
                }
                RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {ds.Tables[0].Rows[i]["item_quantity"]}  WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}");
            }
        }

        public static DataSet GetItemsInGroup()
        {
            DataSet ds = RunSql($"SELECT * FROM floor_group_item_relations WHERE group_id = {TempLocationsModel.GroupID}");
            DataSet output = RunSql("SELECT * FROM item_objects");
            output.Tables[0].Rows.Clear();
            output.Tables[0].Columns.Add("GroupQuantity");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                output.Tables[0].ImportRow(RunSql($"SELECT * FROM item_objects WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}").Tables[0].Rows[0]);
                output.Tables[0].Rows[i]["GroupQuantity"] = ds.Tables[0].Rows[i]["item_quantity"].ToString();
            }
            return output;
        }
        public static bool DeleteGroup()
        {
            RunSqlExec($"DELETE FROM group_objects WHERE group_id = {TempLocationsModel.GroupID} ");
            string floorId = RunSql($"SELECT * FROM floor_group_objects WHERE group_id = {TempLocationsModel.GroupID}").Tables[0].Rows[0]["floor_id"].ToString();
            RunSqlExec($"DELETE FROM floor_group_objects WHERE floor_id = {floorId} AND group_id = {TempLocationsModel.GroupID}");
            RunSqlExec($"UPDATE floor_objects SET floor_quantity = floor_quantity - 1 WHERE floor_id = {floorId}");

            DataSet groupItems = RunSql($"SELECT * FROM floor_group_item_relations WHERE group_id = {TempLocationsModel.GroupID}");
            for (int i = 0; i < groupItems.Tables[0].Rows.Count; i++)
            {

                RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total - {groupItems.Tables[0].Rows[i]["item_quantity"]} WHERE item_id = {groupItems.Tables[0].Rows[i]["item_id"]}");

            }
            RunSqlExec($"DELETE FROM floor_group_item_relations WHERE group_id = {TempLocationsModel.GroupID}");
            return true;
        }
        public static void DeleteItemFromGroup()
        {
            RunSqlExec($"DELETE FROM floor_group_item_relations WHERE item_id = {CurrentRentModel.ItemIdent} AND group_id = {TempLocationsModel.GroupID} ");
            RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total - {TempLocationsModel.ItemQuant} WHERE item_id = {CurrentRentModel.ItemIdent}");


            DataSet GroupSelected = RunSql($"SELECT * FROM group_objects WHERE group_id = {TempLocationsModel.GroupID}");
            if ((int.Parse(GroupSelected.Tables[0].Rows[0]["group_quantity"].ToString()) - int.Parse(TempLocationsModel.ItemQuant)) <= 0)
            {
                RunSqlExec($"DELETE FROM group_objects WHERE group_id = {TempLocationsModel.GroupID}");
                RunSqlExec($"DELETE FROM floor_group_objects WHERE group_id = {TempLocationsModel.GroupID} AND floor_id = {TempLocationsModel.FloorID}");
                RunSqlExec($"UPDATE floor_objects SET floor_quantity = floor_quantity - 1 WHERE floor_id = {TempLocationsModel.FloorID}");
            }
            else
            {
                RunSqlExec($"UPDATE group_objects SET group_quantity = group_quantity - {TempLocationsModel.ItemQuant} WHERE group_id = {TempLocationsModel.GroupID}");
            }

            HistoryLogger.CreateHistory(CurrentRentModel.ItemIdent, TempLocationsModel.ItemQuant, "0", "0", TempLocationsModel.GroupID, "0", "6");

            ErrorHandlerModel.ErrorText = $"Der Artikel {TempLocationsModel.ItemName} wurde erfolgreich von der Palette entfernt.";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();

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
                ErrorHandlerModel.SQLQuery = query;
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();

            }
            catch (MySqlException ex)
            {
                ErrorLogger.LogSqlError(ex);

            }
            finally { conn.Close(); }
        }
    }
}
