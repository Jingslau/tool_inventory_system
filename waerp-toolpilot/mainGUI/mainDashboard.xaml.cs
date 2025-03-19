using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using MySqlConnector;
using waerp_toolpilot.application;
using waerp_toolpilot.application.returnItem;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.modules.Administration.MeasuringEquip;
using waerp_toolpilot.modules.MeasuringEquip;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.main
{
    /// <summary>
    /// Interaction logic for mainDashboard.xaml
    /// </summary>
    public partial class mainDashboard : Page
    {

        DataSet measureItems = new DataSet();
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public mainDashboard()
        {
            InitializeComponent();
            // getAllMeasuringEquipData();
            GetDashboardData();
            GetStatistic1();
            GetVendorStatistic();
            GetMainStatistic();
            getOrderData();
            loadMachineData();
            fullname.Text = MainWindowViewModel.Fullname;
            PointLabel = chartPoint =>
               string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);



            DataContext = this;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM item_rents", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                if (ds.Tables.Count == 0)
                {
                    // dashboardRents.DataContext = new DataSet();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        //dashboardRents.DataContext = new DataSet();
                        //ReturnSelectedItemBtn.IsEnabled = false;
                    }
                    else
                    {
                        // ReturnSelectedItemBtn.IsEnabled = true;
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


                        var sql = string.Format("SELECT * FROM item_objects WHERE item_id IN ({0})", string.Join(", ", tmpArr));
                        cmd = new MySqlCommand(sql, conn);
                        adp = new MySqlDataAdapter(cmd);
                        DataSet ds2 = new DataSet();
                        adp.Fill(ds2, "dashboardRents");
                        ds2.Tables[0].Columns.Add("user");
                        ds2.Tables[0].Columns.Add("item_used");
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            foreach (DataRow row2 in ds2.Tables[0].Rows)
                            {
                                if (row["item_id"].ToString() == row2["item_id"].ToString())
                                {
                                    DataSet user = AdministrationQueries.RunSql($"SELECT * FROM users WHERE user_id = {row["user_id"]}");
                                    row2["user"] = user.Tables[0].Rows[0]["username"];
                                    row2["item_quantity_total"] = row["rent_quantity"];
                                    row2["item_image_path"] = row["createdAt"].ToString();
                                    row2["item_used"] = row["item_used"];
                                }
                            }
                        }
                        //  dashboardRents.DataContext = ds2;
                        // dashboardRents.SelectedIndex = 0;
                    }
                }




            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void changeView_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/mainGUI/DashboardTouchView.xaml", UriKind.Relative));
        }
        private void loadMachineData()
        {
            DataSet machines = AdministrationQueries.RunSql("SELECT * FROM machines");

            machines.Tables[0].Columns.Add("quantity");
            machines.Tables[0].Columns.Add("last_employee");

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

                    string user_id = tmp.Tables[0].Rows[tmp.Tables[0].Rows.Count - 1]["user_id"].ToString();
                    DataSet user_info = AdministrationQueries.RunSql($"SELECT * FROM users WHERE user_id  = {user_id}");
                    machines.Tables[0].Rows[i]["last_employee"] = user_info.Tables[0].Rows[0]["name"].ToString() + " " + user_info.Tables[0].Rows[0]["surname"].ToString();
                }
                else
                {
                    machines.Tables[0].Rows[i]["quantity"] = "0";
                }

            }

            MachineDataItems.DataContext = machines;
            MachineDataItems.ItemsSource = new DataView(machines.Tables[0]);
        }

        public void GetMainStatistic()
        {
            int rentQuantity = 0;
            DataSet rents = AdministrationQueries.RunSql("SELECT * FROM item_rents");
            if (rents.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < rents.Tables[0].Rows.Count; i++)
                {
                    rentQuantity += int.Parse(rents.Tables[0].Rows[i]["rent_quantity"].ToString());
                }
            }

            int ordersQuantity = 0;
            DataSet orders = AdministrationQueries.RunSql("SELECT * FROM order_item_relations");
            if (orders.Tables[0].Rows.Count > 0)
            {


                for (int i = 0; i < orders.Tables[0].Rows.Count; i++)
                {
                    ordersQuantity += int.Parse(orders.Tables[0].Rows[i]["order_quantity"].ToString());
                }
            }

            currentRents.Text = rentQuantity.ToString();
            currentOrders.Text = ordersQuantity.ToString();

        }
        static string[] GetLast7Days()
        {
            string[] last7Days = new string[7];

            // Get today's date
            DateTime currentDate = DateTime.Today;

            // Fill the array with the last 7 days
            for (int i = 0; i < 7; i++)
            {
                last7Days[i] = currentDate.AddDays(-i).ToString("yyyy-MM-dd");
            }

            return last7Days;
        }

        static string[] GetLast7DaysStr()
        {
            string[] last7Days = new string[7];

            // Get today's date
            DateTime currentDate = DateTime.Today;

            // Fill the array with the last 7 days
            for (int i = 0; i < 7; i++)
            {
                last7Days[i] = currentDate.AddDays(-i).ToString("yyyy-MM-dd");
            }

            return last7Days;
        }
        static DateTime[] GetLast7DaysDateTime()
        {
            DateTime[] last7Days = new DateTime[7];

            // Get today's date
            DateTime currentDate = DateTime.Today;

            // Fill the array with the last 7 days
            for (int i = 0; i < 7; i++)
            {
                last7Days[i] = currentDate.AddDays(-i).Date;
            }

            return last7Days;
        }

        private void GetStatistic1()
        {
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM history_log WHERE QUARTER(createdAt) = QUARTER(NOW()) AND YEAR(createdAt) = YEAR(NOW()) AND action_id = 1");
            DataSet ds2 = AdministrationQueries.RunSql("SELECT * FROM history_log WHERE QUARTER(createdAt) = QUARTER(NOW()) AND YEAR(createdAt) = YEAR(NOW()) AND action_id = 2");
            DataSet ds3 = AdministrationQueries.RunSql("SELECT * FROM history_log WHERE QUARTER(createdAt) = QUARTER(NOW()) AND YEAR(createdAt) = YEAR(NOW()) AND action_id = 6");


            SeriesCollection = new SeriesCollection();
            Statistic1.Series = SeriesCollection;

            var values = new ChartValues<double> { ds.Tables[0].Rows.Count };

            var pieSeries = new PieSeries
            {
                Title = "Entnahmen",
                Values = values,
                DataLabels = true, // Show data labels
                LabelPoint = point => $"{point.Y} ({point.Participation:P})", // Customize label format
                PushOut = 0 // Optional: Push out the slices
            };



            var values2 = new ChartValues<double> { ds2.Tables[0].Rows.Count };

            var pieSeries2 = new PieSeries
            {
                Title = "Rückgaben",
                Values = values2,
                DataLabels = true, // Show data labels
                LabelPoint = point => $"{point.Y} ({point.Participation:P})", // Customize label format
                PushOut = 0 // Optional: Push out the slices
            };
            var values3 = new ChartValues<double> { ds3.Tables[0].Rows.Count };

            var pieSeries3 = new PieSeries
            {
                Title = "Verschrottet",
                Values = values3,
                DataLabels = true, // Show data labels
                LabelPoint = point => $"{point.Y} ({point.Participation:P})", // Customize label format
                PushOut = 0 // Optional: Push out the slices
            };

            SeriesCollection.Add(pieSeries);
            SeriesCollection.Add(pieSeries2);
            SeriesCollection.Add(pieSeries3);



        }

        private void GetVendorStatistic()
        {
            DataSet dataset = AdministrationQueries.RunSql("SELECT * FROM order_item_relations");

            dataset.Tables[0].Columns.Add("total_price", typeof(double));

            DataTable dataTable = dataset.Tables[0]; // Assuming your data is in the first table

            dataTable.Columns.Add("vendorName", typeof(string));

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataSet price = AdministrationQueries.RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {dataTable.Rows[i]["item_id"]}");


                if (double.TryParse(price.Tables[0].Rows[0]["item_price"].ToString(), out double itemPrice) && price.Tables[0].Rows[0]["item_price"].ToString() != "" && price.Tables[0].Rows[0]["item_price"].ToString() != null)
                {
                    double orderQuantity = double.Parse(dataTable.Rows[i]["order_quantity"].ToString());
                    double totalPrice = itemPrice * orderQuantity;
                    dataTable.Rows[i]["total_price"] = totalPrice;
                    //    MessageBox.Show(totalPrice.ToString());
                    dataTable.Rows[i]["vendorName"] = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_id = {dataTable.Rows[i]["vendor_id"]}").Tables[0].Rows[0]["vendor_name"].ToString();
                }

            }

            // Convert DataTable to Enumerable of anonymous types for LINQ manipulation
            var query = from row in dataTable.AsEnumerable()
                        group row by new { VendorId = row.Field<int>("vendor_id"), VendorName = row.Field<string>("vendorName") } into grouped
                        select new
                        {
                            VendorId = grouped.Key.VendorId,
                            VendorName = grouped.Key.VendorName,
                            TotalQuantity = Math.Round(grouped.Sum(x => x.Field<double?>("total_price") ?? 0), 2) // Rounding to two decimal places
                        };



            // Sort the groups by total quantity in descending order
            var sortedData = query.OrderByDescending(x => x.TotalQuantity);

            // Selecting the top 3 vendors and summing up all others
            var top3Vendors = sortedData.Take(3);
            var otherVendorsTotal = sortedData.Skip(3).Sum(x => x.TotalQuantity);

            // Printing the result
            Console.WriteLine("Top 3 Vendors:");

            SeriesCollection = new SeriesCollection();
            Statistic2.Series = SeriesCollection;

            foreach (var vendor in top3Vendors)
            {
                var pieSeries = new PieSeries
                {
                    Title = vendor.VendorName.ToString(),
                    Values = new ChartValues<double> { vendor.TotalQuantity },
                    DataLabels = true, // Show data labels
                    LabelPoint = point => $"{point.Y} ({point.Participation:P})", // Customize label format
                    PushOut = 0 // Optional: Push out the slices
                };

                SeriesCollection.Add(pieSeries);
                Console.WriteLine($"Vendor ID: {vendor.VendorId}, Total Order Quantity: {vendor.TotalQuantity}");
            }

            Console.WriteLine($"Total Order Quantity for Other Vendors: {otherVendorsTotal}");
        }


        private void getOrderData()
        {
            DataSet orders = AdministrationQueries.RunSql($"SELECT * FROM order_item_relations WHERE createdAt >= DATE_SUB(NOW(), INTERVAL 7 DAY);");

            orders.Tables[0].Columns.Add("vendor_name");
            if (orders.Tables[0].Rows.Count > 0)
            {
                List<int> vendorIds = new List<int>();

                foreach (DataRow row in orders.Tables[0].Rows)
                {
                    int vendorId = (int)row["vendor_id"];

                    if (!vendorIds.Contains(vendorId))
                    {
                        vendorIds.Add(vendorId);
                    }
                }

                // Initialize the SeriesCollection once
                SeriesCollection = new SeriesCollection();
                vendorOrderGraph.Series = SeriesCollection;

                foreach (int vendorId in vendorIds)
                {
                    DataSet vendorOrders = AdministrationQueries.RunSql($"SELECT * FROM order_item_relations WHERE createdAt >= DATE_SUB(NOW(), INTERVAL 7 DAY) AND vendor_id = {vendorId};");

                    DateTime[] dates = GetLast7DaysDateTime();
                    Int32[] data = { 0, 0, 0, 0, 0, 0, 0 };

                    for (int i = 0; i < vendorOrders.Tables[0].Rows.Count; i++)
                    {
                        DateTime currentDay = DateTime.Parse(vendorOrders.Tables[0].Rows[i]["createdAt"].ToString());

                        for (int j = 0; j < dates.Length; j++)
                        {
                            if (currentDay.Date == dates[j].Date)
                            {
                                data[j] += Int32.Parse(vendorOrders.Tables[0].Rows[i]["order_quantity_org"].ToString());
                            }
                        }
                    }

                    Array.Reverse(data);
                    String[] datesStr = GetLast7DaysStr();
                    Array.Reverse(datesStr);

                    var values = new ChartValues<int>(data);

                    // Create a new line series for each vendor
                    var lineSeries = new LineSeries
                    {
                        Title = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_id = {vendorId}").Tables[0].Rows[0]["vendor_name"].ToString(),
                        Values = values,
                        PointGeometrySize = 10 // Optional: Customize point size
                    };

                    // Add series to SeriesCollection
                    SeriesCollection.Add(lineSeries);

                    // Set the X axis labels only once
                    if (SeriesCollection.Count == 1)
                    {
                        vendorOrderGraph.AxisX[0].Labels = datesStr;
                    }
                }
            }
        }


        private void GetDashboardData()
        {
            DataSet mainGraphData = AdministrationQueries.RunSql($"SELECT * FROM history_log WHERE createdAt >= DATE_SUB(NOW(), INTERVAL 7 DAY) AND action_id = 1;");

            DateTime[] dates = GetLast7DaysDateTime();
            Int32[] data = { 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < mainGraphData.Tables[0].Rows.Count; i++)
            {
                DateTime currentDay = DateTime.Parse(mainGraphData.Tables[0].Rows[i]["createdAt"].ToString());

                for (int j = 0; j < dates.Length; j++)
                {
                    if (currentDay.Date == dates[j].Date)
                    {
                        data[j] += Int32.Parse(mainGraphData.Tables[0].Rows[i]["item_quantity"].ToString());
                    }
                }

            }

            Array.Reverse(data);
            String[] datesStr = GetLast7DaysStr();
            Array.Reverse(datesStr);

            SeriesCollection = new SeriesCollection();
            mainGraph.Series = SeriesCollection;

            var values = new ChartValues<int>(data);

            // Example line series
            var lineSeries = new LineSeries
            {
                Title = "Entnahmen",
                Values = values,
                PointGeometrySize = 10 // Optional: Customize point size
            };

            // Add series to SeriesCollection

            mainGraph.AxisX[0].Labels = datesStr;


            SeriesCollection.Add(lineSeries);


            mainGraphData = AdministrationQueries.RunSql($"SELECT * FROM history_log WHERE createdAt >= DATE_SUB(NOW(), INTERVAL 7 DAY) AND action_id = 2;");

            Int32[] data2 = { 0, 0, 0, 0, 0, 0, 0 };
            dates = GetLast7DaysDateTime();
            for (int i = 0; i < mainGraphData.Tables[0].Rows.Count; i++)
            {
                DateTime currentDay = DateTime.Parse(mainGraphData.Tables[0].Rows[i]["createdAt"].ToString());

                for (int j = 0; j < dates.Length; j++)
                {
                    if (currentDay.Date == dates[j].Date)
                    {
                        data2[j] += Int32.Parse(mainGraphData.Tables[0].Rows[i]["item_quantity"].ToString());
                    }
                }

            }

            Array.Reverse(data2);

            values = new ChartValues<int>(data2);

            // Example line series
            lineSeries = new LineSeries
            {
                Title = "Rückgabe",
                Values = values,
                PointGeometrySize = 10 // Optional: Customize point size
            };

            SeriesCollection.Add(lineSeries);

            mainGraphData = AdministrationQueries.RunSql($"SELECT * FROM history_log WHERE createdAt >= DATE_SUB(NOW(), INTERVAL 7 DAY) AND action_id = 6;");

            Int32[] data3 = { 0, 0, 0, 0, 0, 0, 0 };
            dates = GetLast7DaysDateTime();
            for (int i = 0; i < mainGraphData.Tables[0].Rows.Count; i++)
            {
                DateTime currentDay = DateTime.Parse(mainGraphData.Tables[0].Rows[i]["createdAt"].ToString());

                for (int j = 0; j < dates.Length; j++)
                {
                    if (currentDay.Date == dates[j].Date)
                    {
                        data3[j] += Int32.Parse(mainGraphData.Tables[0].Rows[i]["item_quantity"].ToString());
                    }
                }

            }

            Array.Reverse(data3);

            values = new ChartValues<int>(data3);

            // Example line series
            lineSeries = new LineSeries
            {
                Title = "Verschrottet",
                Values = values,
                PointGeometrySize = 10 // Optional: Customize point size
            };

            SeriesCollection.Add(lineSeries);

        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }


        public bool MultiSelect { get; set; }
        public class Item
        {
            public string Artikelnummer { get; set; }
            public string Bezeichnung { get; set; }
            public int Menge { get; set; }
            public PackIconKind IconKind { get; set; }
        }

        private void getAllMeasuringEquipData()
        {
            DateTime nextCheckUp = DateTime.MinValue;
            DataSet itemsData = AdministrationQueries.RunSql("SELECT * FROM measuring_equip_objects");

            itemsData.Tables[0].Columns.Add("nextCheckUp");
            itemsData.Tables[0].Columns.Add("lastCheckUp");
            itemsData.Tables[0].Columns.Add("currentLocation");
            itemsData.Tables[0].Columns.Add("currentLocationID");

            if (itemsData.Tables[0].Rows.Count > 0)
            {
                for (var i = 0; i < itemsData.Tables[0].Rows.Count; i++)
                {
                    DataSet location = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_location_relations WHERE measuring_equip_id = {itemsData.Tables[0].Rows[i]["measuring_equip_id"]}");


                    if (location.Tables[0].Rows.Count > 0)
                    {
                        itemsData.Tables[0].Rows[i]["currentLocationID"] = location.Tables[0].Rows[0]["mlocation_id"];
                        itemsData.Tables[0].Rows[i]["currentLocation"] = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_locations WHERE mlocation_id = {location.Tables[0].Rows[0]["mlocation_id"]}").Tables[0].Rows[0]["mlocation_name"];
                    }

                    if (DateTime.TryParse(itemsData.Tables[0].Rows[i]["measuring_equip_lastCheckUp"].ToString(), out DateTime parsedDateTime))
                    {
                        string toInsert = DateTime.Parse(itemsData.Tables[0].Rows[i]["measuring_equip_lastCheckUp"].ToString()).ToString("d");
                        itemsData.Tables[0].Rows[i]["lastCheckUp"] = toInsert;
                    }




                    DataSet currentHistoryLogs = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_history WHERE measuring_equip_id = {itemsData.Tables[0].Rows[i]["measuring_equip_id"]} AND measuring_equip_history_isChecked = 0");
                    if (currentHistoryLogs.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < currentHistoryLogs.Tables[0].Rows.Count; j++)
                        {
                            itemsData.Tables[0].Rows[i]["nextCheckUp"] = DateTime.Parse(currentHistoryLogs.Tables[0].Rows[j]["measuring_equip_planned_date"].ToString()).ToString("d");


                            if (nextCheckUp == DateTime.MinValue)
                            {
                                nextCheckUp = DateTime.Parse(currentHistoryLogs.Tables[0].Rows[j]["measuring_equip_planned_date"].ToString());
                            }
                            else
                            {
                                if (nextCheckUp > DateTime.Parse(currentHistoryLogs.Tables[0].Rows[j]["measuring_equip_planned_date"].ToString()))
                                {
                                    nextCheckUp = DateTime.Parse(currentHistoryLogs.Tables[0].Rows[j]["measuring_equip_planned_date"].ToString());
                                }
                            }
                        }

                    }

                }

                measureItems = itemsData;
                nextCheckUpDate.Text = nextCheckUp.ToString("d");
                //measureItemsData.DataContext = itemsData;
                //measureItemsData.ItemsSource = new DataView(itemsData.Tables[0]);
                //measureItemsData.SelectedIndex = 0;
            }
            else
            {
                //addCheckUp.IsEnabled = false;
                //MeasureAdmin.IsEnabled = false;

            }





        }

        private void dataGridCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                CurrentReturnModel.ItemIdent = row_selected["item_id"].ToString();
                CurrentReturnModel.ItemIdentStr = row_selected["item_ident"].ToString();
                CurrentReturnModel.ItemDescription = row_selected["item_description"].ToString();
                CurrentReturnModel.ItemTotalQuantity = row_selected["item_quantity_total"].ToString();
                CurrentReturnModel.ItemImagePath = row_selected["item_image_path"].ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/application/RentItem/RentItemView.xaml", UriKind.Relative));

        }
        private void ReturnItem_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/application/returnItem/ReturnItemSelect.xaml", UriKind.Relative));

        }
        private void RebookItem(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/application/rebookItem/RebookViewSelection.xaml", UriKind.Relative));

        }
        private void History(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/application/History/HistoryView.xaml", UriKind.Relative));

        }

        private void OpenBarcodeScan_Click(object sender, RoutedEventArgs e)
        {
            ScanBarcodeWindow BarcodeScan = new ScanBarcodeWindow();
            Nullable<bool> dialogResult = BarcodeScan.ShowDialog();
        }

        private void OpenRentItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReturnSelectedItemBtn_Click(object sender, RoutedEventArgs e)
        {
            ReturnSelectionView openReturn = new ReturnSelectionView();
            openReturn.ShowDialog();
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

        private void addCheckUp_Click(object sender, RoutedEventArgs e)
        {
            AddNextCheckUp openAddNewCheck = new AddNextCheckUp();
            openAddNewCheck.ShowDialog();
            getAllMeasuringEquipData();
        }

        private void checkUpHistory_Click(object sender, RoutedEventArgs e)
        {
            MeasuringEquipHistoryWindow openHistory = new MeasuringEquipHistoryWindow();
            openHistory.ShowDialog();

        }

        //private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (searchBox.Text != "")
        //    {
        //        DataSet output = measureItems.Copy();
        //        output.Tables[0].Rows.Clear();

        //        foreach (DataRow row in measureItems.Tables[0].Rows)
        //        {
        //            if (row["measuring_equip_id"].ToString().Contains(searchBox.Text) | row["measuring_equip_name"].ToString().Contains(searchBox.Text) | row["measuring_equip_vendor"].ToString().Contains(searchBox.Text) | row["lastCheckUp"].ToString().Contains(searchBox.Text) | row["nextCheckUp"].ToString().Contains(searchBox.Text))
        //            {
        //                output.Tables[0].ImportRow(row);
        //            }
        //        }
        //        measureItemsData.DataContext = output;
        //        measureItemsData.ItemsSource = new DataView(output.Tables[0]);
        //        addCheckUp.IsEnabled = false;
        //        MeasureAdmin.IsEnabled = false;

        //    }
        //    else
        //    {
        //        measureItemsData.DataContext = measureItems;
        //        measureItemsData.ItemsSource = new DataView(measureItems.Tables[0]);
        //        addCheckUp.IsEnabled = false;
        //        MeasureAdmin.IsEnabled = false;

        //    }
        //}

        //private void measureItemsData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
        //    DataRowView row_selected = gd.SelectedItem as DataRowView;
        //    if (row_selected != null)
        //    {
        //        MeasuringEquipModel.MeasuringEquipID = row_selected["measuring_equip_id"].ToString();
        //        MeasuringEquipModel.MeasuringEquipName = row_selected["measuring_equip_name"].ToString();
        //        MeasuringEquipModel.MeasuringEquipVendor = row_selected["measuring_equip_vendor"].ToString();

        //        if (row_selected["currentLocation"].ToString() == "")
        //        {
        //            TakeOutME.IsEnabled = false;
        //            ReturnME.IsEnabled = true;
        //        }
        //        else
        //        {
        //            ReturnME.IsEnabled = false;
        //            TakeOutME.IsEnabled = true;
        //        }
        //        addCheckUp.IsEnabled = true;
        //        MeasureAdmin.IsEnabled = true;


        //    }
        //    else
        //    {
        //        addCheckUp.IsEnabled = false;
        //        MeasureAdmin.IsEnabled = false;

        //    }
        //}

        private void confirmCheckUp_Click(object sender, RoutedEventArgs e)
        {

            AddCheckUp openCheckUp = new AddCheckUp();
            openCheckUp.ShowDialog();
            getAllMeasuringEquipData();
        }
    }
}
