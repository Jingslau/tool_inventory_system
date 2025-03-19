using System;
using System.IO;
using System.Net;
using System.Windows;
using waerp_management.config.SettingsStore;
using waerp_management.loginHandling;
using waerp_management.LoginTouch;

namespace waerp_management.main
{
    /// <summary>
    /// Interaktionslogik für StartUpView.xaml
    /// </summary>
    public partial class StartUpView : Window
    {
        public StartUpView()
        {
            InitializeComponent();
            //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\wærp-stockpilot", true);

            //if (CheckForInternetConnection())
            //{

            //    LicenseServerConnector.CheckLicenseStartUp();
            //    if (key.GetValue("hasLicense").ToString() == "True")
            //    {
            //        //  LoginViewTouch LoginScreen2 = new LoginViewTouch();
            //        loginView LoginScreen = new loginView();
            //        //  LoginTouchWindow LoginScreen = new LoginTouchWindow();
            //        this.Close();

            //        LoginScreen.Show();
            //    }



            //}
            //else
            //{
            //    if (key.GetValue("hasLicense").ToString() == "True")
            //    {
            //        DateTime thisDay = DateTime.Now;
            //        DateTime lastCheckin = DateTime.Parse(key.GetValue("checkIn").ToString());
            //        int timeDifference = (lastCheckin - thisDay).Days;

            //        if (timeDifference < 8)
            //        {
            //            //  LoginViewTouch LoginScreen2 = new LoginViewTouch();
            //            loginView LoginScreen = new loginView();
            //            //  LoginTouchWindow LoginScreen = new LoginTouchWindow();
            //            this.Close();

            //            LoginScreen.Show();
            //        }
            //        else if (timeDifference < 10 && timeDifference >= 8)
            //        {
            //            ErrorHandlerModel.ErrorText = "Bitte verbinden Sie das Gerät in den nächsten Tagen mit dem Internet, damit Sie das Programm weiter nutzen können!";
            //            ErrorHandlerModel.ErrorType = "INFO";
            //            ErrorWindow showInfo = new ErrorWindow();
            //            showInfo.ShowDialog();
            //            //  LoginViewTouch LoginScreen2 = new LoginViewTouch();
            //            loginView LoginScreen = new loginView();
            //            //  LoginTouchWindow LoginScreen = new LoginTouchWindow();
            //            this.Close();

            //            LoginScreen.Show();
            //        }
            //    }
            //    else
            //    {
            //        ErrorHandlerModel.ErrorText = "Bitte stellen Sie eine Verbindung mit dem Internet her, damit Sie wærp.stockpilot nutzen können";
            //        ErrorHandlerModel.ErrorType = "NOTALLOWED";
            //        ErrorWindow showError = new ErrorWindow();
            //        showError.ShowDialog();
            //    }

            //}






            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataFolderPath, "waerp-stockpilot");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                string subfolder1Path = Path.Combine(folderPath, "Bestellungen");
                Directory.CreateDirectory(subfolder1Path);

                string subfolder2Path = Path.Combine(folderPath, "SQL_Error_Logs");
                Directory.CreateDirectory(subfolder2Path);

                string subfolder3Path = Path.Combine(folderPath, "Error_Logs");
                Directory.CreateDirectory(subfolder3Path);

                string subfolder4Path = Path.Combine(folderPath, "History_Files");
                Directory.CreateDirectory(subfolder4Path);
            }



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

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
