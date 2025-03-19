using System;
using System.Data;
using System.Reflection;
using System.Windows;
using MySqlConnector;
using waerp_toolpilot.config.DatabaseSettingsView;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.main;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.LoginTouch
{
    /// <summary>
    /// Interaction logic for LoginTouchWindow.xaml
    /// </summary>
    public partial class LoginTouchWindow : Window
    {

        public Boolean showInput = false;
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public LoginTouchWindow()
        {
            WindowState = WindowState.Maximized;
            InitializeComponent();
            CurrentVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public string username { get; set; }
        public string password { get; set; }

        string inputPin = "";

        private void loginHandler(object sender, RoutedEventArgs e)
        {
            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE user_ident = '{inputPin}'", conn);
                DataSet ds = new DataSet();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                conn.Close();
                if (ds.Tables[0].Rows.Count == 1)
                {
                    if (ds.Tables[0].Rows[0]["user_ident"].ToString() == inputPin)
                    {
                        MainWindowViewModel.Firstname = ds.Tables[0].Rows[0]["name"].ToString();
                        MainWindowViewModel.Lastname = ds.Tables[0].Rows[0]["surname"].ToString();
                        MainWindowViewModel.UserID = ds.Tables[0].Rows[0]["user_id"].ToString();
                        MainWindowViewModel.username = ds.Tables[0].Rows[0]["username"].ToString();
                        MainWindowViewModel.RoleID = int.Parse(ds.Tables[0].Rows[0]["role_id"].ToString());
                        DataSet ds4 = AdministrationQueries.RunSql("SELECT * FROM company_settings");
                        DataSet ds5 = AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = {ds4.Tables[0].Rows[22][2]}");

                        string originalLanguage = ds5.Tables[0].Rows[0]["culture_code"].ToString();

                        ResourceDictionary languageDictionaryOld = new ResourceDictionary
                        {
                            Source = new Uri($"/Language/{originalLanguage}.xaml", UriKind.Relative)
                        };



                        ResourceDictionary languageDictionaryNew = new ResourceDictionary
                        {
                            Source = new Uri($"/Language/{AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = {ds.Tables[0].Rows[0]["culture_id"]}").Tables[0].Rows[0]["culture_code"].ToString()}.xaml", UriKind.Relative)
                        };

                        MainWindowViewModel.originLanguageDic = languageDictionaryOld;
                        MainWindowViewModel.currentLanguageDic = languageDictionaryNew;

                        Application.Current.Resources.MergedDictionaries.Remove(languageDictionaryOld);
                        Application.Current.Resources.MergedDictionaries.Add(languageDictionaryNew);

                        DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM user_roles WHERE role_id = {MainWindowViewModel.RoleID}");
                        MainWindowViewModel.RoleStr = ds2.Tables[0].Rows[0]["name"].ToString();
                        ds = new DataSet();
                        ds = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID}");

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["privilege_id"].ToString() == "1")
                            {
                                MainWindowViewModel.showAdministration = true;
                                MainWindowViewModel.showSettings = true;
                            }
                            else if (ds.Tables[0].Rows[i]["privilege_id"].ToString() == "2")
                            {
                                MainWindowViewModel.showRebook = true;
                            }
                            else if (ds.Tables[0].Rows[i]["privilege_id"].ToString() == "3")
                            {
                                MainWindowViewModel.showOrdersystem = true;
                            }
                        }
                        conn.Close();
                        MainWindow win2 = new MainWindow();
                        win2.Show();
                        Close();

                    }
                    else
                    {
                        SnackbarError.IsActive = true;
                    }

                }
                else
                {
                    SnackbarError.IsActive = true;
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Es ist ein Serverfehler aufgetreten!" + ex.ToString());
                conn.Close();
            }

        }

        public void loginUser()
        {
            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE user_ident = '{inputPin}'", conn);
                DataSet ds = new DataSet();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                conn.Close();
                if (ds.Tables[0].Rows.Count == 1)
                {
                    if (ds.Tables[0].Rows[0]["user_ident"].ToString() == inputPin)
                    {
                        MainWindowViewModel.Firstname = ds.Tables[0].Rows[0]["name"].ToString();
                        MainWindowViewModel.Lastname = ds.Tables[0].Rows[0]["surname"].ToString();
                        MainWindowViewModel.UserID = ds.Tables[0].Rows[0]["user_id"].ToString();
                        MainWindowViewModel.username = ds.Tables[0].Rows[0]["username"].ToString();
                        MainWindowViewModel.RoleID = int.Parse(ds.Tables[0].Rows[0]["role_id"].ToString());
                        DataSet ds4 = AdministrationQueries.RunSql("SELECT * FROM company_settings");
                        DataSet ds5 = AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = {ds4.Tables[0].Rows[22][2]}");

                        string originalLanguage = ds5.Tables[0].Rows[0]["culture_code"].ToString();

                        ResourceDictionary languageDictionaryOld = new ResourceDictionary
                        {
                            Source = new Uri($"/Language/{originalLanguage}.xaml", UriKind.Relative)
                        };



                        ResourceDictionary languageDictionaryNew = new ResourceDictionary
                        {
                            Source = new Uri($"/Language/{AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = {ds.Tables[0].Rows[0]["culture_id"]}").Tables[0].Rows[0]["culture_code"].ToString()}.xaml", UriKind.Relative)
                        };

                        MainWindowViewModel.originLanguageDic = languageDictionaryOld;
                        MainWindowViewModel.currentLanguageDic = languageDictionaryNew;

                        Application.Current.Resources.MergedDictionaries.Remove(languageDictionaryOld);
                        Application.Current.Resources.MergedDictionaries.Add(languageDictionaryNew);

                        DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM user_roles WHERE role_id = {MainWindowViewModel.RoleID}");
                        MainWindowViewModel.RoleStr = ds2.Tables[0].Rows[0]["name"].ToString();
                        ds = new DataSet();
                        ds = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID}");

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["privilege_id"].ToString() == "1")
                            {
                                MainWindowViewModel.showAdministration = true;
                                MainWindowViewModel.showSettings = true;
                            }
                            else if (ds.Tables[0].Rows[i]["privilege_id"].ToString() == "2")
                            {
                                MainWindowViewModel.showRebook = true;
                            }
                            else if (ds.Tables[0].Rows[i]["privilege_id"].ToString() == "3")
                            {
                                MainWindowViewModel.showOrdersystem = true;
                            }
                        }
                        conn.Close();
                        MainWindow win2 = new MainWindow();
                        win2.Show();
                        Close();

                    }
                    else
                    {
                        SnackbarError.IsActive = true;
                    }

                }
                else
                {
                    SnackbarError.IsActive = true;
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Es ist ein Serverfehler aufgetreten!" + ex.ToString());
                conn.Close();
            }

        }

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSettingsView openSettings = new DatabaseSettingsView();
            Nullable<bool> dialogResult = openSettings.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void checkInput(string num)
        {
            if (PinPosition1.Text == "")
            {
                if (showInput)
                {
                    PinPosition1.Text = num;
                }
                else
                {
                    PinPosition1.Text = "*";
                }
            }
            else if (PinPosition2.Text == "")
            {
                if (showInput)
                {
                    PinPosition2.Text = num;
                }
                else
                {
                    PinPosition2.Text = "*";
                }
            }
            else if (PinPosition3.Text == "")
            {
                if (showInput)
                {
                    PinPosition3.Text = num;
                }
                else
                {
                    PinPosition3.Text = "*";
                }
            }
            else if (PinPosition4.Text == "")
            {
                if (showInput)
                {
                    PinPosition4.Text = num;
                }
                else
                {
                    PinPosition4.Text = "*";

                }
            }
        }

        private void NumberNine_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {
                inputPin += "9";

                checkInput("9");
            }
            else if (inputPin.Length < 4)
            {
                inputPin += "9"; checkInput("9");
                loginUser();
            }
        }
        private void NumberEight_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {
                inputPin += "5";
                checkInput("5");

            }
            else if (inputPin.Length < 4)
            {
                inputPin += "5"; checkInput("5");
                loginUser();
            }
        }
        private void NumberSeven_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {
                inputPin += "7"; checkInput("7");
            }
            else if (inputPin.Length < 4)
            {
                inputPin += "7"; checkInput("7");
                loginUser();
            }
        }
        private void NumberSix_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {
                inputPin += "6";
                checkInput("6");
            }
            else if (inputPin.Length < 4)
            {
                inputPin += "6"; checkInput("6");
                loginUser();
            }
        }
        private void NumberFive_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {
                inputPin += "5"; checkInput("5");
            }
            else if (inputPin.Length < 4)
            {
                inputPin += "5"; checkInput("5");
                loginUser();
            }
        }
        private void NumberFour_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {
                inputPin += "4"; checkInput("4");
            }
            else if (inputPin.Length < 4)
            {
                inputPin += "4"; checkInput("4");
                loginUser();
            }
        }
        private void NumberThree_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {

                inputPin += "3"; checkInput("3");

            }
            else if (inputPin.Length < 4)
            {
                inputPin += "3"; checkInput("3");
                loginUser();
            }
        }
        private void NumberTwo_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {

                inputPin += "2"; checkInput("2");

            }
            else if (inputPin.Length < 4)
            {
                inputPin += "2"; checkInput("2");
                loginUser();
            }
        }
        private void NumberOne_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {

                inputPin += "1"; checkInput("1");

            }
            else if (inputPin.Length < 4)
            {
                inputPin += "1"; checkInput("1");
                loginUser();
            }
        }
        private void NumberZero_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length < 3)
            {

                inputPin += "0"; checkInput("0");

            }
            else if (inputPin.Length < 4)
            {
                inputPin += "0"; checkInput("0");
                loginUser();
            }
        }

        private void DeleteLastDigit_Click(object sender, RoutedEventArgs e)
        {
            if (inputPin.Length == 1)
            {
                inputPin = "";

                PinPosition1.Text = "";
                PinPosition2.Text = "";
                PinPosition3.Text = "";
                PinPosition4.Text = "";

            }
            else if (inputPin.Length > 1)
            {
                if (inputPin.Length == 4)
                {
                    PinPosition4.Text = "";
                }
                else if (inputPin.Length == 3)
                {
                    PinPosition3.Text = "";
                }
                else if (inputPin.Length == 2)
                {
                    PinPosition2.Text = "";
                }
                else if (inputPin.Length == 1)
                {
                    PinPosition1.Text = "";
                }


                inputPin = inputPin.Remove(inputPin.Length - 1);



            }
        }

        private void ShowInput(object sender, RoutedEventArgs e)
        {
            showInput = !showInput;
            if (showInput)
            {
                if (inputPin.Length == 1)
                {
                    PinPosition1.Text = inputPin[0].ToString();
                }
                else if (inputPin.Length == 2)
                {
                    PinPosition1.Text = inputPin[0].ToString();
                    PinPosition2.Text = inputPin[1].ToString();
                }
                else if (inputPin.Length == 3)
                {
                    PinPosition1.Text = inputPin[0].ToString();
                    PinPosition2.Text = inputPin[1].ToString();
                    PinPosition3.Text = inputPin[2].ToString();
                }
                else if (inputPin.Length == 4)
                {
                    PinPosition1.Text = inputPin[0].ToString();
                    PinPosition2.Text = inputPin[1].ToString();
                    PinPosition3.Text = inputPin[2].ToString();
                    PinPosition4.Text = inputPin[3].ToString();
                }
            }
            else
            {
                if (inputPin.Length == 1)
                {
                    PinPosition1.Text = "*";
                }
                else if (inputPin.Length == 2)
                {
                    PinPosition1.Text = "*";
                    PinPosition2.Text = "*";
                }
                else if (inputPin.Length == 3)
                {
                    PinPosition1.Text = "*";
                    PinPosition2.Text = "*";
                    PinPosition3.Text = "*";
                }
                else if (inputPin.Length == 4)
                {
                    PinPosition1.Text = "*";
                    PinPosition2.Text = "*";
                    PinPosition3.Text = "*";
                    PinPosition4.Text = "*";
                }
            }

        }
        private void SaveNumberInput_Click(object sender, RoutedEventArgs e)
        {
            CurrentRentModel.RentQuantity = inputPin;
            DialogResult = false;
        }


    }
}
