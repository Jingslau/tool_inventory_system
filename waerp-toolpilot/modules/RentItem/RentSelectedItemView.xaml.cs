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
using waerp_toolpilot.application.RentItem;
using waerp_toolpilot.application.ReportLocation;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.RentItem;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.rentItem
{
    /// <summary>
    /// Interaction logic for RentSelectedItemView.xaml
    /// </summary>
    public partial class RentSelectedItemView : Window
    {
        public string locationID;
        public string compartmentID;
        public string locationName;
        public string locationQuantity;
        public string groupID;
        public string zoneName;
        public string groupName;
        public string groupQuantity;
        public bool IsGroup = false;
        public DataRowView SelectedLocationRow;
        public DataSet AllLocations = new DataSet();
        public string isUsed = "";
        public DataSet FilteredLocations = new DataSet();
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public string documentationPath = "";
        public RentSelectedItemView()
        {
            InitializeComponent();
            GetItemContent();
            GetLocations();
            ReportWrongLocationModel.ItemIdent = CurrentRentModel.ItemIdentStr;
            RentBtn.IsEnabled = false;
            Boolean check = false;
            for (int i = 0; i < CurrentRentModel.ItemIdentStr.Length; i++)
            {
                if (CurrentRentModel.ItemIdentStr[i].ToString() == "ä" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "ü" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "ö" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "Ä" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "Ü" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "Ö" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "ß" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "%" ||
                    CurrentRentModel.ItemIdentStr[i].ToString() == "&")
                {
                    check = true;
                }
            }
            if (check == false)
            {

                Barcode.Text = "*" + CurrentRentModel.ItemIdentStr + "*";
            }
            else
            {
                Barcode.Text = "";
            }

        }

        private void GetItemContent()
        {
            conn.Open();
            string query = $@"SELECT 
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
		item_objects WHERE item_id =  {CurrentRentModel.ItemIdent}
	ORDER BY 
		item_ident ASC;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);



            CurrentRentModel.ItemIdentStr = ds.Tables[0].Rows[0]["item_ident"].ToString();
            CurrentRentModel.ItemTotalQuantity = ds.Tables[0].Rows[0]["item_quantity_total"].ToString();
            CurrentRentModel.ItemDescription = ds.Tables[0].Rows[0]["item_description"].ToString();
            CurrentRentModel.ItemDescription2 = ds.Tables[0].Rows[0]["item_description_2"].ToString();
            CurrentRentModel.ItemImagePath = ds.Tables[0].Rows[0]["item_image_path"].ToString();

            if (ds.Tables[0].Rows[0]["item_documentation"].ToString() != "")
            {
                documentationPath = ds.Tables[0].Rows[0]["item_documentation"].ToString();
                ShowDocumentation.Visibility = Visibility.Visible;
            }
            else
            {
                ShowDocumentation.Visibility = Visibility.Collapsed;
            }

            ItemIdent.Text = CurrentRentModel.ItemIdentStr;
            ItemDescription.Text = CurrentRentModel.ItemDescription;
            ItemDescription2.Text = CurrentRentModel.ItemDescription2;
            ItemDiameter.Text = ds.Tables[0].Rows[0]["item_diameter"].ToString();

            ItemTotalQuantity.Text = CurrentRentModel.ItemTotalQuantity;
            if (CurrentRentModel.ItemImagePath != "")
            {
                try
                {
                    Uri imageUri = new Uri(CurrentRentModel.ItemImagePath, UriKind.RelativeOrAbsolute);

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

            //DataSet ds = RentItemQueries.GetLocations();
            //locationData.DataContext = ds;
            //if (ds.Tables.Count > 0)
            //{
            //    AllLocations = ds;
            //    locationData.ItemsSource = new DataView(ds.Tables[0]);
            //}

            DataSet ds = TempLocationsQueries.GetItemLocations(CurrentRentModel.ItemIdent);

            if (ds.Tables.Count > 0)
            {
                AllLocations = ds;
                locationData.ItemsSource = new DataView(ds.Tables[0]);
            }


        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public static DateTime Now { get; }

        private void RentItem(object sender, RoutedEventArgs e)
        {
            CurrentRentModel.IsGroup = IsGroup;
            if (IsGroup)
            {
                int NewQuant = int.Parse(CurrentRentModel.ItemTotalQuantity) - int.Parse(QuantityInput.Text);
                if (QuantityInput.Text == "")
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText42");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow notAllowed = new ErrorWindow();
                    notAllowed.ShowDialog();
                }
                else
                {
                    if (int.Parse(groupQuantity) < int.Parse(QuantityInput.Text))
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("errorText57");
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow notAllowed = new ErrorWindow();
                        notAllowed.ShowDialog();
                    }
                    else
                    {
                        if (int.Parse(QuantityInput.Text) > 0)
                        {
                            if (RentItemQueries.RentItemExec(NewQuant.ToString(), QuantityInput.Text, groupID, groupQuantity, zoneName, isUsed))
                            {
                                CurrentRentModel.RentLocation = locationName;
                                CurrentRentModel.RentQuantity = QuantityInput.Text.ToString();


                                SuccessRentView successDialog = new SuccessRentView();
                                Nullable<bool> dialogResult = successDialog.ShowDialog();
                                DialogResult = false;
                            }
                            else
                            {
                                ErrorHandlerModel.ErrorText = (string)FindResource("errorText58");
                                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                                ErrorWindow notAllowed = new ErrorWindow();
                                notAllowed.ShowDialog();
                            }
                        }
                        else if (int.Parse(QuantityInput.Text) == 0)
                        {
                            MessageBox.Show((string)FindResource("errorText41"));
                            ErrorHandlerModel.ErrorText = "";
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow notAllowed = new ErrorWindow();
                            notAllowed.ShowDialog();
                        }
                        else if (int.Parse(groupQuantity) - int.Parse(QuantityInput.Text) < 0)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText59");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow notAllowed = new ErrorWindow();
                            notAllowed.ShowDialog();
                        }
                        else
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText60");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow notAllowed = new ErrorWindow();
                            notAllowed.ShowDialog();
                        }

                    }
                }
            }
            else
            {
                int NewQuant = int.Parse(CurrentRentModel.ItemTotalQuantity) - int.Parse(QuantityInput.Text);

                if (QuantityInput.Text == "")
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText40");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow notAllowed = new ErrorWindow();
                    notAllowed.ShowDialog();
                }
                else
                {
                    if (int.Parse(locationQuantity) < int.Parse(QuantityInput.Text))
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("errorText59");
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow notAllowed = new ErrorWindow();
                        notAllowed.ShowDialog();
                    }
                    else
                    {
                        if (int.Parse(QuantityInput.Text) > 0)
                        {
                            CurrentRentModel.newQuant = NewQuant.ToString();
                            CurrentRentModel.RentLocation = locationName;
                            CurrentRentModel.RentQuantity = QuantityInput.Text.ToString();
                            CurrentRentModel.locationID = locationID;
                            CurrentRentModel.compartmentID = compartmentID;
                            CurrentRentModel.locationQuantity = locationQuantity;

                            CurrentRentModel.zoneName = zoneName;

                            CurrentRentModel.isUsed = isUsed;
                            SelectMachineWindow openMachineSelection = new SelectMachineWindow();
                            openMachineSelection.ShowDialog();


                            if (CurrentRentModel.closeWindow) { CurrentRentModel.closeWindow = false; DialogResult = false; }

                        }
                        else if (int.Parse(QuantityInput.Text) == 0)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText61");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow notAllowed = new ErrorWindow();
                            notAllowed.ShowDialog();
                        }
                        else if (int.Parse(locationQuantity) - int.Parse(QuantityInput.Text) < 0)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText59");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow notAllowed = new ErrorWindow();
                            notAllowed.ShowDialog();
                        }
                        else
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText60");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow notAllowed = new ErrorWindow();
                            notAllowed.ShowDialog();
                        }

                    }



                }
            }


        }

        private void locationData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ZoneGroupData.SelectedIndex = -1;
            IsGroup = false;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ReportWrongLocationModel.LocationIdent = row_selected["location_name"].ToString();
                locationID = row_selected["location_id"].ToString();
                compartmentID = row_selected["compartment_id"].ToString();
                SelectedLocationRow = row_selected;
                locationName = row_selected["location_name"].ToString();
                locationQuantity = row_selected["item_quantity"].ToString();
                CurrentRentModel.RentLocation = row_selected["location_name"].ToString();
                if (row_selected["is_dynamic"].ToString() == "1")
                {
                    CurrentRentModel.is_dynamic = true;
                }
                else
                {
                    CurrentRentModel.is_dynamic = false;
                }
                if (row_selected["item_used"].ToString() == "1")
                {
                    CurrentRentModel.isUsed = "1";
                    isUsed = "1";
                }
                else
                {
                    CurrentRentModel.isUsed = "0";

                    isUsed = "0";
                }
            }
            if (int.Parse(locationQuantity) > 0)
            {
                RentBtn.IsEnabled = true;
            }
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
            CurrentRentModel.RentQuantity = QuantityInput.Text.ToString();
            RentNumberInputView numberInput = new RentNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = CurrentRentModel.RentQuantity.ToString();
        }

        private void ZoneGroupData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsGroup = true;
            locationData.SelectedIndex = -1;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ReportWrongLocationModel.LocationIdent = row_selected["group_name"].ToString();
                zoneName = row_selected["floor_name"].ToString();
                groupID = row_selected["group_id"].ToString();
                groupName = row_selected["group_name"].ToString();
                groupQuantity = row_selected["item_quantity"].ToString();
                CurrentRentModel.RentLocation = row_selected["group_name"].ToString();
            }
            if (int.Parse(groupQuantity) > 0)
            {
                RentBtn.IsEnabled = true;
            }
        }

        private void resetSearch(object sender, RoutedEventArgs e)
        {

        }

        private void ReportErrorLocationBtn_Click(object sender, RoutedEventArgs e)
        {

            ReportLocationView openReport = new ReportLocationView();
            openReport.ShowDialog();
        }

        //private void ConstructedItems_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ConstructedItems.IsChecked == true)
        //    {
        //        UsedItems.IsChecked = false;
        //        FilteredLocations.Clear();
        //        FilteredLocations = AllLocations.Copy();
        //        FilteredLocations.Tables[0].Rows.Clear();

        //        for (int i = 0; i < AllLocations.Tables[0].Rows.Count; i++)
        //        {
        //            if (AllLocations.Tables[0].Rows[i]["item_constructed"].ToString() == "1")
        //            {
        //                FilteredLocations.Tables[0].ImportRow(AllLocations.Tables[0].Rows[i]);
        //            }
        //        }
        //        locationData.DataContext = FilteredLocations;
        //        locationData.ItemsSource = new DataView(FilteredLocations.Tables[0]);
        //    }
        //    else
        //    {
        //        locationData.DataContext = AllLocations;
        //        locationData.ItemsSource = new DataView(AllLocations.Tables[0]);
        //    }
        //}

        private void UsedItems_Click(object sender, RoutedEventArgs e)
        {
            if (UsedItems.IsChecked == true)
            {
                FilteredLocations.Clear();
                FilteredLocations = AllLocations.Copy();
                FilteredLocations.Tables[0].Rows.Clear();

                for (int i = 0; i < AllLocations.Tables[0].Rows.Count; i++)
                {
                    if (AllLocations.Tables[0].Rows[i]["item_used"].ToString() == "1")
                    {
                        FilteredLocations.Tables[0].ImportRow(AllLocations.Tables[0].Rows[i]);
                    }
                }
                locationData.DataContext = FilteredLocations;
                locationData.ItemsSource = new DataView(FilteredLocations.Tables[0]);
            }
            else
            {
                locationData.DataContext = AllLocations;
                locationData.ItemsSource = new DataView(AllLocations.Tables[0]);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
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

