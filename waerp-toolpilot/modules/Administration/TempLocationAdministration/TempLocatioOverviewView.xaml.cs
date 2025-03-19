using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.TempLocationAdministration
{
    /// <summary>
    /// Interaction logic for TempLocatioOverviewView.xaml
    /// </summary>
    public partial class TempLocatioOverviewView : UserControl
    {
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public TempLocatioOverviewView()
        {
            InitializeComponent();

            GetLocations();
            DataLocationItems.SelectedIndex = 0;

        }
        static class TempLocationParams
        {
            public static DataSet TempLocationsDB = new DataSet();
        }
        private void GetLocations()
        {
            conn.Open();

            TempLocationParams.TempLocationsDB = new DataSet();
            MySqlCommand cmd = new MySqlCommand("Select * from floor_objects", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            adp.Fill(ds, "itemData");


            TempLocationParams.TempLocationsDB = ds;



            DataLocationItems.DataContext = ds;
            conn.Close();
            DataLocationItems.SelectedIndex = 0;
        }
        private void CreateTempLocation_Click(object sender, RoutedEventArgs e)
        {
            AddTempLocationWindow openCreate = new AddTempLocationWindow();
            openCreate.ShowDialog();
            GetLocations();
            DataLocationItems.SelectedIndex = 0;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = TempLocationParams.TempLocationsDB.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in TempLocationParams.TempLocationsDB.Tables[0].Rows)
                {
                    if (row["floor_name"].ToString().Contains(searchBox.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                DataLocationItems.DataContext = output;
            }
            else
            {
                DataLocationItems.DataContext = TempLocationParams.TempLocationsDB;
                DataLocationItems.SelectedIndex = 0;
            }
        }

        private void DataLocationItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                CurrentLocationAdministrationModel.SelectedLocationName = row_selected["floor_name"].ToString();
                CurrentLocationAdministrationModel.SelectedLocationId = row_selected["floor_id"].ToString();
                conn.Open();

                TempLocationParams.TempLocationsDB = new DataSet();
                MySqlCommand cmd = new MySqlCommand($"Select * from floor_group_objects WHERE floor_id = {row_selected["floor_id"]}", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string[] tmpArr = new string[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tmpArr[i] = ds.Tables[0].Rows[i]["group_id"].ToString();
                    }

                    cmd = new MySqlCommand(string.Format("SELECT * FROM floor_group_item_relations WHERE group_id IN ({0})", string.Join(", ", tmpArr)), conn);
                    adp = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adp.Fill(ds);
                    string[] tmpArr2 = new string[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tmpArr2[i] = ds.Tables[0].Rows[i]["item_id"].ToString();
                    }

                    cmd = new MySqlCommand(string.Format("SELECT * FROM item_objects WHERE item_id IN ({0})", string.Join(", ", tmpArr2)), conn);
                    adp = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adp.Fill(ds);
                    GroupItemData.DataContext = ds;
                    GroupItemData.ItemsSource = new DataView(ds.Tables[0]);

                }
                conn.Close();
            }
        }

        private void EditTempLocation_Click(object sender, RoutedEventArgs e)
        {
            EditTempLocationWindow openEdit = new EditTempLocationWindow();
            openEdit.ShowDialog();
            GetLocations();
            DataLocationItems.SelectedIndex = 0;
        }

        private void DeleteTempLocation_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteTempLocationWindow openConfirm = new ConfirmDeleteTempLocationWindow();
            openConfirm.ShowDialog();
            GetLocations();
            DataLocationItems.SelectedIndex = 0;

        }


    }
}