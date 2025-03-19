
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySqlConnector;
using waerp_toolpilot.application.ReportLocation;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.returnItem
{
    /// <summary>
    /// Interaction logic for ReturnSelectionView.xaml
    /// </summary>
    public partial class ReturnSelectionView : Window
    {
        public string locationID;
        public string locationName;

        public string locationQuantity;
        public bool isUsedRent = false;
        public DataSet AllLocation = new DataSet();
        public DataRowView CurrentLocationRow;
        public bool isNewLocation = false;
        public DataSet AllLocations = new DataSet();
        public string documentationPath = "";

        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());


        public ReturnSelectionView()
        {
            InitializeComponent();
            GetItemContent();
            GetLocations();
            ReportWrongLocationModel.ItemIdent = CurrentReturnModel.ItemIdentStr;
            ReturnBtn.IsEnabled = false;

            isUsedRent = CurrentReturnModel.ReturnIsUsed;
            CurrentReturnModel.ReturnIsUsed = true;
            Boolean check = false;
            for (int i = 0; i < CurrentReturnModel.ItemIdentStr.Length; i++)
            {
                if (CurrentReturnModel.ItemIdentStr[i].ToString() == "ä" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "ü" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "ö" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "Ä" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "Ü" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "Ö" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "ß" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "%" ||
                    CurrentReturnModel.ItemIdentStr[i].ToString() == "&")
                {
                    check = true;
                }
            }
            if (check == false)
            {

                Barcode.Text = "*" + CurrentReturnModel.ItemIdentStr + "*";
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
            MySqlCommand cmd = new MySqlCommand("Select * from item_objects WHERE item_id = '" + CurrentReturnModel.ItemIdent + "'", conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                CurrentReturnModel.ItemIdentStr = reader.GetString("item_ident");
                CurrentReturnModel.ItemDescription = reader.GetString("item_description");
                CurrentReturnModel.ItemDescription2 = reader.GetString("item_description_2");

                if (reader.GetString("item_documentation") != "")
                {
                    documentationPath = reader.GetString("item_documentation");
                    ShowDocumentation.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowDocumentation.Visibility = Visibility.Collapsed;
                }
            }



            ItemIdent.Text = CurrentReturnModel.ItemIdentStr;
            ItemDescription.Text = CurrentReturnModel.ItemDescription;
            ItemDescription2.Text = CurrentReturnModel.ItemDescription2;

            ItemTotalQuantity.Text = CurrentReturnModel.ItemTotalQuantity;

            if (CurrentReturnModel.ItemImagePath != "")
            {
                try
                {
                    Uri imageUri = new Uri(CurrentReturnModel.ItemImagePath, UriKind.RelativeOrAbsolute);

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
            DataSet ds = TempLocationsQueries.GetItemLocations(CurrentReturnModel.ItemIdent);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    AllLocations = ds;
                    locationData.DataContext = ds;
                    locationData.ItemsSource = new DataView(ds.Tables[0]);
                }
            }
            DataSet ds2 = TempLocationsQueries.GetEmptyLocation(CurrentReturnModel.ItemIdent);
            if (ds2.Tables.Count > 0)
            {
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    locationEmptyData.DataContext = ds2;
                    locationEmptyData.ItemsSource = new DataView(ds2.Tables[0]);
                    AllLocation = TempLocationsQueries.GetEmptyLocation(CurrentReturnModel.ItemIdent);
                }
            }



        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void ReturnItem(object sender, RoutedEventArgs e)
        {
            string used = "0";
            if (UsedItems.IsChecked == true)
            {
                used = "1";
            }

            bool check = false;
            if (QuantityInput.Text == "")
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText36");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }

            else
            {
                check = false;
                if (int.Parse(QuantityInput.Text) > 0 && int.Parse(QuantityInput.Text) <= int.Parse(CurrentReturnModel.ItemTotalQuantity))
                {

                    if (isNewLocation)
                    {
                        CurrentReturnModel.ReturnQuantity = QuantityInput.Text;
                        ReturnItemQueries.ReturnItemNewLocation("0");
                        check = true;
                    }
                    else
                    {
                        if (CurrentLocationRow["onlyUsed"].ToString() == "1" && CurrentLocationRow["onlyUsed"].ToString() != used)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText67");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                        else if (CurrentLocationRow["item_used"].ToString() != used)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText67");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                        else
                        {
                            //if (Convert.ToBoolean(CurrentLocationRow["item_constructed"].ToString()) != CurrentReturnModel.ItemIsConstructed)
                            //{
                            //    ErrorHandlerModel.ErrorText = "Zusammengebautes Werkzeug kann nur mit zusammengebauten zusammen gelagert werden!";
                            //    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            //    ErrorWindow showError = new ErrorWindow();
                            //    showError.ShowDialog();
                            //    check = false;
                            //}
                            //else
                            //{
                            CurrentReturnModel.ReturnQuantity = QuantityInput.Text;
                            ReturnItemQueries.ReturnItemLocation("0");
                            check = true;
                            //}
                        }

                    }
                    if (check == true)
                    {
                        if (int.Parse(CurrentReturnModel.ItemTotalQuantity) - int.Parse(QuantityInput.Text) == 0)
                        {
                            ReturnItemQueries.DeleteRent();
                        }
                        else if (int.Parse(CurrentReturnModel.ItemTotalQuantity) - int.Parse(QuantityInput.Text) > 0)
                        {
                            ReturnItemQueries.UpdateRent();
                        }


                        CurrentReturnModel.ReturnLocation = locationName;
                        CurrentReturnModel.ReturnQuantity = QuantityInput.Text.ToString();


                        SuccessReturnView successDialog = new SuccessReturnView();
                        Nullable<bool> dialogResult = successDialog.ShowDialog();
                        DialogResult = false;
                    }

                    //else
                    //{
                    //    ErrorHandlerModel.ErrorText = (string)FindResource("errorText53");
                    //    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    //    ErrorWindow showError = new ErrorWindow();
                    //    showError.ShowDialog();
                    //}
                }
                else if (int.Parse(QuantityInput.Text) == 0)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText61");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
                else
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText36");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }

            }

        }

        private void locationData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                isNewLocation = false;
                string used = "0";
                if (UsedItems.IsChecked == true)
                {
                    used = "1";
                }
                if (row_selected["item_used"].ToString() != used)
                {
                    ReturnBtn.IsEnabled = false;
                }
                else
                {
                    ReturnBtn.IsEnabled = true;
                }

                locationEmptyData.SelectedIndex = -1;

                locationID = row_selected["compartment_id"].ToString();
                CurrentReturnModel.ReturnLocationID = locationID;
                CurrentLocationRow = row_selected;
                locationName = row_selected["location_name"].ToString();
                ReportWrongLocationModel.LocationIdent = row_selected["location_name"].ToString();
                locationQuantity = row_selected["item_quantity"].ToString();
            }

        }
        private void locationEmptyData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                isNewLocation = true;

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
        }

        private void MinusQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) > 0)
            {
                int quant = int.Parse(QuantityInput.Text);
                quant--;
                QuantityInput.Text = quant.ToString();

            }

        }

        private void QuantityNumInput_Click(object sender, RoutedEventArgs e)
        {
            CurrentReturnModel.ReturnQuantity = QuantityInput.Text.ToString();
            ReturnNumberInputView numberInput = new ReturnNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = CurrentReturnModel.ReturnQuantity.ToString();
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
                locationEmptyData.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                locationEmptyData.DataContext = AllLocation;
                locationEmptyData.ItemsSource = new DataView(AllLocation.Tables[0]);

            }
        }

        private void ReportErrorLocationBtn_Click(object sender, RoutedEventArgs e)
        {

            ReportLocationView openReport = new ReportLocationView();
            openReport.ShowDialog();
        }

        private void UsedItems_Click(object sender, RoutedEventArgs e)
        {
            string used = "0";
            if (UsedItems.IsChecked == true)
            {
                used = "1";
                CurrentReturnModel.ReturnIsUsed = true;
            }
            if (UsedItems.IsChecked == true)
            {
                CurrentReturnModel.ReturnIsUsed = true;
            }
            else
            {
                if (isUsedRent)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText68");
                    ErrorHandlerModel.ErrorType = "INFO";
                    ErrorWindow showInfo = new ErrorWindow();
                    showInfo.ShowDialog();
                }
                CurrentReturnModel.ReturnIsUsed = false;
            }
            if (CurrentLocationRow != null)
            {
                if (CurrentLocationRow["item_used"].ToString() != used)
                {
                    ReturnBtn.IsEnabled = false;
                }
                else
                {
                    ReturnBtn.IsEnabled = true;

                }
            }
        }

        private void ItemConstructed_Click(object sender, RoutedEventArgs e)
        {


            if (UsedItems.IsChecked == true)
            {
                CurrentReturnModel.ItemIsConstructed = true;
            }
            else
            {
                CurrentReturnModel.ItemIsConstructed = false;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ShowDocumentation_Click(object sender, RoutedEventArgs e)
        {
            string documentationPathGlobal = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'").Tables[0].Rows[0][2].ToString();
            documentationPathGlobal = Path.Combine(documentationPathGlobal, documentationPath);

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
