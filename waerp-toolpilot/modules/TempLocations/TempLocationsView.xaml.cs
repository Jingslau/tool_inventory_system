using System;
using System.Data;
using System.Windows.Controls;
using waerp_toolpilot.application.RebookSystem.RebookFloorGroup;
using waerp_toolpilot.application.rentItem;
using waerp_toolpilot.modules.TempLocations;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.TempLocations
{
    /// <summary>
    /// Interaction logic for TempLocationsView.xaml
    /// </summary>
    public partial class TempLocationsView : UserControl
    {
        public TempLocationsView()
        {
            InitializeComponent();

            DataFloorItems.DataContext = TempLocationsQueries.GetFloorZones();
            DataFloorItems.ItemsSource = new DataView(TempLocationsQueries.GetFloorZones().Tables[0]);
            DataFloorItems.SelectedIndex = 0;

            DataGroupItemInFloor.SelectedIndex = -1;
            DataItemsInGroup.SelectedIndex = -1;
            RentItem.IsEnabled = false;
            DeleteItem.IsEnabled = false;
            MoveGroup.IsEnabled = false;

            DeleteGroup.IsEnabled = false;
        }



        private void DataFloorItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                TempLocationsModel.FloorID = row_selected["floor_id"].ToString();
                DataGroupItemInFloor.DataContext = TempLocationsQueries.GetZoneGroups();
                DataGroupItemInFloor.ItemsSource = new DataView(TempLocationsQueries.GetZoneGroups().Tables[0]);
                if (TempLocationsQueries.GetZoneGroups().Tables[0].Rows.Count == 0)
                {

                    DataGroupItemInFloor.SelectedIndex = -1;
                    DataItemsInGroup.SelectedIndex = -1;
                    DataItemsInGroup.DataContext = new DataSet();
                    DataItemsInGroup.ItemsSource = new DataView();

                    RentItem.IsEnabled = false;
                    DeleteItem.IsEnabled = false;
                    MoveGroup.IsEnabled = false;
                    DeleteGroup.IsEnabled = false;

                }
                else
                {
                    DataGroupItemInFloor.SelectedIndex = 0;
                    DataItemsInGroup.SelectedIndex = -1;
                    MoveGroup.IsEnabled = true;
                    DeleteGroup.IsEnabled = true;
                }

            }
        }

        private void DataGroupItemInFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                MoveGroup.IsEnabled = true;
                DeleteGroup.IsEnabled = true;
                RebookGroupModel.CurrentGroupQuantity = int.Parse(row_selected["group_quantity"].ToString());
                TempLocationsModel.GroupID = row_selected["group_id"].ToString();
                DataSet items = TempLocationsQueries.GetItemsInGroup();
                DataItemsInGroup.DataContext = items;
                DataItemsInGroup.ItemsSource = new DataView(items.Tables[0]);
                DataItemsInGroup.SelectedIndex = 0;
                if (items.Tables[0].Rows.Count > 0)
                {
                    RentItem.IsEnabled = true;
                    DeleteItem.IsEnabled = true;
                }
                else
                {
                    RentItem.IsEnabled = false;
                    DeleteItem.IsEnabled = false;
                }
                RebookGroupModel.CurrentGroupName = row_selected["group_name"].ToString();
                RebookGroupModel.CurrentGroupItems = TempLocationsQueries.GetItemsInGroup();
                RebookGroupModel.CurrentGroupId = row_selected["group_id"].ToString();

            }

        }

        private void DataItemsInGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                RentItem.IsEnabled = true;
                DeleteItem.IsEnabled = true;

                CurrentRentModel.ItemIdent = row_selected["item_id"].ToString();
                TempLocationsModel.ItemName = row_selected["item_ident"].ToString();
                TempLocationsModel.ItemQuant = row_selected["GroupQuantity"].ToString();
            }

        }

        private void RentItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RentSelectedItemView openRent = new RentSelectedItemView();
            Nullable<bool> dialogResult = openRent.ShowDialog();
            RefreshGroups();
        }

        private void DeleteItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmDeleteFromGroupWindow openDeleteConfirm = new ConfirmDeleteFromGroupWindow();
            openDeleteConfirm.ShowDialog();
            RefreshGroups();
        }

        private void MoveGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RebookSelectedFloorGroupWindow openMoveGroup = new RebookSelectedFloorGroupWindow();
            openMoveGroup.ShowDialog();
            RefreshGroups();
        }

        public void RefreshGroups()
        {
            DataFloorItems.DataContext = TempLocationsQueries.GetFloorZones();
            DataFloorItems.ItemsSource = new DataView(TempLocationsQueries.GetFloorZones().Tables[0]);
            DataFloorItems.SelectedIndex = 0;
            DataGroupItemInFloor.SelectedIndex = 0;
            DataItemsInGroup.SelectedIndex = 0;

        }

        private void DeleteGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmDeleteGroupWindow openConfirm = new ConfirmDeleteGroupWindow();
            openConfirm.ShowDialog();
            RefreshGroups();
        }
    }
}
