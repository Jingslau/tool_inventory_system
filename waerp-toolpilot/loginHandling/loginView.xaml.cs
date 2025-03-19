using MySqlConnector;
using System;
using System.Reflection;
using System.Windows;
using waerp_management.config;
using waerp_management.config.DatabaseSettingsView;
using waerp_management.main;

namespace waerp_management.loginHandling
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
            CurrentVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " [DEMO]";

        }
        public string username { get; set; }
        public string password { get; set; }

        private void loginHandler(object sender, RoutedEventArgs e)
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
                    MainWindow win2 = new MainWindow();
                    this.Close();

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
                    MainWindow win2 = new MainWindow();
                    this.Close();

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
                MessageBox.Show("Es ist ein Serverfehler aufgetreten!" + ex.ToString());
                conn.Close();
            }
        }
    }
}
