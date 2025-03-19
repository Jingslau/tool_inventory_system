using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.RebookSystem.RebookFloorGroup
{
    /// <summary>
    /// Interaction logic for RebookSelectedFloorGroupWindow.xaml
    /// </summary>
    public partial class RebookSelectedFloorGroupWindow : Window
    {
        public int QuantityNewLocation;
        public bool IsEmpty;
        public RebookSelectedFloorGroupWindow()
        {
            InitializeComponent();
            GroupIdent.Text = RebookGroupModel.CurrentGroupName;
            Barcode.Text = RebookGroupModel.CurrentGroupName;
            CurrentGroupItems.DataContext = RebookGroupModel.CurrentGroupItems;
            CurrentGroupItems.ItemsSource = new DataView(RebookGroupModel.CurrentGroupItems.Tables[0]);

            DataSet tmp = RebookGroupQueries.GetLocationsToRebookGroup();



            AllItemGroupsData.DataContext = tmp;
            AllItemGroupsData.ItemsSource = new DataView(tmp.Tables[0]);
        }

        private void AllItemGroupsData_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                RebookGroupModel.NewLocationName = row_selected["location_name"].ToString();
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

            RebookGroupModel.RebookGroupText1 = "Bitte Lagern Sie die Palette im Lagerort";
            RebookGroupModel.RebookGroupText2 = "in den Lagerort";
            ConfirmRebookFloorGroupWindow openConfirm = new ConfirmRebookFloorGroupWindow();
            openConfirm.ShowDialog();
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
