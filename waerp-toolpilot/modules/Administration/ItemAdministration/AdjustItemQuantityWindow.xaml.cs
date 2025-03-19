using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.modules.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for AdjustItemQuantityWindow.xaml
    /// </summary>
    public partial class AdjustItemQuantityWindow : Window
    {
        public AdjustItemQuantityWindow()
        {
            InitializeComponent();
            QuantityTotal.Text = CurrentItemAdministrationModel.SelectedItem["item_quantity_total"].ToString();
            QuantityTotalNew.Text = CurrentItemAdministrationModel.SelectedItem["item_quantity_total_new"].ToString();
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
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);


        }

        private void SaveQuant(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityTotal.Text) >= 0 && int.Parse(QuantityTotalNew.Text) >= 0)
            {
                AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total = {QuantityTotal.Text}, item_quantity_total_new = {QuantityTotalNew.Text} WHERE item_id = '{CurrentItemAdministrationModel.SelectedItem["item_id"]}'");
                ErrorHandlerModel.ErrorText = $"Der Bestand für die Artikelnummer {CurrentItemAdministrationModel.SelectedItem["item_ident"]} wurde erfolgreich aktualisiert!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSucces = new ErrorWindow();
                showSucces.ShowDialog();
                DialogResult = false;
            }
            else
            {
                ErrorHandlerModel.ErrorText = $"Bitte gebe einen gültigen Bestand ein. Der Wert muss größer oder gleich 0 sein!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSucces = new ErrorWindow();
                showSucces.ShowDialog();
            }
        }
    }
}
