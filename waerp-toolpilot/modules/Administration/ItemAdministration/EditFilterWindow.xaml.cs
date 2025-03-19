using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for EditFIlterWindow.xaml
    /// </summary>
    public partial class EditFilterWindow : Window
    {
        public string filterNo;
        public string filterID;
        public string newFilterName;
        public EditFilterWindow()
        {
            InitializeComponent();
            FilterIDSelector.Items.Add("1");

            FilterIDSelector.Items.Add("2");
            FilterIDSelector.Items.Add("3");
            FilterIDSelector.Items.Add("4");
            FilterIDSelector.Items.Add("5");

            DeleteFilter.IsEnabled = false;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void FilterIDSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FilterIDSelector.SelectedItem != null)
            {
                filterNo = FilterIDSelector.SelectedItem.ToString();
                oldFiltername.Items.Clear();
                DataSet allFilters = AdministrationQueries.RunSql($"SELECT * FROM filter{filterNo}_names ORDER BY name ASC");
                if (allFilters.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < allFilters.Tables[0].Rows.Count; i++)
                    {
                        oldFiltername.Items.Add(allFilters.Tables[0].Rows[i]["name"]);
                    }
                    oldFiltername.IsEnabled = true;
                    oldFiltername.SelectedItem = -1;
                    newFiltername.IsEnabled = false;
                    EditFilter.IsEnabled = false;
                    newFiltername.Text = "";
                    SelectedItemsNo.Text = "0";
                }
                else
                {
                    EditFilter.IsEnabled = false;

                    newFiltername.Text = "";
                    SelectedItemsNo.Text = "0";
                    oldFiltername.SelectedItem = -1;
                    oldFiltername.IsEnabled = false;
                    newFiltername.IsEnabled = false;
                    DeleteFilter.IsEnabled = false;
                }
            }
            else
            {
                newFiltername.Text = "";
                SelectedItemsNo.Text = "0";
                EditFilter.IsEnabled = false;

                oldFiltername.SelectedItem = -1;
                oldFiltername.IsEnabled = false;
                newFiltername.IsEnabled = false;
                DeleteFilter.IsEnabled = false;
            }
        }

        private void oldFiltername_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (oldFiltername.SelectedItem != null)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter{filterNo}_names WHERE name = '{oldFiltername.SelectedItem}'");
                filterID = ds.Tables[0].Rows[0]["filter_id"].ToString();
                int SelectedItems = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter{filterNo}_id = {filterID}").Tables[0].Rows.Count;
                SelectedItemsNo.Text = SelectedItems.ToString();
                if (SelectedItems == 0)
                {
                    DeleteFilter.IsEnabled = true;
                    EditFilter.IsEnabled = true;

                }
                else
                {
                    EditFilter.IsEnabled = true;

                    DeleteFilter.IsEnabled = false;
                }
                newFiltername.IsEnabled = true;
                newFiltername.Text = "";
            }
            else
            {
                newFiltername.IsEnabled = false;
                newFiltername.Text = "";
                EditFilter.IsEnabled = false;
                DeleteFilter.IsEnabled = false;
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EditFilter_Click(object sender, RoutedEventArgs e)
        {
            if (newFiltername.Text.Length <= 0)
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText17");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter{filterNo}_names WHERE name = '{newFiltername.Text}'");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    TempLocationsModel.filterNameOld = oldFiltername.Text;
                    TempLocationsModel.filterNameNew = newFiltername.Text;
                    TempLocationsModel.filterStage = filterNo;

                    FilterAlreadyExistsWindow openMerge = new FilterAlreadyExistsWindow();
                    openMerge.ShowDialog();
                    if (TempLocationsModel.itemsMerged)
                    {
                        TempLocationsModel.itemsMerged = false;
                        FilterIDSelector.SelectedIndex = -1;
                        oldFiltername.SelectedIndex = -1;
                    }
                }
                else
                {
                    AdministrationQueries.RunSql($"UPDATE filter{filterNo}_names SET name = '{newFiltername.Text}' WHERE filter_id = {filterID}");
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText18");
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    FilterIDSelector.SelectedIndex = -1;
                    oldFiltername.SelectedIndex = -1;

                }
            }
        }


        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.RunSql($"DELETE FROM filter{filterNo}_names WHERE filter_id = {filterID}");
            ErrorHandlerModel.ErrorText = $"Der Filter mit dem Namen {oldFiltername.SelectedItem} wurde erfolgreich gelöscht!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            DialogResult = false;
        }

        private void diameterSymbolAddBtn_Click(object sender, RoutedEventArgs e)
        {
            newFiltername.Text += "⌀";
        }
    }
}
