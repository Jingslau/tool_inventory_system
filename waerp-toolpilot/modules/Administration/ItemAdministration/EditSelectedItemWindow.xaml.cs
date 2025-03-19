using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.modules.Administration.ItemAdministration;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for EditSelectedItemSaveWindow.xaml
    /// </summary>
    public partial class EditSelectedItemWindow : Window
    {
        public string ImagePathStr;
        public EditSelectedItemWindow()
        {

            InitializeComponent();
            ItemIdent.Text = CurrentItemAdministrationModel.SelectedItemSave["item_ident"].ToString();
            ImagePathStr = CurrentItemAdministrationModel.SelectedItemSave["item_image_path"].ToString();
            ImagePath.Text = ImagePathStr;

            DocumentationPath.Text = CurrentItemAdministrationModel.SelectedItemSave["item_documentation"].ToString();

            if (ImagePathStr != "")
            {
                try
                {
                    Uri imageUri = new Uri(ImagePathStr, UriKind.RelativeOrAbsolute);

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

            //string[] ImagePathArr = ImagePathStr.Split('/');
            //if (ImagePathArr[ImagePathArr.Length - 1] == "default.jpg")
            //{
            //    Uri imageUri = new Uri(ImagePathStr, UriKind.Relative);

            //    ItemImage.Source = new BitmapImage(imageUri);
            //}
            //else
            //{
            //    try
            //    {
            //        Uri imageUri = new Uri(ImagePathStr);

            //        ItemImage.Source = new BitmapImage(imageUri);
            //        ImagePath.Text = ImagePathStr;
            //    }

            //    catch (Exception exp)
            //    {
            //        ErrorLogger.LogSysError(exp);
            //    }

            //}

            Filter1.Items.Add(" ");
            Filter2.Items.Add(" ");
            Filter3.Items.Add(" ");
            Filter4.Items.Add(" ");
            Filter5.Items.Add(" ");


            for (int i = 1; i <= 5; i++)
            {
                DataSet ds3 = AdministrationQueries.GetFilterList(i);
                for (int j = 0; j < ds3.Tables[0].Rows.Count; j++)
                {
                    if (i == 1)
                    {
                        Filter1.Items.Add(ds3.Tables[0].Rows[j]["name"].ToString());
                    }
                    else if (i == 2)
                    {
                        Filter2.Items.Add(ds3.Tables[0].Rows[j]["name"].ToString());
                    }
                    else if (i == 3)
                    {
                        Filter3.Items.Add(ds3.Tables[0].Rows[j]["name"].ToString());

                    }
                    else if (i == 4)
                    {
                        Filter4.Items.Add(ds3.Tables[0].Rows[j]["name"].ToString());
                    }
                    else if (i == 5)
                    {
                        Filter5.Items.Add(ds3.Tables[0].Rows[j]["name"].ToString());
                    }

                }
            }

            DataSet ds2 = ItemAdministrationQueries.RunSql("SELECT * FROM vendor_objects");

            foreach (DataRow row in ds2.Tables[0].Rows)
            {
                VendorCombobox.Items.Add(row["vendor_name"].ToString());
            }

            if (CurrentItemAdministrationModel.SelectedItemSave["item_orderable"].ToString() == "True")
            {
                MinQuant_Text.Text = CurrentItemAdministrationModel.SelectedItemSave["item_quantity_min"].ToString();
                MinOrder_Text.Text = CurrentItemAdministrationModel.SelectedItemSave["item_orderquant_min"].ToString();
                OrderCheckbox.IsChecked = true;
                DataSet ds4 = ItemAdministrationQueries.RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()}");
                if (ds4.Tables[0].Rows.Count > 0)
                {
                    CurrencyCombobox.SelectedIndex = int.Parse(ds4.Tables[0].Rows[0]["currency_id"].ToString());
                    price_Text.Text = ds4.Tables[0].Rows[0]["item_price"].ToString();

                    DataSet vendors = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_id = {ds4.Tables[0].Rows[0]["vendor_id"].ToString()}");

                    int selectedIndex = 0;

                    for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                    {
                        if (ds2.Tables[0].Rows[i]["vendor_name"].ToString() == vendors.Tables[0].Rows[0]["vendor_name"].ToString())
                        {
                            selectedIndex = i;
                        }
                    }

                    VendorCombobox.SelectedIndex = selectedIndex;

                }
            }




            DataSet ds = ItemAdministrationQueries.RunSql("SELECT * FROM currency");
            int defaultIndexCurrency = -1;
            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
            {
                CurrencyCombobox.Items.Add(ds.Tables[0].Rows[j]["currency_code"].ToString());
                if (ds.Tables[0].Rows[j]["currency_isDefault"].ToString() == "True")
                {
                    defaultIndexCurrency = j;
                }
            }


            CurrencyCombobox.SelectedIndex = defaultIndexCurrency;

            DataSet item_ds = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE item_id = '{CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()}'");
            for (int i = 1; i <= 5; i++)
            {
                if (item_ds.Tables[0].Rows[0][$"filter{i}_id"].ToString() != "" || item_ds.Tables[0].Rows[0][$"filter{i}_id"].ToString() != "null")
                {
                    if (i == 1)
                    {
                        if (ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()) == "null")
                        {
                            Filter1.SelectedIndex = -1;
                        }
                        for (int j = 0; j < Filter1.Items.Count; j++)
                        {
                            if (Filter1.Items[j].ToString() == ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()))
                            {
                                Filter1.SelectedIndex = j;
                                break;
                            }
                        }
                    }
                    else if (i == 2)
                    {
                        if (ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()) == "null")
                        {
                            Filter2.SelectedIndex = -1;
                        }
                        for (int j = 0; j < Filter2.Items.Count; j++)
                        {
                            if (Filter2.Items[j].ToString() == ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()))
                            {
                                Filter2.SelectedIndex = j;
                                break;
                            }
                        }
                    }
                    else if (i == 3)
                    {
                        if (ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()) == "null")
                        {
                            Filter3.SelectedIndex = -1;
                        }
                        for (int j = 0; j < Filter3.Items.Count; j++)
                        {
                            if (Filter3.Items[j].ToString() == ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()))
                            {
                                Filter3.SelectedIndex = j;
                                break;
                            }
                        }
                    }
                    else if (i == 4)
                    {
                        if (ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()) == "null")
                        {
                            Filter4.SelectedIndex = -1;
                        }
                        for (int j = 0; j < Filter4.Items.Count; j++)
                        {
                            if (Filter4.Items[j].ToString() == ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()))
                            {
                                Filter4.SelectedIndex = j;
                                break;
                            }
                        }
                    }
                    else if (i == 5)
                    {
                        if (ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()) == "null")
                        {
                            Filter5.SelectedIndex = -1;
                        }
                        for (int j = 0; j < Filter5.Items.Count; j++)
                        {
                            if (Filter5.Items[j].ToString() == ItemAdministrationQueries.GetFilterName(i, CurrentItemAdministrationModel.SelectedItemSave["item_id"].ToString()))
                            {
                                Filter5.SelectedIndex = j;
                                break;
                            }
                        }
                    }
                }
            }



            ItemDescription.Text = CurrentItemAdministrationModel.SelectedItemSave["item_description"].ToString();
            ItemDescription2.Text = CurrentItemAdministrationModel.SelectedItemSave["item_description_2"].ToString();
            ItemDiameter.Text = CurrentItemAdministrationModel.SelectedItemSave["item_diameter"].ToString();

        }



        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            bool EditImage = false;
            string destinationFilePath = "";
            if (ImagePath.Text != CurrentItemAdministrationModel.SelectedItemSave["item_image_path"].ToString() | ImagePath.Text != "")
            {
                try
                {
                    ItemImage.Source = null;
                    ItemImage.InvalidateVisual();

                    EditImage = true;
                    DataSet globalImagePath = AdministrationQueries.RunSql($"SELECT * FROM company_settings WHERE settings_name = 'global_image_path'");
                    string sourcePath = $@"{ImagePath.Text}";
                    string destinationFolderPath = $@"{globalImagePath.Tables[0].Rows[0][2]}";
                    fileName = Path.GetFileName(sourcePath);
                    destinationFilePath = Path.Combine(destinationFolderPath, fileName);

                    if (!File.Exists(destinationFilePath))
                    {
                        File.Copy(sourcePath, destinationFilePath);
                    }
                }
                catch (ArgumentException)
                {

                }
                catch (Exception ex)
                {
                    // Handle other types of exceptions
                    Console.WriteLine("An error occurred: " + ex.Message);
                }





            }

            string documentationPath = $@"{DocumentationPath.Text}";
            DataSet globalDocumentationPath = AdministrationQueries.RunSql($"SELECT * FROM company_settings WHERE settings_name = 'global_history_path'");
            string destinationFolderPathDocumentation = $@"{globalDocumentationPath.Tables[0].Rows[0][2]}";
            string fileNameDocumentation = Path.GetFileName(documentationPath);

            if (documentationPath.Length > 0)
            {
                try
                {
                    // Ensure the source file exists
                    if (!File.Exists(documentationPath))
                    {
                        Console.WriteLine($"Source file does not exist: {documentationPath}");
                    }
                    else
                    {
                        // Build the destination file path
                        string destinationDocumentationPath = Path.Combine(destinationFolderPathDocumentation, fileNameDocumentation);

                        // Check if the file already exists in the destination
                        if (File.Exists(destinationDocumentationPath))
                        {
                            Console.WriteLine($"File already exists in the destination: {destinationDocumentationPath}");
                        }
                        else
                        {
                            // Ensure the destination directory exists
                            if (!Directory.Exists(destinationFolderPathDocumentation))
                            {
                                Directory.CreateDirectory(destinationFolderPathDocumentation);
                                Console.WriteLine($"Destination directory created: {destinationFolderPathDocumentation}");
                            }

                            // Move the file
                            File.Copy(documentationPath, destinationDocumentationPath);
                            Console.WriteLine($"File moved to: {destinationDocumentationPath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                documentationPath = "";
            }


            if (ItemIdent.Text != "" | ItemDescription.Text != "" | Filter1.Text != "")
            {


                CurrentItemAdministrationModel.NewItemIdentStr = ItemIdent.Text;
                CurrentItemAdministrationModel.NewItemDescription = ItemDescription.Text;
                CurrentItemAdministrationModel.NewItemFilter1 = Filter1.Text;
                CurrentItemAdministrationModel.NewItemFilter2 = Filter2.Text;
                CurrentItemAdministrationModel.NewItemFilter3 = Filter3.Text;
                CurrentItemAdministrationModel.NewItemFilter4 = Filter4.Text;
                CurrentItemAdministrationModel.NewItemFilter5 = Filter5.Text;






                if (OrderCheckbox.IsChecked == true)
                {
                    CurrentItemAdministrationModel.NewItemIsOrderable = true;
                    if (VendorCombobox.Text == "" | MinQuant_Text.Text == "" | MinOrder_Text.Text == "" | price_Text.Text == "" | CurrencyCombobox.Text == "")
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("errorText19");
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow openError = new ErrorWindow();
                        openError.ShowDialog();

                    }
                    else
                    {
                        DataSet selectedVendor = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_name = '{VendorCombobox.SelectedItem.ToString()}'");
                        int selectedVendorID = int.Parse(selectedVendor.Tables[0].Rows[0][0].ToString());

                        CurrentItemAdministrationModel.NewItemPrice = price_Text.Text;
                        CurrentItemAdministrationModel.NewItemCurrencyID = CurrencyCombobox.SelectedIndex;
                        CurrentItemAdministrationModel.NewItemVendorID = selectedVendorID;

                        CurrentItemAdministrationModel.NewItemMinQuant = MinQuant_Text.Text;
                        CurrentItemAdministrationModel.NewItemMinOrder = MinOrder_Text.Text;
                        ItemAdministrationQueries.SaveEditedItem(fileName, EditImage, ItemDescription2.Text, ItemDiameter.Text, fileNameDocumentation);

                        DialogResult = false;
                    }
                }
                else
                {
                    ItemAdministrationQueries.SaveEditedItem(fileName, EditImage, ItemDescription2.Text, ItemDiameter.Text, fileNameDocumentation);

                    DialogResult = false;
                }
            }
            else
            {
                CurrentItemAdministrationModel.NewItemIsOrderable = false;
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText13");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }



        }


        private void Filter1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Filter1.SelectedItem != null)
            {
                Filter2.IsEnabled = true;
            }
            else
            {
                Filter2.IsEnabled = false;
                Filter2.Text = "";
                Filter2.SelectedIndex = -1;
                Filter3.SelectedIndex = -1;
                Filter3.Text = "";
                Filter3.IsEnabled = false;
                Filter4.SelectedIndex = -1;
                Filter4.Text = "";
                Filter4.IsEnabled = false;
                Filter5.SelectedIndex = -1;
                Filter5.Text = "";
                Filter5.IsEnabled = false;
            }
        }


        private void Filter2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Filter2.SelectedItem != null && Filter2.SelectedIndex != 0)
            {
                Filter3.IsEnabled = true;
            }
            else
            {
                Filter3.SelectedIndex = -1;
                Filter3.Text = "";
                Filter3.IsEnabled = false;
                Filter4.SelectedIndex = -1;
                Filter4.Text = "";
                Filter4.IsEnabled = false;
                Filter5.SelectedIndex = -1;
                Filter5.Text = "";
                Filter5.IsEnabled = false;
            }
        }



        private void Filter3_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Filter3.SelectedItem != null && Filter3.SelectedIndex != 0)
            {
                Filter4.IsEnabled = true;
            }
            else
            {
                Filter4.SelectedIndex = -1;
                Filter4.Text = "";
                Filter4.IsEnabled = false;
                Filter5.SelectedIndex = -1;
                Filter5.Text = "";
                Filter5.IsEnabled = false;
            }
        }
        private void Filter4_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Filter4.SelectedItem != null && Filter4.SelectedIndex != 0)
            {
                Filter5.IsEnabled = true;
            }
            else
            {
                Filter5.SelectedIndex = -1;
                Filter5.Text = "";
                Filter5.IsEnabled = false;
            }
        }


        private void OrderCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (OrderCheckbox.IsChecked == true)
            {
                VendorCombobox.IsEnabled = true;
                MinQuant_Text.IsEnabled = true;
                MinOrder_Text.IsEnabled = true;
                price_Text.IsEnabled = true;
                CurrencyCombobox.IsEnabled = true;
            }

        }

        private void OrderCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (OrderCheckbox.IsChecked == false)
            {
                VendorCombobox.IsEnabled = false;
                MinQuant_Text.IsEnabled = false;
                MinOrder_Text.IsEnabled = false;
                CurrencyCombobox.IsEnabled = false;
                price_Text.IsEnabled = false;
                VendorCombobox.SelectedIndex = -1;
            }
        }

        private void MinQuant_Text_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Get the TextBox reference
            TextBox textBox = (TextBox)sender;

            // Get the current caret position
            int caretIndex = textBox.CaretIndex;

            // Remove non-numeric characters
            string text = new string(textBox.Text.Where(c => char.IsDigit(c)).ToArray());

            // Handle leading zeros
            if (text.Length > 1 && text[0] == '0')
            {
                text = text.TrimStart('0');
            }

            // Set the modified text back to the TextBox
            textBox.Text = text;

            // Ensure the value is greater than or equal to 0
            if (!string.IsNullOrEmpty(text) && int.Parse(text) < 0)
            {
                textBox.Text = "0";
            }

            // Adjust the caret position if necessary
            if (caretIndex > textBox.Text.Length)
            {
                caretIndex = textBox.Text.Length;
            }

            // Set the new caret position
            textBox.CaretIndex = caretIndex;
        }

        private void MinOrder_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the TextBox reference
            TextBox textBox = (TextBox)sender;

            // Get the current caret position
            int caretIndex = textBox.CaretIndex;

            // Remove non-numeric characters
            string text = new string(textBox.Text.Where(c => char.IsDigit(c)).ToArray());

            // Handle leading zeros
            if (text.Length > 1 && text[0] == '0')
            {
                text = text.TrimStart('0');
            }

            // Set the modified text back to the TextBox
            textBox.Text = text;

            // Ensure the value is greater than or equal to 0
            if (!string.IsNullOrEmpty(text) && int.Parse(text) < 0)
            {
                textBox.Text = "0";
            }

            // Adjust the caret position if necessary
            if (caretIndex > textBox.Text.Length)
            {
                caretIndex = textBox.Text.Length;
            }

            // Set the new caret position
            textBox.CaretIndex = caretIndex;
        }


        private void CurrencyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string decimalString = ItemAdministrationQueries.RunSql("SELECT * FROM settings WHERE settings_name = 'decimal'").Tables[0].Rows[0]["settings_input"].ToString();
            char decimalChar = char.Parse(ItemAdministrationQueries.RunSql("SELECT * FROM settings WHERE settings_name = 'decimal'").Tables[0].Rows[0]["settings_input"].ToString());

            // Allow only numeric and decimal input
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]{0,2})?$");

            string newText = GetNewText(price_Text, e.Text);
            string updatedText = FormatCurrencyValue(newText);

            bool isValidInput = regex.IsMatch(updatedText);

            e.Handled = !isValidInput || updatedText != newText;

            if (isValidInput)
            {
                int caretIndex = price_Text.CaretIndex;
                string text = price_Text.Text;

                // If a decimal point is added, adjust the caret position
                if (e.Text == decimalString && text.Contains(decimalString))
                {
                    int dotIndex = text.IndexOf(decimalString);
                    price_Text.CaretIndex = dotIndex + 1;
                    e.Handled = true;
                }
                else if (caretIndex < text.Length && text[caretIndex] == decimalChar)
                {
                    price_Text.CaretIndex = caretIndex + 1;
                }
            }
        }

        private void price_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the text format
            price_Text.Text = FormatCurrencyValue(price_Text.Text);
        }

        private string GetNewText(TextBox textBox, string input)
        {
            int caretIndex = textBox.CaretIndex;
            string text = textBox.Text;
            return text.Substring(0, caretIndex) + input + text.Substring(caretIndex + textBox.SelectionLength);
        }

        private string FormatCurrencyValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            string decimalString = ItemAdministrationQueries.RunSql("SELECT * FROM settings WHERE settings_name = 'decimal'").Tables[0].Rows[0]["settings_input"].ToString();
            char decimalChar = char.Parse(ItemAdministrationQueries.RunSql("SELECT * FROM settings WHERE settings_name = 'decimal'").Tables[0].Rows[0]["settings_input"].ToString());

            // Remove non-numeric and non-decimal characters
            string formattedValue = new string(value.Where(c => char.IsDigit(c) || c == decimalChar).ToArray());

            // Split into whole number and decimal parts
            string[] parts = formattedValue.Split(decimalChar);

            if (parts.Length > 1)
            {
                // Truncate decimal part to two digits
                parts[1] = parts[1].PadRight(2, '0');
            }
            parts[0] = RemoveLeadingZeros(parts[0]);
            // Reconstruct the formatted value
            formattedValue = string.Join(decimalString, parts);

            return formattedValue;
        }
        private string RemoveLeadingZeros(string input)
        {
            if (input.Length > 1 && input[0] == '0')
            {
                int firstNonZeroIndex = input.IndexOfAny(new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                if (firstNonZeroIndex != -1)
                {
                    return input.Substring(firstNonZeroIndex);
                }
                return "0";
            }
            return input;
        }

        private void ImagePathBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "JPEG|*.jpg|PNG|*.png";
            if (openFileDialog.ShowDialog() == true)
            {

                ImagePath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);

                string selectedFilePath = openFileDialog.FileName;
                Uri imageUri = new Uri(selectedFilePath);
                ItemImage.Source = new BitmapImage(imageUri);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric characters, comma, and period
            e.Handled = !IsTextAllowed(e.Text);


        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemDiameter.Text))
            {
                // Replace commas with periods and try parsing as a double
                string text = ItemDiameter.Text.Replace(',', '.');
                if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    // Round the value to two decimal places
                    value = Math.Round(value, 2);

                    // Format the value to two decimal places and correct the input if necessary
                    ItemDiameter.Text = value.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',');
                }
                else
                {
                    // Clear the text if it's not a valid double
                    ItemDiameter.Clear();
                }
            }

        }
        private bool IsTextAllowed(string text)
        {
            // Allow only numeric characters, comma, and period
            return Array.TrueForAll(text.ToCharArray(), c => char.IsDigit(c) || c == ',' || c == '.');
        }

        private void addSymbolBtn2_Click(object sender, RoutedEventArgs e)
        {
            ItemDescription2.Text += "⌀";
        }

        private void addSymbolBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemDescription.Text += "⌀";
        }

        private void AddFilterWindow(object sender, RoutedEventArgs e)
        {
            AddNewFilterWindow openFilterCreation = new AddNewFilterWindow();
            openFilterCreation.ShowDialog();

            int Filter1SelectedIndex = Filter1.SelectedIndex;
            int Filter2SelectedIndex = Filter2.SelectedIndex;
            int Filter3SelectedIndex = Filter3.SelectedIndex;
            int Filter4SelectedIndex = Filter4.SelectedIndex;
            int Filter5SelectedIndex = Filter5.SelectedIndex;


            Filter1.Items.Clear();
            Filter2.Items.Clear();
            Filter3.Items.Clear();
            Filter4.Items.Clear();
            Filter5.Items.Clear();


            Filter1.Items.Add("");
            Filter2.Items.Add("");
            Filter3.Items.Add("");
            Filter4.Items.Add("");
            Filter5.Items.Add("");

            bool stop1 = false;
            DataSet ds = new DataSet();

            for (int i = 1; i <= 5; i++)
            {
                ds = AdministrationQueries.GetFilterList(i);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        if (i == 1)
                        {
                            Filter1.Items.Add(ds.Tables[0].Rows[j]["name"].ToString());
                        }
                        else if (i == 2)
                        {
                            Filter2.Items.Add(ds.Tables[0].Rows[j]["name"].ToString());
                        }
                        else if (i == 3)
                        {
                            Filter3.Items.Add(ds.Tables[0].Rows[j]["name"].ToString());
                        }
                        else if (i == 4)
                        {
                            Filter4.Items.Add(ds.Tables[0].Rows[j]["name"].ToString());
                        }
                        else if (i == 5)
                        {
                            Filter5.Items.Add(ds.Tables[0].Rows[j]["name"].ToString());
                        }
                    }
                }
                else
                {
                    stop1 = true;
                }
            }
            int FilterCounter = 1;
            bool stop = false;
            DataSet allItems = AdministrationQueries.RunSql("SELECT * FROM item_objects");
            if (allItems.Tables[0].Rows.Count > 0)
            {
                if (!stop1)
                {
                    string FilterName = ItemAdministrationQueries.GetFilterName(FilterCounter, CurrentItemAdministrationModel.SelectedItem["item_id"].ToString());

                    if (FilterName != "")
                    {
                        for (int i = 0; i < Filter1.Items.Count; i++)
                        {
                            if (Filter1.Items[i].ToString() == FilterName)
                            {
                                Filter1.SelectedIndex = i;
                                FilterCounter++;

                                break;
                            }
                        }
                        Filter2.IsEnabled = true;
                    }
                    else
                    {

                        stop = true;
                    }

                    FilterName = ItemAdministrationQueries.GetFilterName(FilterCounter, CurrentItemAdministrationModel.SelectedItem["item_id"].ToString());

                    if (FilterName != "" && !stop)
                    {
                        for (int i = 0; i < Filter2.Items.Count; i++)
                        {
                            if (Filter2.Items[i].ToString() == FilterName)
                            {
                                Filter2.SelectedIndex = i;
                                FilterCounter++;
                                Filter3.IsEnabled = true;
                                break;
                            }
                        }
                    }

                    FilterName = ItemAdministrationQueries.GetFilterName(FilterCounter, CurrentItemAdministrationModel.SelectedItem["item_id"].ToString());

                    if (FilterName != "" && !stop)
                    {
                        for (int i = 0; i < Filter3.Items.Count; i++)
                        {
                            if (Filter3.Items[i].ToString() == FilterName)
                            {
                                Filter3.SelectedIndex = i;
                                FilterCounter++;
                                Filter4.IsEnabled = true;
                                break;
                            }
                        }
                    }


                    FilterName = ItemAdministrationQueries.GetFilterName(FilterCounter, CurrentItemAdministrationModel.SelectedItem["item_id"].ToString());

                    if (FilterName != "" && !stop)
                    {
                        for (int i = 0; i < Filter4.Items.Count; i++)
                        {
                            if (Filter4.Items[i].ToString() == FilterName)
                            {
                                Filter4.SelectedIndex = i;
                                FilterCounter++;
                                Filter5.IsEnabled = true;
                                break;
                            }
                        }
                    }

                    FilterName = ItemAdministrationQueries.GetFilterName(FilterCounter, CurrentItemAdministrationModel.SelectedItem["item_id"].ToString());

                    if (FilterName != "" && !stop)
                    {
                        for (int i = 0; i < Filter5.Items.Count; i++)
                        {
                            if (Filter5.Items[i].ToString() == FilterName)
                            {
                                Filter5.SelectedIndex = i;
                                FilterCounter++;
                                break;
                            }
                        }
                    }
                }
            }
            Filter1.SelectedIndex = Filter1SelectedIndex;
            Filter2.SelectedIndex = Filter2SelectedIndex;
            Filter3.SelectedIndex = Filter3SelectedIndex;
            Filter4.SelectedIndex = Filter4SelectedIndex;
            Filter5.SelectedIndex = Filter5SelectedIndex;
        }

        private void DocumentationPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "PDF|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {

                DocumentationPath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);



            }
        }

        private void DeleteDocumentation_Click(object sender, RoutedEventArgs e)
        {
            DocumentationPath.Text = "";
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            ImagePath.Text = "";
        }
    }
}
