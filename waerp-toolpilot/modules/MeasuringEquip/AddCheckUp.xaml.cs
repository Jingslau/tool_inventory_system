using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasuringEquip
{
    /// <summary>
    /// Interaction logic for AddCheckUp.xaml
    /// </summary>
    public partial class AddCheckUp : Window
    {
        public AddCheckUp()
        {
            InitializeComponent();
            MeasureEquipName.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_name"].ToString();
            MeasureEquipSerialnumber.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_serialnumber"].ToString();
            if (MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_lastCheckUp"].ToString() != "")
            {
                LastCheckUp.Text = DateTime.Parse(MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_lastCheckUp"].ToString()).ToString("dd.MM.yyyy");
            }
            else
            {
                LastCheckUp.Text = "nicht vorhanden";
            }
            CurrentStatus.Text = MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_status"].ToString();

            CurrentStatusCombobox.Items.Add("Geprüft");
            CurrentStatusCombobox.Items.Add("Nicht geprüft");
            CurrentStatusCombobox.Items.Add("Gesperrt");
            CurrentStatusCombobox.Items.Add("Reperatur");
            CurrentStatusCombobox.Items.Add("Ausmusterung");
            CurrentStatusCombobox.Items.Add("Neukalibrierung");

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
        private void CreateCheckUp_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            if (AuditorPersonName.Text != "" && AuditorCombobox.SelectedIndex > -1 && CurrentStatusCombobox.SelectedIndex > -1 && selectedDate.Text.Length > 0)
            {
                string filename = "";
                if (PDFFilePath.Text.Length > 0)
                {
                    filename = SaveProtocol(PDFFilePath.Text);
                    if (filename == "")
                    {
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorHandlerModel.ErrorText = "Es ist ein unbekannter Fehler beim abspeichern des Prüfprotokolls aufgetreten!";
                        ErrorWindow errorWindow = new ErrorWindow();
                        errorWindow.ShowDialog();
                        flag = true;
                    }
                }

                if (!flag)
                {
                    string query = $"INSERT INTO measuring_equip_history (" +
                        $"measuring_equip_history_auditDate," +
                        $"measuring_equip_history_auditor_id, " +
                        $"measuring_equip_history_auditorPersonName, " +
                        $"measuring_equip_history_status, " +
                        $"measuring_equip_history_protocol," +
                        $"measuring_equip_history_itemID" +
                        $")  VALUES (" +
                        $"'{DateTime.ParseExact(selectedDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss")}'," +
                        $"'{AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_auditor_objects WHERE auditor_name = '{AuditorCombobox.SelectedItem}'").Tables[0].Rows[0]["auditor_id"]}'," +
                        $"'{AuditorPersonName.Text}'," +
                        $"'{CurrentStatusCombobox.SelectedItem}'," +
                        $"'{filename}'," +
                        $"'{MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"].ToString()}')";


                    AdministrationQueries.RunSqlExec(query);

                    query = $"UPDATE measuring_equip_objects SET measuring_equip_status = '{CurrentStatusCombobox.SelectedItem}', " +
                        $"measuring_equip_lastCheckUp = '{DateTime.ParseExact(selectedDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss")}' " +
                        $"WHERE measuring_equip_id = {MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"].ToString()}";

                    AdministrationQueries.RunSqlExec(query);

                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorHandlerModel.ErrorText = "Die Messmittelprüfung wurde erfolgreich abgespeichert!";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();

                    DialogResult = false;
                }
            }
            else
            {
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorHandlerModel.ErrorText = "Bitte füllen Sie alle Pflichtfelder aus!";
                ErrorWindow errorWindow = new ErrorWindow();
                errorWindow.ShowDialog();
            }
        }


        private string SaveProtocol(string filepath)
        {
            string documentationPath = $@"{filepath}";
            DataSet globalDocumentationPath = AdministrationQueries.RunSql($"SELECT * FROM company_settings WHERE settings_name = 'global_history_path'");
            string destinationFolderPathDocumentation = $@"{globalDocumentationPath.Tables[0].Rows[0][2]}";
            string fileNameDocumentation = Path.GetFileNameWithoutExtension(documentationPath); // Get filename without extension
            string fileExtension = Path.GetExtension(documentationPath); // Get the file extension (.pdf)

            // Create timestamp for the filename
            string dateTimeStr = "_" + DateTime.Now.Day.ToString("D2") + "_" + DateTime.Now.Month.ToString("D2") + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString("D2") + "_" + DateTime.Now.Minute.ToString("D2");

            try
            {
                // Ensure the source file exists
                if (!File.Exists(documentationPath))
                {
                    Console.WriteLine($"Source file does not exist: {documentationPath}");
                    return "";
                }
                else
                {
                    // Build the new filename with the timestamp
                    string newFileName = fileNameDocumentation + dateTimeStr + fileExtension;
                    string destinationDocumentationPath = Path.Combine(destinationFolderPathDocumentation, "Messmittel Protokolle", newFileName);

                    // Ensure the destination directory exists
                    if (!Directory.Exists(Path.Combine(destinationFolderPathDocumentation, "Messmittel Protokolle")))
                    {
                        Directory.CreateDirectory(Path.Combine(destinationFolderPathDocumentation, "Messmittel Protokolle"));
                        Console.WriteLine($"Destination directory created: {Path.Combine(destinationFolderPathDocumentation, "Messmittel Protokolle")}");
                    }

                    // Copy the file with the new filename
                    File.Copy(documentationPath, destinationDocumentationPath);
                    Console.WriteLine($"File copied to: {destinationDocumentationPath}");
                    return newFileName; // Return the new filename with timestamp
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "";
            }
        }



        private void selectPdfDocument(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "PDF Dokument|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                PDFFilePath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
