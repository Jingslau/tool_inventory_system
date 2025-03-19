using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for EditDrawerWindow.xaml
    /// </summary>
    public partial class EditDrawerWindow : Window
    {
        DataSet drawerInfo = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE drawer_id = {TempLocationsModel.DrawerID}");
        public EditDrawerWindow()
        {
            InitializeComponent();
            DrawerName.Text = drawerInfo.Tables[0].Rows[0]["drawer_name"].ToString();

        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void EditDrawer_Click(object sender, RoutedEventArgs e)
        {
            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE container_id = '{TempLocationsModel.ContainerID}' AND drawer_name = '{DrawerName.Text}'");
            if (DrawerName.Text == drawerInfo.Tables[0].Rows[0]["drawer_name"].ToString())
            {
                DialogResult = false;
            }
            else
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("editLocationText2");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
                else
                {
                    AdministrationQueries.RunSqlExec($"UPDATE drawer_objects SET drawer_name = '{DrawerName.Text}' WHERE drawer_id = {TempLocationsModel.DrawerID}");
                    ErrorHandlerModel.ErrorText = (string)FindResource("editLocationText2a");
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    DialogResult = false;
                }
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
