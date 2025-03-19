using System.Data;
using System.Windows.Controls;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.RentItem
{
    /// <summary>
    /// Interaction logic for SelectMachineView.xaml
    /// </summary>
    public partial class SelectMachineView : UserControl
    {
        public SelectMachineView()
        {
            InitializeComponent();
            machineData.DataContext = RentItemQueries.GetAllMachines();
            machineData.ItemsSource = new DataView(RentItemQueries.GetAllMachines().Tables[0]);
        }

        private void machineData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                if (CurrentRentModel.ItemIdent != "")
                {
                    saveSelection.IsEnabled = true;
                }
                else
                {
                    saveSelection.IsEnabled = false;
                }
                SearchParams.machine = row_selected["name"].ToString();
                CurrentRentModel.MachineID = row_selected["machine_id"].ToString();
            }
        }

        private void getLastStage(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void rentItem(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
