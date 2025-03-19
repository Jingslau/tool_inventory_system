using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.models;
using waerp_toolpilot.store;

namespace waerp_toolpilot.sql
{

    internal class RentItemQueries
    {

        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());


        public static DataSet GetAllMachines()
        {
            String que = "Select * from machines";
            return RunSql(que);
        }

        public static DataSet GetAllFilters_1()
        {
            String que = "Select * from filter1_names ORDER BY name ASC";
            return RunSql(que);
        }

        public static DataSet GetItemFilterRelationsSQL(int counter, string[] SearchParams)
        {
            String que = "SELECT * FROM item_filter_relations WHERE ";
            for (int i = 0; i < counter; i++)
            {
                que += $"filter{i + 1}_id = '{SearchParams[i]}'";
                if (i < counter - 1)
                {
                    que += " AND ";
                }
            }



            return RunSql(que);
        }

        public static DataSet GetFilterNames(int filterId, string[] FilterParams)
        {
            String que1 = ($"SELECT * FROM filter{filterId}_names ");
            String que2 = string.Format("WHERE filter_id IN ({0})", string.Join(", ", FilterParams));
            String queFinal = que1 + que2 + "ORDER BY name ASC";
            return RunSql(queFinal);
        }

        public static DataSet GetItemFilterRelations(string StageSearchStr)
        {
            String que = $"SELECT * FROM item_filter_relations WHERE " + StageSearchStr;
            return RunSql(que);
        }

        public static DataSet GetItemDetails(string[] ItemIdArr)
        {
            String que = string.Format(" ({0}) ", string.Join(", ", ItemIdArr));

            String query = $@"SELECT 
	`item_objects`.`item_id`,
	`item_objects`.`item_ident`,
	`item_objects`.`item_quantity_total`,
	`item_objects`.`item_quantity_total_new`,
	`item_objects`.`item_onetime_use`,
	`item_objects`.`item_quantity_min`,
	`item_objects`.`item_orderquant_min`,
	`item_objects`.`item_orderable`,
	`item_objects`.`item_onorder`,
	`item_objects`.`item_description`,
	`item_objects`.`item_description_2`,
	`item_objects`.`item_diameter`,
	`item_objects`.`item_documentation`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_id IN {que}
	ORDER BY 
		item_ident ASC;";
            return RunSql(query);
        }

