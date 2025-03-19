using System;
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
    /// Interaction logic for AssignItemToCompartmentWindow.xaml
    /// </summary>
    /// 

    public partial class AssignItemToCompartmentWindow : Window
    {
        String itemUsed = "0";
        String itemReserved = "0";
        public AssignItemToCompartmentWindow()
        {
            InitializeComponent();
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM item_objects ORDER BY item_ident ASC");

            string[] itemIdents = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < itemIdents.Length; i++)
            {
                itemIdents[i] = ds.Tables[0].Rows[i]["item_ident"].ToString();
            }
            ItemIdentReserved.ItemsSource = itemIdents;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void assignItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemIdentReserved.Text != "" && QuantityInput.Text != "")
            {
                DataSet item_info = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{ItemIdentReserved.Text}'");

                if (item_info.Tables[0].Rows.Count > 0)
                {
                    DataSet checkCompartment = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                    if (checkCompartment.Tables[0].Rows.Count > 0)
                    {
                        AdministrationQueries.RunSqlExec($"DELETE FROM compartment_item_relations WHERE compartment_id = {TempLocationsModel.CompartmentID}");
                    }

                    AdministrationQueries.RunSqlExec($"INSERT INTO compartment_item_relations (compartment_id, item_id, item_quantity, item_constructed, item_used) VALUES ({TempLocationsModel.CompartmentID}, {item_info.Tables[0].Rows[0]["item_id"]}, {QuantityInput.Text},0 , {itemUsed})");

                    if (itemUsed == "0")
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new + {QuantityInput.Text} WHERE item_id = {item_info.Tables[0].Rows[0]["item_id"]}");
                    }

                    if (itemReserved == "1")
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE compartment_objects SET is_dynamic = 1, reserved_item_id = {item_info.Tables[0].Rows[0]["item_id"]} WHERE compartment_id = {TempLocationsModel.CompartmentID}");
                    }

                    AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {QuantityInput.Text} WHERE item_id = {item_info.Tables[0].Rows[0]["item_id"]}");


                    ErrorHandlerModel.ErrorText = "Der Artikel wurde erfolgreich dem Lagerfach zugeordnet!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();

                    DialogResult = false;
                }
                else
                {
                    ErrorHandlerModel.ErrorText = "Die angegebene Artikelnummer ist unbekannt!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie eine Artikelnummer und einen gültigen Bestand ein!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }

        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ItemIdentReserved_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            String item_ident = ItemIdentReserved.Text;

            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{item_ident}'");

            if (ds.Tables[0].Rows.Count > 0)
            {
                ItemInformation.Text = ds.Tables[0].Rows[0]["item_description"].ToString() + " " + ds.Tables[0].Rows[0]["item_description_2"].ToString();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);


        }

        private void isUsed_Click(object sender, RoutedEventArgs e)
        {
            if (isUsed.IsChecked == true)
            {
                itemUsed = "1";
            }
            else
            {
                itemUsed = "0";
            }
        }

        private void ItemIdentReserved_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            String item_ident = ItemIdentReserved.Text;

            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{item_ident}'");

            if (ds.Tables[0].Rows.Count > 0)
            {
                ItemInformation.Text = ds.Tables[0].Rows[0]["item_description"].ToString() + " " + ds.Tables[0].Rows[0]["item_description_2"].ToString();
            }
        }

        private void isReserved_Click(object sender, RoutedEventArgs e)
        {
            if (isReserved.IsChecked == true)
            {
                itemReserved = "1";
            }
            else
            {
                itemReserved = "0";
            }
        }
    }
}
