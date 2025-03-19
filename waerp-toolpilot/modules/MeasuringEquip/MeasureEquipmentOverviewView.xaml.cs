using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.main;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.Administration.MeasureEquipAdministration;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasuringEquip
{
    /// <summary>
    /// Interaction logic for MeasureEquipmentOverviewView.xaml
    /// </summary>



    public partial class MeasureEquipmentOverviewView : UserControl
    {
        DataRow currentSelectedItem;
        DataSet currentItems;
        DataSet filteredItems;
        public MeasureEquipmentOverviewView()
        {
            InitializeComponent();

            loadMeasureEquipItems();

        }

        public void loadMeasureEquipItems()
        {
            string query = @"
SELECT
    obj.measuring_equip_id,
    obj.measuring_equip_name,
    obj.measuring_equip_serialnumber,
    obj.measuring_equip_cycle,
    obj.measuring_equip_remark,
    obj.measuring_equip_status,
    obj.measuring_equip_dateOfAcquisition,
    lastCheckUpData.lastCheckUp AS measuring_equip_lastCheckUp,
    obj.measuring_equip_image,
    obj.measuring_equip_documentation,
    auditor.auditor_name AS auditor_name,
    loc.mlocation_isRented AS isRented,
    loc.mlocation_id as location_id,
    loc.mlocation_name as location_name,
    usr.username AS rentedUsername
FROM
    measuring_equip_objects obj
LEFT JOIN
    measuring_equip_auditor_objects auditor
    ON obj.measuring_equip_auditor_id = auditor.auditor_id
LEFT JOIN
    measuring_equip_locations loc
    ON obj.measuring_equip_id = loc.mlocation_itemID
LEFT JOIN
    users usr
    ON loc.mlocation_rentedUserID = usr.user_id
LEFT JOIN
    (
        SELECT 
            measuring_equip_history_itemID, 
            MAX(measuring_equip_history_auditDate) AS lastCheckUp
        FROM 
            measuring_equip_history
        GROUP BY 
            measuring_equip_history_itemID
    ) lastCheckUpData
    ON obj.measuring_equip_id = lastCheckUpData.measuring_equip_history_itemID;
";
            DataSet itemData = AdministrationQueries.RunSql(query);

            DataSet userPrivilege = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID} AND privilege_id = 5");
            if (userPrivilege.Tables[0].Rows.Count > 0)
            {
                AuditionHistory.Visibility = Visibility.Visible;
                CreateAudition.Visibility = Visibility.Visible;
            }
            else
            {
                AuditionHistory.Visibility = Visibility.Collapsed;
                CreateAudition.Visibility = Visibility.Collapsed;
            }

            if (itemData.Tables[0].Rows.Count > 0)
            {
                itemData.Tables[0].Columns.Add("dateOfAcquisitionStr");
                itemData.Tables[0].Columns.Add("lastCheckUpStr");
                itemData.Tables[0].Columns.Add("itemImage");
                itemData.Tables[0].Columns.Add("nextCheckUp");



                for (int i = 0; i < itemData.Tables[0].Rows.Count; i++)
                {
                    itemData.Tables[0].Rows[i]["itemImage"] = Path.Combine(RunningParameters.imagePath, itemData.Tables[0].Rows[i]["measuring_equip_image"].ToString());
                    itemData.Tables[0].Rows[i]["dateOfAcquisitionStr"] = DateTime.Parse(itemData.Tables[0].Rows[i]["measuring_equip_dateOfAcquisition"].ToString()).ToString("dd.MM.yyyy");

                    if (itemData.Tables[0].Rows[i]["measuring_equip_lastCheckUp"].ToString().Length > 0)
                    {
                        itemData.Tables[0].Rows[i]["lastCheckUpStr"] = DateTime.Parse(itemData.Tables[0].Rows[i]["measuring_equip_lastCheckUp"].ToString()).ToString("dd.MM.yyyy");
                        itemData.Tables[0].Rows[i]["nextCheckUp"] = DateTime.Parse(itemData.Tables[0].Rows[i]["measuring_equip_lastCheckUp"].ToString()).AddMonths(int.Parse(itemData.Tables[0].Rows[i]["measuring_equip_cycle"].ToString())).ToString("dd.MM.yyyy");

                    }

                }

                measureEquipItems.DataContext = itemData;
                measureEquipItems.ItemsSource = new DataView(itemData.Tables[0]);

                measureEquipItems.SelectedIndex = 0;
            }


        }

        private void measureEquipItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            int currentSelectedIndex = measureEquipItems.SelectedIndex;

            EditMeasureEquipWindow openEdit = new EditMeasureEquipWindow();
            openEdit.ShowDialog();

            loadMeasureEquipItems();

            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = currentSelectedIndex;
        }

        private void measureEquipItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.DataGrid gd = (System.Windows.Controls.DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                currentSelectedItem = row_selected.Row;
                MeasureEquipModels.CurrentMeasureEquipItem = row_selected.Row;

                if (row_selected.Row["measuring_equip_documentation"].ToString().Length != 0)
                {
                    OpenDocumentation.IsEnabled = true;
                }
                else
                {
                    OpenDocumentation.IsEnabled = false;
                }

                if (row_selected.Row["location_name"].ToString().Length == 0)
                {
                    TakeoutItem.IsEnabled = false;
                    PutBackItem.IsEnabled = false;
                }
                else
                {
                    if (row_selected.Row["isRented"].ToString() == "0")
                    {
                        TakeoutItem.IsEnabled = true;
                        PutBackItem.IsEnabled = false;
                    }
                    else
                    {
                        PutBackItem.IsEnabled = true;
                        TakeoutItem.IsEnabled = false;
                    }
                }



            }
        }

        private void AuditionHistory_Click(object sender, RoutedEventArgs e)
        {
            MeasuringEquipHistoryWindow openHistory = new MeasuringEquipHistoryWindow();
            openHistory.ShowDialog();
        }

        private void CreateAudition_Click(object sender, RoutedEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            int currentSelectedIndex = measureEquipItems.SelectedIndex;

            AddCheckUp addCheckUp = new AddCheckUp();
            addCheckUp.ShowDialog();

            loadMeasureEquipItems();

            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = currentSelectedIndex;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = currentItems.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in currentItems.Tables[0].Rows)
                {
                    if (row["measuring_equip_name"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["measuring_equip_serialnumber"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["measuring_equip_remark"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                measureEquipItems.DataContext = output;
                measureEquipItems.ItemsSource = new DataView(output.Tables[0]);
                measureEquipItems.SelectedIndex = -1;
            }
            else
            {
                measureEquipItems.DataContext = currentItems;
                measureEquipItems.ItemsSource = new DataView(currentItems.Tables[0]);

                measureEquipItems.SelectedIndex = 0;
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            int currentSelectedIndex = measureEquipItems.SelectedIndex;

            EditMeasureEquipWindow openEdit = new EditMeasureEquipWindow();
            openEdit.ShowDialog();

            loadMeasureEquipItems();

            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = currentSelectedIndex;
        }

        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            DeleteMeasureEquipWindow openDelete = new DeleteMeasureEquipWindow();
            openDelete.ShowDialog();
            loadMeasureEquipItems();

            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = -1;
        }

        private void OpenNewItemDialog_Click(object sender, RoutedEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            int currentSelectedIndex = measureEquipItems.SelectedIndex;
            AddNewMeasureEquipWindow openAdd = new AddNewMeasureEquipWindow();
            openAdd.ShowDialog();
            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = currentSelectedIndex;

        }

        private void OpenDocumentation_Click(object sender, RoutedEventArgs e)
        {
            string documentationPathGlobal = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'").Tables[0].Rows[0][2].ToString();
            documentationPathGlobal = Path.Combine(documentationPathGlobal, currentSelectedItem["measuring_equip_documentation"].ToString());

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


        private void CopyItemIdent(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(currentSelectedItem["measuring_equip_name"].ToString());
        }

        private void CopySerialnumber(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(currentSelectedItem["measuring_equip_serialnumber"].ToString());
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(currentSelectedItem["measuring_equip_name"].ToString()
                + "; " + currentSelectedItem["measuring_equip_serialnumber"].ToString()
                  + "; " + currentSelectedItem["measuring_equip_remark"].ToString());
        }

        private void TakeoutItem_Click(object sender, RoutedEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            int currentSelectedIndex = measureEquipItems.SelectedIndex;


            string sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string username = AdministrationQueries.RunSql($"SELECT * FROM users WHERE user_id = {MainWindowViewModel.UserID}").Tables[0].Rows[0]["username"].ToString();
            string query = $"UPDATE measuring_equip_locations SET mlocation_isRented = 1, mlocation_rentedUserID = {MainWindowViewModel.UserID} WHERE mlocation_id = {MeasureEquipModels.CurrentMeasureEquipItem["location_id"]}";
            AdministrationQueries.RunSql(query);
            query = $"INSERT INTO history_log(history_id, item_id, item_quantity, item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES(" +
                $"{HistoryLogger.GetMaxId(AdministrationQueries.RunSql($"SELECT * FROM history_log"), "history_id")}, " +
                $"'{MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_name"]}', " +
                $"1, " +
                $"'{MeasureEquipModels.CurrentMeasureEquipItem["location_name"]}', " +
                $"'', " +
                $"0, " +
                $"0, " +
                $"1, " +
                $"'{username}', " +
                $"'{sqlFormattedDate}', " +
                $"'{sqlFormattedDate}', " +
                $"{1})";
            AdministrationQueries.RunSql(query);

            loadMeasureEquipItems();

            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = currentSelectedIndex;
        }

        private void PutBackItem_Click(object sender, RoutedEventArgs e)
        {
            string currentSearchboxText = searchBox.Text;
            int currentSelectedIndex = measureEquipItems.SelectedIndex;


            string sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string username = AdministrationQueries.RunSql($"SELECT * FROM users WHERE user_id = {MainWindowViewModel.UserID}").Tables[0].Rows[0]["username"].ToString();
            string query = $"UPDATE measuring_equip_locations SET mlocation_isRented = 0, mlocation_rentedUserID = NULL WHERE mlocation_id = {MeasureEquipModels.CurrentMeasureEquipItem["location_id"]}";
            AdministrationQueries.RunSql(query);
            query = $"INSERT INTO history_log(history_id, item_id, item_quantity, item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES(" +
                $"{HistoryLogger.GetMaxId(AdministrationQueries.RunSql($"SELECT * FROM history_log"), "history_id")}, " +
                $"'{MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_name"]}', " +
                $"1, " +
                $"'', " +
                $"'{MeasureEquipModels.CurrentMeasureEquipItem["location_name"]}', " +
                $"0, " +
                $"0, " +
                $"2, " +
                $"'{username}', " +
                $"'{sqlFormattedDate}', " +
                $"'{sqlFormattedDate}', " +
                $"{1})";
            AdministrationQueries.RunSql(query);

            loadMeasureEquipItems();

            searchBox.Text = currentSearchboxText;
            measureEquipItems.SelectedIndex = currentSelectedIndex;
        }


    }
}
