using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using waerp_management.config;
using waerp_management.config.SettingsStore;
using waerp_management.errorHandling;
using waerp_management.loginHandling;
using waerp_management.LoginTouch;
using waerp_management.main;
using waerp_management.sql;

namespace waerp_management
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataSet settings = new DataSet();
            settings = AdministrationQueries.RunSql("SELECT * FROM company_settings");

            if (LocalDevice.Default.isTouch)
            {
                this.WindowState = WindowState.Maximized;
                navframe.Navigate(new System.Uri("/main/DashboardTouchView.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                navframe.Navigate(new System.Uri("/main/mainDashboard.xaml", UriKind.RelativeOrAbsolute));
            }






            LicenseText.Text = "Licensed for " + settings.Tables[0].Rows[0]["settings_value"];

            Breadcrumbs.Text = "Dashboard";
            if (MainWindowViewModel.RoleID != 1)
            {
                RoleNameUser.Text = "Mitarbeiter";
                ManagementExpander.Visibility = Visibility.Hidden;
                //       OrdersystemExpander.Visibility = Visibility.Hidden;
            }
            else
            {
                RoleNameUser.Text = "Administrator";
                ManagementExpander.Visibility = Visibility.Visible;
                //       OrdersystemExpander.Visibility = Visibility.Visible;
            }
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = SidebarMain.SelectedItem as NavButton;


        }




        private void NavButton_Selected_Dashboard(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Dashboard";
            if (LocalDevice.Default.isTouch)
            {
                this.WindowState = WindowState.Maximized;
                navframe.Navigate(new Uri("/main/DashboardTouchView.xaml", UriKind.Relative));
            }
            else
            {
                navframe.Navigate(new Uri("/main/mainDashboard.xaml", UriKind.Relative));
            }
        }
        private void NavButton_Selected_RentItem(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Entnahme";
            navframe.Navigate(new Uri("/application/RentItem/RentItemView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_OutsideLocation(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Zwischenlager";
            navframe.Navigate(new Uri("/application/TempLocations/TempLocationsView.xaml", UriKind.Relative));
        }


        private void NavButton_Selected_ReturnItem(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Rückgabe";
            navframe.Navigate(new Uri("/application/returnItem/ReturnItemSelect.xaml", UriKind.Relative));

        }

        private void NavButton_Selected_History(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Historie";
            navframe.Navigate(new Uri("/application/History/HistoryView.xaml", UriKind.Relative));

        }
        private void NavButton_Selected_ItemOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Artikelverwaltung";
            navframe.Navigate(new Uri("/application/Administration/ItemAdministration/ItemOverviewView.xaml", UriKind.Relative));


        }
        private void NavButton_Selected_RebookItem(object sender, RoutedEventArgs e)
        {
            SidebarMngt.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Umbuchen : Artikel";
            navframe.Navigate(new Uri("/application/RebookSystem/RebookItem/RebookViewSelection.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_RebookGroup(object sender, RoutedEventArgs e)
        {
            SidebarMngt.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Umbuchen : Palette";
            navframe.Navigate(new Uri("/application/RebookSystem/RebookGroup/RebookGroupView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_StockOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Lagerübersicht";
            navframe.Navigate(new Uri("/application/OrderSystem/StorageOverview/StorageOverviewView.xaml", UriKind.Relative));

        }
        private void NavButton_Selected_ItemOverviewOrder(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            Breadcrumbs.Text = "Bestellsystem : Artikelübersicht";
            navframe.Navigate(new Uri("/application/OrderSystem/ItemOverviewShop/ItemOverviewShopView.xaml", UriKind.Relative));

        }
        private void NavButton_Selected_ActiveOrderOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            Breadcrumbs.Text = "Bestellsystem : Aktive Bestellungen";
            navframe.Navigate(new Uri("/application/OrderSystem/CurrentOrders/CurrentOrdersView.xaml", UriKind.Relative));

        }

        private void NavButton_Selected_LocationOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            // SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Lagerortverwaltung";
            navframe.Navigate(new Uri("/application/Administration/LocationAdministration/LocationOverviewView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_TempLocationOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            // SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Zwischenlagerverwaltung";
            navframe.Navigate(new Uri("/application/Administration/TempLocationAdministration/TempLocatioOverviewView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_CustomerOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            // SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Kundenverwaltung";
            navframe.Navigate(new Uri("/application/Administration/CustomerAdministration/CustomerAdministrationView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_VendorOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            // SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Lieferantenverwaltung";
            navframe.Navigate(new Uri("/application/Administration/VendorAdministration/VendorAdministrationView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_UserOverview(object sender, RoutedEventArgs e)
        {
            SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            // SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung : Mitarbeiterverwaltung";
            navframe.Navigate(new Uri("/application/Administration/UserAdministration/UserAdministrationView.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_OldOrderOverview(object sender, RoutedEventArgs e)
        {
            ErrorHandlerModel.ErrorText = "Diese Funktion ist aktuell nicht verfügbar!";
            ErrorHandlerModel.ErrorType = "ERROR";
            ErrorWindow showError = new ErrorWindow();
            Nullable<bool> dialogResult = showError.ShowDialog();
        }
        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(4);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    base.OnMouseLeftButtonDown(e);
                    this.WindowState = WindowState.Normal;
                    this.DragMove();
                }
            }

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void AdjustWindowSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaxButton.Content = FindResource("maxBtn");
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaxButton.Content = FindResource("minBtn");
            }
        }

        private void NavButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("TEST");
            loginView loginWindow = new loginView();
            this.Close();
            loginWindow.Show();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocalDevice.Default.isTouch)
            {
                LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                this.Close();
                LoginWindowTouch.Show();
            }
            else
            {
                loginView LoginScreen = new loginView();
                this.Close();
                LoginScreen.Show();
            }
        }
        private void OpenAdmin_Click(object sender, RoutedEventArgs e)
        {
            Breadcrumbs.Text = "Verwaltung";
            navframe.Navigate(new System.Uri("/application/Administration/AdministrationOverviewView.xaml", UriKind.RelativeOrAbsolute));
        }



        private void SidebarMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            UserSettingsWindow openSettings = new UserSettingsWindow();
            Nullable<bool> dialogResult = openSettings.ShowDialog();
        }

        private void SidebarRebook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
