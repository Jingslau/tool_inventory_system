using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.models;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.rebookItem
{
    /// <summary>
    /// Interaction logic for RebookViewSelection.xaml
    /// </summary>
    public partial class RebookViewSelection : UserControl
    {


        static class SearchParams
        {
            public static int counter = 1;
            public static string filter_1;
            public static string currentSelectedFilter;
            public static List<string> breadcrumbList = new List<string> { "", "", "", "", "", "" };
            public static string filter_1_id;
            public static string selectedItem;
            public static string[] breadcrumbs = new string[5];
            public static string[] SearchParamArr = new string[5];
            public static DataSet AllItems = new DataSet();
            public static DataSet CurrentItems = new DataSet();
        }


        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public RebookViewSelection()
        {


            InitializeComponent();

            SearchParams.counter = 1;
            SearchParams.filter_1 = "";
            SearchParams.currentSelectedFilter = "";
            SearchParams.filter_1_id = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];
            backBtn.IsEnabled = false;
            nextBtn.IsEnabled = false;
            try
            {

                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand("Select * from filter1_names", conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally { conn.Close(); }
            GetAllItems();
        }

        private void GetAllItems()
        {
            try
            {
                string query = $@"SELECT 
	`item_objects`.`item_id`, `item_objects`.`item_ident`, `item_objects`.`item_quantity_total`, `item_objects`.`item_quantity_total_new`, `item_objects`.`item_onetime_use`, `item_objects`.`item_quantity_min`, `item_objects`.`item_orderquant_min`, `item_objects`.`item_orderable`, `item_objects`.`item_onorder`, `item_objects`.`item_description`, `item_objects`.`item_description_2`, `item_objects`.`item_diameter`, `item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_quantity_total > 0
	ORDER BY 
		item_ident ASC;";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "itemData");
                SearchParams.AllItems = ds;
                SearchParams.CurrentItems = ds;
                dataGridItems.DataContext = ds;
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally { conn.Close(); }

        }



        private void filterData_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RebookBtn.IsEnabled = false;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                nextBtn.IsEnabled = true;
                SearchParams.currentSelectedFilter = row_selected["filter_id"].ToString();
                SearchParams.SearchParamArr[SearchParams.counter - 1] = row_selected["filter_id"].ToString();
                List<string> itemsReturnList = new List<string>();
                int index = SearchParams.counter - 1;
                SearchParams.breadcrumbs[SearchParams.counter - 1] = row_selected["name"].ToString();
                SearchParams.breadcrumbList[index] = row_selected["name"].ToString();
                breadcrumbBuild(false);
                conn.Open();


                String stageSearchStr = "";

                for (int i = 1; i <= SearchParams.counter; i++)
                {
                    stageSearchStr += "filter" + i.ToString() + "_id = '" + SearchParams.SearchParamArr[i - 1] + "'";
                    if (i != SearchParams.counter)
                    {
                        stageSearchStr += " AND ";
                    }
                }


                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM item_filter_relations WHERE " + stageSearchStr, conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "itemData");

                dataGridItems.DataContext = ds;

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    strDetailIDList.Add(row["item_id"].ToString());
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }

                string item_ids = string.Format("({0})", string.Join(", ", tmpArr));
                string sql = $@"SELECT 
	`item_objects`.`item_id`, `item_objects`.`item_ident`, `item_objects`.`item_quantity_total`, `item_objects`.`item_quantity_total_new`, `item_objects`.`item_onetime_use`, `item_objects`.`item_quantity_min`, `item_objects`.`item_orderquant_min`, `item_objects`.`item_orderable`, `item_objects`.`item_onorder`, `item_objects`.`item_description`, `item_objects`.`item_description_2`, `item_objects`.`item_diameter`, `item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_id IN {item_ids}
	ORDER BY 
		item_ident ASC;";

                MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2, "itemData");
                dataGridItems.DataContext = ds2;
                SearchParams.CurrentItems = ds2;
                stageSearchStr = "";
                checkButtons(ds2);
                conn.Close();
            }



        }

        private void ItemData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                if (int.Parse(row_selected["item_quantity_total"].ToString()) > 0)
                {
                    RebookBtn.IsEnabled = true;

                }
                else
                {
                    RebookBtn.IsEnabled = false;
                }
                CurrentRebookModel.ItemIdent = row_selected["item_id"].ToString();
                CurrentRebookModel.ItemIdentStr = row_selected["item_ident"].ToString();
                CurrentRebookModel.ItemDescription = row_selected["item_description"].ToString();
                CurrentRebookModel.ItemDescription2 = row_selected["item_description_2"].ToString();
                CurrentRebookModel.ItemDiameter = row_selected["item_diameter"].ToString();
                CurrentRebookModel.ItemTotalQuantity = row_selected["item_quantity_total"].ToString();
                CurrentRebookModel.ItemImagePath = row_selected["item_image_path"].ToString();
                breadcrumbBuild(true);

            }
        }




        private void breadcrumbBuild(Boolean itemSelected)
        {
            breadcrumbData.Text = "";
            if (itemSelected == true)
            {
                breadcrumbData.Text = "❯❯ " + SearchParams.breadcrumbList.ElementAt(0);
                if (SearchParams.counter > 1)
                {
                    for (var i = 1; i < SearchParams.counter; i++)
                    {
                        breadcrumbData.Text += " ❯ " + SearchParams.breadcrumbList.ElementAt(i);
                    }
                }
                breadcrumbData.Text = breadcrumbData.Text + " ➔ " + CurrentRentModel.ItemIdentStr;

            }
            else
            {
                if (SearchParams.counter == 1)
                {
                    breadcrumbData.Text = "❯❯ " + SearchParams.breadcrumbList.ElementAt(0);
                }

                else
                {
                    breadcrumbData.Text = "❯❯ " + SearchParams.breadcrumbList.ElementAt(0);
                    for (var i = 1; i < SearchParams.counter; i++)
                    {
                        breadcrumbData.Text += " ❯ " + SearchParams.breadcrumbList.ElementAt(i);
                    }
                }
            }
        }

        private void getNextStage(object sender, RoutedEventArgs e)
        {
            backBtn.IsEnabled = true;
            if (SearchParams.counter == 1)
            {
                backBtn.IsEnabled = true;

                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row[$"filter2_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter2_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }

                var sql = string.Format("SELECT * FROM filter2_names WHERE filter_id IN ({0})", string.Join(", ", tmpArr));
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds, "filterData");
                dataGridFilter.DataContext = ds;
                conn.Close();


                SearchParams.counter++;
            }
            else if (SearchParams.counter == 2)
            {
                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[$"filter3_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter3_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }


                var sql = string.Format("SELECT * FROM filter3_names WHERE filter_id IN ({0})", string.Join(", ", tmpArr));
                cmd2 = new MySqlCommand(sql, conn);
                adp2 = new MySqlDataAdapter(cmd2);
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();


                checkButtons(ds2);

                SearchParams.counter++;
            }
            else if (SearchParams.counter == 3)
            {
                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[$"filter4_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter4_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }


                var sql = string.Format("SELECT * FROM filter4_names WHERE filter_id IN ('1', '2')");
                cmd2 = new MySqlCommand(sql, conn);
                adp2 = new MySqlDataAdapter(cmd2);
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();


                checkButtons(ds2);

                SearchParams.counter++;
            }
            else if (SearchParams.counter == 4)
            {
                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[$"filter5_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter5_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }


                var sql = string.Format("SELECT * FROM filter5_names WHERE filter_id IN ('1', '2')");
                cmd2 = new MySqlCommand(sql, conn);
                adp2 = new MySqlDataAdapter(cmd2);
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();


                checkButtons(ds2);

                SearchParams.counter++;
            }

        }

        private String BobTheSQLBuilder()
        {
            String returnSQL = "SELECT * FROM item_filter_relations WHERE ";
            for (int i = 0; i < SearchParams.counter; i++)
            {
                returnSQL += $"filter{i + 1}_id = '{SearchParams.SearchParamArr[i]}'";
                if (i < SearchParams.counter - 1)
                {
                    returnSQL += " AND ";
                }
            }



            return returnSQL;
        }

        private void checkButtons(DataSet ds)
        {
            List<string> strDetailIDList = new List<string>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strDetailIDList.Add(row["item_id"].ToString());
            }

            String[] tmpArr = new string[strDetailIDList.Count];
            for (int i = 0; i < strDetailIDList.Count; i++)
            {
                tmpArr[i] = strDetailIDList[i].ToString();
            }
            if (tmpArr.Length == 1)
            {
                nextBtn.IsEnabled = false;
            }
            else
            {
                nextBtn.IsEnabled = true;
            }


        }

        private void getLastStage(object sender, EventArgs e)
        {

            RebookBtn.IsEnabled = false;
            nextBtn.IsEnabled = false;
            backBtn.IsEnabled = false;
            if (SearchParams.counter == 2)
            {
                breadcrumbBuild(true);
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand("Select * from filter1_names", conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                dataGridItems.DataContext = SearchParams.AllItems;
                conn.Close();
                SearchParams.counter--;
                RebookBtn.IsEnabled = false;
                backBtn.IsEnabled = false;
            }
            else if (SearchParams.counter == 3)
            {
                breadcrumbBuild(true);
                getNextStageConst();
                SearchParams.counter--;
                RebookBtn.IsEnabled = false;
                backBtn.IsEnabled = false;
            }


        }
        private void getNextStageConst()
        {
            backBtn.IsEnabled = true;
            if (SearchParams.counter == 1)
            {
                backBtn.IsEnabled = true;

                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row[$"filter2_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter2_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }

                var sql = string.Format("SELECT * FROM filter2_names WHERE filter_id IN ({0})", string.Join(", ", tmpArr));
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds, "filterData");
                dataGridFilter.DataContext = ds;
                conn.Close();


                SearchParams.counter++;
            }
            else if (SearchParams.counter == 2)
            {
                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[$"filter3_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter3_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }


                var sql = string.Format("SELECT * FROM filter3_names WHERE filter_id IN ({0})", string.Join(", ", tmpArr));
                cmd2 = new MySqlCommand(sql, conn);
                adp2 = new MySqlDataAdapter(cmd2);
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();


                checkButtons(ds2);

                SearchParams.counter++;
            }
            else if (SearchParams.counter == 3)
            {
                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[$"filter4_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter4_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }


                var sql = string.Format("SELECT * FROM filter4_names WHERE filter_id IN ('1', '2')");
                cmd2 = new MySqlCommand(sql, conn);
                adp2 = new MySqlDataAdapter(cmd2);
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();


                checkButtons(ds2);

                SearchParams.counter++;
            }
            else if (SearchParams.counter == 4)
            {
                dataGridFilter.DataContext = null;
                dataGridFilter.Items.Refresh();
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(BobTheSQLBuilder(), conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                List<string> strDetailIDList = new List<string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[$"filter5_id"].ToString() != "")
                    {
                        strDetailIDList.Add(row[$"filter5_id"].ToString());
                    }
                }

                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }


                var sql = string.Format("SELECT * FROM filter5_names WHERE filter_id IN ('1', '2')");
                cmd2 = new MySqlCommand(sql, conn);
                adp2 = new MySqlDataAdapter(cmd2);
                adp2.Fill(ds2, "filterData");
                dataGridFilter.DataContext = ds2;
                conn.Close();


                checkButtons(ds2);

                SearchParams.counter++;
            }
        }

        private void resetSearch(object sender, EventArgs e)
        {
            SearchParams.counter = 1;
            SearchParams.filter_1 = "";
            SearchParams.currentSelectedFilter = "";
            SearchParams.filter_1_id = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];

            breadcrumbData.Text = "..::";

            dataGridItems.DataContext = null;
            conn.Open();
            MySqlCommand cmd2 = new MySqlCommand("Select * from filter1_names", conn);
            MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
            DataSet ds2 = new DataSet();
            adp2.Fill(ds2, "filterData");
            dataGridFilter.DataContext = ds2;
            nextBtn.IsEnabled = false;
            backBtn.IsEnabled = false;
            conn.Close();
            GetAllItems();

        }

        private void RebookBtn_Click(object sender, RoutedEventArgs e)
        {
            RebookItemSelectedWindow RebookWindow = new RebookItemSelectedWindow();
            Nullable<bool> dialogResult = RebookWindow.ShowDialog();
            dialogResult = false;

        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = SearchParams.CurrentItems.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in SearchParams.CurrentItems.Tables[0].Rows)
                {
                    if (row["item_ident"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description_2"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_diameter"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                dataGridItems.DataContext = output;
                dataGridItems.ItemsSource = new DataView(output.Tables[0]);
                dataGridFilter.IsEnabled = false;
                backBtn.IsEnabled = false;
                nextBtn.IsEnabled = false;

            }
            else
            {
                dataGridFilter.IsEnabled = true;
                backBtn.IsEnabled = true;
                nextBtn.IsEnabled = true;
                dataGridItems.DataContext = SearchParams.CurrentItems;
                dataGridItems.ItemsSource = new DataView(SearchParams.CurrentItems.Tables[0]);
            }

        }

        private void dataGridItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGridItems.SelectedIndex > -1)
            {
                RebookItemSelectedWindow RebookWindow = new RebookItemSelectedWindow();
                Nullable<bool> dialogResult = RebookWindow.ShowDialog();
                dialogResult = false;
            }
        }
    }

}
