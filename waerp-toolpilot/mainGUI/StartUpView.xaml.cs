using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.config.DatabaseSettingsView;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.FirstTimeStartup;
using waerp_toolpilot.License;
using waerp_toolpilot.loginHandling;
using waerp_toolpilot.LoginTouch;


namespace waerp_toolpilot.main
{
    /// <summary>
    /// Interaktionslogik für StartUpView.xaml
    /// </summary>
    public partial class StartUpView : Window
    {
        public StartUpView()
        {
            InitializeComponent();
            Show();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key == null)
            {
                Close();
            }
            else
            {
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                if (bool.Parse(key.GetValue("IsTouch").ToString()))
                {

                    key.Close();
                    LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                    Close();
                    LoginWindowTouch.Show();
                }
                else
                {

                    key.Close();
                    loginView LoginScreen = new loginView();
                    Close();
                    LoginScreen.Show();
                }
            }

            // StartUpRoutine();
        }

        private bool CheckPoint_Folders()
        {
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataFolderPath, "toolpilot");

            if (!Directory.Exists(folderPath))
            {
                //General Programfunction Folders
                Directory.CreateDirectory(folderPath);
                string subfolder1Path = Path.Combine(folderPath, "Bestellungen");
                Directory.CreateDirectory(subfolder1Path);




                string subfolder2Path = Path.Combine(folderPath, "SQL_Error_Logs");
                Directory.CreateDirectory(subfolder2Path);

                string subfolder3Path = Path.Combine(folderPath, "Error_Logs");
                Directory.CreateDirectory(subfolder3Path);

                string subfolder4Path = Path.Combine(folderPath, "History_Files");
                Directory.CreateDirectory(subfolder4Path);

                string subfolder5Path = Path.Combine(folderPath, "ApplicationFiles");
                Directory.CreateDirectory(subfolder5Path);

                string subfolder6Path = Path.Combine(folderPath, "logs");
                Directory.CreateDirectory(subfolder6Path);

                string sqlErrorLog = Path.Combine(subfolder6Path, "sql-error.log");
                string systemErrorLog = Path.Combine(subfolder6Path, "system-Error.log");

                File.Create(sqlErrorLog);
                File.Create(systemErrorLog);

            }
            return true;
        }

        private bool Checkpoint_SubKey()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key == null)
            {

                Registry.CurrentUser.CreateSubKey(@"SOFTWARE\toolpilot", true);
                FirstTimeStartUpWindow openStart = new FirstTimeStartUpWindow();
                openStart.ShowDialog();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void Checkpoint_License()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key.GetValue("hasLicense").ToString() == "True")
            {
                DateTime thisDay = DateTime.Now;
                DateTime lastCheckin = DateTime.Parse(key.GetValue("checkIn").ToString());
                int timeDifference = (lastCheckin - thisDay).Days;

                if (timeDifference < 10 && timeDifference >= 8)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("cbString3");
                    ErrorHandlerModel.ErrorType = "INFO";
                    ErrorWindow showInfo = new ErrorWindow();
                    showInfo.ShowDialog();

                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("cbString4");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
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


        public static bool CheckDBConnection()
        {
            //Test 1 DB Connection
            MySqlConnection conn = new MySqlConnection($"Server={Database.Default.dbServer};userid={Database.Default.dbUser};password={Database.Default.dbPassword};Database={Database.Default.dbSchema}");
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd = new MySqlCommand("SELECT 1", conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException)
            {
                ErrorHandlerModel.ErrorText = "Die Datenbankeinstellungen sind nicht korrekt! Bitte geben Sie die korrkten Daten ein und starten Sie die Anwendung neu!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
                DatabaseSettingsView openSettings = new DatabaseSettingsView();
                openSettings.ShowDialog();
                return false;
            }
            finally
            {
                conn.Close();
            }





        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key == null)
            {
                Close();
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerAsync();
            }

        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckPoint_Folders();
            (sender as BackgroundWorker).ReportProgress(0);


            //StartUpRoutine();
            (sender as BackgroundWorker).ReportProgress(100);

        }

        //Diese Funktion ist theoretisch obsolet
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;

        }


        private void StartUpRoutine()
        {
            Dispatcher.Invoke(() => { ProgressText.Text = (string)FindResource("cbString6"); });


            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataFolderPath, "toolpilot");

            if (!Directory.Exists(folderPath))
            {

                Dispatcher.Invoke(() => { ProgressText.Text = (string)FindResource("cbString7"); });
                //General Programfunction Folders
                Directory.CreateDirectory(folderPath);
                string subfolder1Path = Path.Combine(folderPath, "Bestellungen");
                Directory.CreateDirectory(subfolder1Path);

                string subfolder2Path = Path.Combine(folderPath, "SQL_Error_Logs");
                Directory.CreateDirectory(subfolder2Path);

                string subfolder3Path = Path.Combine(folderPath, "Error_Logs");
                Directory.CreateDirectory(subfolder3Path);

                string subfolder4Path = Path.Combine(folderPath, "History_Files");
                Directory.CreateDirectory(subfolder4Path);

                string subfolder5Path = Path.Combine(folderPath, "ApplicationFiles");
                Directory.CreateDirectory(subfolder5Path);
            }

            Dispatcher.Invoke(() => { ProgressText.Text = (string)FindResource("cbString8"); });
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key == null)
            {
                Dispatcher.Invoke(() => { ProgressText.Text = (string)FindResource("cbString9"); });
                Dispatcher.Invoke(() =>
                {
                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\toolpilot", true);
                    FirstTimeStartUpWindow openStart = new FirstTimeStartUpWindow();
                    openStart.ShowDialog();
                });

            }
            else
            {

                Dispatcher.Invoke(() =>
                {
                    ProgressText.Text = "Datenbankversion wird abgegleichen";
                });

                Dispatcher.Invoke(() =>
                {
                    if (!dbUpdate.IsCurrentDBVersionValid())
                    {
                        dbUpdate.UpdateDatabase();
                    }
                });

                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                Dispatcher.Invoke(() => { ProgressText.Text = (string)FindResource("cbString14"); });
                bool test = true;
                //CheckForInternetConnection()
                if (!test)
                {
                    Dispatcher.Invoke(() => { ProgressText.Text = (string)FindResource("cbString10"); });

                    int licenseCheckReturn = LicenseServerConnector.CheckLicenseStartUp();

                    if (licenseCheckReturn == 1)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("cbString11");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        });




                        LicenseActivationWindow openLicenseActivator = new LicenseActivationWindow();
                        openLicenseActivator.ShowDialog();
                    }
                    else if (licenseCheckReturn == 2)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("cbString11");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                            LicenseActivationWindow openLicenseActivator = new LicenseActivationWindow();
                            openLicenseActivator.ShowDialog();
                        });

                    }
                    else if (licenseCheckReturn == 3)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("cbString11");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                            LicenseActivationWindow openLicenseActivator = new LicenseActivationWindow();
                            openLicenseActivator.ShowDialog();
                        });

                    }
                    else if (licenseCheckReturn == 4)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Registry.CurrentUser.CreateSubKey(@"SOFTWARE\toolpilot");
                            RegistryKey key2 = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                            key2.SetValue("hasLicense", false);
                            key2.Close();
                            SystemHardwareReader.WriteNewSystemHash();
                            ErrorHandlerModel.ErrorText = (string)FindResource("cbString11");
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                            LicenseActivationWindow openLicenseActivator = new LicenseActivationWindow();
                            openLicenseActivator.ShowDialog();
                        });

                    }

                    if (key.GetValue("hasLicense").ToString() == "True")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                            if (bool.Parse(key.GetValue("IsTouch").ToString()))
                            {

                                key.Close();
                                LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                                Close();
                                LoginWindowTouch.Show();
                            }
                            else
                            {

                                key.Close();
                                loginView LoginScreen = new loginView();
                                Close();
                                LoginScreen.Show();
                            }
                        });
                    }



                }
                else
                {
                    if (key.GetValue("hasLicense").ToString() == "True")
                    {
                        DateTime thisDay = DateTime.Now;
                        DateTime lastCheckin = DateTime.Parse(key.GetValue("checkIn").ToString());
                        int timeDifference = (lastCheckin - thisDay).Days;

                        if (timeDifference < 8)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                                if (bool.Parse(key.GetValue("IsTouch").ToString()))
                                {

                                    key.Close();
                                    LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                                    Close();
                                    LoginWindowTouch.Show();
                                }
                                else
                                {

                                    key.Close();
                                    loginView LoginScreen = new loginView();
                                    Close();
                                    LoginScreen.Show();
                                }
                            });


                        }
                        else if (timeDifference < 10 && timeDifference >= 8)
                        {
                            ErrorHandlerModel.ErrorText = (string)FindResource("cbString12");
                            ErrorHandlerModel.ErrorType = "INFO";
                            ErrorWindow showInfo = new ErrorWindow();
                            showInfo.ShowDialog();
                            Dispatcher.Invoke(() =>
                            {
                                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                                if (bool.Parse(key.GetValue("IsTouch").ToString()))
                                {

                                    key.Close();
                                    LoginTouchWindow LoginWindowTouch = new LoginTouchWindow();
                                    Close();
                                    LoginWindowTouch.Show();
                                }
                                else
                                {

                                    key.Close();
                                    loginView LoginScreen = new loginView();
                                    Close();
                                    LoginScreen.Show();
                                }
                            });
                        }
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("cbString13");
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }

                }


            }
        }
    }
}
