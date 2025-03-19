using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for AddNewCompartmentWindow.xaml
    /// </summary>
    public partial class AddNewCompartmentWindow : Window
    {
        public bool onlyNew, onlyUsed, isReserved;
        public AddNewCompartmentWindow()
        {
            InitializeComponent();
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM item_objects");

            string[] itemIdents = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < itemIdents.Length; i++)
            {
                itemIdents[i] = ds.Tables[0].Rows[i]["item_ident"].ToString();
            }
            ItemIdentReserved.ItemsSource = itemIdents;
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void CompartmentIsDynamic_Click(object sender, RoutedEventArgs e)
        {
            if (CompartmentIsDynamic.IsChecked == true)
            {
                isReserved = true;
                CompartmentSettings.IsEnabled = true;
            }
            else
            {
                isReserved = false;
                CompartmentSettings.IsEnabled = false;
            }
        }

        private void OnlyNew_Click(object sender, RoutedEventArgs e)
        {
            if (OnlyNew.IsChecked == true)
            {
                OnlyUsed.IsChecked = false;
                onlyNew = true;
                onlyUsed = false;

            }
            else
            {
                onlyNew = false;
            }
        }

        private void OnlyUsed_Click(object sender, RoutedEventArgs e)
        {
            if (OnlyUsed.IsChecked == true)
            {
                OnlyNew.IsChecked = false;
                onlyNew = false;
                onlyUsed = true;

            }
            else
            {
                onlyUsed = false;
            }
        }

        private void CreateCompartment_Click(object sender, RoutedEventArgs e)
        {
            if (CompartmentName.Text != "")
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE compartment_name = '{CompartmentName.Text}' AND drawer_id = {TempLocationsModel.DrawerID}");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    if (isReserved)
                    {
                        if (ItemIdentReserved.Text != "")
                        {
                            ds = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{ItemIdentReserved.Text}'");

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                AdministrationQueries.RunSqlExec($"INSERT INTO compartment_objects (compartment_name, drawer_id, is_dynamic, reserved_item_id, onlyUsed, onlyNew) VALUES " +
                                    $"('{CompartmentName.Text}', {TempLocationsModel.DrawerID}, 1, {ds.Tables[0].Rows[0]["item_id"]}, {(OnlyUsed.IsChecked.Value ? 1 : 0)}, {(OnlyNew.IsChecked.Value ? 1 : 0)})");

                                ErrorHandlerModel.ErrorText = "Das Lagerfach wurde erfolgreich angelegt!";
                                ErrorHandlerModel.ErrorType = "SUCCESS";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                                DialogResult = false;
                            }
                            else
                            {
                                ErrorHandlerModel.ErrorText = "Die Artikelnummer konnte nicht gefunden werden!";
                                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                            }
                        }
                        else
                        {
                            ErrorHandlerModel.ErrorText = "Bitte wählen Sie eine gültige Artikelnummer aus für die das Fach reseviert sein soll!";
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"INSERT INTO compartment_objects (compartment_name, drawer_id, is_dynamic, onlyUsed, onlyNew) VALUES " +
                                 $"('{CompartmentName.Text}', {TempLocationsModel.DrawerID}, 0, {(OnlyUsed.IsChecked.Value ? 1 : 0)}, {(OnlyNew.IsChecked.Value ? 1 : 0)})");
                        ErrorHandlerModel.ErrorText = "Das Lagerfach wurde erfolgreich angelegt!";
                        ErrorHandlerModel.ErrorType = "SUCCESS";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                        DialogResult = false;
                    }
                }
                else
                {
                    ErrorHandlerModel.ErrorText = "Es existiert bereits ein Lagerfach in dieser Schublade mit dem selben Namen!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen Namen für dieses Lagerfach an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }

        }
    }
}
