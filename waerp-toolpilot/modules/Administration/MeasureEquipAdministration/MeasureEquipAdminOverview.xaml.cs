using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for MeasureEquipAdminOverview.xaml
    /// </summary>
    public partial class MeasureEquipAdminOverview : UserControl
    {
        DataRow currentSelectedItem;
        DataSet currentItems;
        public DataSet allItems = new DataSet();
        public MeasureEquipAdminOverview()
        {
            InitializeComponent();
            initPage();
        }

        private void initPage()
        {
            DataSet Locations = AdministrationQueries.RunSql(@"
SELECT 
    l.mlocation_id,
    l.mlocation_name,
    l.mlocation_itemID,
    l.mlocation_isRented,
    l.mlocation_rentedUserID,
    u.username AS rented_username,
    o.measuring_equip_name AS object_name,
    o.measuring_equip_image AS object_imagepath
FROM 
    measuring_equip_locations l
LEFT JOIN 
    users u ON l.mlocation_rentedUserID = u.user_id
LEFT JOIN 
    measuring_equip_objects o ON l.mlocation_itemID = o.measuring_equip_id;
");

            loadMeasureEquipItems();
            if (Locations.Tables[0].Rows.Count > 0)
            {
                measureEquipLocations.DataContext = Locations;
                measureEquipLocations.ItemsSource = new DataView(Locations.Tables[0]);

                measureEquipLocations.SelectedIndex = 0;
                EditMeasureLocation.IsEnabled = true;
                DeleteMeasureLocation.IsEnabled = true;
            }
            else
            {
                EditMeasureLocation.IsEnabled = false;
                DeleteMeasureLocation.IsEnabled = false;
            }





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
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddMeasureItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewMeasureEquipWindow openAdd = new AddNewMeasureEquipWindow();
            openAdd.ShowDialog();
            initPage();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = allItems.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in allItems.Tables[0].Rows)
                {
                    if (row["measuring_equip_name"].ToString().Contains(searchBox.Text) | row["measuring_equip_serialnumber"].ToString().Contains(searchBox.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                measureEquipItems.DataContext = output;
                measureEquipItems.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                measureEquipItems.DataContext = allItems;
                measureEquipItems.ItemsSource = new DataView(allItems.Tables[0]);

                measureEquipItems.SelectedIndex = 0;
            }

        }

        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                TempLocationsModel.MeasureEquipID = row_selected["measuring_equip_id"].ToString();
                TempLocationsModel.MeasureEquipName = row_selected["measuring_equip_name"].ToString();
                TempLocationsModel.SerialNumber = row_selected["measuring_equip_serialnumber"].ToString();
                TempLocationsModel.AuditorID = row_selected["measuring_equip_auditor_id"].ToString();
            }
        }

        private void DeleteMeasureItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteMeasureEquipWindow openDelete = new DeleteMeasureEquipWindow();
            openDelete.ShowDialog();
            initPage();
        }



        private void DeleteMeasureLocation_Click(object sender, RoutedEventArgs e)
        {
            DeleteMeasureEquipLocationWindow openDelete = new DeleteMeasureEquipLocationWindow();
            openDelete.ShowDialog();
            initPage();
        }

        private void EditMeasureLocation_Click(object sender, RoutedEventArgs e)
        {
            EditMeasureEquipLocationWindow openEdit = new EditMeasureEquipLocationWindow();
            openEdit.ShowDialog();
            initPage();
        }

        private void AddMeasureLocation_Click(object sender, RoutedEventArgs e)
        {
            AddMeasureEquipLocationWindow openAdd = new AddMeasureEquipLocationWindow();
            openAdd.ShowDialog();
            initPage();
        }

        private void EditMeasureItem_Click(object sender, RoutedEventArgs e)
        {
            EditMeasureEquipWindow openEdit = new EditMeasureEquipWindow();
            openEdit.ShowDialog();
            initPage();
        }

        private void LinkMeasureItem_Click(object sender, RoutedEventArgs e)
        {
            string search = searchBox.Text;
            int selectedIndexObject = measureEquipItems.SelectedIndex;
            int selectedIndexLocation = measureEquipLocations.SelectedIndex;

            DataSet currentLocationRelation = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_locations WHERE mlocation_itemID = {currentSelectedItem["measuring_equip_id"]}");
            string query = "";
            query = $"UPDATE measuring_equip_locations SET mlocation_itemID = NULL, mlocation_isRented = NULL,  mlocation_rentedUserID = NULL WHERE mlocation_itemID = {currentSelectedItem["measuring_equip_id"]}";
            AdministrationQueries.RunSqlExec(query);
            if (currentLocationRelation.Tables[0].Rows.Count > 0)
            {


                query = $"UPDATE measuring_equip_locations SET " +
         $"mlocation_itemID = {currentSelectedItem["measuring_equip_id"]}, " +
         $"mlocation_isRented = {(string.IsNullOrEmpty(currentLocationRelation.Tables[0].Rows[0]["mlocation_isRented"].ToString()) ? "NULL" : currentLocationRelation.Tables[0].Rows[0]["mlocation_isRented"])}, " +
         $"mlocation_rentedUserID = {(string.IsNullOrEmpty(currentLocationRelation.Tables[0].Rows[0]["mlocation_rentedUserID"].ToString()) ? "NULL" : currentLocationRelation.Tables[0].Rows[0]["mlocation_rentedUserID"])} " +
         $"WHERE mlocation_id = {TempLocationsModel.ContainerID}";

                AdministrationQueries.RunSqlExec(query);



            }
            else
            {

                query = $"UPDATE measuring_equip_locations SET mlocation_itemID = {currentSelectedItem["measuring_equip_id"]}, mlocation_isRented = 0 WHERE mlocation_id = {TempLocationsModel.ContainerID}";

                AdministrationQueries.RunSqlExec(query);

            }



            initPage();

            searchBox.Text = search;
            measureEquipItems.SelectedIndex = selectedIndexObject;
            measureEquipLocations.SelectedIndex = selectedIndexLocation;
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


            }

        }
        private void measureEquipLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                if (row_selected["mlocation_ItemID"].ToString().Length == 0)
                {
                    if (currentSelectedItem != null)
                    {
                        LinkMeasureItem.IsEnabled = true;
                    }
                    else
                    {
                        LinkMeasureItem.IsEnabled = false;
                    }
                    UnlinkMeasureItem.IsEnabled = false;
                }
                else
                {
                    LinkMeasureItem.IsEnabled = false;
                    UnlinkMeasureItem.IsEnabled = true;
                }




                TempLocationsModel.ContainerName = row_selected["mlocation_name"].ToString();
                TempLocationsModel.ContainerID = row_selected["mlocation_id"].ToString();
            }

        }

        private void UnlinkMeasureItem_Click(object sender, RoutedEventArgs e)
        {
            string search = searchBox.Text;
            int selectedIndexObject = measureEquipItems.SelectedIndex;
            int selectedIndexLocation = measureEquipLocations.SelectedIndex;

            AdministrationQueries.RunSqlExec($"UPDATE measuring_equip_locations SET mlocation_itemID = NULL WHERE mlocation_id = {TempLocationsModel.ContainerID}");
            initPage();

            searchBox.Text = search;
            measureEquipItems.SelectedIndex = selectedIndexObject;
            measureEquipLocations.SelectedIndex = selectedIndexLocation;
        }
    }
}
