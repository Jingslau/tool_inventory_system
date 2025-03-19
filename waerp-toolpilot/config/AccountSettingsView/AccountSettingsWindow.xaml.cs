using System;
using System.Data;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.config.AccountSettingsView
{
    /// <summary>
    /// Interaction logic for AccountSettingsWindow.xaml
    /// </summary>
    public partial class AccountSettingsWindow : Window
    {
        public static string password = "";
        public static string user_id = "";
        public static string originalLanguage = "";
        public AccountSettingsWindow()
        {
            InitializeComponent();
            try
            {
                DataSet user = AdministrationQueries.RunSql($"SELECT * FROM users WHERE user_id = {MainWindowViewModel.UserID}");
                DataSet languages = AdministrationQueries.RunSql("SELECT * FROM culture_objects");

                for (int i = 0; i < languages.Tables[0].Rows.Count; i++)
                {
                    // languageCombobox.Items.Add(languages.Tables[0].Rows[i][2].ToString() + " - " + languages.Tables[0].Rows[i][1].ToString());

                }

                //  languageCombobox.SelectedIndex = int.Parse(user.Tables[0].Rows[0]["culture_id"].ToString()) - 1;


                originalLanguage = languages.Tables[0].Rows[int.Parse(user.Tables[0].Rows[0]["culture_id"].ToString())][2].ToString();

                UserID.Text = user.Tables[0].Rows[0]["user_ident"].ToString();
                vname.Text = user.Tables[0].Rows[0]["name"].ToString();
                Surname.Text = user.Tables[0].Rows[0]["surname"].ToString();
                mail.Text = user.Tables[0].Rows[0]["email"].ToString();
                Username.Text = user.Tables[0].Rows[0]["username"].ToString();

                password = user.Tables[0].Rows[0]["user_password"].ToString();
                user_id = user.Tables[0].Rows[0]["user_id"].ToString();
            }
            catch (Exception exp)
            {
                ErrorLogger.LogSysError(exp);
            }
        }

        private void SaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (oldPW.Password != password)
                {
                    ErrorHandlerModel.ErrorText = "Das bisherige Kennwort ist nicht korrekt!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
                else if (newPW.Password != newPW2.Password)
                {
                    ErrorHandlerModel.ErrorText = "Die Kennwörter müssen übereinstimmen!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
                else
                {
                    AdministrationQueries.EditPassword(newPW.Password, user_id);
                    ErrorHandlerModel.ErrorText = "Das Kennwort wurde erfolgreich geändert!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogSysError(exp);
            }
        }

        private void CloseAccountSettings_Click(object sender, RoutedEventArgs e)
        {

            //  string[] selectedLanguageArr = languageCombobox.SelectedItem.ToString().Split(' ');
            //  int selectedLanguageID = languageCombobox.SelectedIndex + 1;
            //     AdministrationQueries.RunSqlExec($"UPDATE users SET culture_id = {selectedLanguageID} WHERE user_id = {MainWindowViewModel.UserID}");



            //ResourceDictionary languageDictionaryOld = new ResourceDictionary
            //{
            //    Source = new Uri($"/Language/{originalLanguage}.xaml", UriKind.Relative)
            //};

            //ResourceDictionary languageDictionaryNew = new ResourceDictionary
            //{
            //    Source = new Uri($"/Language/{selectedLanguageArr[0]}.xaml", UriKind.Relative)
            //};


            //Application.Current.Resources.MergedDictionaries.Remove(MainWindowViewModel.currentLanguageDic);
            //Application.Current.Resources.MergedDictionaries.Add(languageDictionaryNew);
            //MainWindowViewModel.currentLanguageDic = languageDictionaryNew;

            DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
