using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.Administration.LocationAdministration;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for LocationOverviewView.xaml
    /// </summary>
    public partial class LocationOverviewView : UserControl
    {
        static class LocationParams
        {
            public static DataSet LocationsDB = new DataSet();
            public static DataSet CurrentDrawers = new DataSet();
            public static DataSet CurrentCompartments = new DataSet();
            public static string CurrentContainerID;
            public static string CurrentDrawerID;
            public static string CurrentCompartmentID;



        }
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public LocationOverviewView()
        {
            InitializeComponent();

            GetLocations();
            //     try
            //    {

            //conn.Open();



            //MySqlCommand cmd = new MySqlCommand("Select * from location_objects", conn);
            //MySqlDataAdapter adp = new MySqlDataAdapter(cmd);

            //DataSet ds = new DataSet();
            //DataSet ds2 = new DataSet();

            //adp.Fill(LocationParams.LocationsDB, "itemData");

            //LocationParams.LocationsDB.Tables[0].Columns.Add("item_ident", typeof(string));





            //foreach (DataRow row in LocationParams.LocationsDB.Tables[0].Rows)
            //{
            //    if (row["location_quantity"].ToString() != "0")
            //    {
            //        cmd = new MySqlCommand("Select * FROM item_location_relations WHERE location_id = " + row["location_id"].ToString(), conn);
            //        adp = new MySqlDataAdapter(cmd);
            //        adp.Fill(ds);
            //        foreach (DataRow row2 in ds.Tables[0].Rows)
            //        {
            //            cmd = new MySqlCommand("SELECT * FROM item_objects WHERE item_id =" + row2["item_id"].ToString(), conn);
            //            adp = new MySqlDataAdapter(cmd);
            //            adp.Fill(ds2);
            //            foreach (DataRow row3 in ds2.Tables[0].Rows)
            //            {
            //                row["item_ident"] = row3["item_ident"];
            //            }
            //            ds2 = new DataSet();

            //        }
            //        ds = new DataSet();
            //    }

            //}



            //DataLocationItems.DataContext = LocationParams.LocationsDB;
            //conn.Close();




            // }
            //catch (MySqlException ex)
            //{
            //    MessageBox.Show(ex.ToString());

            //}
            //finally
            //{
            //    conn.Close();
            //}
        }

        public void GetLocations()
        {
            DataSet containerItems = AdministrationQueries.RunSql("SELECT * FROM container_objects");
            if (containerItems.Tables.Count > 0)
            {
                ExportContainerBtn.IsEnabled = true;
                ContainerItems.DataContext = containerItems;
                ContainerItems.ItemsSource = new DataView(containerItems.Tables[0]);
                if (containerItems.Tables[0].Rows.Count > 0)
                {
                    ContainerItems.SelectedIndex = 0;

                }
                else
                {
                    ItemIdent.Text = "";
                    ItemDescription.Text = "";
                    ItemDescription2.Text = "";
                    ItemTotalQuantity.Text = "";
                    ItemImage.Source = null;
                    CompartmentItems.ItemsSource = null;
                    CompartmentItems.DataContext = null;
                    EditDrawer.IsEnabled = false;
                    DeleteDrawer.IsEnabled = false;
                    AddDrawer.IsEnabled = false;
                    EditCompartment.IsEnabled = false;
                    DeleteCompartment.IsEnabled = false;
                    AddCompartment.IsEnabled = false;
                    EditConatiner.IsEnabled = false;
                    ExportContainerBtn.IsEnabled = false;
                    DeleteContainer.IsEnabled = false;
                }
            }



        }


        private void OpenNewItemDialog_Click(object sender, RoutedEventArgs e)
        {
            AddNewLocationView test = new AddNewLocationView();
            Nullable<bool> DialogResult = test.ShowDialog(); conn.Close();
            GetLocations();
            //DataLocationItems.SelectedIndex = 0;

        }

        private void DeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteLocationWindow openConfirm = new ConfirmDeleteLocationWindow();
            openConfirm.ShowDialog();
            GetLocations();
            //  DataLocationItems.SelectedIndex = 0;
        }

        private void DataLocationItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                if (int.Parse(row_selected["location_quantity"].ToString()) == 0)
                {

                    ItemIdent.Text = "";
                    ItemDescription.Text = "";
                    ItemDescription2.Text = "";
                    ItemTotalQuantity.Text = "";
                    ItemQuality.Text = "";
                    ItemDiameter.Text = "";
                    ItemImage.Source = null;
                }
                else
                {

                    CurrentLocationAdministrationModel.SelectedLocationName = row_selected["location_name"].ToString();
                    CurrentLocationAdministrationModel.SelectedLocationId = row_selected["location_id"].ToString();

                    conn.Close();
                    conn.Open();


                    MySqlCommand cmd = new MySqlCommand($"Select * from item_location_relations WHERE location_id = {row_selected["location_id"]}", conn);
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string[] tmpArr = new string[ds.Tables[0].Rows.Count];
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            tmpArr[i] = ds.Tables[0].Rows[i]["item_id"].ToString();
                        }

                        cmd = new MySqlCommand(string.Format("SELECT * FROM item_objects WHERE item_id IN ({0})", string.Join(", ", tmpArr)), conn);
                        adp = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adp.Fill(ds);

                        ItemIdent.Text = ds.Tables[0].Rows[0]["item_ident"].ToString();
                        ItemDescription.Text = ds.Tables[0].Rows[0]["item_description"].ToString();
                        ItemDescription2.Text = ds.Tables[0].Rows[0]["item_description_2"].ToString();
                        ItemTotalQuantity.Text = ds.Tables[0].Rows[0]["item_quantity_total"].ToString();
                        ItemDiameter.Text = ds.Tables[0].Rows[0]["item_diameter"].ToString();

                        if (row_selected["item_used"].ToString() == "1")
                        {
                            ItemQuality.Text = "Neu";
                        }
                        else
                        {
                            ItemQuality.Text = "Gebraucht";
                        }
                    }



                    if (ds.Tables[0].Rows[0]["item_image_path"].ToString() != "")
                    {
                        try
                        {
                            Uri imageUri = new Uri(ds.Tables[0].Rows[0]["item_image_path"].ToString(), UriKind.RelativeOrAbsolute);

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
            }
            conn.Close();
        }



        private void EditLocation_Click(object sender, RoutedEventArgs e)
        {
            EditLocationWindow openEdit = new EditLocationWindow();
            openEdit.ShowDialog();
            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
            // DataLocationItems.SelectedIndex = 0;
        }

        private void ExportAsCSV_Click(object sender, RoutedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add("locationInfo");
            ds.Tables[0].Columns.Add("location_ident");
            ds.Tables[0].Columns.Add("item_ident");
            ds.Tables[0].Columns.Add("item_description1");
            ds.Tables[0].Columns.Add("item_description2");
            ds.Tables[0].Columns.Add("item_quantity");

            //for (int i = 0; i < LocationParams.CurrentLocations.Tables[0].Rows.Count; i++)
            //{
            //    DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_location_relations WHERE location_id = {LocationParams.CurrentLocations.Tables[0].Rows[i]["location_id"]}");
            //    for (int j = 0; j < tmp.Tables[0].Rows.Count; j++)
            //    {
            //        int currentIndex = ds.Tables[0].Rows.Count;
            //        ds.Tables[0].Rows.Add();
            //        ds.Tables[0].Rows[currentIndex]["location_ident"] = LocationParams.CurrentLocations.Tables[0].Rows[i]["location_name"];
            //        ds.Tables[0].Rows[currentIndex]["item_quantity"] = tmp.Tables[0].Rows[j]["location_item_quantity"];

            //        DataSet tmpItem = AdministrationQueries.RunSql($"SELECT * FROM item_objects where item_id = {tmp.Tables[0].Rows[j]["item_id"]}");
            //        ds.Tables[0].Rows[currentIndex]["item_ident"] = tmpItem.Tables[0].Rows[0]["item_ident"];
            //        ds.Tables[0].Rows[currentIndex]["item_description1"] = tmpItem.Tables[0].Rows[0]["item_description"];
            //        ds.Tables[0].Rows[currentIndex]["item_description2"] = tmpItem.Tables[0].Rows[0]["item_description_2"];
            //    }
            //}
            string currentTime = DateTime.Now.ToString("d") + "_" + DateTime.Now.ToString("T");
            currentTime = currentTime.Replace(":", ".");
            currentTime = currentTime.Replace("/", "-");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            string path = key.GetValue("HistoryLogsPath").ToString() + "\\location_overview_" + currentTime + ".csv";

            DataTable dt = ds.Tables[0].Copy();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() == "")
                    {
                        dt.Rows[i][j] = dt.Rows[i][j] = "0";
                    }


                    if (dt.Rows[i][j].ToString().Contains(";") | dt.Rows[i][j].ToString().Contains(",") | dt.Rows[i][j].ToString().Contains(":"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace(";", " ");
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace(",", " ");
                    }


                    if (dt.Rows[i][j].ToString().Contains("Ö"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("Ö", "OE");
                    }
                    if (dt.Rows[i][j].ToString().Contains("Ü"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("Ü", "UE");
                    }
                    if (dt.Rows[i][j].ToString().Contains("Ö"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("Ä", "AE");
                    }
                    if (dt.Rows[i][j].ToString().Contains("ß"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("ß", "ss");
                    }


                }

            }

            ToCSV(dt, path);
            ErrorHandlerModel.ErrorText = (string)FindResource("errorText22");
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
                    sw.Write("; ");
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
                        if (value.Contains(';'))
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
                        sw.Write("; ");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        private void DrawerItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                TempLocationsModel.DrawerID = row_selected["drawer_id"].ToString();
                TempLocationsModel.DrawerName = row_selected["drawer_name"].ToString();
                LocationParams.CurrentDrawerID = row_selected["drawer_id"].ToString();
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE drawer_id = {row_selected["drawer_id"]}");
                if (ds.Tables.Count > 0)
                {
                    CompartmentItems.DataContext = ds;
                    CompartmentItems.ItemsSource = new DataView(ds.Tables[0]);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        CompartmentItems.SelectedIndex = 0;
                        EditCompartment.IsEnabled = true;
                        AddCompartment.IsEnabled = true;
                        DeleteCompartment.IsEnabled = true;
                    }
                    else
                    {
                        ItemIdent.Text = "";
                        ItemDescription.Text = "";
                        ItemDescription2.Text = "";
                        ItemTotalQuantity.Text = "";
                        ItemQuality.Text = "";
                        ItemImage.Source = null;
                        CompartmentItems.DataContext = null;
                        CompartmentItems.ItemsSource = null;
                        EditCompartment.IsEnabled = false;
                        AddCompartment.IsEnabled = true;
                        DeleteCompartment.IsEnabled = false;
                    }
                }

            }
        }

        private void CompartmentItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                LocationParams.CurrentCompartmentID = row_selected["compartment_id"].ToString();
                TempLocationsModel.CompartmentID = row_selected["compartment_id"].ToString();
                TempLocationsModel.CompartmentName = row_selected["compartment_name"].ToString();
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {row_selected["compartment_id"]}");
                if (ds.Tables.Count > 0)
                {


                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (row_selected["is_dynamic"].ToString() == "1")
                        {
                            if (ds.Tables[0].Rows[0]["item_quantity"].ToString() == "0")
                            {
                                assignItemToCompartment.IsEnabled = true;

                            }
                            else
                            {
                                assignItemToCompartment.IsEnabled = false;

                            }
                        }
                        else
                        {
                            if (ds.Tables[0].Rows[0]["item_quantity"].ToString() == "0")
                            {
                                assignItemToCompartment.IsEnabled = true;
                            }
                            else
                            {
                                assignItemToCompartment.IsEnabled = false;
                            }
                        }


                        if (ds.Tables[0].Rows[0]["item_id"].ToString().Length > 0)
                        {


                            CurrentReturnModel.ItemIdent = ds.Tables[0].Rows[0]["item_id"].ToString();


                            DataSet item = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {ds.Tables[0].Rows[0]["item_id"]}");
                            if (item.Tables.Count > 0)
                            {
                                if (item.Tables[0].Rows.Count > 0)
                                {
                                    correctLocationQuantity.IsEnabled = true;
                                    ItemIdent.Text = item.Tables[0].Rows[0]["item_ident"].ToString();
                                    ItemDescription.Text = item.Tables[0].Rows[0]["item_description"].ToString();
                                    ItemDescription2.Text = item.Tables[0].Rows[0]["item_description_2"].ToString();
                                    ItemTotalQuantity.Text = ds.Tables[0].Rows[0]["item_quantity"].ToString();
                                    ItemDiameter.Text = item.Tables[0].Rows[0]["item_diameter"].ToString();

                                    Uri imageUri = new Uri(Path.Combine(RunningParameters.imagePath, item.Tables[0].Rows[0]["item_image_path"].ToString()), UriKind.RelativeOrAbsolute);
                                    try
                                    {
                                        BitmapImage bitmapImage = new BitmapImage();
                                        bitmapImage.BeginInit();
                                        bitmapImage.UriSource = imageUri;
                                        bitmapImage.EndInit();

                                        ItemImage.Source = bitmapImage;
                                    }
                                    catch (Exception exp)
                                    {
                                        ErrorLogger.LogSysError(exp);
                                    }

                                    if (ds.Tables[0].Rows[0]["item_used"].ToString() == "1")
                                    {
                                        ItemQuality.Text = "Gebraucht";
                                    }
                                    else
                                    {
                                        ItemQuality.Text = "Neu";
                                    }
                                    EditCompartment.IsEnabled = true;
                                    AddCompartment.IsEnabled = true;
                                    DeleteCompartment.IsEnabled = true;
                                }
                                else
                                {
                                    correctLocationQuantity.IsEnabled = false;

                                }
                            }
                        }
                        else
                        {
                            ItemIdent.Text = "";
                            ItemDescription.Text = "";
                            ItemDescription2.Text = "";
                            ItemTotalQuantity.Text = "";
                            ItemQuality.Text = "";
                            ItemDiameter.Text = "";
                            ItemImage.Source = null;
                            correctLocationQuantity.IsEnabled = false;
                        }
                    }
                    else if (row_selected["is_dynamic"].ToString() == "1")
                    {
                        DataSet dstmp = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = '{row_selected["reserved_item_id"]}'");
                        ItemIdent.Text = dstmp.Tables[0].Rows[0]["item_ident"].ToString();
                        ItemDescription.Text = dstmp.Tables[0].Rows[0]["item_description"].ToString();
                        ItemDescription2.Text = dstmp.Tables[0].Rows[0]["item_description_2"].ToString();
                        ItemTotalQuantity.Text = "0";
                        ItemQuality.Text = "";
                        ItemDiameter.Text = dstmp.Tables[0].Rows[0]["item_diameter"].ToString();
                        Uri imageUri = new Uri(Path.Combine(RunningParameters.imagePath, dstmp.Tables[0].Rows[0]["item_image_path"].ToString()), UriKind.RelativeOrAbsolute);

                        try
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = imageUri;
                            bitmapImage.EndInit();

                            ItemImage.Source = bitmapImage;
                        }
                        catch (Exception exp)
                        {
                            ErrorLogger.LogSysError(exp);
                        }

                    }


                    else
                    {
                        ItemIdent.Text = "";
                        ItemDescription.Text = "";
                        ItemDescription2.Text = "";
                        ItemDiameter.Text = "";
                        ItemTotalQuantity.Text = "";
                        ItemQuality.Text = "";
                        ItemImage.Source = null;
                        assignItemToCompartment.IsEnabled = true;
                        correctLocationQuantity.IsEnabled = false;
                    }
                }
            }
            else
            {
                ItemIdent.Text = "";
                ItemDescription.Text = "";
                ItemDescription2.Text = "";
                ItemTotalQuantity.Text = "";
                ItemQuality.Text = "";
                ItemImage.Source = null;
                correctLocationQuantity.IsEnabled = false;
            }
        }

        private void ContainerItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                AddContainer.IsEnabled = true;
                EditConatiner.IsEnabled = true;
                DeleteContainer.IsEnabled = true;
                ExportContainerBtn.IsEnabled = true;
                LocationParams.CurrentContainerID = row_selected["container_id"].ToString();
                TempLocationsModel.ContainerID = row_selected["container_id"].ToString();
                TempLocationsModel.ContainerName = row_selected["container_name_long"].ToString();
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE container_id = {row_selected["container_id"]}");
                if (ds.Tables.Count > 0)
                {

                    DrawerItems.DataContext = ds;
                    DrawerItems.ItemsSource = new DataView(ds.Tables[0]);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DrawerItems.SelectedIndex = 0;
                        EditDrawer.IsEnabled = true;
                        DeleteDrawer.IsEnabled = true;
                        AddDrawer.IsEnabled = true;

                    }
                    else
                    {
                        ItemIdent.Text = "";
                        ItemDescription.Text = "";
                        ItemDescription2.Text = "";
                        ItemTotalQuantity.Text = "";
                        ItemQuality.Text = "";
                        ItemImage.Source = null;
                        CompartmentItems.ItemsSource = null;
                        CompartmentItems.DataContext = null;
                        DrawerItems.DataContext = null;
                        DrawerItems.ItemsSource = null;
                        EditDrawer.IsEnabled = false;
                        DeleteDrawer.IsEnabled = false;
                        AddDrawer.IsEnabled = true;
                        EditCompartment.IsEnabled = false;
                        DeleteCompartment.IsEnabled = false;
                        AddCompartment.IsEnabled = false;
                        ExportContainerBtn.IsEnabled = false;
                    }
                }

            }
            else
            {
                DrawerItems.DataContext = null;
                DrawerItems.ItemsSource = null;
                ExportContainerBtn.IsEnabled = false;
            }
        }

        private void AddContainer_Click(object sender, RoutedEventArgs e)
        {
            AddNewContainerWindow openAddContainer = new AddNewContainerWindow();
            openAddContainer.ShowDialog();
            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void AddDrawer_Click(object sender, RoutedEventArgs e)
        {
            AddNewDrawerWindow openAddDrawer = new AddNewDrawerWindow();
            openAddDrawer.ShowDialog(); int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void AddCompartment_Click(object sender, RoutedEventArgs e)
        {
            AddNewCompartmentWindow openAddCompartment = new AddNewCompartmentWindow();
            openAddCompartment.ShowDialog(); int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void DeleteContainer_Click(object sender, RoutedEventArgs e)
        {
            DataSet relatedDrawers = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE container_id = {TempLocationsModel.ContainerID}");
            if (relatedDrawers.Tables[0].Rows.Count > 0)
            {

                var drawerIds = relatedDrawers.Tables[0].AsEnumerable()
                    .Select(row => row.Field<int>("drawer_id"))
                    .ToArray();

                DataSet relatedCompartments = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE drawer_id IN ({string.Join(", ", drawerIds)})");

                if (relatedCompartments.Tables[0].Rows.Count > 0)
                {
                    var compartmentIds = relatedCompartments.Tables[0].AsEnumerable().Select(row => row.Field<int>("compartment_id")).ToArray();
                    DataSet itemCompartments = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id IN ({string.Join(", ", compartmentIds)})");
                    int quantity = 0;
                    for (int i = 0; i < itemCompartments.Tables[0].Rows.Count; i++)
                    {
                        quantity += int.Parse(itemCompartments.Tables[0].Rows[i]["item_quantity"].ToString());
                    }
                    if (quantity > 0)
                    {
                        ErrorHandlerModel.ErrorText = "In diesem Schrank lagern noch Artikel! Bitte lagern Sie diese Artikel um, bevor Sie den Schrank löschen!";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();

                    }
                    else
                    {
                        TempLocationsModel.SelectedAction = 1;
                        DeleteSelectedItemWindow openDelet = new DeleteSelectedItemWindow();
                        openDelet.ShowDialog(); int containerIndex = ContainerItems.SelectedIndex;
                        int drawerIndex = DrawerItems.SelectedIndex;
                        int compartmentIndex = CompartmentItems.SelectedIndex;

                        GetLocations();

                        ContainerItems.SelectedIndex = containerIndex;
                        DrawerItems.SelectedIndex = drawerIndex;
                        CompartmentItems.SelectedIndex = compartmentIndex;
                    }
                }
                else
                {
                    TempLocationsModel.SelectedAction = 1;
                    DeleteSelectedItemWindow openDelet = new DeleteSelectedItemWindow();
                    openDelet.ShowDialog(); int containerIndex = ContainerItems.SelectedIndex;
                    int drawerIndex = DrawerItems.SelectedIndex;
                    int compartmentIndex = CompartmentItems.SelectedIndex;

                    GetLocations();

                    ContainerItems.SelectedIndex = containerIndex;
                    DrawerItems.SelectedIndex = drawerIndex;
                    CompartmentItems.SelectedIndex = compartmentIndex;
                }
            }
            else
            {
                TempLocationsModel.SelectedAction = 1;
                DeleteSelectedItemWindow openDelet = new DeleteSelectedItemWindow();
                openDelet.ShowDialog();
                int containerIndex = ContainerItems.SelectedIndex;
                int drawerIndex = DrawerItems.SelectedIndex;
                int compartmentIndex = CompartmentItems.SelectedIndex;

                GetLocations();

                ContainerItems.SelectedIndex = containerIndex;
                DrawerItems.SelectedIndex = drawerIndex;
                CompartmentItems.SelectedIndex = compartmentIndex;
            }
        }

        private void DeleteDrawer_Click(object sender, RoutedEventArgs e)
        {
            TempLocationsModel.SelectedAction = 2;
            DeleteSelectedItemWindow openDelet = new DeleteSelectedItemWindow();
            openDelet.ShowDialog();
            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void DeleteCompartment_Click(object sender, RoutedEventArgs e)
        {
            TempLocationsModel.SelectedAction = 3;
            DeleteSelectedItemWindow openDelet = new DeleteSelectedItemWindow();
            openDelet.ShowDialog();
            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void EditConatiner_Click(object sender, RoutedEventArgs e)
        {
            EditContainerWindow showEdit = new EditContainerWindow();
            showEdit.ShowDialog();
            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void EditDrawer_Click(object sender, RoutedEventArgs e)
        {
            EditDrawerWindow showEdit = new EditDrawerWindow();
            showEdit.ShowDialog();

            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void EditCompartment_Click(object sender, RoutedEventArgs e)
        {
            EditCompartmentWindow openEdit = new EditCompartmentWindow();
            openEdit.ShowDialog();

            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void ExportContainerBtn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Schublade");
            dt.Columns.Add("Fach");
            dt.Columns.Add("Reseviertes Lagerfach");
            dt.Columns.Add("Artikel benutzt");
            dt.Columns.Add("Artikelnummer");
            dt.Columns.Add("Artikelbezeichnung");
            dt.Columns.Add("Menge im System");
            dt.Columns.Add("Menge gezählt");
            dt.Columns.Add("Bemerkung");

            //Get Container Name
            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM container_objects WHERE container_id = {LocationParams.CurrentContainerID}");
            String containerName = ds.Tables[0].Rows[0][1].ToString();

            //Get Drawers
            DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE container_id = {LocationParams.CurrentContainerID}");

            //Fill dt with values
            for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
            {
                DataSet ds3 = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE drawer_id = {ds2.Tables[0].Rows[i][0]}");

                for (int j = 0; j < ds3.Tables[0].Rows.Count; j++)
                {
                    // Get Item information
                    DataSet ds4 = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {ds3.Tables[0].Rows[j]["compartment_id"]}");

                    if (ds4.Tables[0].Rows.Count > 0)
                    {
                        DataSet item = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {ds4.Tables[0].Rows[0]["item_id"]}");
                        DataRow newRow = dt.NewRow();

                        newRow["Schublade"] = ds2.Tables[0].Rows[i]["drawer_name"].ToString().Replace(';', ',');
                        newRow["Fach"] = ds3.Tables[0].Rows[j]["compartment_name"].ToString().Replace(';', ',');
                        newRow["Artikelnummer"] = item.Tables[0].Rows[0]["item_ident"].ToString().Replace(';', ',');
                        newRow["Artikelbezeichnung"] = item.Tables[0].Rows[0]["item_description"].ToString().Replace(';', ',') + " " + item.Tables[0].Rows[0]["item_description_2"].ToString().Replace(';', ',');
                        newRow["Menge im System"] = ds4.Tables[0].Rows[0]["item_quantity"].ToString().Replace(';', ',');

                        if (ds3.Tables[0].Rows[j]["is_dynamic"].ToString() == "1")
                        {
                            newRow["Reseviertes Lagerfach"] = "JA";
                        }
                        else
                        {
                            newRow["Reseviertes Lagerfach"] = "NEIN";
                        }

                        if (ds4.Tables[0].Rows[0]["item_used"].ToString() == "1")
                        {
                            newRow["Artikel benutzt"] = "JA";
                        }
                        else
                        {
                            newRow["Artikel benutzt"] = "NEIN";
                        }


                        dt.Rows.Add(newRow);
                    }
                }
            }

            string currentTime = DateTime.Now.ToString("d") + "_" + DateTime.Now.ToString("T");
            currentTime = currentTime.Replace(":", ".");
            currentTime = currentTime.Replace("/", "-");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            string path = key.GetValue("HistoryLogsPath").ToString() + $"\\OVERVIEW_{containerName}_" + currentTime + ".csv";

            ToCSV(dt, path);

            ErrorHandlerModel.ErrorText = $"Die Inventurtabelle wurde erfolgreich exportiert und unter folgenden Pfad gespeichert: {path}";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();
        }

        private void correctLocationQuantity_Click(object sender, RoutedEventArgs e)
        {
            EditLocationQuantity showEdit = new EditLocationQuantity();
            showEdit.ShowDialog();

            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;


        }

        private void assignItemToCompartment_Click(object sender, RoutedEventArgs e)
        {
            AssignItemToCompartmentWindow openAssign = new AssignItemToCompartmentWindow();
            openAssign.ShowDialog();


            int containerIndex = ContainerItems.SelectedIndex;
            int drawerIndex = DrawerItems.SelectedIndex;
            int compartmentIndex = CompartmentItems.SelectedIndex;

            GetLocations();

            ContainerItems.SelectedIndex = containerIndex;
            DrawerItems.SelectedIndex = drawerIndex;
            CompartmentItems.SelectedIndex = compartmentIndex;
        }

        private void unassignItemFromCompartment_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
