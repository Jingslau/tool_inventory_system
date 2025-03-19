using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.RentItem;
using waerp_toolpilot.modules.returnItem;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.returnItem
{
    /// <summary>
    /// Interaction logic for ReturnItemSelect.xaml
    /// </summary>
    static class SearchParams
    {
        public static int counter = 1;
        public static string machine;
        public static string selectedUserID;
        public static string filter_1;
        public static string currentSelectedFilter;
        public static string filter_1_id;
        public static string selectedItem;
        public static string[] breadcrumbs = new string[5];
        public static string[] SearchParamArr = new string[5];
        public static DataSet currentDataItems = new DataSet();

        public static List<string> breadcrumbList = new List<string> { "", "", "", "", "", "" };
        public static DataSet allRents = new DataSet();

        public static DataSet CurrentSelectedUserRents = new DataSet();
    }

    public partial class ReturnItemSelect : UserControl
    {
        DataRow currentSelectedItem;
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public ReturnItemSelect()
        {
            InitializeComponent();
            loadData();

            //DataSet rentItems = ReturnItemQueries.GetAllRents();
            //dataGridItems.DataContext = rentItems;
            //dataGridItems.ItemsSource = new DataView(rentItems.Tables[0]);

        }

        private void loadData()
        {
            SearchParams.counter = 1;
            SearchParams.filter_1 = "";
            SearchParams.currentSelectedFilter = "";
            SearchParams.filter_1_id = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbList = new List<string> { "", "", "", "", "", "" };
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];

            GetAllRents();

            DataSet machines = AdministrationQueries.RunSql("SELECT * FROM machines");

            machines.Tables[0].Columns.Add("quantity");

            for (int i = 0; i < machines.Tables[0].Rows.Count; i++)
            {
                DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE machine_id = {machines.Tables[0].Rows[i]["machine_id"]}");
                if (tmp.Tables[0].Rows.Count > 0)
                {
                    int totalQuant = 0;

                    for (int j = 0; j < tmp.Tables[0].Rows.Count; j++)
                    {
                        totalQuant += int.Parse(tmp.Tables[0].Rows[j]["rent_quantity"].ToString());
                    }

                    machines.Tables[0].Rows[i]["quantity"] = totalQuant.ToString();
                }
                else
                {
                    machines.Tables[0].Rows[i]["quantity"] = "0";
                }
            }

            machineData.DataContext = machines;
            machineData.ItemsSource = new DataView(machines.Tables[0]);

            if (machines.Tables[0].Rows.Count > 0)
            {
                machineData.SelectedIndex = 0;
            }

        }
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (searchBox.Text.Length > 0)
            {
                DataSet output = SearchParams.currentDataItems.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in SearchParams.currentDataItems.Tables[0].Rows)
                {
                    if (row["item_ident"].ToString().Contains(searchBox.Text) | row["item_description"].ToString().Contains(searchBox.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                dataGridItems.DataContext = output;
                dataGridItems.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                dataGridItems.DataContext = SearchParams.currentDataItems;
                dataGridItems.ItemsSource = new DataView(SearchParams.currentDataItems.Tables[0]);
            }


        }
        private void GetAllFilterForSelection()
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT * FROM item_rents WHERE user_id = {SearchParams.selectedUserID}", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            SearchParams.allRents = new DataSet();
            adp.Fill(SearchParams.allRents, "itemData");

            List<string> AllRentsItem = new List<string>();
            for (int i = 0; i < SearchParams.allRents.Tables[0].Rows.Count; i++)
            {
                AllRentsItem.Add(SearchParams.allRents.Tables[0].Rows[i]["item_id"].ToString());
            }
            string[] RentItemsArr = new string[AllRentsItem.Count];
            for (int i = 0; i < AllRentsItem.Count; i++)
            {
                RentItemsArr[i] = AllRentsItem[i].ToString();
            }
            var sql = string.Format("SELECT * FROM item_filter_relations WHERE item_id IN ({0})", string.Join(", ", RentItemsArr));
            cmd = new MySqlCommand(sql, conn);
            adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();


            adp.Fill(ds);

            bool checkFilter = false;
            List<string> FilterIDs = new List<string>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                checkFilter = false;
                for (int j = 0; j < FilterIDs.Count; j++)
                {
                    if (FilterIDs[j] == ds.Tables[0].Rows[i]["filter1_id"].ToString())
                    {
                        checkFilter = true;
                    }
                }
                if (!checkFilter)
                {
                    FilterIDs.Add(ds.Tables[0].Rows[i]["filter1_id"].ToString());
                }
            }

            String[] FilterIDArr = new String[FilterIDs.Count];
            for (int i = 0; i < FilterIDs.Count; i++)
            {
                FilterIDArr[i] = FilterIDs[i].ToString();
            }



            sql = string.Format("SELECT * FROM filter1_names WHERE filter_id IN ({0})", string.Join(", ", FilterIDArr));
            cmd = new MySqlCommand(sql, conn);
            adp = new MySqlDataAdapter(cmd);
            ds = new DataSet();
            adp.Fill(ds, "filterData");
            conn.Close();
            dataGridFilter.DataContext = ds;
        }

        private void GetAllRents()
        {
            SearchParams.counter = 1;
            SearchParams.filter_1 = "";
            SearchParams.currentSelectedFilter = "";
            SearchParams.filter_1_id = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbList = new List<string> { "", "", "", "", "", "" };
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM item_rents", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            SearchParams.allRents = new DataSet();
            adp.Fill(SearchParams.allRents, "itemData");



            List<string> AllRentsItem = new List<string>();
            for (int i = 0; i < SearchParams.allRents.Tables[0].Rows.Count; i++)
            {
                AllRentsItem.Add(SearchParams.allRents.Tables[0].Rows[i]["item_id"].ToString());
            }
            string[] RentItemsArr = new string[AllRentsItem.Count];
            for (int i = 0; i < AllRentsItem.Count; i++)
            {
                RentItemsArr[i] = AllRentsItem[i].ToString();
            }
            if (SearchParams.allRents.Tables[0].Rows.Count > 0)
            {
                var sql = string.Format("SELECT * FROM item_filter_relations WHERE item_id IN ({0})", string.Join(", ", RentItemsArr));
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();


                adp.Fill(ds);

                bool checkFilter = false;
                List<string> FilterIDs = new List<string>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    checkFilter = false;
                    for (int j = 0; j < FilterIDs.Count; j++)
                    {
                        if (FilterIDs[j] == ds.Tables[0].Rows[i]["filter1_id"].ToString())
                        {
                            checkFilter = true;
                        }
                    }
                    if (!checkFilter)
                    {
                        FilterIDs.Add(ds.Tables[0].Rows[i]["filter1_id"].ToString());
                    }
                }

                String[] FilterIDArr = new String[FilterIDs.Count];
                for (int i = 0; i < FilterIDs.Count; i++)
                {
                    FilterIDArr[i] = FilterIDs[i].ToString();
                }



                sql = string.Format("SELECT * FROM filter1_names WHERE filter_id IN ({0})", string.Join(", ", FilterIDArr));
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adp.Fill(ds, "filterData");
                dataGridFilter.DataContext = ds;
                dataGridFilter.ItemsSource = new DataView(ds.Tables[0]);


                List<string> strDetailIDList = new List<string>();

                List<string> UserList = new List<string>();
                bool check = false;

                foreach (DataRow row in SearchParams.allRents.Tables[0].Rows)
                {
                    check = false;
                    foreach (string User in UserList)
                    {
                        if (row["user_id"].ToString() == User)
                        {
                            check = true;
                        }
                    }
                    if (!check)
                    {
                        UserList.Add(row["user_id"].ToString());
                    }
                    strDetailIDList.Add(row["item_id"].ToString());
                }



                String[] tmpArr = new string[strDetailIDList.Count];
                for (int i = 0; i < strDetailIDList.Count; i++)
                {
                    tmpArr[i] = strDetailIDList[i].ToString();
                }

                if (UserList.Count > 0)
                {
                    String[] userArr = new string[UserList.Count];

                    for (int i = 0; i < UserList.Count; i++)
                    {
                        userArr[i] = UserList[i].ToString();
                    }
                    sql = string.Format("SELECT * FROM users WHERE user_id IN ({0})", string.Join(", ", userArr));


                    cmd = new MySqlCommand(sql, conn);
                    adp = new MySqlDataAdapter(cmd);
                    ds = new DataSet();

                    adp.Fill(ds, "userData");

                    userData.DataContext = ds;

                }




                if (tmpArr.Length == 0)
                {
                    SearchParams.allRents = new DataSet();
                    adp.Fill(SearchParams.allRents, "itemData");
                    dataGridItems.DataContext = SearchParams.allRents;
                    SearchParams.currentDataItems = SearchParams.allRents;
                }
                else
                {
                    string item_ids = string.Format(" ({0}) ", string.Join(", ", tmpArr));
                    sql = $@"SELECT 
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
		item_objects WHERE item_id IN {item_ids}
	ORDER BY 
		item_ident ASC;";
                    cmd = new MySqlCommand(sql, conn);
                    adp = new MySqlDataAdapter(cmd);
                    DataSet ds2 = new DataSet();

                    adp.Fill(ds2, "itemData");
                    ds2.Tables[0].Columns.Add("item_used");

                    foreach (DataRow row in SearchParams.allRents.Tables[0].Rows)
                    {
                        foreach (DataRow row2 in ds2.Tables[0].Rows)
                        {

                            if (row["item_id"].ToString() == row2["item_id"].ToString())
                            {
                                row2["item_quantity_total"] = row["rent_quantity"];
                                row2["item_used"] = row["item_used"];
                            }
                        }
                    }

                    dataGridItems.DataContext = ds2;
                    SearchParams.currentDataItems = ds2;
                    SearchParams.CurrentSelectedUserRents = ds2;
                    conn.Close();
                }
                conn.Close();
            }
            else
            {
                toggleUsers.IsEnabled = false;
                searchBox.IsEnabled = false;
            }
            conn.Close();
        }

        private void toggleAllUsers(object sender, RoutedEventArgs e)
        {
            ResetSearchPrompt();
            if (toggleUsers.IsChecked == true)
            {
                searchBox.Text = "";
                searchBox.IsEnabled = true;
                GetAllRents();
                CurrentReturnModel.IsSelectedUser = false;
                CurrentReturnModel.CurrentUserId = "";
                DataSet rentItems = ReturnItemQueries.GetAllRents();
                dataGridItems.DataContext = rentItems;
                dataGridItems.ItemsSource = new DataView(rentItems.Tables[0]);
                userData.IsEnabled = false;
                nextBtn.IsEnabled = false;
                backBtn.IsEnabled = false;
                breadcrumbData.Text = "";
            }
            else
            {
                searchBox.IsEnabled = false;
                searchBox.Text = "";
                userData.IsEnabled = true;
                SearchParams.counter = 1;
                SearchParams.filter_1 = "";
                SearchParams.currentSelectedFilter = "";
                SearchParams.filter_1_id = "";
                SearchParams.selectedItem = "";
                SearchParams.breadcrumbs = new string[5];
                SearchParams.SearchParamArr = new string[5];

                breadcrumbData.Text = string.Empty;
                dataGridFilter.DataContext = null;
                dataGridItems.DataContext = null;
                dataGridFilter.ItemsSource = null;
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand("Select * from filter1_names", conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2, "filterData");
                SearchParams.currentDataItems = ds2;
                dataGridFilter.DataContext = ds2;
                conn.Close();
                nextBtn.IsEnabled = false;
                backBtn.IsEnabled = false;
                breadcrumbData.Text = "";
            }
        }

        private void userData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            SearchParams.counter = 1;
            SearchParams.currentSelectedFilter = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];
            OpenRentBtn.IsEnabled = false;
            DelteRentBtn.IsEnabled = false;


            dataGridItems.DataContext = null;

            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {

                SearchParams.selectedUserID = row_selected["user_id"].ToString();
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from item_rents WHERE user_id = '" + SearchParams.selectedUserID + "'", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                CurrentReturnModel.IsSelectedUser = true;
                CurrentReturnModel.CurrentUserId = row_selected["user_id"].ToString();
                SearchParams.CurrentSelectedUserRents = ds;

                searchBox.Text = "";
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

                if (tmpArr.Length == 0)
                {
                    ds = new DataSet();
                    adp.Fill(ds, "itemData");
                    dataGridItems.DataContext = ds;
                    SearchParams.currentDataItems = ds;
                }
                else
                {
                    string item_ids = string.Format(" ({0}) ", string.Join(", ", tmpArr));
                    var sql = $@"SELECT 
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
		item_objects WHERE item_id IN {item_ids}
	ORDER BY 
		item_ident ASC;";
                    cmd = new MySqlCommand(sql, conn);
                    adp = new MySqlDataAdapter(cmd);
                    DataSet ds2 = new DataSet();
                    adp.Fill(ds2, "itemData");
                    ds2.Tables[0].Columns.Add("item_used");
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        foreach (DataRow row2 in ds2.Tables[0].Rows)
                        {
                            if (row["item_id"].ToString() == row2["item_id"].ToString())
                            {
                                row2["item_quantity_total"] = row["rent_quantity"];
                                row2["item_used"] = row["item_used"];
                            }
                        }
                    }

                    DataSet rentItems = ReturnItemQueries.GetRentsByUser(SearchParams.selectedUserID);
                    dataGridItems.DataContext = rentItems;
                    dataGridItems.ItemsSource = new DataView(rentItems.Tables[0]);

                    List<string> AllRentsItem = new List<string>();
                    for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                    {
                        AllRentsItem.Add(ds2.Tables[0].Rows[i]["item_id"].ToString());
                    }
                    string[] RentItemsArr = new string[AllRentsItem.Count];
                    for (int i = 0; i < AllRentsItem.Count; i++)
                    {
                        RentItemsArr[i] = AllRentsItem[i].ToString();
                    }
                    sql = string.Format("SELECT * FROM item_filter_relations WHERE item_id IN ({0})", string.Join(", ", RentItemsArr));
                    cmd = new MySqlCommand(sql, conn);
                    adp = new MySqlDataAdapter(cmd);
                    ds = new DataSet();


                    adp.Fill(ds);

                    bool checkFilter = false;
                    List<string> FilterIDs = new List<string>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        checkFilter = false;
                        for (int j = 0; j < FilterIDs.Count; j++)
                        {
                            if (FilterIDs[j] == ds.Tables[0].Rows[i]["filter1_id"].ToString())
                            {
                                checkFilter = true;
                            }
                        }
                        if (!checkFilter)
                        {
                            FilterIDs.Add(ds.Tables[0].Rows[i]["filter1_id"].ToString());
                        }
                    }

                    String[] FilterIDArr = new String[FilterIDs.Count];
                    for (int i = 0; i < FilterIDs.Count; i++)
                    {
                        FilterIDArr[i] = FilterIDs[i].ToString();
                    }



                    sql = string.Format("SELECT * FROM filter1_names WHERE filter_id IN ({0})", string.Join(", ", FilterIDArr));
                    cmd = new MySqlCommand(sql, conn);
                    adp = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adp.Fill(ds, "filterData");
                    dataGridFilter.DataContext = ds;
                    dataGridFilter.ItemsSource = new DataView(ds.Tables[0]);
                    conn.Close();
                }



                conn.Close();
                GetAllFilterForSelection();

            }
            conn.Close();
        }
        private void filterData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                OpenRentBtn.IsEnabled = false;
                DelteRentBtn.IsEnabled = false;
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
                adp.Fill(ds);



                dataGridItems.DataContext = ds;
                SearchParams.currentDataItems = ds;
                List<string> strDetailIDList = new List<string>();

                if (toggleUsers.IsChecked == true)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        foreach (DataRow row2 in SearchParams.allRents.Tables[0].Rows)
                        {
                            if (row["item_id"].ToString() == row2["item_id"].ToString())
                            {
                                strDetailIDList.Add(row["item_id"].ToString());
                            }
                        }

                    }

                    String[] tmpArr = new string[strDetailIDList.Count];
                    for (int i = 0; i < strDetailIDList.Count; i++)
                    {
                        tmpArr[i] = strDetailIDList[i].ToString();
                    }


                    if (tmpArr.Length > 0)
                    {
                        string item_ids = string.Format(" ({0}) ", string.Join(", ", tmpArr));
                        var sql = $@"SELECT 
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
		item_objects WHERE item_id IN {item_ids}
	ORDER BY 
		item_ident ASC;";
                        MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                        MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                        DataSet ds2 = new DataSet();
                        adp2.Fill(ds2, "itemData");
                        conn.Close();
                        ds2.Tables[0].Columns.Add("item_used");

                        foreach (DataRow row in ds2.Tables[0].Rows)
                        {
                            foreach (DataRow row2 in SearchParams.allRents.Tables[0].Rows)
                            {
                                if (row["item_id"].ToString() == row2["item_id"].ToString())
                                {
                                    row["item_quantity_total"] = row2["rent_quantity"];
                                    row["item_used"] = row2["item_used"];
                                }
                            }

                        }
                        dataGridItems.DataContext = ds2;
                        SearchParams.currentDataItems = ds2;
                        checkButtons(ds2);
                    }
                    else
                    {
                        dataGridItems.DataContext = null;
                    }
                    conn.Close();
                }
                else
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        foreach (DataRow row2 in SearchParams.CurrentSelectedUserRents.Tables[0].Rows)
                        {
                            if (row["item_id"].ToString() == row2["item_id"].ToString())
                            {
                                strDetailIDList.Add(row["item_id"].ToString());
                            }
                        }

                    }

                    String[] tmpArr = new string[strDetailIDList.Count];
                    for (int i = 0; i < strDetailIDList.Count; i++)
                    {
                        tmpArr[i] = strDetailIDList[i].ToString();
                    }


                    if (tmpArr.Length > 0)
                    {
                        string item_ids = string.Format(" ({0}) ", string.Join(", ", tmpArr));
                        var sql = $@"SELECT 
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
		item_objects WHERE item_id IN {item_ids}
	ORDER BY 
		item_ident ASC;";
                        MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                        MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                        DataSet ds2 = new DataSet();
                        adp2.Fill(ds2, "itemData");
                        ds2.Tables[0].Columns.Add("item_used");
                        conn.Close();

                        foreach (DataRow row in ds2.Tables[0].Rows)
                        {
                            foreach (DataRow row2 in SearchParams.CurrentSelectedUserRents.Tables[0].Rows)
                            {
                                if (row["item_id"].ToString() == row2["item_id"].ToString())
                                {
                                    row["item_quantity_total"] = row2["rent_quantity"];
                                    row["item_used"] = row2["item_used"];
                                }
                            }

                        }
                        dataGridItems.DataContext = ds2;
                        SearchParams.currentDataItems = ds2;
                        checkButtons(ds2);
                    }
                    else
                    {
                        dataGridItems.DataContext = null;
                    }
                    conn.Close();

                }

                conn.Close();

            }

            conn.Close();

        }

        private void machineData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                SearchParams.machine = row_selected["name"].ToString();
            }
        }

        private void breadcrumbBuild(Boolean itemSelected)
        {
            breadcrumbData.Text = "";
            if (itemSelected == true)
            {
                breadcrumbData.Text = "..:: " + SearchParams.breadcrumbList.ElementAt(0);
                if (SearchParams.counter > 1)
                {
                    for (var i = 1; i < SearchParams.counter; i++)
                    {
                        breadcrumbData.Text += " : " + SearchParams.breadcrumbList.ElementAt(i);
                    }
                }
                breadcrumbData.Text = breadcrumbData.Text + " :: " + CurrentReturnModel.ItemIdentStr;

            }
            else
            {
                if (SearchParams.counter == 1)
                {
                    breadcrumbData.Text = "..:: " + SearchParams.breadcrumbList.ElementAt(0);
                }
                else
                {
                    breadcrumbData.Text = "..:: " + SearchParams.breadcrumbList.ElementAt(0);
                    for (var i = 1; i < SearchParams.counter; i++)
                    {
                        breadcrumbData.Text += " : " + SearchParams.breadcrumbList.ElementAt(i);
                    }
                }
            }
        }

        private void getNextStage(object sender, RoutedEventArgs e)
        {
            if (SearchParams.counter == 0)
            {
                conn.Open();

                var sql = $@"SELECT 
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
		item_objects 
	ORDER BY 
		item_ident ASC;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                adp.Fill(ds);
                dataGridItems.DataContext = ds;
                dataGridItems.ItemsSource = new DataView(ds.Tables[0]);
                conn.Close();
            }
            else
            {
                if (SearchParams.counter < 5)
                {

                    string[] itemArr = new String[SearchParams.currentDataItems.Tables[0].Rows.Count];
                    int counter = 0;
                    for (int i = 0; i < SearchParams.currentDataItems.Tables[0].Rows.Count; i++)
                    {
                        itemArr[counter] = SearchParams.currentDataItems.Tables[0].Rows[i]["item_id"].ToString();
                        counter++;
                    }

                    DataSet ds = new DataSet();
                    ds = ReturnItemQueries.GetItemFilterRelationsSQL(SearchParams.counter, SearchParams.SearchParamArr, itemArr);
                    ds = RentItemQueries.GetFilterNames(SearchParams.counter + 1, FilterIDBuilder(ds, SearchParams.counter + 1));
                    dataGridFilter.DataContext = ds;
                    dataGridFilter.ItemsSource = new DataView(ds.Tables[0]);
                    SearchParams.counter++;
                    backBtn.IsEnabled = true;
                    nextBtn.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("ERR");
                }

            }

        }
        private string[] FilterIDBuilder(DataSet ds, int id)
        {
            List<string> strDetailIDList = new List<string>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strDetailIDList.Add(row[$"filter{id}_id"].ToString());
            }

            String[] tmpArr = new string[strDetailIDList.Count];
            for (int i = 0; i < strDetailIDList.Count; i++)
            {
                tmpArr[i] = strDetailIDList[i].ToString();
            }
            return tmpArr;

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

            instructionText.Text = returnSQL;


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
            DataSet ds = new DataSet();

            if (SearchParams.counter == 2)
            {
                backBtn.IsEnabled = false;

                nextBtn.IsEnabled = false;
                breadcrumbData.Text = "";
                if (toggleUsers.IsChecked == true)
                {
                    GetAllRents();
                }
                else
                {

                    LoadUserDataItems();
                }
            }
            else
            {


                if (SearchParams.counter < 4)
                {
                    string[] itemArr = new String[SearchParams.currentDataItems.Tables[0].Rows.Count];
                    int counter = 0;
                    for (int i = 0; i < SearchParams.currentDataItems.Tables[0].Rows.Count; i++)
                    {
                        itemArr[counter] = SearchParams.currentDataItems.Tables[0].Rows[i]["item_id"].ToString();
                        counter++;
                    }

                    SearchParams.SearchParamArr[SearchParams.counter - 2] = "";
                    ds = ReturnItemQueries.GetItemFilterRelationsSQL(SearchParams.counter - 2, SearchParams.SearchParamArr, itemArr);
                    ds = RentItemQueries.GetFilterNames(SearchParams.counter - 1, FilterIDBuilder(ds, SearchParams.counter - 1));
                    dataGridFilter.DataContext = ds;
                    SearchParams.currentDataItems = ds;
                    dataGridFilter.ItemsSource = new DataView(ds.Tables[0]);
                    SearchParams.counter--;
                    breadcrumbBuild(false);
                    nextBtn.IsEnabled = false;
                }
            }
            if (SearchParams.counter == 1)
            {
                breadcrumbData.Text = "";
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
            int index = userData.SelectedIndex;
            backBtn.IsEnabled = false;

            nextBtn.IsEnabled = false;
            breadcrumbData.Text = "";
            OpenRentBtn.IsEnabled = false;
            DelteRentBtn.IsEnabled = false;
            if (toggleUsers.IsChecked == true)
            {
                GetAllRents();
            }
            else
            {
                LoadUserDataItems();
                dataGridFilter.DataContext = null;
                dataGridFilter.ItemsSource = null;
                dataGridItems.DataContext = null;
                dataGridItems.ItemsSource = null;
            }






        }
        private void ResetSearchPrompt()
        {

            SearchParams.counter = 1;
            SearchParams.currentSelectedFilter = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];
            OpenRentBtn.IsEnabled = false;
            DelteRentBtn.IsEnabled = false;


            dataGridItems.DataContext = null;
            conn.Open();
            MySqlCommand cmd2 = new MySqlCommand("Select * from filter1_names", conn);
            MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
            DataSet ds2 = new DataSet();
            adp2.Fill(ds2, "filterData");

            breadcrumbData.Text = "";
            dataGridFilter.DataContext = ds2;
            conn.Close();
        }
        private void ItemData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenRentBtn.IsEnabled = true;
            DelteRentBtn.IsEnabled = true;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                currentSelectedItem = row_selected.Row;
                RebookMachineItem.IsEnabled = true;
                CurrentReturnModel.currentReturnItem = row_selected;
                if (currentSelectedItem["item_documentation"].ToString() != "")
                {
                    OpenDocumentation.IsEnabled = true;
                }
                else
                {
                    OpenDocumentation.IsEnabled = false;
                }

                if (CurrentReturnModel.MachineID != "")
                {
                    OpenRentBtn.IsEnabled = true;
                    DelteRentBtn.IsEnabled = true;
                }
                CurrentReturnModel.ItemIdent = row_selected["item_id"].ToString();
                CurrentReturnModel.ItemIdentStr = row_selected["item_ident"].ToString();
                CurrentReturnModel.ItemDescription = row_selected["item_description"].ToString();
                CurrentReturnModel.ItemTotalQuantity = row_selected["rent_quantity"].ToString();
                CurrentReturnModel.ItemImagePath = row_selected["item_image_path"].ToString();
                CurrentReturnModel.CurrentUserId = row_selected["user_id"].ToString();


                if (row_selected["item_used"].ToString() == "1")
                {
                    CurrentReturnModel.ReturnIsUsed = true;
                }
                else
                {
                    CurrentReturnModel.ReturnIsUsed = false;
                }
                breadcrumbBuild(true);
            }
        }

        private void openRentWindow(object sender, RoutedEventArgs e)
        {
            if (CurrentReturnModel.MachineID != null || CurrentReturnModel.MachineID != "")
            {
                ReturnSelectionView test = new ReturnSelectionView();
                Nullable<bool> dialogResult = test.ShowDialog();
                loadData();

            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Maschine!");
            }


        }

        private void DelteRentBtn_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteReturn openConfirm = new ConfirmDeleteReturn();
            openConfirm.ShowDialog();
            OpenRentBtn.IsEnabled = false;
            DelteRentBtn.IsEnabled = false;
            loadData();
        }

        private void resetSearch(object sender, RoutedEventArgs e)
        {
            OpenRentBtn.IsEnabled = false;
            DelteRentBtn.IsEnabled = false;

            SearchParams.counter = 1;
            SearchParams.filter_1 = "";
            SearchParams.currentSelectedFilter = "";
            SearchParams.filter_1_id = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbList = new List<string> { "", "", "", "", "", "" };
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];

            int index = userData.SelectedIndex;
            backBtn.IsEnabled = false;

            nextBtn.IsEnabled = false;
            breadcrumbData.Text = "";
            OpenRentBtn.IsEnabled = false;
            DelteRentBtn.IsEnabled = false;
            if (toggleUsers.IsChecked == true)
            {
                GetAllRents();
            }
            else
            {
                LoadUserDataItems();
                userData.SelectedIndex = index;
            }

        }

        private void LoadUserDataItems()
        {
            SearchParams.counter = 1;
            SearchParams.filter_1 = "";
            SearchParams.currentSelectedFilter = "";
            SearchParams.filter_1_id = "";
            SearchParams.selectedItem = "";
            SearchParams.breadcrumbList = new List<string> { "", "", "", "", "", "" };
            SearchParams.breadcrumbs = new string[5];
            SearchParams.SearchParamArr = new string[5];
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("Select * from item_rents WHERE user_id = '" + SearchParams.selectedUserID + "'", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            SearchParams.CurrentSelectedUserRents = ds;
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

            if (tmpArr.Length == 0)
            {
                ds = new DataSet();
                adp.Fill(ds, "itemData");
                dataGridItems.DataContext = ds;
                SearchParams.currentDataItems = ds;
            }
            else
            {
                string item_ids = string.Format(" ({0}) ", string.Join(", ", tmpArr));
                var sql = $@"SELECT 
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
		item_objects WHERE item_id IN {item_ids}
	ORDER BY 
		item_ident ASC;";
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                DataSet ds2 = new DataSet();
                adp.Fill(ds2, "itemData");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    foreach (DataRow row2 in ds2.Tables[0].Rows)
                    {
                        if (row["item_id"].ToString() == row2["item_id"].ToString())
                        {
                            row2["item_quantity_total"] = row["rent_quantity"];
                        }
                    }
                }

                dataGridItems.DataContext = ds2;

                List<string> AllRentsItem = new List<string>();
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    AllRentsItem.Add(ds2.Tables[0].Rows[i]["item_id"].ToString());
                }
                string[] RentItemsArr = new string[AllRentsItem.Count];
                for (int i = 0; i < AllRentsItem.Count; i++)
                {
                    RentItemsArr[i] = AllRentsItem[i].ToString();
                }
                sql = string.Format("SELECT * FROM item_filter_relations WHERE item_id IN ({0})", string.Join(", ", RentItemsArr));
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                ds = new DataSet();


                adp.Fill(ds);

                bool checkFilter = false;
                List<string> FilterIDs = new List<string>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    checkFilter = false;
                    for (int j = 0; j < FilterIDs.Count; j++)
                    {
                        if (FilterIDs[j] == ds.Tables[0].Rows[i]["filter1_id"].ToString())
                        {
                            checkFilter = true;
                        }
                    }
                    if (!checkFilter)
                    {
                        FilterIDs.Add(ds.Tables[0].Rows[i]["filter1_id"].ToString());
                    }
                }

                String[] FilterIDArr = new String[FilterIDs.Count];
                for (int i = 0; i < FilterIDs.Count; i++)
                {
                    FilterIDArr[i] = FilterIDs[i].ToString();
                }



                sql = string.Format("SELECT * FROM filter1_names WHERE filter_id IN ({0})", string.Join(", ", FilterIDArr));
                cmd = new MySqlCommand(sql, conn);
                adp = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adp.Fill(ds, "filterData");
                dataGridFilter.DataContext = ds;
                dataGridFilter.ItemsSource = new DataView(ds.Tables[0]);
            }
            conn.Close();

        }
        private void CopyItemIdent(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentReturnModel.ItemIdentStr);
        }

        private void CopyDescription(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentReturnModel.ItemDescription);
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentReturnModel.ItemIdentStr + "; " + CurrentReturnModel.ItemDescription + "; Bestand:" + CurrentReturnModel.ItemTotalQuantity);
        }

        private void machineData_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                DataSet rentItems = ReturnItemQueries.GetAllRentsMachine(row_selected["machine_id"].ToString());
                SearchParams.currentDataItems = rentItems;
                dataGridItems.DataContext = rentItems;
                dataGridItems.ItemsSource = new DataView(rentItems.Tables[0]);
                CurrentReturnModel.MachineName = row_selected["name"].ToString();

                if (rentItems.Tables[0].Rows.Count > 0)
                {
                    dataGridItems.SelectedIndex = 0;
                }
            }

        }

        private void RebookMachineItem_Click(object sender, RoutedEventArgs e)
        {
            MachineSelectionRebookWindow openRebook = new MachineSelectionRebookWindow();
            openRebook.ShowDialog();
            loadData();
        }
        private bool CheckIfDateValid(string date)
        {
            // Parse the datetime string
            DateTime dateTime = DateTime.Parse(date);

            // Get the current datetime
            DateTime currentDateTime = DateTime.Now;

            // Calculate the threshold datetime (one hour ago)
            DateTime oneHourAgo = currentDateTime.Subtract(TimeSpan.FromHours(1));

            // Check if the parsed datetime is within the past hour
            return oneHourAgo <= dateTime && dateTime < currentDateTime;
        }


        private void revertLastRent_Click(object sender, RoutedEventArgs e)
        {
            DataSet LastUserRent = AdministrationQueries.RunSql($"SELECT * FROM item_rents t1 WHERE t1.user_id = {MainWindowViewModel.UserID} AND t1.createdAt = (SELECT MAX(createdAt) FROM item_rents t2 WHERE t2.user_id = {MainWindowViewModel.UserID} )");

            if (LastUserRent.Tables[0].Rows.Count > 0)
            {
                if (!CheckIfDateValid(LastUserRent.Tables[0].Rows[0]["createdAt"].ToString()))
                {
                    ErrorHandlerModel.ErrorText = "Die letzte Entnahme liegt länger als eine Stunge zurück! Bitte buchen Sie den Artikel manuell wieder in das Ursprüngliche Lagerfach!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();

                }
                else
                {
                    DataSet FoundCompartment = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {LastUserRent.Tables[0].Rows[0]["location_id"]}");

                    if (FoundCompartment.Tables[0].Rows.Count > 0)
                    {
                        if (FoundCompartment.Tables[0].Rows[0]["item_id"].ToString() != LastUserRent.Tables[0].Rows[0]["item_id"].ToString())
                        {
                            ErrorHandlerModel.ErrorText = "In dem letzten Lagerfach lagert aktuell ein anderes Werkzeug! Bitte buchen Sie den Artikel manuell wieder in ein anderes Lagerfach zurück!";
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                        else
                        {
                            ConfirmRevertLastRentWindow openRevert = new ConfirmRevertLastRentWindow();
                            openRevert.ShowDialog();
                            loadData();
                        }
                    }
                    else
                    {
                        ConfirmRevertLastRentWindow openRevert = new ConfirmRevertLastRentWindow();
                        openRevert.ShowDialog();
                        loadData();
                    }


                }

            }
            else
            {
                ErrorHandlerModel.ErrorText = "Es konnte keine Entnahme gefunden werden!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();

            }
        }

        private void dataGridItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGridItems.SelectedIndex > -1)
            {
                if (CurrentReturnModel.MachineID != null || CurrentReturnModel.MachineID != "")
                {
                    ReturnSelectionView test = new ReturnSelectionView();
                    Nullable<bool> dialogResult = test.ShowDialog();
                    loadData();

                }
                else
                {
                    MessageBox.Show("Bitte wählen Sie eine Maschine!");
                }
            }
        }

        private void OpenDocumentation_Click(object sender, RoutedEventArgs e)
        {
            string documentationPathGlobal = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'").Tables[0].Rows[0][2].ToString();
            documentationPathGlobal = Path.Combine(documentationPathGlobal, currentSelectedItem["item_documentation"].ToString());

            try
            {
                // Check if the file exists
                if (!File.Exists(documentationPathGlobal))
                {
                    Console.WriteLine($"File not found: {documentationPathGlobal}");
                    return;
                }

                // Start the process to open the PDF file with the default application
                Process.Start(new ProcessStartInfo
                {
                    FileName = documentationPathGlobal,
                    UseShellExecute = true // Ensures the default app is used
                });

                Console.WriteLine($"Opened file: {documentationPathGlobal}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to open the file: {ex.Message}");
            }
        }

    }
}
