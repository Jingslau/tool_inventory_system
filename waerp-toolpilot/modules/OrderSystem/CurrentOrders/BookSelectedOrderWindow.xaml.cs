using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySqlConnector;
using waerp_toolpilot.application.returnItem;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.OrderSystem.CurrentOrders
{
    /// <summary>
    /// Interaction logic for BookSelectedOrderWindow.xaml
    /// </summary>
    public partial class BookSelectedOrderWindow : Window
    {
        public string locationID;
        public string locationName;
        public string locationQuantity; public DataRowView CurrentLocationRow;
        public bool isNewLocation = false;
        public DataSet AllLocation;
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());


        public BookSelectedOrderWindow()
        {
            InitializeComponent();
            GetItemContent();
            GetLocations();
            ReturnBtn.IsEnabled = false;
            Boolean check = false;
            for (int i = 0; i < ActiveOrderModel.ItemIdentStr.Length; i++)
            {
                if (ActiveOrderModel.ItemIdentStr[i].ToString() == "ä" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "ü" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "ö" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "Ä" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "Ü" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "Ö" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "ß" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "%" ||
                    ActiveOrderModel.ItemIdentStr[i].ToString() == "&")
                {
                    check = true;
                }
            }
            if (check == false)
            {

                Barcode.Text = "*" + ActiveOrderModel.ItemIdentStr + "*";
            }
            else
            {
                Barcode.Text = "";
            }
        }

        private void GetItemContent()
        {
            conn.Open();
            MySqlDataReader reader = null;
            string query = $@"SELECT 
	`item_objects`.`item_id`, `item_objects`.`item_ident`, `item_objects`.`item_quantity_total`, `item_objects`.`item_quantity_total_new`, `item_objects`.`item_onetime_use`, `item_objects`.`item_quantity_min`, `item_objects`.`item_orderquant_min`, `item_objects`.`item_orderable`, `item_objects`.`item_onorder`, `item_objects`.`item_description`, `item_objects`.`item_description_2`, `item_objects`.`item_diameter`, `item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_id =  {ActiveOrderModel.CurrentItemId}
	ORDER BY 
		item_ident ASC;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ActiveOrderModel.ItemIdentStr = reader.GetString("item_ident");
                ActiveOrderModel.ItemDescription = reader.GetString("item_description");
                ActiveOrderModel.ItemImagePath = reader.GetString("item_image_path");
                ItemDiameter.Text = reader.GetString("item_diameter");
            }

            ItemIdent.Text = ActiveOrderModel.ItemIdentStr;
            ItemDescription.Text = ActiveOrderModel.ItemDescription;
            ItemTotalQuantity.Text = ActiveOrderModel.ItemQuantity;
            OrderIdent.Text = ActiveOrderModel.Order_Ident;

            if (ActiveOrderModel.ItemImagePath != "")
            {
                try
                {
                    Uri imageUri = new Uri(ActiveOrderModel.ItemImagePath, UriKind.RelativeOrAbsolute);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = imageUri;
                    bitmapImage.EndInit();

                    ItemImage.Source = bitmapImage;
                }
                catch (Exception exp)
                {
                    ErrorLogger.LogSysError(exp);
                    Uri imageUri = new Uri("pack://application:,,,/assets/images/default/default.jpg", UriKind.RelativeOrAbsolute);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = imageUri;
                    bitmapImage.EndInit();

                    ItemImage.Source = bitmapImage;
                }
            }
            else
            {
                Uri imageUri = new Uri("pack://application:,,,/assets/images/default/default.jpg", UriKind.RelativeOrAbsolute);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = imageUri;
                bitmapImage.EndInit();

                ItemImage.Source = bitmapImage;

            }
            conn.Close();
        }

        private void GetLocations()
        {
            DataSet ds = TempLocationsQueries.GetOnlyNewLocations(ActiveOrderModel.CurrentItemId);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    AllLocation = ds;
                    locationData.DataContext = ds;
                    locationData.ItemsSource = new DataView(ds.Tables[0]);
                }
            }
            DataSet ds2 = TempLocationsQueries.GetEmptyLocation(ActiveOrderModel.CurrentItemId);
            if (ds2.Tables.Count > 0)
            {
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    locationEmptyData.DataContext = ds2;
                    locationEmptyData.ItemsSource = new DataView(ds2.Tables[0]);
                    AllLocation = TempLocationsQueries.GetEmptyLocation(ActiveOrderModel.CurrentItemId);
                }
            }



        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ReturnItemLocation()
        {
            if (isNewLocation)
            {
                AdministrationQueries.RunSqlExec($"INSERT INTO compartment_item_relations (compartment_id, item_id, item_quantity, item_constructed, item_used) VALUES (" +
              $"{CurrentReturnModel.ReturnLocationID}," +
              $"{ActiveOrderModel.CurrentItemId}," +
              $"{ActiveOrderModel.BookQuantity}," +
              $"0," +
              $"0)");
            }
            else
            {
                AdministrationQueries.RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = item_quantity + {ActiveOrderModel.BookQuantity} WHERE location_id = {CurrentReturnModel.ReturnLocationID}");
            }

            AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new + {ActiveOrderModel.BookQuantity} WHERE item_id = {ActiveOrderModel.CurrentItemId}");
            AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {ActiveOrderModel.BookQuantity} WHERE item_id = {ActiveOrderModel.CurrentItemId}");
        }

        private void ReturnItem(object sender, RoutedEventArgs e)
        {
            if (QuantityInput.Text == "")
            {
                MessageBox.Show((string)FindResource("errorText40"));
            }
            else
            {
                if (int.Parse(QuantityInput.Text) > 0 && int.Parse(QuantityInput.Text) <= int.Parse(ActiveOrderModel.ItemQuantity))
                {
                    if (int.Parse(ItemTotalQuantity.Text) == int.Parse(QuantityInput.Text))
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE order_item_relations SET isOpen = 0 WHERE order_ident = '{ActiveOrderModel.Order_Ident}' AND item_id = {ActiveOrderModel.CurrentItemId}");

                        DataSet CheckOpenOrders = AdministrationQueries.RunSql($"SELECT * FROM order_item_relations WHERE order_ident = '{ActiveOrderModel.Order_Ident}' AND isOpen = 1");

                        if (CheckOpenOrders.Tables[0].Rows.Count <= 0)
                        {
                            AdministrationQueries.RunSqlExec($"UPDATE order_objects SET order_status = 0 WHERE order_ident = '{ActiveOrderModel.Order_Ident}'");
                        }


                        AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_onorder = 0 WHERE item_id = {ActiveOrderModel.CurrentItemId}");
                    }
                    else
                    {
                        int newQuant = int.Parse(ItemTotalQuantity.Text) - int.Parse(QuantityInput.Text);
                        AdministrationQueries.RunSqlExec($"UPDATE order_item_relations SET order_quantity = {newQuant} WHERE item_id = {ActiveOrderModel.CurrentItemId} AND order_ident = '{ActiveOrderModel.Order_Ident}'");
                    }

                    ReturnItemLocation();
                    CurrentReturnModel.ReturnLocation = locationName;
                    CurrentReturnModel.ReturnQuantity = QuantityInput.Text.ToString();


                    SuccessReturnView successDialog = new SuccessReturnView();
                    Nullable<bool> dialogResult = successDialog.ShowDialog();
                    DialogResult = false;

                }
                else if (int.Parse(QuantityInput.Text) == 0)
                {
                    MessageBox.Show((string)FindResource("errorText41"));
                }
                else
                {
                    MessageBox.Show((string)FindResource("errorText42"));
                }

            }

        }

        private void locationData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isNewLocation = false;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                locationEmptyData.SelectedIndex = -1;
                locationID = row_selected["location_id"].ToString();
                CurrentReturnModel.ReturnLocationID = locationID;
                CurrentLocationRow = row_selected;
                locationName = row_selected["location_name"].ToString();
                ReportWrongLocationModel.LocationIdent = row_selected["location_name"].ToString();
                locationQuantity = row_selected["item_quantity"].ToString();
            }

            ReturnBtn.IsEnabled = true;
        }
        private void locationEmptyData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isNewLocation = true;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                locationData.SelectedIndex = -1;
                ReportWrongLocationModel.LocationIdent = row_selected["location_name"].ToString();
                CurrentLocationRow = row_selected;
                locationID = row_selected["compartment_id"].ToString();
                CurrentReturnModel.ReturnLocationID = locationID;
                locationName = row_selected["location_name"].ToString();
                // locationQuantity = row_selected["location_quantity"].ToString();
            }
            ReturnBtn.IsEnabled = true;
        }

        private void CloseCurrentDialog(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
        }
        private void PlusQuantity_Click(object sender, RoutedEventArgs e)
        {
            int quant = int.Parse(QuantityInput.Text);
            quant++;
            QuantityInput.Text = quant.ToString();
            ActiveOrderModel.BookQuantity = quant.ToString();
        }

        private void MinusQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) > 0)
            {
                int quant = int.Parse(QuantityInput.Text);
                quant--;
                QuantityInput.Text = quant.ToString();
                ActiveOrderModel.BookQuantity = quant.ToString();

            }

        }

        private void QuantityNumInput_Click(object sender, RoutedEventArgs e)
        {
            ActiveOrderModel.BookQuantity = QuantityInput.Text.ToString();
            BookOrderNumberInputWindow numberInput = new BookOrderNumberInputWindow();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = ActiveOrderModel.BookQuantity.ToString();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = AllLocation.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in AllLocation.Tables[0].Rows)
                {
                    if (row["location_name"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                locationEmptyData.DataContext = output;
            }
            else
            {
                locationEmptyData.DataContext = AllLocation;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
