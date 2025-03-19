using System;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using waerp_toolpilot.config;
using waerp_toolpilot.config.AccountSettingsView;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.loginHandling;
using waerp_toolpilot.LoginTouch;
using waerp_toolpilot.main;
using waerp_toolpilot.mainGUI;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot
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



            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key != null)
            {

                DataSet settings = new DataSet();
                settings = AdministrationQueries.RunSql("SELECT * FROM company_settings");
                LicenseText.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString() + " | " + "Licensed for " + settings.Tables[0].Rows[0]["settings_value"];
                RunningParameters.imagePath = settings.Tables[0].Rows[21][2].ToString();

                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                DataSet userPrivilege = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID} AND privilege_id = 4");



                if (bool.Parse(key.GetValue("IsTouch").ToString()))
                {
                    DashboardTouch.Visibility = Visibility.Visible;
                    DashboardMKß.Visibility = Visibility.Collapsed;
                    WindowState = WindowState.Maximized;
                    if (userPrivilege.Tables[0].Rows.Count > 0)
                    {
                        navframe.Navigate(new Uri("/mainGUI/DashboardView.xaml", UriKind.Relative));
                    }
                    else
                    {
                        navframe.Navigate(new Uri("/mainGUI/DashboardTouchView.xaml", UriKind.Relative));
                    }
                }
                else
                {
                    DashboardTouch.Visibility = Visibility.Collapsed;
                    DashboardMKß.Visibility = Visibility.Visible;
                    if (userPrivilege.Tables[0].Rows.Count > 0)
                    {
                        navframe.Navigate(new Uri("/mainGUI/DashboardView.xaml", UriKind.Relative));
                    }
                    else
                    {
                        navframe.Navigate(new Uri("/mainGUI/DashboardTouchView.xaml", UriKind.Relative));
                    }
                }
            }

            RoleNameUser.Text = MainWindowViewModel.RoleStr;



            Breadcrumbs.Text = "Dashboard";
            if (!MainWindowViewModel.showAdministration)
            {
                Settings.Visibility = Visibility.Collapsed;
                ManagementExpander.Visibility = Visibility.Collapsed;
            }
            else
            {
                Settings.Visibility = Visibility.Visible;
                ManagementExpander.Visibility = Visibility.Visible;
            }
            //if (!MainWindowViewModel.showRebook)
            //{
            //    RebookExpander.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    RebookExpander.Visibility = Visibility.Visible;
            //}
            if (!MainWindowViewModel.showOrdersystem)
            {
                OrdersystemExpander.Visibility = Visibility.Collapsed;
            }
            else
            {
                OrdersystemExpander.Visibility = Visibility.Visible;
                if (key.GetValue("RemindMeLaterDate").ToString() != "")
                {
                    DateTime enteredDate = DateTime.Parse(key.GetValue("RemindMeLaterDate").ToString());
                    DateTime now = DateTime.Now;
                    TimeSpan difference = now - enteredDate;

                    int differenceDay = difference.Days;

                    if (differenceDay > 7)
                    {
                        key.SetValue("RemindMeLaterDate", "");
                        if (MainWindowViewModel.ItemOnMin > 0)
                        {
                            if (MainWindowViewModel.ItemsToOrder > 0)
                            {
                                ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                    (string)FindResource("errorText1b") + " " + MainWindowViewModel.ItemsToOrder.ToString() +
                                    "\n \n" +
                                    (string)FindResource("errorText1c") + " " + MainWindowViewModel.ItemOnMin.ToString();
                                ErrorHandlerModel.ErrorType = "INFO";
                                OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                                showAlert.ShowDialog();
                            }
                            else
                            {
                                ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                (string)FindResource("errorText1c") + " " + MainWindowViewModel.ItemOnMin.ToString();
                                ErrorHandlerModel.ErrorType = "INFO";
                                OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                                showAlert.ShowDialog();
                            }
                        }
                        else if (MainWindowViewModel.ItemsToOrder > 0)
                        {
                            if (MainWindowViewModel.ItemOnMin > 0)
                            {
                                ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                    (string)FindResource("errorText1b") + " " + MainWindowViewModel.ItemsToOrder.ToString() +
                                    "\n \n" +
                                    (string)FindResource("errorText1c") + " " + MainWindowViewModel.ItemOnMin.ToString();
                                ErrorHandlerModel.ErrorType = "INFO";
                                OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                                showAlert.ShowDialog();
                            }
                            else
                            {
                                ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                    (string)FindResource("errorText1b") + " " + MainWindowViewModel.ItemsToOrder.ToString();
                                ErrorHandlerModel.ErrorType = "INFO";
                                OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                                showAlert.ShowDialog();
                            }
                        }
                    }

                }
                else
                {
                    key.SetValue("RemindMeLaterDate", "");
                    if (MainWindowViewModel.ItemOnMin > 0)
                    {
                        if (MainWindowViewModel.ItemsToOrder > 0)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                    (string)FindResource("errorText1b") + " " + MainWindowViewModel.ItemsToOrder.ToString() +
                                    "\n \n" +
                                    (string)FindResource("errorText1c") + " " + MainWindowViewModel.ItemOnMin.ToString();
                            ErrorHandlerModel.ErrorType = "INFO";
                            OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                            showAlert.ShowDialog();
                        }
                        else
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                (string)FindResource("errorText1c") + " " + MainWindowViewModel.ItemOnMin.ToString();
                            ErrorHandlerModel.ErrorType = "INFO";
                            OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                            showAlert.ShowDialog();
                        }
                    }
                    else if (MainWindowViewModel.ItemsToOrder > 0)
                    {
                        if (MainWindowViewModel.ItemOnMin > 0)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                    (string)FindResource("errorText1b") + " " + MainWindowViewModel.ItemsToOrder.ToString() +
                                    "\n \n" +
                                    (string)FindResource("errorText1c") + " " + MainWindowViewModel.ItemOnMin.ToString();
                            ErrorHandlerModel.ErrorType = "INFO";
                            OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                            showAlert.ShowDialog();
                        }
                        else
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("errorText1a") + "\n \n" +
                                    (string)FindResource("errorText1b") + " " + MainWindowViewModel.ItemsToOrder.ToString();
                            ErrorHandlerModel.ErrorType = "INFO";
                            OrderSystemAlertWindow showAlert = new OrderSystemAlertWindow();
                            showAlert.ShowDialog();
                        }
                    }
                }


            }
            //Breadcrumbs.Text = "Dashboard";
            //if (MainWindowViewModel.RoleID != 1)
            //{
            //    RoleNameUser.Text = "Mitarbeiter";
            //    Settings.Visibility = Visibility.Collapsed;
            //    ManagementExpander.Visibility = Visibility.Collapsed;
            //    OrdersystemExpander.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    RoleNameUser.Text = "Administrator";
            //    Settings.Visibility = Visibility.Visible;
            //    ManagementExpander.Visibility = Visibility.Visible;
            //    //       OrdersystemExpander.Visibility = Visibility.Visible;
            //}
        }




        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = SidebarMain.SelectedItem as NavButton;
            Loaded += MainWindow_Loaded;

        }



        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the primary screen dimensions
            var screen = Screen.PrimaryScreen.WorkingArea;

            // Calculate 70% of screen width and 80% of screen height
            double targetWidth = screen.Width * 0.8;
            double targetHeight = screen.Height * 0.9;

            // Set the window's width and height
            Width = targetWidth;
            Height = targetHeight;

            // Center the window on the screen
            Left = (screen.Width - targetWidth) / 2;
            Top = (screen.Height - targetHeight) / 2;
        }
        private void NavButton_Selected_Dashboard(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb1");
            DataSet userPrivilege = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID} AND privilege_id = 4");

            if (userPrivilege.Tables[0].Rows.Count > 0)
            {
                navframe.Navigate(new Uri("/mainGUI/DashboardView.xaml", UriKind.Relative));
            }
            else
            {
                navframe.Navigate(new Uri("/mainGUI/DashboardTouchView.xaml", UriKind.Relative));
            }
        }
        private void NavButton_Selected_RentItem(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Artikelübersicht";
            navframe.Navigate(new Uri("/modules/RentItem/RentItemView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_Machines(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb3");
            navframe.Navigate(new Uri("/modules/Machine/MachineOverviewView.xaml", UriKind.Relative));
        }


        private void NavButton_Selected_OutsideLocation(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Zwischenlager";
            navframe.Navigate(new Uri("/modules/TempLocations/TempLocationsView.xaml", UriKind.Relative));
        }


        private void NavButton_Selected_ReturnItem(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb4");
            navframe.Navigate(new Uri("/modules/returnItem/ReturnItemSelect.xaml", UriKind.Relative));

        }

        private void NavButton_Selected_History(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("mainWindowButton15");
            navframe.Navigate(new Uri("/modules/History/HistoryView.xaml", UriKind.Relative));

        }
        private void NavButton_Selected_ItemOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb5");
            navframe.Navigate(new Uri("/modules/Administration/ItemAdministration/ItemOverviewView.xaml", UriKind.Relative));


        }
        private void NavButton_Selected_RebookItem(object sender, RoutedEventArgs e)
        {
            SidebarMngt.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb6");
            navframe.Navigate(new Uri("/modules/RebookSystem/RebookItem/RebookViewSelection.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_RebookGroup(object sender, RoutedEventArgs e)
        {
            SidebarMngt.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Umbuchen/Palette";
            navframe.Navigate(new Uri("/modules/RebookSystem/RebookGroup/RebookGroupView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_StockOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb7");
            navframe.Navigate(new Uri("/modules/OrderSystem/StorageOverview/StorageOverviewView.xaml", UriKind.Relative));

        }
        private void NavButton_Selected_ItemOverviewOrder(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb8");
            navframe.Navigate(new Uri("/modules/OrderSystem/ItemOverviewShop/ItemOverviewShopView.xaml", UriKind.Relative));

        }
        private void NavButton_Selected_ActiveOrderOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarMngt.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb9");
            navframe.Navigate(new Uri("/modules/OrderSystem/CurrentOrders/CurrentOrdersView.xaml", UriKind.Relative));

        }

        private void NavButton_Selected_LocationOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb10");
            navframe.Navigate(new Uri("/modules/Administration/LocationAdministration/LocationOverviewView.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_MachineOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb17");
            navframe.Navigate(new Uri("/modules/Administration/MachineAdministration/MachineOverviewView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_TempLocationOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Verwaltung / Zwischenlagerverwaltung";
            navframe.Navigate(new Uri("/modules/Administration/TempLocationAdministration/TempLocatioOverviewView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_CustomerOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb11");
            navframe.Navigate(new Uri("/modules/Administration/CustomerAdministration/CustomerAdministrationView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_VendorOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb12");
            navframe.Navigate(new Uri("/modules/Administration/VendorAdministration/VendorAdministrationView.xaml", UriKind.Relative));
        }
        private void NavButton_Selected_UserOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb13");
            navframe.Navigate(new Uri("/modules/Administration/UserAdministration/UserAdministrationView.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_OldOrderOverview(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb14");
            navframe.Navigate(new Uri("/modules/OrderSystem/ManageOldOrders/OldOrdersOverviewView.xaml", UriKind.Relative));

        }
        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new System.Windows.Thickness(4);
            }
            else
            {
                BorderThickness = new System.Windows.Thickness(0);
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
                    WindowState = WindowState.Normal;
                    DragMove();
                }
            }

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void AdjustWindowSize()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaxButton.Content = FindResource("maxBtn");
            }
            else
            {
                WindowState = WindowState.Maximized;
                MaxButton.Content = FindResource("minBtn");
            }
        }

        private void NavButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            loginView loginWindow = new loginView();
            Close();
            loginWindow.Show();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);

            System.Windows.Application.Current.Resources.MergedDictionaries.Remove(MainWindowViewModel.currentLanguageDic);
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(MainWindowViewModel.originLanguageDic);

            MainWindowViewModel.Firstname = "Max";
            MainWindowViewModel.Lastname = "Mustermann";
            MainWindowViewModel.UserID = "";
            MainWindowViewModel.username = "";
            MainWindowViewModel.RoleID = 0;
            MainWindowViewModel.RoleStr = "";
            MainWindowViewModel.CurrentBreadcumb = "";
            MainWindowViewModel.showOrdersystem = false;
            MainWindowViewModel.showRebook = false;
            MainWindowViewModel.showAdministration = false;
            MainWindowViewModel.showSettings = false;
            MainWindowViewModel.openApplication = false;
            MainWindowViewModel.loginSuccesful = false;

            if (bool.Parse(key.GetValue("IsTouch").ToString()))
            {
                LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                Close();
                LoginWindowTouch.Show();
            }
            else
            {
                loginView LoginScreen = new loginView();
                Close();
                LoginScreen.Show();
            }
        }
        private void OpenAdmin_Click(object sender, RoutedEventArgs e)
        {
            Breadcrumbs.Text = "Verwaltung";
            navframe.Navigate(new System.Uri("/modules/Administration/AdministrationOverviewView.xaml", UriKind.RelativeOrAbsolute));
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


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AccountSettingsWindow openAccSettings = new AccountSettingsWindow();
            openAccSettings.ShowDialog();
        }

        private void ShutdownTop_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void LogoutTop_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);

            MainWindowViewModel.Firstname = "Max";
            MainWindowViewModel.Lastname = "Mustermann";
            MainWindowViewModel.UserID = "";
            MainWindowViewModel.username = "";
            MainWindowViewModel.RoleID = 0;
            MainWindowViewModel.RoleStr = "";
            MainWindowViewModel.CurrentBreadcumb = "";
            MainWindowViewModel.showOrdersystem = false;
            MainWindowViewModel.showRebook = false;
            MainWindowViewModel.showAdministration = false;
            MainWindowViewModel.showSettings = false;
            MainWindowViewModel.openApplication = false;
            MainWindowViewModel.loginSuccesful = false;

            if (bool.Parse(key.GetValue("IsTouch").ToString()))
            {
                LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                Close();
                LoginWindowTouch.Show();
            }
            else
            {
                loginView LoginScreen = new loginView();
                Close();
                LoginScreen.Show();
            }
        }

        private void TouchDashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;

            DataSet userPrivilege = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID} AND privilege_id = 4");

            if (userPrivilege.Tables[0].Rows.Count > 0)
            {
                navframe.Navigate(new Uri("/mainGUI/DashboardView.xaml", UriKind.Relative));
            }
            else
            {
                navframe.Navigate(new Uri("/mainGUI/DashboardTouchView.xaml", UriKind.Relative));
            }


        }

        private void NavButton_Selected_MeasureItem(object sender, RoutedEventArgs e)
        {
            SidebarMngt.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Messmittelverwaltung";
            navframe.Navigate(new Uri("/modules/MeasuringEquip/MeasureEquipmentOverviewView.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_Auditor(object sender, RoutedEventArgs e)
        {
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = (string)System.Windows.Application.Current.FindResource("Breadcrumb16");
            navframe.Navigate(new Uri("/modules/Administration/MeasureEquipAdministration/MeasureEquipAuditorOverview.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_Development(object sender, RoutedEventArgs e)
        {
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Development";
            navframe.Navigate(new Uri("/modules/development_test.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_BulkItemEditing(object sender, RoutedEventArgs e)
        {

            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Artikel Massenbearbeitung";
            navframe.Navigate(new Uri("/modules/Administration/ItemAdministration/itemBulkEditingView.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_BulkFilterEditing(object sender, RoutedEventArgs e)
        {
            //SidebarRebook.SelectedIndex = -1;
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Filter Bearbeitung";
            navframe.Navigate(new Uri("/modules/Administration/FilterAdministration/FilterBulkEditingView.xaml", UriKind.Relative));
        }

        private void NavButton_Selected_MeasureItemAdmin(object sender, RoutedEventArgs e)
        {
            SidebarMain.SelectedIndex = -1;
            SidebarShopping.SelectedIndex = -1;
            Breadcrumbs.Text = "Messmittelverwaltung";
            navframe.Navigate(new Uri("/modules/Administration/MeasureEquipAdministration/MeasureEquipAdminOverview.xaml", UriKind.Relative));
        }
    }

}
