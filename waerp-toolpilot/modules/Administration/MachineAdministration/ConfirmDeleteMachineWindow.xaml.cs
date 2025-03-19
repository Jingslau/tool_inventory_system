using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.modules.Administration.MachineAdministration
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteMachineWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteMachineWindow : Window
    {
        public ConfirmDeleteMachineWindow()
        {
            InitializeComponent();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.RunSql($"DELETE FROM machines WHERE machine_id = {CurrentItemAdministrationModel.SelectedItem["machine_id"]}");
            ErrorHandlerModel.ErrorText = $"Die Maschine {CurrentItemAdministrationModel.SelectedItem["name"]} wurde erfolgreich gelöscht!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSucces = new ErrorWindow();
            showSucces.ShowDialog();
            DialogResult = false;
        }
    }
}
