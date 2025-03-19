using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.RebookSystem.RebookGroup
{
    /// <summary>
    /// Interaction logic for RebookGroupToZoneWindow.xaml
    /// </summary>
    public partial class RebookGroupToZoneWindow : Window
    {
        public RebookGroupToZoneWindow()
        {
            InitializeComponent();
            GroupIdent.Text = RebookGroupModel.CurrentGroupName;
            CurrentGroupItems.DataContext = RebookGroupModel.CurrentGroupItems;
            CurrentGroupItems.ItemsSource = new DataView(RebookGroupModel.CurrentGroupItems.Tables[0]);
            AllItemGroupsData.DataContext = RebookGroupQueries.GetAllFloors();
            AllItemGroupsData.ItemsSource = new DataView(RebookGroupQueries.GetAllFloors().Tables[0]);
        }

        private void AllItemGroupsData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                RebookGroupModel.SelectedFloorId = row_selected["floor_id"].ToString();
            }
        }


        private void RebookBtn_Click(object sender, RoutedEventArgs e)
        {
            RebookGroupQueries.RebookGroupToZone();
            DialogResult = false;
        }

        private void CloseCurrentDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
