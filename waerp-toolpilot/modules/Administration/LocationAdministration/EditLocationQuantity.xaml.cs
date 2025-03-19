using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for EditLocationQuantity.xaml
    /// </summary>
    public partial class EditLocationQuantity : Window
    {
        public EditLocationQuantity()
        {
            InitializeComponent();
        }

        private void CloseCurrentDialog(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void SaveQuant(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) > -1)
            {
                DataSet compartment = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                if (compartment.Tables[0].Rows.Count > 0)
                {
                    if (compartment.Tables[0].Rows[0]["item_used"].ToString() == "0")
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new - {compartment.Tables[0].Rows[0]["item_quantity"]} + {QuantityInput.Text} WHERE item_id = {compartment.Tables[0].Rows[0]["item_id"]}");
                    }

                    AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total - {compartment.Tables[0].Rows[0]["item_quantity"]} + {QuantityInput.Text} WHERE item_id = {compartment.Tables[0].Rows[0]["item_id"]}");

                    AdministrationQueries.RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = {QuantityInput.Text} WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                    ErrorHandlerModel.ErrorText = "Bestand wurde erfolgreich angepasst!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();

                    DialogResult = false;
                }
                else
                {
                    ErrorHandlerModel.ErrorText = "Es ist ein unbekannter Fehler aufgetreten!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();

                    DialogResult = false;
                }



            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen gültigen Bestand ein!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);


        }
    }
}
