using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for DeleteMeasureEquipAuditorWindow.xaml
    /// </summary>
    public partial class DeleteMeasureEquipAuditorWindow : Window
    {
        public DeleteMeasureEquipAuditorWindow()
        {
            InitializeComponent();
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.RunSqlExec($"DELETE FROM measuring_equip_auditor_objects WHERE auditor_id = {MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_id"].ToString()}");

            ErrorHandlerModel.ErrorText = "Messmittelprüfer wurde erfolgreich gelöscht!";
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
