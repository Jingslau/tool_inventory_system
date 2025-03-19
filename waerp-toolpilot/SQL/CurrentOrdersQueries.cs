using System.Data;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;

namespace waerp_toolpilot.sql
{
    internal class CurrentOrdersQueries
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());


        public static DataSet GetAllVendors()
        {

            DataSet ds = RunSql("SELECT * FROM vendor_objects");
            DataSet ds2 = ds.Copy();
            ds2.Clear();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataSet tmp = RunSql($"SELECT * FROM order_item_relations WHERE vendor_id = {ds.Tables[0].Rows[i]["vendor_id"]} AND isOpen = 1");

                if (tmp.Tables[0].Rows.Count > 0)
                {
                    ds2.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                }
            }
            return ds2;
        }

        public static bool DeleteOrder()
        {
            RunSqlExec($"DELETE FROM order_item_relations WHERE item_id = {OrderStore.item_id} AND order_ident = '{OrderStore.OrderIdent}'");
            DataSet ds2 = RunSql($"SELECT * FROM order_item_relations WHERE order_ident = '{OrderStore.OrderIdent}'");
            if (ds2.Tables[0].Rows.Count == 0)
            {
                RunSqlExec($"DELETE FROM order_objects WHERE order_ident = '{OrderStore.OrderIdent}'");
            }

            DataSet ds = RunSql($"SELECT * FROM order_item_relations WHERE item_id = {OrderStore.item_id} AND isOpen = 1");
            if (ds.Tables[0].Rows.Count == 0)
            {
                RunSqlExec($"UPDATE item_objects SET item_onorder = 0 WHERE item_id = {OrderStore.item_id}");
            }
            return true;

        }

        public static bool CheckIfUsed(string location_id)
        {
            if (RunSql($"SELECT * FROM item_location_relations WHERE location_id = {location_id}").Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void EditOrder(string newQuant, string item_id)
        {
            RunSqlExec($"UPDATE order_item_relations SET order_quantity = {newQuant} WHERE item_id = {item_id}");
        }


        public static DataSet GetOrderedItems(string vendor_id)
        {
            DataSet ds = RunSql($"SELECT * FROM order_item_relations WHERE vendor_id = {vendor_id} AND isOpen = 1");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Add("item_ident");
                ds.Tables[0].Columns.Add("item_description");
                ds.Tables[0].Columns.Add("item_description_2");
                ds.Tables[0].Columns.Add("item_diameter");
                ds.Tables[0].Columns.Add("item_image_path");



                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string query = $@"SELECT 
	`item_objects`.`item_id`, `item_objects`.`item_ident`, `item_objects`.`item_quantity_total`, `item_objects`.`item_quantity_total_new`, `item_objects`.`item_onetime_use`, `item_objects`.`item_quantity_min`, `item_objects`.`item_orderquant_min`, `item_objects`.`item_orderable`, `item_objects`.`item_onorder`, `item_objects`.`item_description`, `item_objects`.`item_description_2`, `item_objects`.`item_diameter`, `item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_id = {ds.Tables[0].Rows[i]["item_id"].ToString()}
	ORDER BY 
		item_ident ASC;";

                    DataSet CurrentItem = AdministrationQueries.RunSql(query);

                    if (CurrentItem.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].Rows[i]["item_ident"] = CurrentItem.Tables[0].Rows[0]["item_ident"];
                        ds.Tables[0].Rows[i]["item_description"] = CurrentItem.Tables[0].Rows[0]["item_description"];
                        ds.Tables[0].Rows[i]["item_description_2"] = CurrentItem.Tables[0].Rows[0]["item_description_2"];
                        ds.Tables[0].Rows[i]["item_diameter"] = CurrentItem.Tables[0].Rows[0]["item_diameter"];
                        ds.Tables[0].Rows[i]["item_image_path"] = CurrentItem.Tables[0].Rows[0]["item_image_path"];


                    }

                }
                return ds;
            }
            else
            {
                return null;
            }

        }

        public static string GetMaxIdLocation()
        {
            return GetMaxId(RunSql("SELECT * FROM item_location_relations"), "id");
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
                ErrorHandlerModel.SQLQuery = query;
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
        private static bool RunSqlExec(string query)
        {
            try
            {
                ErrorHandlerModel.SQLQuery = query;
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                ErrorLogger.LogSqlError(ex);
                return false;

            }
            finally { conn.Close(); }
        }
    }
}
