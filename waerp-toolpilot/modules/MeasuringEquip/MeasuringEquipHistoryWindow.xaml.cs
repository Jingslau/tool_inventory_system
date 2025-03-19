using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.MeasuringEquip;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasuringEquip
{
    /// <summary>
    /// Interaction logic for MeasuringEquipHistoryWindow.xaml
    /// </summary>
    public partial class MeasuringEquipHistoryWindow : Window
    {
        DataRow currentSelectedItem = null;
        DataSet companySettings = new DataSet();
        public MeasuringEquipHistoryWindow()
        {
            InitializeComponent();

            loadData();

        }

        private void loadData()
        {
            DataSet historyDatesData = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_history WHERE measuring_equip_history_itemID = {MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"]} ORDER BY measuring_equip_history_auditDate DESC");
            if (historyDatesData != null && historyDatesData.Tables[0].Rows.Count > 0)
            {
                string auditor = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_auditor_objects WHERE auditor_id = {historyDatesData.Tables[0].Rows[0]["measuring_equip_history_auditor_id"]}").Tables[0].Rows[0]["auditor_name"].ToString();
                historyDatesData.Tables[0].Columns.Add("auditor_name");
                historyDatesData.Tables[0].Columns.Add("audit_date_str");
                DataSet companySettings = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'");

                for (int i = 0; i < historyDatesData.Tables[0].Rows.Count; i++)
                {
                    historyDatesData.Tables[0].Rows[i]["audit_date_str"] = DateTime.Parse(historyDatesData.Tables[0].Rows[i]["measuring_equip_history_auditDate"].ToString()).ToString("dd.MM.yyyy");
                    historyDatesData.Tables[0].Rows[i]["auditor_name"] = auditor;
                    string protocolPath = Path.Combine(companySettings.Tables[0].Rows[0]["settings_value"].ToString(), "Messmittel Protokolle", historyDatesData.Tables[0].Rows[i]["measuring_equip_history_protocol"].ToString());

                    historyDatesData.Tables[0].Rows[i]["measuring_equip_history_protocol"] = protocolPath;
                }

                historyDates.DataContext = historyDatesData;
                historyDates.ItemsSource = new DataView(historyDatesData.Tables[0]);
                DeleteHistory.IsEnabled = true;
            }
            else
            {
                DeleteHistory.IsEnabled = false;
            }
        }

        private void historyDates_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {

                DeleteHistory.IsEnabled = true;
                MeasureEquipModels.CurrentMeasureEquipHistoryEntry = row_selected.Row;
                currentSelectedItem = row_selected.Row;





            }
            else
            {
                DeleteHistory.IsEnabled = false;
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CreateCheckUp_Click(object sender, RoutedEventArgs e)
        {

        }



        private void EditHistoryEntry_Click(object sender, RoutedEventArgs e)
        {
            MeasuringEquipModel.CurrentSelectedHistory_CheckDate = DateTime.Parse(currentSelectedItem[3].ToString());
            MeasuringEquipModel.CurrentSelectedHistory_DocPath = currentSelectedItem[4].ToString();

            EditCheckUp openEdit = new EditCheckUp();
            openEdit.ShowDialog();

        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OpenProtocolBtn_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string documentationPathGlobal = currentSelectedItem["measuring_equip_history_protocol"].ToString();

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

        private void DeleteHistory_Click(object sender, RoutedEventArgs e)
        {
            DeleteAuditHistoryEntryWindow openDelete = new DeleteAuditHistoryEntryWindow();
            openDelete.ShowDialog();

            loadData();
        }
    }
}
