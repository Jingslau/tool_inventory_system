using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for DeleteMeasureEquipWindow.xaml
    /// </summary>
    public partial class DeleteMeasureEquipWindow : Window
    {
        public DeleteMeasureEquipWindow()
        {
            InitializeComponent();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.RunSqlExec($"DELETE FROM measuring_equip_objects WHERE measuring_equip_id = {MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM measuring_equip_location_relations WHERE measuring_equip_id = {MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM measuring_equip_history WHERE measuring_equip_history_itemID = {MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"]}");

            ErrorHandlerModel.ErrorText = "Das Messmittel wurde erfolgreich gelöscht!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            DialogResult = false;
        }
    }
}
