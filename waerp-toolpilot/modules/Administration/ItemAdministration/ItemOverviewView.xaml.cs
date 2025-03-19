using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.application.Administration.ItemAdministration;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.Administration.ItemAdministration;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.ItemAdministration
{
    /// <summary>
    /// Interaction logic for ItemOverviewView.xaml
    /// </summary>
    public partial class ItemOverviewView : UserControl
    {
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        DataRow CurrentSelectedItem;

        static class LocationParams
        {
            public static DataSet AllItems = new DataSet();
        }

        public ItemOverviewView()
        {

            InitializeComponent();

            try
            {

                conn.Open();



                MySqlCommand cmd2 = new MySqlCommand("Select * from item_objects", conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);
                conn.Close();

                ds2.Tables[0].Columns.Add("vendor");

                if (ds2.Tables[0].Rows.Count > 0)
                {
                    for (var i = 0; i < ds2.Tables[0].Rows.Count; i++)
                    {
                        ds2.Tables[0].Rows[i]["item_image_path"] = Path.Combine(RunningParameters.imagePath, ds2.Tables[0].Rows[i]["item_image_path"].ToString());
                        DataSet vendorId = AdministrationQueries.RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {ds2.Tables[0].Rows[i]["item_id"]}");
                        if (vendorId.Tables[0].Rows.Count > 0)
                        {
                            DataSet vendor = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_id = {vendorId.Tables[0].Rows[0]["vendor_id"]}");
                            ds2.Tables[0].Rows[i]["vendor"] = vendor.Tables[0].Rows[0]["vendor_name"].ToString();
                        }
                    }
                }

                LocationParams.AllItems = ds2;



                dataGridItems.DataContext = ds2;
                dataGridItems.ItemsSource = new DataView(ds2.Tables[0]);




                if (ds2.Tables[0].Rows.Count > 0)
                {
                    EditItem.IsEnabled = true;
                    AdjustQuantity.IsEnabled = true;
                    dataGridItems.SelectedIndex = 0;
                }
                else
                {
                    dataGridItems.SelectedIndex = -1;
                    EditItem.IsEnabled = false;
                    AdjustQuantity.IsEnabled = false;
                }



            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
                conn.Close();
            }
        }

        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                CurrentSelectedItem = row_selected.Row;
                CurrentItemAdministrationModel.SelectedItem = row_selected.Row;

                if (CurrentSelectedItem["item_documentation"].ToString() != "")
                {
                    OpenDocumentation.IsEnabled = true;
                }
                else
                {
                    OpenDocumentation.IsEnabled = false;
                }

                deleteItem.IsEnabled = true;
                EditItem.IsEnabled = true;
                AdjustQuantity.IsEnabled = true;
            }
            else
            {
                deleteItem.IsEnabled = false;
                EditItem.IsEnabled = false;
                AdjustQuantity.IsEnabled = false;
            }
        }
        private void OpenNewItemDialog_Click(object sender, RoutedEventArgs e)
        {
            String searchbox_str = searchBox.Text;
            AddNewItemView test = new AddNewItemView();

            Nullable<bool> DialogResult = test.ShowDialog();
            DialogResult = false;
            LoadItemData();
            searchBox.Text = "";
            searchBox.Text = searchbox_str;
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            String searchbox_str = searchBox.Text;
            CurrentItemAdministrationModel.currentSelectedTableIndex = dataGridItems.SelectedIndex;
            CurrentItemAdministrationModel.SelectedItemSave = CurrentItemAdministrationModel.SelectedItem;
            EditSelectedItemWindow openEdit = new EditSelectedItemWindow();


            openEdit.ShowDialog();
            LoadItemData();
            searchBox.Text = "";
            searchBox.Text = searchbox_str;
            dataGridItems.SelectedIndex = CurrentItemAdministrationModel.currentSelectedTableIndex;

        }

        private void LoadItemData()
        {
            try
            {

                conn.Open();



                MySqlCommand cmd2 = new MySqlCommand("Select * from item_objects", conn);
                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd2);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);

                LocationParams.AllItems = ds2;
                conn.Close();

                ds2.Tables[0].Columns.Add("vendor");

                if (ds2.Tables[0].Rows.Count > 0)
                {
                    for (var i = 0; i < ds2.Tables[0].Rows.Count; i++)
                    {
                        ds2.Tables[0].Rows[i]["item_image_path"] = Path.Combine(RunningParameters.imagePath, ds2.Tables[0].Rows[i]["item_image_path"].ToString());
                        DataSet vendorId = AdministrationQueries.RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {ds2.Tables[0].Rows[i]["item_id"]}");
                        if (vendorId.Tables[0].Rows.Count > 0)
                        {
                            DataSet vendor = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_id = {vendorId.Tables[0].Rows[0]["vendor_id"]}");
                            ds2.Tables[0].Rows[i]["vendor"] = vendor.Tables[0].Rows[0]["vendor_name"].ToString();
                        }
                    }
                }
                dataGridItems.DataContext = ds2;
                dataGridItems.ItemsSource = new DataView(ds2.Tables[0]);

                dataGridItems.SelectedIndex = 0;




            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
                conn.Close();
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = LocationParams.AllItems.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in LocationParams.AllItems.Tables[0].Rows)
                {
                    if (row["item_ident"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description_2"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_diameter"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                dataGridItems.DataContext = output;
                dataGridItems.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                dataGridItems.DataContext = LocationParams.AllItems;
                dataGridItems.ItemsSource = new DataView(LocationParams.AllItems.Tables[0]);

                dataGridItems.SelectedIndex = 0;
            }


        }

        private void AddItemFilter_Click(object sender, RoutedEventArgs e)
        {
            AddNewFilterWindow openAddFilter = new AddNewFilterWindow();
            openAddFilter.ShowDialog();
            LoadItemData();
        }

        private void EditItemFilter_Click(object sender, RoutedEventArgs e)
        {
            EditFilterWindow openFilterEdit = new EditFilterWindow();
            openFilterEdit.ShowDialog();
            LoadItemData();
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteItemWindow openConfirm = new ConfirmDeleteItemWindow();
            openConfirm.ShowDialog();
            LoadItemData();
        }


        private void CopyItemIdent(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentItemAdministrationModel.SelectedItem["item_ident"].ToString());
        }

        private void CopyDescription(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentItemAdministrationModel.SelectedItem["item_description"].ToString());
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentItemAdministrationModel.SelectedItem["item_ident"].ToString() + "; " + CurrentItemAdministrationModel.SelectedItem["item_description"].ToString());
        }

        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteItemWindow confirmOpen = new ConfirmDeleteItemWindow();
            confirmOpen.ShowDialog();
            LoadItemData();
        }

        private void DeleteItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void EditItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Export_FilterCSV(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Filter Stufe");
            dt.Columns.Add("Filter ID");
            dt.Columns.Add("Filter Name");
            dt.Columns.Add("Aktuell zugewiesene Artikel");


            DataSet ds_Filter1 = AdministrationQueries.RunSql($"SELECT * FROM filter1_names");
            DataSet ds_Filter2 = AdministrationQueries.RunSql($"SELECT * FROM filter2_names");
            DataSet ds_Filter3 = AdministrationQueries.RunSql($"SELECT * FROM filter3_names");
            DataSet ds_Filter4 = AdministrationQueries.RunSql($"SELECT * FROM filter4_names");
            DataSet ds_Filter5 = AdministrationQueries.RunSql($"SELECT * FROM filter5_names");

            if (ds_Filter1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_Filter1.Tables[0].Rows.Count; i++)
                {
                    DataSet connectedItems = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter1_id = {ds_Filter1.Tables[0].Rows[i]["filter_id"]}");
                    DataRow newRow = dt.NewRow();

                    newRow["Filter Stufe"] = "1";
                    newRow["Filter ID"] = ds_Filter1.Tables[0].Rows[i]["filter_id"].ToString();
                    newRow["Filter Name"] = ds_Filter1.Tables[0].Rows[i]["name"].ToString();
                    newRow["Aktuell zugewiesene Artikel"] = connectedItems.Tables[0].Rows.Count.ToString();

                    dt.Rows.Add(newRow);

                }
            }

            if (ds_Filter2.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_Filter2.Tables[0].Rows.Count; i++)
                {
                    DataSet connectedItems = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter2_id = {ds_Filter2.Tables[0].Rows[i]["filter_id"]}");
                    DataRow newRow = dt.NewRow();

                    newRow["Filter Stufe"] = "2";
                    newRow["Filter ID"] = ds_Filter2.Tables[0].Rows[i]["filter_id"].ToString();
                    newRow["Filter Name"] = ds_Filter2.Tables[0].Rows[i]["name"].ToString();
                    newRow["Aktuell zugewiesene Artikel"] = connectedItems.Tables[0].Rows.Count.ToString();

                    dt.Rows.Add(newRow);

                }
            }

            if (ds_Filter3.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_Filter3.Tables[0].Rows.Count; i++)
                {
                    DataSet connectedItems = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter1_id = {ds_Filter3.Tables[0].Rows[i]["filter_id"]}");
                    DataRow newRow = dt.NewRow();

                    newRow["Filter Stufe"] = "3";
                    newRow["Filter ID"] = ds_Filter3.Tables[0].Rows[i]["filter_id"].ToString();
                    newRow["Filter Name"] = ds_Filter3.Tables[0].Rows[i]["name"].ToString();
                    newRow["Aktuell zugewiesene Artikel"] = connectedItems.Tables[0].Rows.Count.ToString();

                    dt.Rows.Add(newRow);

                }
            }
            if (ds_Filter4.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_Filter4.Tables[0].Rows.Count; i++)
                {
                    DataSet connectedItems = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter1_id = {ds_Filter4.Tables[0].Rows[i]["filter_id"]}");
                    DataRow newRow = dt.NewRow();

                    newRow["Filter Stufe"] = "4";
                    newRow["Filter ID"] = ds_Filter4.Tables[0].Rows[i]["filter_id"].ToString();
                    newRow["Filter Name"] = ds_Filter4.Tables[0].Rows[i]["name"].ToString();
                    newRow["Aktuell zugewiesene Artikel"] = connectedItems.Tables[0].Rows.Count.ToString();

                    dt.Rows.Add(newRow);
                }
            }
            if (ds_Filter5.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_Filter5.Tables[0].Rows.Count; i++)
                {
                    DataSet connectedItems = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter1_id = {ds_Filter5.Tables[0].Rows[i]["filter_id"]}");
                    DataRow newRow = dt.NewRow();

                    newRow["Filter Stufe"] = "5";
                    newRow["Filter ID"] = ds_Filter5.Tables[0].Rows[i]["filter_id"].ToString();
                    newRow["Filter Name"] = ds_Filter5.Tables[0].Rows[i]["name"].ToString();
                    newRow["Aktuell zugewiesene Artikel"] = connectedItems.Tables[0].Rows.Count.ToString();

                    dt.Rows.Add(newRow);

                }
            }




            string currentTime = DateTime.Now.ToString("d") + "_" + DateTime.Now.ToString("T");
            currentTime = currentTime.Replace(":", ".");
            currentTime = currentTime.Replace("/", "-");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            string path = key.GetValue("HistoryLogsPath").ToString() + $"\\Filter_List_" + currentTime + ".csv";

            ToCSV(dt, path);

            ErrorHandlerModel.ErrorText = $"Die Filterlsite wurde erfolgreich exportiert und unter folgenden Pfad gespeichert: {path}";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();
        }

        private void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(", ");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(", ");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        private void CreateCatalog_Click(object sender, RoutedEventArgs e)
        {
            SelectCatalogDetailsWindow openCatalogExport = new SelectCatalogDetailsWindow();
            openCatalogExport.ShowDialog();
        }

        private void AdjustQuantity_Click(object sender, RoutedEventArgs e)
        {

            String searchbox_str = searchBox.Text;


            AdjustItemQuantityWindow openAdjust = new AdjustItemQuantityWindow();
            openAdjust.ShowDialog();

            LoadItemData();
            searchBox.Text = "";
            searchBox.Text = searchbox_str;
        }

        private void OpenDocumentation_Click(object sender, RoutedEventArgs e)
        {
            string documentationPathGlobal = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'").Tables[0].Rows[0][2].ToString();
            documentationPathGlobal = Path.Combine(documentationPathGlobal, CurrentSelectedItem["item_documentation"].ToString());

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
