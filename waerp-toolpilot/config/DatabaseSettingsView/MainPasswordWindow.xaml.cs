using System;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.config.DatabaseSettingsView
{
    /// <summary>
    /// Interaction logic for MainPasswordWindow.xaml
    /// </summary>
    public partial class MainPasswordWindow : Window
    {
        public MainPasswordWindow()
        {
            InitializeComponent();
        }

        private void CheckMasterPassword_Click(object sender, RoutedEventArgs e)
        {

            if (AdministrationQueries.RunSql($"SELECT * FROM company_settings WHERE settings_value = '{PasswordInput.Password}'").Tables[0].Rows.Count > 0)
            {
                DatabaseSettingsView openSettings = new DatabaseSettingsView();
                Nullable<bool> dialogResult = openSettings.ShowDialog();
                DialogResult = false;
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Das Masterpasswort war nicht korrekt!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