        public static DataSet GetLocations()
        {
            DataSet ds = RunSql($"SELECT * FROM item_location_relations WHERE item_id = {CurrentRentModel.ItemIdent}");
            List<string> strDetailIDList = new List<string>();

            foreach (DataRow row in ds.Tables[0].Rows)
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
                string que = string.Format("SELECT * FROM location_objects WHERE location_id IN ({0})", string.Join(", ", tmpArr));
                DataSet ds2 = RunSql(que);
                ds2.Tables[0].Columns.Add("item_quantity");
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    ds2.Tables[0].Rows[i]["item_quantity"] = RunSql($"SELECT * FROM item_location_relations WHERE " +
                        $"item_id = {CurrentRentModel.ItemIdent} " +
                        $"AND " +
                        $"location_id = {ds2.Tables[0].Rows[i]["location_id"]}").Tables[0].Rows[0]["location_item_quantity"];
                }
                return ds2;
            }
            else
            {
                return new DataSet();
            }
        }



        public static DataSet GetGroups()
        {
            DataSet ds = RunSql($"SELECT * FROM floor_group_item_relations WHERE item_id = {CurrentRentModel.ItemIdent}");

            ds.Tables[0].Columns.Add("group_name");
            ds.Tables[0].Columns.Add("floor_name");

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string tmpFloorId = RunSql($"SELECT * FROM floor_group_objects WHERE group_id = {ds.Tables[0].Rows[i]["group_id"]}").Tables[0].Rows[0]["floor_id"].ToString();

                    ds.Tables[0].Rows[i]["floor_name"] = RunSql($"SELECT * FROM floor_objects WHERE floor_id = {tmpFloorId}").Tables[0].Rows[0]["floor_name"].ToString();
                    ds.Tables[0].Rows[i]["group_name"] = RunSql($"SELECT * FROM group_objects WHERE group_id = {ds.Tables[0].Rows[i]["group_id"]}").Tables[0].Rows[0]["group_name"].ToString();

                }
                return ds;
            }

            return new DataSet();

        }

        public static Boolean RentItemExec(string NewQuant, string InputQuant, string LocationId, string LocationQuant, string floor_name, string isUsed)
        {
            if (CurrentRentModel.isUsed == "0")
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total={NewQuant}, item_quantity_total_new = item_quantity_total_new - {InputQuant} WHERE item_id={CurrentRentModel.ItemIdent}");

            }
            else
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total={NewQuant} WHERE item_id={CurrentRentModel.ItemIdent}");
            }

            if (int.Parse(LocationQuant) - int.Parse(InputQuant) == 0)
            {
                if (CurrentRentModel.is_dynamic)
                {
                    DataSet locationDS = RunSql($"SELECT * FROM compartment_item_relations WHERE location_id = {LocationId}");
                    DataSet compartment = RunSql($"SELECT * FROM compartment_objects WHERE compartment_id = {locationDS.Tables[0].Rows[0]["compartment_id"]}");

                    if (compartment.Tables[0].Rows[0]["onlyUsed"].ToString() == "0")
                    {
                        RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = 0, item_used = 0 WHERE location_id = {LocationId}");
                    }
                    else
                    {
                        RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = 0, item_used = 1 WHERE location_id = {LocationId}");
                    }


                }
                else
                {
                    RunSqlExec($"DELETE FROM compartment_item_relations WHERE location_id = {LocationId}");
                }

                DataSet ds = RunSql("SELECT * FROM item_rents");
                DateTime rentDateTime = DateTime.Now;
                string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string maxIdStr = GetMaxId(ds, "rent_id");
                RunSqlExec($"INSERT INTO item_rents (rent_id, item_id, user_id, location_id, rent_quantity, machine_id, item_used, createdAt) VALUES ({maxIdStr},  {CurrentRentModel.ItemIdent}, {MainWindowViewModel.UserID}, {CurrentRentModel.compartmentID}, {InputQuant}, {CurrentRentModel.MachineID}, {isUsed}, '{sqlFormattedDate}')");
                HistoryLogger.CreateHistory(CurrentRentModel.ItemIdent, InputQuant, CurrentRentModel.compartmentID, "0", "0", "0", "1");
                return true;
            }
            else
            {
                RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = {LocationQuant} - {InputQuant} WHERE location_id = {LocationId}");

                DataSet ds = RunSql("SELECT * FROM item_rents");
                DateTime rentDateTime = DateTime.Now;
                string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string maxIdStr = GetMaxId(ds, "rent_id");
                RunSqlExec($"INSERT INTO item_rents (rent_id, item_id, user_id, location_id, rent_quantity, machine_id, item_used, createdAt) VALUES ({maxIdStr},  {CurrentRentModel.ItemIdent}, {MainWindowViewModel.UserID}, {CurrentRentModel.compartmentID}, {InputQuant}, {CurrentRentModel.MachineID}, {isUsed}, '{sqlFormattedDate}')");
                HistoryLogger.CreateHistory(CurrentRentModel.ItemIdent, InputQuant, CurrentRentModel.compartmentID, "0", "0", "0", "1");
                return true;
            }
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

        public static DataSet GetLastStage(int counter, string[] filterArr)
        {
            if (counter == 1)
            {
                String que = "SELECT * FROM filter1_names";
                return RunSql(que);
            }
            else
            {
                MessageBox.Show(filterArr.Length.ToString());
                return new DataSet();
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
                MessageBox.Show(query + " : " + ex.Message);
                ErrorHandlerModel.SQLQuery = query;
                ErrorLogger.LogSqlError(ex);
                return false;

            }
            finally { conn.Close(); }
        }
    }
}
