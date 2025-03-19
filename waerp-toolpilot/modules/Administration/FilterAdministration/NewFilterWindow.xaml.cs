using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.FilterAdministration
{
    /// <summary>
    /// Interaction logic for NewFilterWindow.xaml
    /// </summary>
    public partial class NewFilterWindow : Window
    {
        public NewFilterWindow()
        {
            InitializeComponent();
            DataSet allFilters = AdministrationQueries.RunSql($"SELECT * FROM filter{FilterModel.filterNumber}_names ORDER BY name ASC");
            if (allFilters.Tables[0].Rows.Count > 0)
            {
                newFiltername.Items.Clear();
                for (int i = 0; i < allFilters.Tables[0].Rows.Count; i++)
                {
                    newFiltername.Items.Add(allFilters.Tables[0].Rows[i]["name"].ToString());
                }
            }
            newFiltername.IsEnabled = true;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EditFilter_Click(object sender, RoutedEventArgs e)
        {
            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter{FilterModel.filterNumber.ToString()}_names WHERE name = '{newFiltername.Text}'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText8");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else if (newFiltername.Text.Length <= 0)
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText9");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                AdministrationQueries.RunSql($"INSERT INTO filter{FilterModel.filterNumber.ToString()}_names (filter_id, name) VALUES ({AdministrationQueries.GetMaxId(AdministrationQueries.GetAllInfo($"filter{FilterModel.filterNumber.ToString()}_names"), "filter_id")}, '{newFiltername.Text}')");
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText10");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSuccess = new ErrorWindow();
                showSuccess.ShowDialog();
                DialogResult = false;
            }
        }

        private void newFiltername_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void diameterSymbolAddBtn_Click(object sender, RoutedEventArgs e)
        {
            newFiltername.Text += "⌀";
        }
    }
}
