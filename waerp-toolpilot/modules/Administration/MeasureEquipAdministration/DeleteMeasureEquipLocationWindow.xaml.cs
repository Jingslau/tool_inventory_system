using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for DeleteMeasureEquipLocationWindow.xaml
    /// </summary>
    public partial class DeleteMeasureEquipLocationWindow : Window
    {
        public DeleteMeasureEquipLocationWindow()
        {
            InitializeComponent();
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.RunSqlExec($"DELETE FROM measuring_equip_location_relations WHERE mlocation_id = {TempLocationsModel.ContainerID}");
            AdministrationQueries.RunSqlExec($"DELETE FROM measuring_equip_locations WHERE mlocation_id = {TempLocationsModel.ContainerID}");
            ErrorHandlerModel.ErrorText = "Lagerort wurde erfolgreich gelöscht!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            DialogResult = false;
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
