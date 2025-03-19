using System;
using System.Data;
using System.Windows.Controls;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.RebookSystem.RebookGroup
{
    /// <summary>
    /// Interaction logic for RebookGroupView.xaml
    /// </summary>
    public partial class RebookGroupView : UserControl
    {
        public DataSet allGroups = new DataSet();
        public RebookGroupView()
        {
            InitializeComponent();
            GetAllGroups();
        }

        private void DataGridGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                MoveGroupToFloor.IsEnabled = true;
                RebookBtn.IsEnabled = true;
                RebookGroupModel.CurrentGroupName = row_selected["location_name"].ToString();
                RebookGroupModel.CurrentGroupId = row_selected["location_id"].ToString();
                RebookGroupModel.CurrentGroupQuantity = int.Parse(row_selected["location_quantity"].ToString());
                GroupItemData.DataContext = RebookGroupQueries.GetSelectedGroup();
                GroupItemData.ItemsSource = new DataView(RebookGroupQueries.GetSelectedGroup().Tables[0]);
            }
            else
            {
                MoveGroupToFloor.IsEnabled = false;
                RebookBtn.IsEnabled = false;
            }
        }

        private void GroupItemData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GetAllGroups()
        {
            allGroups = RebookGroupQueries.GetAllUsedGroups();
            RebookBtn.IsEnabled = true;
            DataGridGroups.DataContext = RebookGroupQueries.GetAllUsedGroups();
            DataGridGroups.ItemsSource = new DataView(RebookGroupQueries.GetAllUsedGroups().Tables[0]);
            DataGridGroups.SelectedIndex = 0;
            DataSet ds = RebookGroupQueries.GetAllUsedGroups();
            if (ds.Tables[0].Rows.Count > 0)
            {
                RebookGroupModel.CurrentGroupId = ds.Tables[0].Rows[0]["location_id"].ToString();
            }

        }

        private void RebookBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RebookGroupModel.CurrentGroupItems = RebookGroupQueries.GetSelectedGroup();
            RebookSelectedGroupWindow openRebook = new RebookSelectedGroupWindow();
            Nullable<bool> dialogResult = openRebook.ShowDialog();
            GetAllGroups();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = allGroups.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in allGroups.Tables[0].Rows)
                {
                    if (row["location_name"].ToString().Contains(searchBox.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                DataGridGroups.DataContext = output;
                DataGridGroups.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                DataGridGroups.DataContext = allGroups;
                DataGridGroups.ItemsSource = new DataView(allGroups.Tables[0]);
            }

        }

        private void MoveGroupToFloor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RebookGroupModel.CurrentGroupItems = RebookGroupQueries.GetSelectedGroup();
            RebookGroupToZoneWindow openRebook = new RebookGroupToZoneWindow();
            openRebook.ShowDialog();
        }
    }
}
