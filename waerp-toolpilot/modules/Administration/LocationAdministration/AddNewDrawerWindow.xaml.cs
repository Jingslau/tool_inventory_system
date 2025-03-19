using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for AddNewDrawerWindow.xaml
    /// </summary>
    public partial class AddNewDrawerWindow : Window
    {
        public AddNewDrawerWindow()
        {
            InitializeComponent();
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
        private void CreateLocation_Click(object sender, RoutedEventArgs e)
        {
            string newDrawerName = DrawerName.Text;

            DataSet currentDrawers = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE container_id = {TempLocationsModel.ContainerID} AND drawer_name = '{newDrawerName}'");
            if (currentDrawers.Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = "Diese Schublade existiert bereits in diesem Schrank! Bitte wählen Sie einen anderne Namen.";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                AdministrationQueries.RunSqlExec($"INSERT INTO drawer_objects (drawer_name, container_id) VALUES ('{newDrawerName}', {TempLocationsModel.ContainerID})");
                string newDrawerID = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE drawer_name = '{newDrawerName}' AND container_id = {TempLocationsModel.ContainerID}").Tables[0].Rows[0]["drawer_id"].ToString();
                if (AutoCreateCompartments.IsChecked == true)
                {
                    if (int.Parse(DrawerRowCount.Text) > 0 && int.Parse(CompartmentCount.Text) > 0)
                    {
                        int drawerCount = int.Parse(DrawerRowCount.Text);
                        int compartmentEachRow = int.Parse(CompartmentCount.Text);

                        for (int row = 1; row <= drawerCount; row++)
                        {
                            for (int col = 1; col <= compartmentEachRow; col++)
                            {
                                string compartment = GetCompartmentName(row, col);

                                AdministrationQueries.RunSqlExec($"INSERT INTO compartment_objects (compartment_name, drawer_id) VALUES ('{compartment}', {newDrawerID})");
                            }
                        }
                        ErrorHandlerModel.ErrorText = "Die Schublade wurde erfolgreich angelegt!";
                        ErrorHandlerModel.ErrorType = "SUCCESS";
                        ErrorWindow showSuccess = new ErrorWindow();
                        showSuccess.ShowDialog();
                        DialogResult = false;
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = "Bitte geben Sie Werte für die Anzahl der Reihen und Fächer!";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }
                }
                else if (AutoCreateCompartments.IsChecked == false)
                {
                    ErrorHandlerModel.ErrorText = "Die Schublade wurde erfolgreich angelegt!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    DialogResult = false;
                }

            }
        }

        static string GetCompartmentName(int row, int col)
        {
            string rowName = GetRowName(row);
            string columnName = col.ToString();
            return $"{rowName}{columnName}";
        }

        static string GetRowName(int row)
        {
            string rowName = "";

            while (row > 0)
            {
                int modulo = (row - 1) % 26;
                rowName = (char)('A' + modulo) + rowName;
                row = (row - 1) / 26;
            }

            return rowName;
        }
        private void AutoCreateCompartments_Click(object sender, RoutedEventArgs e)
        {
            if (AutoCreateCompartments.IsChecked == true)
            {
                CompartmentSettings.IsEnabled = true;
            }
            else
            {
                CompartmentSettings.IsEnabled = false;
            }
        }
    }
}
