using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store;

namespace waerp_toolpilot.sql
{
    internal class RebookGroupQueries
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());


        public static DataSet GetAllUsedGroups()
        {
            return RunSql("SELECT * FROM location_objects WHERE location_quantity > 0");
        }

        public static DataSet GetSelectedGroup()
        {
            DataSet ds = RunSql($"SELECT * FROM item_location_relations WHERE location_id = {RebookGroupModel.CurrentGroupId}");
            DataSet ItemsDs = RunSql("SELECT * FROM item_objects");
            DataSet tmpDS = new DataSet();
            ItemsDs.Tables[0].Rows.Clear();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                tmpDS = RunSql($"SELECT * FROM item_objects WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}");
                ItemsDs.Tables[0].ImportRow(tmpDS.Tables[0].Rows[0]);
                ItemsDs.Tables[0].Rows[i]["item_quantity_total"] = ds.Tables[0].Rows[i]["location_item_quantity"].ToString();
            }
            return ItemsDs;

        }

        public static DataSet GetItemLocations()
        {
            DataSet tmp = RunSql($"SELECT * FROM item_location_relations WHERE item_id = {CurrentRebookModel.ItemIdent}");
            List<string> strDetailIDList = new List<string>();

            foreach (DataRow row in tmp.Tables[0].Rows)
            {
                strDetailIDList.Add(row["location_id"].ToString());
            }

            String[] tmpArr = new string[strDetailIDList.Count];
            for (int i = 0; i < strDetailIDList.Count; i++)
            {
                tmpArr[i] = strDetailIDList[i].ToString();
            }
            if (tmpArr.Length > 0)
            {
                return RunSql(string.Format("SELECT * FROM location_objects WHERE location_id IN ({0})", string.Join(", ", tmpArr)));
            }
            else
            {
                return new DataSet();
            }
        }
        public static DataSet GetAllLocations()
        {
            return RunSql("SELECT * FROM location_objects");
        }

        public static DataSet GetAllFloors()
        {
            return RunSql("SELECT * FROM floor_objects");
        }

        public static void RebookGroupToZone()
        {
            string MaxIDStr = GetMaxId(RunSql($"SELECT * FROM group_objects"), "group_id");
            string NewGroupName = "P" + MaxIDStr;
            string ItemCount = RunSql($"SELECT * FROM item_location_relations WHERE location_id = {RebookGroupModel.CurrentGroupId}").Tables[0].Rows.Count.ToString();

            RunSqlExec($"INSERT INTO group_objects (group_id, group_name, group_quantity) VALUES ({MaxIDStr}, '{NewGroupName}', {ItemCount})");
            RunSqlExec($"UPDATE location_objects SET location_quantity = 0 WHERE location_id = {RebookGroupModel.CurrentGroupId}");
            RunSqlExec($"INSERT INTO floor_group_objects (id, floor_id, group_id) VALUES ({GetMaxId(RunSql("SELECT * FROM floor_group_objects"), "id")}, {RebookGroupModel.SelectedFloorId}, {MaxIDStr})");
            RunSqlExec($"UPDATE floor_objects SET floor_quantity = floor_quantity + 1 WHERE floor_id = {RebookGroupModel.SelectedFloorId}");

            string MaxIDStrItems = GetMaxId(RunSql($"SELECT * FROM floor_group_item_relations"), "id");
            int MaxID = int.Parse(MaxIDStrItems);

            for (int i = 0; i < RebookGroupModel.CurrentGroupItems.Tables[0].Rows.Count; i++)
            {

                RunSqlExec($"INSERT INTO floor_group_item_relations (id, item_id, group_id, item_quantity) VALUES ({MaxID}, {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["item_id"]}, {MaxIDStr}, {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["item_quantity_total"]})");
                RunSqlExec($"DELETE FROM item_location_relations WHERE item_id = {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["item_id"]} AND location_id = {RebookGroupModel.CurrentGroupId}");
                MaxID++;
            }
            HistoryLogger.CreateHistory("0", "1", RebookGroupModel.CurrentGroupId, "0", "0", MaxIDStr, "8");
            ErrorHandlerModel.ErrorText = $"Die Palette {NewGroupName} wurde erfolgreich in Zone {RebookGroupModel.SelectedFloorId} gelagert! Bitte bewege die Palette nun in die korrekte Zone!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();




        }

        public static DataSet GetLocationsToRebook()
        {
            DataSet tmp = RunSql($"SELECT * FROM location_objects WHERE location_id != {RebookGroupModel.CurrentGroupId}");

            tmp.Tables[0].Columns.Add("IsEmpty");

            for (int i = 0; i < tmp.Tables[0].Rows.Count; i++)
            {
                if (int.Parse(tmp.Tables[0].Rows[i]["location_quantity"].ToString()) > 0)
                {
                    tmp.Tables[0].Rows[i]["IsEmpty"] = 0;
                }
                else
                {
                    tmp.Tables[0].Rows[i]["IsEmpty"] = 1;
                }
            }

            return tmp;

        }


        public static DataSet GetLocationsToRebookGroup()
        {
            DataSet tmp = RunSql($"SELECT * FROM location_objects WHERE location_quantity = 0");


            return tmp;

        }



        public static void RebookGroup()
        {
            if (RebookGroupModel.IsEmpty)
            {
                DataSet ds = RunSql($"SELECT * FROM item_location_relations WHERE location_id = {RebookGroupModel.CurrentGroupId}");
                RunSqlExec($"UPDATE location_objects SET location_quantity = {RebookGroupModel.CurrentGroupQuantity} WHERE location_id = {RebookGroupModel.NewGroupId}");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    RunSqlExec($"UPDATE item_location_relations SET location_id = {RebookGroupModel.NewGroupId} WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]} AND location_id = {RebookGroupModel.CurrentGroupId} AND location_item_quantity = {ds.Tables[0].Rows[i]["location_item_quantity"]}");
                }
                RunSqlExec($"UPDATE location_objects SET location_quantity = 0 WHERE location_id = {RebookGroupModel.CurrentGroupId}");
                HistoryLogger.CreateHistory("0", "1", RebookGroupModel.CurrentGroupId, RebookGroupModel.NewGroupId, "0", "0", "8");
            }
            else
            {
                DataSet ds1 = RunSql($"SELECT * FROM item_location_relations WHERE location_id = {RebookGroupModel.CurrentGroupId}");
                DataSet ds2 = RunSql($"SELECT * FROM item_location_relations WHERE location_id = {RebookGroupModel.NewGroupId}");
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    RunSqlExec($"UPDATE item_location_relations SET location_id = {RebookGroupModel.NewGroupId} WHERE item_id = {ds1.Tables[0].Rows[i]["item_id"]} AND location_id = {RebookGroupModel.CurrentGroupId} AND location_item_quantity = {ds1.Tables[0].Rows[i]["location_item_quantity"]}");
                }
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    RunSqlExec($"UPDATE item_location_relations SET location_id = {RebookGroupModel.CurrentGroupId} WHERE item_id = {ds2.Tables[0].Rows[i]["item_id"]} AND location_id = {RebookGroupModel.NewGroupId} AND location_item_quantity = {ds2.Tables[0].Rows[i]["location_item_quantity"]}");
                }
                RunSqlExec($"UPDATE location_objects SET location_quantity = {RebookGroupModel.CurrentGroupQuantity} WHERE location_id = {RebookGroupModel.NewGroupId}");
                RunSqlExec($"UPDATE location_objects SET location_quantity = {RebookGroupModel.QuantityNewLocation} WHERE location_id = {RebookGroupModel.CurrentGroupId}");
                HistoryLogger.CreateHistory("0", "1", RebookGroupModel.CurrentGroupId, RebookGroupModel.NewGroupId, "0", "0", "8");
            }
        }

        public static void RebookFloorGroup()
        {
            RunSqlExec($"UPDATE location_objects SET location_quantity = {RebookGroupModel.CurrentGroupQuantity} WHERE location_id = {RebookGroupModel.NewGroupId}");
            for (int i = 0; i < RebookGroupModel.CurrentGroupItems.Tables[0].Rows.Count; i++)
            {
                DataSet tmp = RunSql($"SELECT * FROM item_location_relations WHERE item_id = {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["item_id"]} AND location_id =  {RebookGroupModel.NewGroupId}");
                if (tmp.Tables[0].Rows.Count > 0)
                {
                    RunSqlExec($"UPDATE item_location_relations SET location_item_quantity = location_item_quantity + {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["GroupQuantity"]} WHERE item_id = {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["item_id"]} AND location_id =  {RebookGroupModel.NewGroupId}");
                }
                else
                {
                    string MaxIdStr = GetMaxId(RunSql("SELECT * FROM item_location_relations"), "id");
                    RunSqlExec($"INSERT INTO item_location_relations (id, item_id, location_id, location_item_quantity) VALUES ({MaxIdStr}, {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["item_id"]},  {RebookGroupModel.NewGroupId}, {RebookGroupModel.CurrentGroupItems.Tables[0].Rows[i]["GroupQuantity"]}) ");
                }

            }
            RunSqlExec($"DELETE FROM floor_group_objects WHERE group_id = {TempLocationsModel.GroupID}");
            RunSqlExec($"UPDATE floor_objects SET floor_quantity = floor_quantity-1 WHERE floor_id = {TempLocationsModel.FloorID}");
            RunSqlExec($"DELETE FROM floor_group_item_relations WHERE group_id = {TempLocationsModel.GroupID}");
            RunSqlExec($"UPDATE group_objects SET group_quantity = 0 WHERE group_id = {TempLocationsModel.GroupID}");
            HistoryLogger.CreateHistory("0", "1", "0", RebookGroupModel.NewGroupId, TempLocationsModel.FloorID, "0", "8");
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
