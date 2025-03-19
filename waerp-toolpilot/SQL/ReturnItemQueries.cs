using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.models;
using waerp_toolpilot.store;

namespace waerp_toolpilot.sql
{
    internal class ReturnItemQueries
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());

        public static DataSet GetAllRents()
        {
            DataSet ds = RunSql("SELECT * FROM item_rents");

            DataSet dsTmp = ds.Clone();
            dsTmp.Tables[0].Clear();
            dsTmp.Tables[0].Columns.Add(new DataColumn("username"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("machine_name"));

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int foundIndex = -1;

                for (int j = 0; j < dsTmp.Tables[0].Rows.Count; j++)
                {
                    if (ds.Tables[0].Rows[i]["item_id"].ToString() == dsTmp.Tables[0].Rows[j]["item_id"].ToString())
                    {
                        foundIndex = j;
                    }
                }

                //if (check)
                //{
                //    int insert = Int32.Parse(dsTmp.Tables[0].Rows[foundIndex]["rent_quantity"].ToString()) + Int32.Parse(ds.Tables[0].Rows[i]["rent_quantity"].ToString());
                //    dsTmp.Tables[0].Rows[foundIndex]["rent_quantity"] = insert.ToString();
                //}
                //else
                //{
                dsTmp.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                DataSet user = RunSql($"SELECT * FROM users WHERE user_id = {ds.Tables[0].Rows[i]["user_id"]}");
                dsTmp.Tables[0].Rows[i]["username"] = user.Tables[0].Rows[0]["username"];

                dsTmp.Tables[0].Rows[i]["machine_name"] = RunSql($"SELECT * FROM machines WHERE machine_id = {dsTmp.Tables[0].Rows[i]["machine_id"]}").Tables[0].Rows[0]["name"];
                //}
            }

            dsTmp.Tables[0].Columns.Add(new DataColumn("item_ident"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_description"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_description_2"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_image_path"));

            for (int i = 0; i < dsTmp.Tables[0].Rows.Count; i++)
            {
                DataSet currentItem = RunSql($"SELECT * FROM item_objects WHERE item_id = {dsTmp.Tables[0].Rows[i]["item_id"]}");

                dsTmp.Tables[0].Rows[i]["item_ident"] = currentItem.Tables[0].Rows[0]["item_ident"];
                dsTmp.Tables[0].Rows[i]["item_description"] = currentItem.Tables[0].Rows[0]["item_description"];
                dsTmp.Tables[0].Rows[i]["item_description_2"] = currentItem.Tables[0].Rows[0]["item_description_2"];
                dsTmp.Tables[0].Rows[i]["item_image_path"] = currentItem.Tables[0].Rows[0]["item_image_path"];

            }

            return dsTmp;

        }

        public static DataSet GetAllRentsMachine(string machineID)
        {
            DataSet ds = RunSql($"SELECT * FROM item_rents WHERE machine_id = {machineID}");

            DataSet dsTmp = ds.Clone();
            dsTmp.Tables[0].Clear();
            dsTmp.Tables[0].Columns.Add(new DataColumn("username"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("machine_name"));

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int foundIndex = -1;

                for (int j = 0; j < dsTmp.Tables[0].Rows.Count; j++)
                {
                    if (ds.Tables[0].Rows[i]["item_id"].ToString() == dsTmp.Tables[0].Rows[j]["item_id"].ToString())
                    {
                        foundIndex = j;
                    }
                }

                //if (check)
                //{
                //    int insert = Int32.Parse(dsTmp.Tables[0].Rows[foundIndex]["rent_quantity"].ToString()) + Int32.Parse(ds.Tables[0].Rows[i]["rent_quantity"].ToString());
                //    dsTmp.Tables[0].Rows[foundIndex]["rent_quantity"] = insert.ToString();
                //}
                //else
                //{
                dsTmp.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                DataSet user = RunSql($"SELECT * FROM users WHERE user_id = {ds.Tables[0].Rows[i]["user_id"]}");
                dsTmp.Tables[0].Rows[i]["username"] = user.Tables[0].Rows[0]["username"];

                dsTmp.Tables[0].Rows[i]["machine_name"] = RunSql($"SELECT * FROM machines WHERE machine_id = {dsTmp.Tables[0].Rows[i]["machine_id"]}").Tables[0].Rows[0]["name"];
                //}
            }

            dsTmp.Tables[0].Columns.Add(new DataColumn("item_ident"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_description"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_description_2"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_diameter"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_documentation"));

            dsTmp.Tables[0].Columns.Add(new DataColumn("item_image_path"));

            for (int i = 0; i < dsTmp.Tables[0].Rows.Count; i++)
            {
                DataSet currentItem = RunSql($"SELECT * FROM item_objects WHERE item_id = {dsTmp.Tables[0].Rows[i]["item_id"]}");

                dsTmp.Tables[0].Rows[i]["item_ident"] = currentItem.Tables[0].Rows[0]["item_ident"];
                dsTmp.Tables[0].Rows[i]["item_description"] = currentItem.Tables[0].Rows[0]["item_description"];
                dsTmp.Tables[0].Rows[i]["item_description_2"] = currentItem.Tables[0].Rows[0]["item_description_2"];
                dsTmp.Tables[0].Rows[i]["item_diameter"] = currentItem.Tables[0].Rows[0]["item_diameter"];
                dsTmp.Tables[0].Rows[i]["item_documentation"] = currentItem.Tables[0].Rows[0]["item_documentation"];


                dsTmp.Tables[0].Rows[i]["item_image_path"] = Path.Combine(RunningParameters.imagePath, currentItem.Tables[0].Rows[0]["item_image_path"].ToString());

            }

            return dsTmp;

        }

        public static DataSet GetRentsByUser(string userID)
        {
            DataSet ds = RunSql($"SELECT * FROM item_rents WHERE user_id = {userID}");

            DataSet dsTmp = ds.Clone();
            dsTmp.Tables[0].Clear();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int foundIndex = -1;

                for (int j = 0; j < dsTmp.Tables[0].Rows.Count; j++)
                {
                    if (ds.Tables[0].Rows[i]["item_id"].ToString() == dsTmp.Tables[0].Rows[j]["item_id"].ToString())
                    {
                        foundIndex = j;
                    }
                }

                //if (check)
                //{
                //    int insert = Int32.Parse(dsTmp.Tables[0].Rows[foundIndex]["rent_quantity"].ToString()) + Int32.Parse(ds.Tables[0].Rows[i]["rent_quantity"].ToString());
                //    dsTmp.Tables[0].Rows[foundIndex]["rent_quantity"] = insert.ToString();
                //}
                //else
                //{
                dsTmp.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                //}
            }

            dsTmp.Tables[0].Columns.Add(new DataColumn("item_ident"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_description"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_description_2"));
            dsTmp.Tables[0].Columns.Add(new DataColumn("item_image_path"));

            for (int i = 0; i < dsTmp.Tables[0].Rows.Count; i++)
            {
                DataSet currentItem = RunSql($"SELECT * FROM item_objects WHERE item_id = {dsTmp.Tables[0].Rows[i]["item_id"]}");

                dsTmp.Tables[0].Rows[i]["item_ident"] = currentItem.Tables[0].Rows[0]["item_ident"];
                dsTmp.Tables[0].Rows[i]["item_description"] = currentItem.Tables[0].Rows[0]["item_description"];
                dsTmp.Tables[0].Rows[i]["item_description_2"] = currentItem.Tables[0].Rows[0]["item_description_2"];
                dsTmp.Tables[0].Rows[i]["item_image_path"] = currentItem.Tables[0].Rows[0]["item_image_path"];

            }

            return dsTmp;
        }


        public static DataSet GetAllLocations()
        {
            return RunSql("SELECT * FROM location_objects WHERE location_quantity = 0");
        }

        public static DataSet GetItemLocations()
        {
            DataSet tmp = RunSql($"Select * from item_location_relations WHERE item_id = {CurrentReturnModel.ItemIdent}");
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
                DataSet ds = RunSql(string.Format("SELECT * FROM location_objects WHERE location_id IN ({0})", string.Join(", ", tmpArr)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ds.Tables[0].Rows[i]["location_quantity"] = RunSql($"SELECT * FROM item_location_relations WHERE " +
                        $"item_id = {CurrentReturnModel.ItemIdent} " +
                        $"AND " +
                        $"location_id = {ds.Tables[0].Rows[i]["location_id"]}").Tables[0].Rows[0]["location_item_quantity"];
                }
                return ds;
            }
            else
            {
                return null;
            }
        }
        public static DataSet GetItemFilterRelationsSQL(int counter, string[] SearchParams, string[] itemIDs)
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

            que = que + " AND " + string.Format(" item_id IN ({0})", string.Join(", ", itemIDs));
            return RunSql(que);
        }


        public static void BookItemIntoLocation()
        {
            if (CurrentReturnModel.ReturnIsUsed)
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity}, item_quantity_total_new = item_quantity_total_new + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            }
            else
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            }


            RunSqlExec($"INSERT INTO compartment_item_relations (compartment_id, item_id, item_quantity, item_constructed, item_used) VALUES (" +
             $"{CurrentReturnModel.ReturnLocationID}," +
             $"{CurrentReturnModel.ItemIdent}," +
             $"{CurrentReturnModel.ReturnQuantity}," +
             $"0," +
             $"{CurrentReturnModel.ReturnIsUsed})");


            //string maxIdStr = GetMaxId(RunSql("SELECT * FROM item_location_relations"), "id");

            //RunSqlExec($"INSERT INTO item_location_relations (id, item_id, location_id, location_item_quantity) VALUES ({maxIdStr}, {CurrentReturnModel.ItemIdent}, {CurrentReturnModel.ReturnLocationID}, {CurrentReturnModel.ReturnQuantity})");
            //RunSqlExec($"UPDATE location_objects SET item_used = {isUsed}, location_quantity = location_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");
            HistoryLogger.CreateHistory(CurrentReturnModel.ItemIdent, CurrentReturnModel.ReturnQuantity, "0", CurrentReturnModel.ReturnLocationID, "0", "0", "4");
        }



        public static void BookItemLocation()
        {
            string isUsed = "";
            if (CurrentReturnModel.ReturnIsUsed)
            {
                isUsed = "1";
            }
            else
            {
                isUsed = "0";
            }

            RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = item_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");

            if (isUsed == "1")
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            }

            RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            //RunSqlExec($"UPDATE item_location_relations SET location_item_quantity = location_item_quantity + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent} AND location_id = {CurrentReturnModel.ReturnLocationID}");
            //RunSqlExec($"UPDATE location_objects SET item_used = {isUsed}, location_quantity = location_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");
            HistoryLogger.CreateHistory(CurrentReturnModel.ItemIdent, CurrentReturnModel.ReturnQuantity, "0", CurrentReturnModel.ReturnLocationID, "0", "0", "4");
        }

        public static void ReturnItemLocation(string type)
        {

            string isUsed = "";
            if (CurrentReturnModel.ReturnIsUsed)
            {
                isUsed = "1";
            }
            else
            {
                isUsed = "0";
            }

            RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = item_quantity + {CurrentReturnModel.ReturnQuantity} WHERE compartment_id = {CurrentReturnModel.ReturnLocationID}");

            if (isUsed == "0")
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            }

            RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            //RunSqlExec($"UPDATE item_location_relations SET location_item_quantity = location_item_quantity + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent} AND location_id = {CurrentReturnModel.ReturnLocationID}");
            //RunSqlExec($"UPDATE location_objects SET item_used = {isUsed}, location_quantity = location_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");

            DateTime rentDateTime = DateTime.Now;
            string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string item_ident_str = RunSql($"SELECT * FROM item_objects WHERE item_id = {CurrentReturnModel.ItemIdent}").Tables[0].Rows[0]["item_ident"].ToString();

            if (CurrentReturnModel.currentReturnItem != null)
            {
                DataSet machineId = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE rent_id = {CurrentReturnModel.currentReturnItem["rent_id"]}");
                DataSet machineName = AdministrationQueries.RunSql($"SELECT * FROM machines WHERE machine_id = {machineId.Tables[0].Rows[0]["machine_id"]}");

                string locationName = TempLocationsQueries.GetLocationNameById(CurrentReturnModel.ReturnLocationID);




                if (type == "1")
                {
                    HistoryLogger.CreateHistory(CurrentReturnModel.ItemIdent, CurrentReturnModel.ReturnQuantity, "0", CurrentReturnModel.ReturnLocationID, "0", "0", "4");
                }
                else
                {
                    RunSqlExec($"INSERT INTO history_log (history_id, item_id, item_quantity,item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES ({GetMaxId(RunSql($"SELECT * FROM history_log"), "history_id")}, '{item_ident_str}', {CurrentReturnModel.ReturnQuantity}, '{machineName.Tables[0].Rows[0]["name"].ToString()}', '{locationName}', 0, 0, 2, '{MainWindowViewModel.username}', '{sqlFormattedDate}','{sqlFormattedDate}',{1})");

                }
            }
        }

        public static void ReturnItemNewLocation(string type)
        {


            if (CurrentReturnModel.ReturnIsUsed)
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            }
            else
            {
                RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {CurrentReturnModel.ReturnQuantity}, item_quantity_total_new = item_quantity_total_new + {CurrentReturnModel.ReturnQuantity} WHERE item_id = {CurrentReturnModel.ItemIdent}");
            }



            RunSqlExec($"INSERT INTO compartment_item_relations (compartment_id, item_id, item_quantity, item_constructed, item_used) VALUES (" +
             $"{CurrentReturnModel.ReturnLocationID}," +
             $"{CurrentReturnModel.ItemIdent}," +
             $"{CurrentReturnModel.ReturnQuantity}," +
             $"0," +
             $"{CurrentReturnModel.ReturnIsUsed})");


            //string maxIdStr = GetMaxId(RunSql("SELECT * FROM item_location_relations"), "id");

            //RunSqlExec($"INSERT INTO item_location_relations (id, item_id, location_id, location_item_quantity) VALUES ({maxIdStr}, {CurrentReturnModel.ItemIdent}, {CurrentReturnModel.ReturnLocationID}, {CurrentReturnModel.ReturnQuantity})");
            //RunSqlExec($"UPDATE location_objects SET item_used = {isUsed}, location_quantity = location_quantity + {CurrentReturnModel.ReturnQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");


            DateTime rentDateTime = DateTime.Now;
            string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string item_ident_str = RunSql($"SELECT * FROM item_objects WHERE item_id = {CurrentReturnModel.ItemIdent}").Tables[0].Rows[0]["item_ident"].ToString();

            if (CurrentReturnModel.currentReturnItem != null)
            {
                DataSet machineId = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE rent_id = {CurrentReturnModel.currentReturnItem["rent_id"]}");
                DataSet machineName = AdministrationQueries.RunSql($"SELECT * FROM machines WHERE machine_id = {machineId.Tables[0].Rows[0]["machine_id"]}");

                string locationName = TempLocationsQueries.GetLocationNameById(CurrentReturnModel.ReturnLocationID);




                if (type == "1")
                {
                    HistoryLogger.CreateHistory(CurrentReturnModel.ItemIdent, CurrentReturnModel.ReturnQuantity, "0", CurrentReturnModel.ReturnLocationID, "0", "0", "4");
                }
                else
                {
                    RunSqlExec($"INSERT INTO history_log (history_id, item_id, item_quantity,item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES ({GetMaxId(RunSql($"SELECT * FROM history_log"), "history_id")}, '{item_ident_str}', {CurrentReturnModel.ReturnQuantity}, '{machineName.Tables[0].Rows[0]["name"].ToString()}', '{locationName}', 0, 0, 2, '{MainWindowViewModel.username}', '{sqlFormattedDate}','{sqlFormattedDate}',{1})");

                }
            }

        }


        public static void DeleteRent()
        {
            RunSqlExec($"DELETE FROM item_rents WHERE rent_id = {CurrentReturnModel.currentReturnItem["rent_id"]}");
            //HistoryLogger.CreateHistory(CurrentReturnModel.currentReturnItem["item_id"].ToString(), CurrentReturnModel.currentReturnItem["rent_quantity"].ToString(), "0", "0", "0", "0", "2");
        }

        public static void UpdateRent()
        {
            RunSqlExec($"UPDATE item_rents SET rent_quantity = rent_quantity - {CurrentReturnModel.ReturnQuantity} WHERE rent_id = {CurrentReturnModel.currentReturnItem["rent_id"]}");
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
