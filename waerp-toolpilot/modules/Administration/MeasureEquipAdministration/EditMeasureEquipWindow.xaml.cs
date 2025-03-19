using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for EditMeasureEquipWindow.xaml
    /// </summary>
    public partial class EditMeasureEquipWindow : Window
    {
        public EditMeasureEquipWindow()
        {
            InitializeComponent();
            CurrentStatusCombobox.Items.Add("Geprüft");

            CurrentStatusCombobox.Items.Add("Nicht geprüft");
            CurrentStatusCombobox.Items.Add("Gesperrt");
            CurrentStatusCombobox.Items.Add("Reperatur");
            CurrentStatusCombobox.Items.Add("Ausmusterung");
            CurrentStatusCombobox.Items.Add("Neukalibrierung");
            loadItemData();
            GetAuditors();

        }

        private void loadItemData()
        {
            ImagePath.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_image"].ToString();
            DocumentationPath.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_documentation"].ToString();
            MeasureEquipName.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_name"].ToString();
            SerialNumber.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_serialnumber"].ToString();
            Remark.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_remark"].ToString();
            AuditCycle.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_cycle"].ToString();



            CurrentStatusCombobox.SelectedItem = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_status"].ToString();

            if (MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_dateOfAcquisition"].ToString() != "")
            {
                DateOfAcquisition.Text = DateTime.Parse(MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_dateOfAcquisition"].ToString()).ToString("dd.MM.yyyy");

            }

            if (MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_lastCheckUp"].ToString() != "")
            {
                LastCheckUp.Text = DateTime.Parse(MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_lastCheckUp"].ToString()).ToString("dd.MM.yyyy");

            }


            if (MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_image"].ToString() != "")
            {
                string itemImagePath = Path.Combine(RunningParameters.imagePath, MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_image"].ToString());
                try
                {
                    Uri imageUri = new Uri(itemImagePath, UriKind.RelativeOrAbsolute);

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

        }

        private void GetAuditors()
        {
            int selectedAuditor = 0;
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM measuring_equip_auditor_objects");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AuditorCombobox.Items.Add(ds.Tables[0].Rows[i]["auditor_name"]);

                    if (ds.Tables[0].Rows[i]["auditor_name"].ToString() == MeasureEquipModels.CurrentMeasureEquipItem["auditor_name"].ToString())
                    {
                        selectedAuditor = i;
                    }
                }
                AuditorCombobox.SelectedIndex = selectedAuditor;
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (MeasureEquipName.Text.Length <= 0)
            {
                ErrorHandlerModel.ErrorText = "Bitte gebe einen Messmittelnamen an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else if (SerialNumber.Text.Length <= 0)
            {
                ErrorHandlerModel.ErrorText = "Bitte gebe die Seriennummer zum Messmittel an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else if (int.Parse(AuditCycle.Text) <= 0)
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen gültigen Prüfzyklus (größer 0) an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else if (DateOfAcquisition.Text.Length == 0)
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen gültigen Anschaffungsdatum an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                string checkQuerySN = $"SELECT * FROM measuring_equip_objects WHERE measuring_equip_serialnumber = '{SerialNumber.Text}'";
                string checkQueryName = $"SELECT * FROM measuring_equip_objects WHERE measuring_equip_name = '{MeasureEquipName.Text}'";

                if (AdministrationQueries.RunSql(checkQuerySN).Tables[0].Rows.Count > 1)
                {
                    ErrorHandlerModel.ErrorText = "Es existiert bereits ein Messmittel mit dieser Seriennummer!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
                else if (AdministrationQueries.RunSql(checkQueryName).Tables[0].Rows.Count > 1)
                {
                    ErrorHandlerModel.ErrorText = "Es existiert bereits ein Messmittel mit diesem Namen!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();

                }
                else
                {

                    // Artikelbild
                    string fileName = "";
                    string destinationFilePath = "";
                    if (ImagePath.Text != "")
                    {
                        if (Path.GetFileName(ImagePath.Text) != MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_image"].ToString())
                        {
                            DataSet globalImagePath = AdministrationQueries.RunSql($"SELECT * FROM company_settings WHERE settings_name = 'global_image_path'");
                            string sourcePath = $@"{ImagePath.Text}";
                            string destinationFolderPath = $@"{globalImagePath.Tables[0].Rows[0][2]}";
                            fileName = Path.GetFileName(sourcePath);
                            destinationFilePath = Path.Combine(destinationFolderPath, fileName);

                            if (!File.Exists(destinationFilePath))
                            {
                                try
                                {
                                    File.Copy(sourcePath, destinationFilePath);
                                }
                                catch (Exception exp)
                                {
                                    ErrorLogger.LogSysError(exp);
                                }
                            }
                        }
                        else
                        {
                            fileName = Path.GetFileName(ImagePath.Text);
                        }
                    }
                    else
                    {
                        fileName = Path.GetFileName(ImagePath.Text);
                    }


                    // Dokumentation

                    string documentationPath = $@"{DocumentationPath.Text}";
                    DataSet globalDocumentationPath = AdministrationQueries.RunSql($"SELECT * FROM company_settings WHERE settings_name = 'global_history_path'");
                    string destinationFolderPathDocumentation = $@"{globalDocumentationPath.Tables[0].Rows[0][2]}";
                    string fileNameDocumentation = Path.GetFileName(documentationPath);

                    if (documentationPath.Length > 0 && fileNameDocumentation != MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_documentation"].ToString())
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
                            fileNameDocumentation = "";
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }
                    else
                    {
                        fileNameDocumentation = Path.GetFileName(documentationPath);
                    }

                    // Datum

                    string acquisitionDate = DateTime.ParseExact(DateOfAcquisition.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");


                    fileNameDocumentation = Path.GetFileName(DocumentationPath.Text);
                    string auditorID = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_auditor_objects WHERE auditor_name = '{AuditorCombobox.SelectedItem}'").Tables[0].Rows[0]["auditor_id"].ToString();


                    string query = $"UPDATE measuring_equip_objects SET " +
                        $"measuring_equip_name = '{MeasureEquipName.Text}'," +
                        $"measuring_equip_serialnumber = '{SerialNumber.Text}'," +
                        $"measuring_equip_cycle = '{AuditCycle.Text}'," +
                        $"measuring_equip_remark = '{Remark.Text}'," +
                        $"measuring_equip_status = '{CurrentStatusCombobox.SelectedItem}'," +
                        $"measuring_equip_auditor_id = '{auditorID}'," +
                        $"measuring_equip_dateOfAcquisition = '{acquisitionDate}'," +
                        $"measuring_equip_image = '{fileName}'," +
                        $"measuring_equip_documentation = '{fileNameDocumentation}'" +
                        $" WHERE measuring_equip_id = '{MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"].ToString()}'";

                    DataSet ds_name = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_objects WHERE measuring_equip_name = '{MeasureEquipName.Text}'");

                    DataSet ds_serialnumber = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_objects WHERE measuring_equip_serialnumber = '{SerialNumber.Text}'");

                    if (ds_name.Tables[0].Rows.Count == 1 && ds_serialnumber.Tables[0].Rows.Count == 1)
                    {
                        AdministrationQueries.RunSqlExec(query);
                        ErrorHandlerModel.ErrorText = "Das Messmittel wurde erfolgreich bearbeitet!";
                        ErrorHandlerModel.ErrorType = "SUCCESS";
                        ErrorWindow showSuccess = new ErrorWindow();
                        showSuccess.ShowDialog();
                        DialogResult = false;
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = "Dieses Messmittel konnte nicht bearbeitet werden!";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }
                }
            }

        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
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

        private void DocumentationPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "PDF|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                DocumentationPath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);

            }
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            ImagePath.Text = "";
            Uri imageUri = new Uri("pack://application:,,,/assets/images/default/default.jpg");
            ItemImage.Source = new BitmapImage(imageUri);
        }

        private void DeleteDocumentation_Click(object sender, RoutedEventArgs e)
        {
            DocumentationPath.Text = "";
        }  // Allow numeric input only for day, month, and year fields.
        private void DateTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Block spaces or special characters
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        // Validate day field (1 to 31)
        private void DateOfAcquisitionDay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Use Regex to allow only numbers and ensure values are 1-31.
            Regex dayRegex = new Regex("^(0?[1-9]|[1-2][0-9]|3[0-1])$");
            e.Handled = !dayRegex.IsMatch(fullText);
        }

        // Validate month field (1 to 12)
        private void DateOfAcquisitionMonth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Use Regex to allow only numbers and ensure values are 1-12.
            Regex monthRegex = new Regex("^(0?[1-9]|1[0-2])$");
            e.Handled = !monthRegex.IsMatch(fullText);
        }

        // Validate year field (1 to 4 digits)
        private void DateOfAcquisitionYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Use Regex to allow only numbers and ensure values are 1 to 4 digits.
            Regex yearRegex = new Regex("^\\d{1,4}$");
            e.Handled = !yearRegex.IsMatch(fullText);
        }
        private void CheckCycle_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Use Regex to allow only numbers and ensure values are 1 to 4 digits.
            Regex yearRegex = new Regex("^\\d{1,2}$");
            e.Handled = !yearRegex.IsMatch(fullText);
        }
        private void DateOfAcquisitionDay_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (int.TryParse(textBox.Text, out int day) && day < 10 && day > 0)
            {
                textBox.Text = $"0{day}";
            }
        }

        // Add leading zero to month field if the number is a single digit
        private void DateOfAcquisitionMonth_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (int.TryParse(textBox.Text, out int month) && month < 10 && month > 0)
            {
                textBox.Text = $"0{month}";
            }
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
