using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for EditCompartmentWindow.xaml
    /// </summary>
    public partial class EditCompartmentWindow : Window
    {
        public bool onlyNew, onlyUsed, isReserved;
        DataSet compartmentInfo = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE compartment_id = {TempLocationsModel.CompartmentID}");
        public EditCompartmentWindow()
        {
            InitializeComponent();
            CompartmentName.Text = compartmentInfo.Tables[0].Rows[0]["compartment_name"].ToString();

            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM item_objects");

            string[] itemIdents = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < itemIdents.Length; i++)
            {
                itemIdents[i] = ds.Tables[0].Rows[i]["item_ident"].ToString();
            }
            ItemIdentReserved.ItemsSource = itemIdents;

            OnlyNew.IsChecked = checkTinyint(compartmentInfo.Tables[0].Rows[0]["onlyNew"].ToString());
            OnlyUsed.IsChecked = checkTinyint(compartmentInfo.Tables[0].Rows[0]["onlyUsed"].ToString());
            CompartmentIsDynamic.IsChecked = checkTinyint(compartmentInfo.Tables[0].Rows[0]["is_dynamic"].ToString());

            if (CompartmentIsDynamic.IsChecked == true)
            {
                CompartmentSettings.IsEnabled = true;

                currentlyReservedItem.Text = (string)FindResource("editCompartmentText1") + AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {compartmentInfo.Tables[0].Rows[0]["reserved_item_id"]}").Tables[0].Rows[0]["item_ident"].ToString();

            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private bool checkTinyint(string val)
        {
            if (val == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void EditCompartment_Click(object sender, RoutedEventArgs e)
        {
            if (CompartmentName.Text != "")
            {

                if (CompartmentName.Text == compartmentInfo.Tables[0].Rows[0]["compartment_name"].ToString())
                {
                    if (CompartmentIsDynamic.IsChecked.Value)
                    {
                        if (ItemIdentReserved.Text.Length == 0)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText3");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                        else
                        {
                            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{ItemIdentReserved.Text}'");

                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                AdministrationQueries.RunSqlExec($"UPDATE compartment_objects SET compartment_name ='{CompartmentName.Text}' , drawer_id = {TempLocationsModel.DrawerID}, is_dynamic = 1, reserved_item_id = {ds.Tables[0].Rows[0]["item_id"]}, onlyUsed = {OnlyNew.IsChecked.Value}, onlyNew = {OnlyUsed.IsChecked.Value} WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                                ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText2");
                                ErrorHandlerModel.ErrorType = "SUCCESS";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                                DialogResult = false;
                            }
                            else
                            {
                                ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText3");
                                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE compartment_objects SET compartment_name ='{CompartmentName.Text}' , drawer_id = {TempLocationsModel.DrawerID}, is_dynamic = 0, reserved_item_id = null, onlyUsed = {OnlyUsed.IsChecked.Value}, onlyNew = {OnlyNew.IsChecked.Value} WHERE compartment_id = {TempLocationsModel.CompartmentID}");


                        ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText2");
                        ErrorHandlerModel.ErrorType = "SUCCESS";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                        DialogResult = false;
                    }
                }
                else
                {
                    DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE compartment_name = '{CompartmentName.Text}'");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        if (CompartmentIsDynamic.IsChecked.Value)
                        {
                            if (ItemIdentReserved.Text.Length == 0)
                            {
                                AdministrationQueries.RunSqlExec($"UPDATE compartment_objects SET compartment_name ='{CompartmentName.Text}' , drawer_id = {TempLocationsModel.DrawerID}, is_dynamic = 0, reserved_item_id = {compartmentInfo.Tables[0].Rows[0]["reserved_item_id"]}, onlyUsed = {OnlyUsed.IsChecked.Value}, onlyNew = {OnlyNew.IsChecked.Value} WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                                ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText2");
                                ErrorHandlerModel.ErrorType = "SUCCESS";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                                DialogResult = false;
                            }
                            else
                            {
                                DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{ItemIdentReserved.Text}'");

                                if (ds2.Tables[0].Rows.Count > 0)
                                {

                                    AdministrationQueries.RunSqlExec($"UPDATE compartment_objects SET compartment_name ='{CompartmentName.Text}' , drawer_id = {TempLocationsModel.DrawerID}, is_dynamic = 0, reserved_item_id = {ds.Tables[0].Rows[0]["item_id"]}, onlyUsed = {OnlyUsed.IsChecked.Value}, onlyNew = {OnlyNew.IsChecked.Value} WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                                    ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText2");
                                    ErrorHandlerModel.ErrorType = "SUCCESS";
                                    ErrorWindow showError = new ErrorWindow();
                                    showError.ShowDialog();
                                    DialogResult = false;
                                }
                                else
                                {
                                    ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText3");
                                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                                    ErrorWindow showError = new ErrorWindow();
                                    showError.ShowDialog();
                                }
                            }
                        }
                        else
                        {
                            AdministrationQueries.RunSqlExec($"UPDATE compartment_objects SET compartment_name ='{CompartmentName.Text}' , drawer_id = {TempLocationsModel.DrawerID}, is_dynamic = 1, reserved_item_id = null, onlyUsed = {OnlyUsed.IsChecked.Value}, onlyNew = {OnlyNew.IsChecked.Value} WHERE compartment_id = {TempLocationsModel.CompartmentID}");


                            ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText2");
                            ErrorHandlerModel.ErrorType = "SUCCESS";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText5");
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }

                }

            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("editCompartmentText6");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }

        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
    }
}
