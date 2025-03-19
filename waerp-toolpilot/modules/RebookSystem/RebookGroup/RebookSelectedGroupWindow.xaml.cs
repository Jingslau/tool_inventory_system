using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.RebookSystem.RebookGroup
{
    /// <summary>
    /// Interaction logic for RebookSelectedGroupWindow.xaml
    /// </summary>
    public partial class RebookSelectedGroupWindow : Window
    {
        public bool IsEmpty;
        public int QuantityNewLocation;
        public DataSet allGroups = new DataSet();
        public bool LastCheckState = true;
        public RebookSelectedGroupWindow()
        {
            InitializeComponent();
            GetDataContext();

        }

        private void CloseCurrentDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        public void GetDataContext()
        {
            allGroups = RebookGroupQueries.GetLocationsToRebook();


            SetEmptyLocations();

            emptyLocation.IsChecked = true;
            usedLocation.IsChecked = false;

            CurrentGroupItems.DataContext = RebookGroupModel.CurrentGroupItems;
            CurrentGroupItems.ItemsSource = new DataView(RebookGroupModel.CurrentGroupItems.Tables[0]);
            GroupIdent.Text = RebookGroupModel.CurrentGroupName;
        }


        private void AllItemGroupsData_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                RebookGroupModel.NewGroupId = row_selected["location_id"].ToString();
                RebookBtn.IsEnabled = true;
                QuantityNewLocation = int.Parse(row_selected["location_quantity"].ToString());
                if (QuantityNewLocation == 0)
                {
                    IsEmpty = true;
                }
                else
                {
                    IsEmpty = false;
                }
            }
        }

        private void RebookBtn_Click(object sender, RoutedEventArgs e)
        {
            RebookGroupModel.IsEmpty = IsEmpty;
            RebookGroupModel.QuantityNewLocation = QuantityNewLocation;
            if (IsEmpty)
            {
                RebookGroupModel.RebookGroupText1 = "Bitte Lagern Sie die Palette im Lagerort";
                RebookGroupModel.RebookGroupText2 = "in den Lagerort";
                RebookGroupConfirmWindow openConfirm = new RebookGroupConfirmWindow();
                Nullable<bool> dialogResult = openConfirm.ShowDialog();
                DialogResult = false;
            }
            else
            {
                RebookGroupModel.RebookGroupText1 = "Bitte tauschen Sie die Palette im Lagerort";
                RebookGroupModel.RebookGroupText2 = "mit der Palette im Lagerort";
                RebookGroupConfirmWindow openConfirm = new RebookGroupConfirmWindow();
                Nullable<bool> dialogResult = openConfirm.ShowDialog();
                DialogResult = false;
            }


        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (searchBox.Text != "")
            {
                emptyLocation.IsChecked = false;
                usedLocation.IsChecked = false;
                DataSet output = allGroups.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in allGroups.Tables[0].Rows)
                {
                    if (row["location_name"].ToString().Contains(searchBox.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                AllItemGroupsData.DataContext = output;
                AllItemGroupsData.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                if (LastCheckState)
                {
                    SetEmptyLocations();
                    emptyLocation.IsChecked = true;
                    usedLocation.IsChecked = false;
                }
                else
                {
                    SetUsedLocations();
                    emptyLocation.IsChecked = false;
                    usedLocation.IsChecked = true;
                }

            }

        }

        private void emptyLocation_Click(object sender, RoutedEventArgs e)
        {
            RebookBtn.IsEnabled = false;
            SetEmptyLocations();
        }

        private void usedLocation_Click(object sender, RoutedEventArgs e)
        {
            RebookBtn.IsEnabled = false;
            SetUsedLocations();
        }

        private void SetUsedLocations()
        {
            DataSet tmp = RebookGroupQueries.GetLocationsToRebook();
            DataSet output = RebookGroupQueries.GetLocationsToRebook();
            output.Tables[0].Rows.Clear();

            for (int i = 0; i < tmp.Tables[0].Rows.Count; i++)
            {

                if (int.Parse(tmp.Tables[0].Rows[i]["location_quantity"].ToString()) > 0)
                {
                    output.Tables[0].ImportRow(tmp.Tables[0].Rows[i]);
                }
            }

            AllItemGroupsData.DataContext = output;
            AllItemGroupsData.ItemsSource = new DataView(output.Tables[0]);
            emptyLocation.IsChecked = false;
            usedLocation.IsChecked = true;

        }
        private void SetEmptyLocations()
        {
            DataSet tmp = RebookGroupQueries.GetLocationsToRebook();
            DataSet output = RebookGroupQueries.GetLocationsToRebook();
            output.Tables[0].Rows.Clear();

            for (int i = 0; i < tmp.Tables[0].Rows.Count; i++)
            {

                if (int.Parse(tmp.Tables[0].Rows[i]["location_quantity"].ToString()) == 0)
                {
                    output.Tables[0].ImportRow(tmp.Tables[0].Rows[i]);
                }
            }

            AllItemGroupsData.DataContext = output;
            AllItemGroupsData.ItemsSource = new DataView(output.Tables[0]);
            emptyLocation.IsChecked = true;
            usedLocation.IsChecked = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
