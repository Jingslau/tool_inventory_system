using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.modules.RentItem;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.main
{
    /// <summary>
    /// Interaction logic for DashboardTouchView.xaml
    /// </summary>
    public partial class DashboardTouchView : Page
    {
        //public MainWindow mainWin = new MainWindow();
        public DashboardTouchView()
        {
            InitializeComponent();
            fullname.Text = MainWindowViewModel.Fullname;
            DataSet userPrivilege = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID} AND privilege_id = 4");
            if (userPrivilege.Tables[0].Rows.Count > 0)
            {
                changeView.Visibility = Visibility.Visible;
            }
            else
            {
                changeView.Visibility = Visibility.Hidden;
            }
        }

        private void FloorLocation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/modules/TempLocations/TempLocationsView.xaml", UriKind.Relative));

        }

        private void ScanItem_Click(object sender, RoutedEventArgs e)
        {
            DataSet LastUserRent = AdministrationQueries.RunSql($"SELECT * FROM item_rents t1 WHERE t1.user_id = {MainWindowViewModel.UserID} AND t1.createdAt = (SELECT MAX(createdAt) FROM item_rents t2 WHERE t2.user_id = {MainWindowViewModel.UserID} )");

            if (LastUserRent.Tables[0].Rows.Count > 0)
            {
                if (!CheckIfDateValid(LastUserRent.Tables[0].Rows[0]["createdAt"].ToString()))
                {
                    ErrorHandlerModel.ErrorText = "Die letzte Entnahme liegt länger als eine Stunge zurück! Bitte buchen Sie den Artikel manuell wieder in das Ursprüngliche Lagerfach!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();

                }
                else
                {
                    DataSet FoundCompartment = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {LastUserRent.Tables[0].Rows[0]["location_id"]}");

                    if (FoundCompartment.Tables[0].Rows.Count > 0)
                    {
                        if (FoundCompartment.Tables[0].Rows[0]["item_id"].ToString() != LastUserRent.Tables[0].Rows[0]["item_id"].ToString())
                        {
                            ErrorHandlerModel.ErrorText = "In dem letzten Lagerfach lagert aktuell ein anderes Werkzeug! Bitte buchen Sie den Artikel manuell wieder in ein anderes Lagerfach zurück!";
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                        else
                        {
                            ConfirmRevertLastRentWindow openRevert = new ConfirmRevertLastRentWindow();
                            openRevert.ShowDialog();

                        }
                    }
                    else
                    {
                        ConfirmRevertLastRentWindow openRevert = new ConfirmRevertLastRentWindow();
                        openRevert.ShowDialog();

                    }


                }

            }
            else
            {
                ErrorHandlerModel.ErrorText = "Es konnte keine Entnahme gefunden werden!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();

            }
        }
        private bool CheckIfDateValid(string date)
        {
            // Parse the datetime string
            DateTime dateTime = DateTime.Parse(date);

            // Get the current datetime
            DateTime currentDateTime = DateTime.Now;

            // Calculate the threshold datetime (one hour ago)
            DateTime oneHourAgo = currentDateTime.Subtract(TimeSpan.FromHours(1));

            // Check if the parsed datetime is within the past hour
            return oneHourAgo <= dateTime && dateTime < currentDateTime;
        }


        private void ReturnItem_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/modules/returnItem/ReturnItemSelect.xaml", UriKind.Relative));
        }

        private void RentItem_Click(object sender, RoutedEventArgs e)
        {
            //    mainWin.SidebarMain.SelectedIndex = 1;
            //  mainWin.Breadcrumbs.Text = "Entnahme";
            NavigationService.Navigate(new Uri("/modules/RentItem/RentItemView.xaml", UriKind.Relative));

        }

        private void changeView_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/mainGUI/DashboardView.xaml", UriKind.Relative));
        }
    }
}
