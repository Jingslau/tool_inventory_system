using System.Data;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for AddMeasureEquipLocationWindow.xaml
    /// </summary>
    public partial class AddMeasureEquipLocationWindow : Window
    {
        public AddMeasureEquipLocationWindow()
        {
            InitializeComponent();
        }

        private void CreateLocation_Click(object sender, RoutedEventArgs e)
        {
            if (LocationName.Text.Length > 0)
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
                    AdministrationQueries.RunSqlExec($"INSERT INTO measuring_equip_locations (mlocation_name) VALUES ('{LocationName.Text}')");
                    ErrorHandlerModel.ErrorText = "Der Lagerort wurde erfolgreich angelegt!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    DialogResult = false;
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen Lagerortnamen an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
