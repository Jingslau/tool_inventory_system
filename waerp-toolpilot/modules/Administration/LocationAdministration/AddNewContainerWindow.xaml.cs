using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for AddNewContainerWindow.xaml
    /// </summary>
    public partial class AddNewContainerWindow : Window
    {
        public AddNewContainerWindow()
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
            if (ContainerNameLong.Text.Length == 0 || ContainerNameShort.Text.Length == 0)
            {
                ErrorHandlerModel.ErrorText = "Es muss mindestens ein langer und ein kurzer Name für den neuen Schrank eingetragen werden.";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();

            }
            else
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM container_objects where container_name_long = '{ContainerNameLong.Text}'");
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        ds = AdministrationQueries.RunSql($"SELECT * FROM container_objects where container_name_short = '{ContainerNameShort.Text}'");
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                AdministrationQueries.RunSqlExec($"INSERT INTO container_objects (container_name_long, container_name_short) VALUES" +
                                               $" " +
                                               $"(" +
                                               $"'{ContainerNameLong.Text}'," +
                                               $"'{ContainerNameShort.Text}'" +
                                               $")");
                                ErrorHandlerModel.ErrorText = $"Schrank mit dem Namen " + ContainerNameLong.Text + " wurde erfolgreich angelegt!";
                                ErrorHandlerModel.ErrorType = "SUCCESS";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                                DialogResult = false;
                            }
                            else
                            {
                                ErrorHandlerModel.ErrorText = "Schrank existiert bereits mit diesem Namen. Bitte wählen Sie einen anderen Namen.";
                                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                                ErrorWindow showError = new ErrorWindow();
                                showError.ShowDialog();
                            }
                        }

                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = "Schrank existiert bereits mit diesem Namen. Bitte wählen Sie einen anderen Namen.";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }

                }

            }
        }
    }
}
