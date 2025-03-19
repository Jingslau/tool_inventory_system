using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.FilterAdministration
{
    /// <summary>
    /// Interaction logic for ConfirmFilterDeletionWindow.xaml
    /// </summary>
    public partial class ConfirmFilterDeletionWindow : Window
    {
        public ConfirmFilterDeletionWindow()
        {
            InitializeComponent();

            ErrorWindowText.Text = $"Sind Sie sich sicher, dass Sie den Filter in Stufe {FilterModel.filterNumber.ToString()} mit dem Namen {FilterModel.selectedFilterName} (ID {FilterModel.selectedFilterID.ToString()}) endgültig löschen möchten?";
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.RunSqlExec($"DELETE FROM filter{FilterModel.filterNumber}_names WHERE filter_id = {FilterModel.selectedFilterID}");
            FilterModel.selectedFilterID = -1;
            FilterModel.selectedFilterName = "";
            FilterModel.filterNumber = -1;

            ErrorHandlerModel.ErrorText = "Der Filter wurde erfolgreich gelöscht!";
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
