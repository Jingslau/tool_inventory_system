using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for EditMeasureEquipLocationWindow.xaml
    /// </summary>
    public partial class EditMeasureEquipLocationWindow : Window
    {
        public EditMeasureEquipLocationWindow()
        {
            InitializeComponent();
            oldLocationName.Text = "Alter Lagerort: " + TempLocationsModel.ContainerName;
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EditLocation_Click(object sender, RoutedEventArgs e)
        {
            if (LocationName.Text.Length == 0)
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen neuen Namen für den Lagerort an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_locations WHERE mlocation_name = '{LocationName.Text}'");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ErrorHandlerModel.ErrorText = "Es besteht bereits ein Lagerort mit diesem Namen!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
                else
                {
                    AdministrationQueries.RunSqlExec($"UPDATE measuring_equip_locations SET mlocation_name = '{LocationName.Text}' WHERE mlocation_id = {TempLocationsModel.ContainerID}");
                    ErrorHandlerModel.ErrorText = "Der Lagerort wurde erfolgreich umbenannt!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    DialogResult = false;
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
