using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for AddNewFilterWindow.xaml
    /// </summary>
    public partial class AddNewFilterWindow : Window
    {
        public string filterNo;
        public AddNewFilterWindow()
        {
            InitializeComponent();
            FilterIDSelector.Items.Add("1");

            FilterIDSelector.Items.Add("2");
            FilterIDSelector.Items.Add("3");
            FilterIDSelector.Items.Add("4");
            FilterIDSelector.Items.Add("5");
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void FilterIDSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterIDSelector.SelectedItem != null)
            {
                filterNo = FilterIDSelector.SelectedItem.ToString();
                DataSet allFilters = AdministrationQueries.RunSql($"SELECT * FROM filter{filterNo}_names ORDER BY name ASC");
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
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EditFilter_Click(object sender, RoutedEventArgs e)
        {
            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter{filterNo}_names WHERE name = '{newFiltername.Text}'");
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
                AdministrationQueries.RunSql($"INSERT INTO filter{filterNo}_names (filter_id, name) VALUES ({AdministrationQueries.GetMaxId(AdministrationQueries.GetAllInfo($"filter{filterNo}_names"), "filter_id")}, '{newFiltername.Text}')");
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
