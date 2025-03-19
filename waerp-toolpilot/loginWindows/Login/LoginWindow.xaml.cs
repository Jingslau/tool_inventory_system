using System;
using System.Data;
using System.Reflection;
using System.Windows;
using MySqlConnector;
using waerp_toolpilot.config.DatabaseSettingsView;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.main;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.loginHandling
{
    /// <summary>
    /// Interaction logic for loginView.xaml
    /// </summary>
    /// 

    public partial class loginView : Window
    {
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public loginView()
        {
            InitializeComponent();

            CurrentVersion.Text = (string)FindResource("cbString2") + " [BETA] " + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            usernameP.Focus();

        }
        public string username { get; set; }
        public string password { get; set; }

        private void loginHandler(object sender, RoutedEventArgs e)
        {

            MainWindowViewModel.ItemsToOrder = 0;
            MainWindowViewModel.ItemOnMin = 0;
            conn.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE username = '{usernameP.Text}'", conn);
            DataSet ds = new DataSet();
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            adp.Fill(ds);
            conn.Close();

            if (ds.Tables[0].Rows.Count == 1)
            {
                if (!bool.Parse(ds.Tables[0].Rows[0]["isDeactivated"].ToString()))
                {
                    if (ds.Tables[0].Rows[0]["user_password"].ToString() == PasswordInput.Password)
                    {
                        MainWindowViewModel.Firstname = ds.Tables[0].Rows[0]["name"].ToString();
                        MainWindowViewModel.Lastname = ds.Tables[0].Rows[0]["surname"].ToString();
                        MainWindowViewModel.UserID = ds.Tables[0].Rows[0]["user_id"].ToString();
                        MainWindowViewModel.username = ds.Tables[0].Rows[0]["username"].ToString();
                        MainWindowViewModel.RoleID = int.Parse(ds.Tables[0].Rows[0]["role_id"].ToString());

                        DataSet ds4 = AdministrationQueries.RunSql("SELECT * FROM company_settings");
                        DataSet ds5 = AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = 4");

                        string originalLanguage = ds5.Tables[0].Rows[0]["culture_code"].ToString();

                        ResourceDictionary languageDictionaryOld = new ResourceDictionary
                        {
                            Source = new Uri($"/Language/{originalLanguage}.xaml", UriKind.Relative)
                        };



                        ResourceDictionary languageDictionaryNew = new ResourceDictionary
                        {
                            Source = new Uri($"/Language/{AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = 4").Tables[0].Rows[0]["culture_code"].ToString()}.xaml", UriKind.Relative)
                        };

                        MainWindowViewModel.originLanguageDic = languageDictionaryOld;
                        MainWindowViewModel.currentLanguageDic = languageDictionaryNew;

                        Application.Current.Resources.MergedDictionaries.Remove(languageDictionaryOld);
                        Application.Current.Resources.MergedDictionaries.Add(languageDictionaryNew);


                        DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM user_roles WHERE role_id = {MainWindowViewModel.RoleID}");
                        MainWindowViewModel.RoleStr = ds2.Tables[0].Rows[0]["name"].ToString();
                        ds = new DataSet();
                        ds = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID}");

                        MainWindowViewModel.globalImagePath = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_image_path'").Tables[0].Rows[0]["settings_value"].ToString();

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
                                MainWindowViewModel.ItemsToOrder = OrderItemOverviewQueries.GetAllItemsNeeded().Tables[0].Rows.Count;

                                MainWindowViewModel.ItemOnMin = OrderItemOverviewQueries.GetAllItemsMin().Tables[0].Rows.Count;
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
                    SnackbarDeactivated.IsActive = true;
                }

            }
            else
            {
                SnackbarError.IsActive = true;
            }








        }

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainPasswordWindow openMasterlogin = new MainPasswordWindow();
            openMasterlogin.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void LoginDev()
        {
            try
            {
                usernameP.Text = "admin";

                conn.Open();
                int matches = 0;
                MySqlDataReader reader = null;
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE username = '{usernameP.Text}'", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MainWindowViewModel.Firstname = (string)reader["name"];
                    MainWindowViewModel.Lastname = (string)reader["surname"];
                    MainWindowViewModel.UserID = reader["user_id"].ToString();
                    MainWindowViewModel.username = reader["username"].ToString();
                    MainWindowViewModel.RoleID = int.Parse(reader["role_id"].ToString());



                    matches++;
                }

                if (matches == 1)
                {
                    DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM user_roles WHERE role_id = {MainWindowViewModel.RoleID}");
                    MainWindowViewModel.RoleStr = ds2.Tables[0].Rows[0]["name"].ToString();
                    DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM user_privilege_relations WHERE user_id = {MainWindowViewModel.UserID}");
                    MessageBox.Show(ds2.Tables[0].Rows[0]["name"].ToString());
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Tables[0].Rows[i]["privileges_id"].ToString() == "1")
                        {
                            MainWindowViewModel.showAdministration = true;
                            MainWindowViewModel.showSettings = true;
                        }
                        else if (ds.Tables[0].Rows[i]["privileges_id"].ToString() == "2")
                        {
                            MainWindowViewModel.showRebook = true;
                        }
                        else if (ds.Tables[0].Rows[i]["privileges_id"].ToString() == "3")
                        {
                            MainWindowViewModel.showOrdersystem = true;
                        }
                    }
                    MainWindow win2 = new MainWindow();
                    Close();

                    win2.Show();
                    conn.Close();
                }
                else
                {
                    SnackbarError.IsActive = true;
                }

                conn.Close();

            }
            catch (MySqlException ex)
            {

                MessageBox.Show((string)FindResource("cbString1") + ex.ToString());
                conn.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            test.SyncSchmidtsDB();
        }

        private void SyncDatabase_Click(object sender, RoutedEventArgs e)
        {
            //DataSet items = AdministrationQueries.RunSql("SELECT * FROM item_objects");
            //DataSet synctable = AdministrationQueries.RunSql("SELECT * FROM syncdatatable");

            //for (int i = 0; i < synctable.Tables[0].Rows.Count; i++)
            //{
            //    DataSet item_tmp = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{synctable.Tables[0].Rows[i]["Art_Nr"]}'");

            //    if (item_tmp.Tables[0].Rows.Count > 0)
            //    {
            //        DataSet checkIfInVendor = AdministrationQueries.RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {item_tmp.Tables[0].Rows[0]["item_id"]}");
            //        if (checkIfInVendor.Tables[0].Rows.Count == 0)
            //        {
            //            DataSet vendorData = AdministrationQueries.RunSql($"SELECT * FROM vendor_objects WHERE vendor_name = '{synctable.Tables[0].Rows[i]["Lief_Kurzname"]}'");

            //            if (vendorData.Tables[0].Rows.Count > 0)
            //            {
            //                AdministrationQueries.RunSqlExec($"INSERT INTO item_vendor_relations (item_id, vendor_id, item_price, currency_id) VALUES (" +
            //                    $"{item_tmp.Tables[0].Rows[0]["item_id"]}," +
            //                    $"{vendorData.Tables[0].Rows[0]["vendor_id"]}," +
            //                    $"'{synctable.Tables[0].Rows[i]["EK_Preis"]}'," +
            //                    $"1" +
            //                    $")");

            //                AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_orderable = 1 WHERE item_id = {item_tmp.Tables[0].Rows[0]["item_id"]}");
            //            }
            //        }
            //    }
            //}

            DataSet synctable = AdministrationQueries.RunSql("SELECT * FROM syncdatatable");

            for (int i = 0; i < synctable.Tables[0].Rows.Count; i++)
            {
                DataSet item_tmp = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_ident = '{synctable.Tables[0].Rows[i]["Art_Nr"]}'");

                AdministrationQueries.RunSqlExec($"INSERT INTO item_filter_relations (item_id, filter1_id, filter2_id, filter3_id, filter4_id, filter5_id) VALUES ({item_tmp.Tables[0].Rows[0]["item_id"]}, -1,-1,-1,-1,-1)");

                if (item_tmp.Tables[0].Rows.Count > 0)
                {
                    for (int j = 1; j <= 5; j++)
                    {
                        DataSet found_filter_id = AdministrationQueries.RunSql($"SELECT * FROM filter{j}_names WHERE name = '{synctable.Tables[0].Rows[i][$"Suche_Stufe{j}"]}'");

                        if (found_filter_id.Tables[0].Rows.Count > 0)
                        {
                            string filter_id = found_filter_id.Tables[0].Rows[0]["filter_id"].ToString();
                            AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter{j}_id = {filter_id} WHERE item_id = {item_tmp.Tables[0].Rows[0]["item_id"]}");

                        }


                    }
                }
            }


        }
    }
}
