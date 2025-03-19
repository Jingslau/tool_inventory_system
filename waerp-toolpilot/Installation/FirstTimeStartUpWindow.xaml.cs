using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using MySqlConnector;
using PdfSharp.Drawing;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.Installation;
using waerp_toolpilot.License;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.FirstTimeStartup
{
    /// <summary>
    /// Interaction logic for FirstTimeStartUpWindow.xaml
    /// </summary>
    public partial class FirstTimeStartUpWindow : Window
    {
        static class InstallationSave
        {

            static string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            static string folderPath = Path.Combine(appDataFolderPath, "toolpilot");
            static string standardOrderPath = Path.Combine(folderPath, "Orderfiles");
            static string standardHistoryPath = Path.Combine(folderPath, "History_Files");
            static string standardProtocolPath = Path.Combine(folderPath, "Protocol");
            static string standardImagePath = Path.Combine(folderPath, "Images");


            //License Infos
            public static bool hasAcceptedLicense = false;
            public static bool licenseValid = false;
            public static bool licenseActivated = false;
            public static bool jumpToFinish = true;
            public static string licenseKey = "";

            //Database Settings
            public static string dbServer = "";
            public static string dbUser = "";
            public static string dbPassword = "";
            public static string dbSheme = "";
            public static bool connectionValid = false;
            public static bool connectionSaved = false;
            public static bool companyInformaitonsSet = true;

            //Paths
            public static string orderPath = standardOrderPath;
            public static string historyPath = standardHistoryPath;
            public static string ControlDocPath = standardProtocolPath;
            public static string imagePath = standardImagePath;

            //Standard Values
            public static string companyName = "";
            public static string companyAdress = "";
            public static string companyCity = "";
            public static string companyZip = "";
            public static string companyMail = "";
            public static string companyPhone = "";
            public static string standardDecimal = "";
            public static int standardCurrencyID;
            public static string standardCurrencyName = "";

            public static string errorEmail = "";
            public static string orderEmail = "";

            //Email Connection Info
            public static string smtpURL = "";
            public static string smtpPort = "";
            public static string smtpPassword = "";



            //Printinformation
            public static string leftMargin = "";
            public static string rightMargin = "";
            public static string topMargin = "";
            public static string bottomMargin = "";

            public static string letterPath = "";
            public static bool emptyLetter = true;

            //Input Control setting
            public static bool isTouch = false;

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


        public static int currentStep = 0;
        public FirstTimeStartUpWindow()
        {
            InitializeComponent();
            if (!CheckForInternetConnection())
            {
                ErrorHandlerModel.ErrorText = "Bitte verbinden Sie ihr Gerät mit dem Internet. Eine aktive Verbindung mit dem Internet ist notwendig, damit Sie das Programm installieren und aktivieren können!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
                System.Windows.Application.Current.Shutdown();
            }
            else
            {

                ImportLicenseText();
                AcceptLicenseCheckbox.IsEnabled = false;
                CurrentUserAdministrationModel.ShowSendReportBtn = false;
                //DataSet ds = ItemAdministrationQueries.RunSql("SELECT * FROM currency");

                //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                //{
                //    CurrencyCombobox.Items.Add(ds.Tables[0].Rows[j]["currency_code"].ToString());
                //}
                //CurrencyCombobox.SelectedIndex = 1;
            }


        }

        private void CancleInstallationBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\toolpilot");
            }
            catch { }
            System.Windows.Application.Current.Shutdown();
        }

        private void StartNextBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStep++;
            InstallationTabs.SelectedIndex = currentStep;

        }

        private void LicenseNextBtn_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\toolpilot", true);
            currentStep++;
            InstallationTabs.SelectedIndex = currentStep;

        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                AcceptLicenseCheckbox.IsEnabled = true;
            }
            else
            {
                AcceptLicenseCheckbox.IsEnabled = false;
            }

        }

        private void AcceptLicenseCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            InstallationSave.hasAcceptedLicense = true;
            LicenseNextBtn.IsEnabled = true;
        }

        private void AcceptLicenseCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            InstallationSave.hasAcceptedLicense = false;
            LicenseNextBtn.IsEnabled = false;
        }

        private void ImportLicenseText()
        {
            LicenseText.Text = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   \r\n\r\nUt wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.   \r\n\r\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis.   \r\n\r\nAt vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua. est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat.   \r\n\r\nConsetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus.   \r\n\r\nLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   \r\n\r\nUt wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.   \r\n\r\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis.   \r\n\r\nAt vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua. est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat.   \r\n\r\nConsetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus.   \r\n\r\nLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   \r\n\r\nUt wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.   \r\n\r\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis.   \r\n\r\nAt vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua. est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat.   \r\n\r\nConsetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus.   \r\n\r\nLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   \r\n\r\nUt wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.   \r\n\r\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis.   \r\n\r\nAt vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua. est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat.   \r\n\r\nConsetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus.   \r\n\r\nLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.   \r\n\r\nUt wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.   \r\n\r\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.   \r\n\r\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis.  ";
        }

        private void InstallationTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InstallationTabs.SelectedIndex = currentStep;
        }

        private void LicenseActivationNextBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStep++;
            InstallationTabs.SelectedIndex = currentStep;
        }

        private void LicenseActivationBtn_Click(object sender, RoutedEventArgs e)
        {


            if (licenseField1.Text.Length == 5 && licenseField2.Text.Length == 5 && licenseField3.Text.Length == 5 && licenseField4.Text.Length == 5 && licenseField5.Text.Length == 5)
            {
                InstallationSave.licenseKey = licenseField1.Text + "-" + licenseField2.Text + "-" + licenseField3.Text + "-" + licenseField4.Text + "-" + licenseField5.Text;
                if (LicenseServerConnector.ActivateLicense(InstallationSave.licenseKey))
                {
                    Color color = (Color)ColorConverter.ConvertFromString("#40B052");
                    Brush brush = new SolidColorBrush(color);
                    LicenseKeyInputBorder.BorderBrush = brush;
                    LicenseKeyInputBorder.BorderThickness = new Thickness(5);

                    InstallationSave.licenseValid = true;
                    InstallationSave.licenseActivated = true;
                    LicenseActivationNextBtn.IsEnabled = true;
                }

                else
                {
                    Color color = (Color)ColorConverter.ConvertFromString("#A02619");
                    Brush brush = new SolidColorBrush(color);
                    LicenseKeyInputBorder.BorderBrush = brush;
                    LicenseKeyInputBorder.BorderThickness = new Thickness(5);

                    InstallationSave.licenseValid = false;
                    LicenseActivationNextBtn.IsEnabled = false;
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie zuerst einen gültigen Lizenzschlüssel ein!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }


        }

        private void DatabaseNextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (InstallationSave.companyInformaitonsSet)
            {
                StandardValues.IsEnabled = false;
                SaveLoations.IsEnabled = false;
                PrintInformations.IsEnabled = false;

                currentStep = 7;
                InstallationTabs.SelectedIndex = 7;
            }
            else
            {
                if (InstallationSave.jumpToFinish)
                {
                    currentStep = 7;
                    InstallationTabs.SelectedIndex = 7;
                }
                else
                {
                    currentStep++;
                    InstallationTabs.SelectedIndex = currentStep;
                }
            }


        }

        private void FolderNextBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStep++;
            InstallationTabs.SelectedIndex = currentStep;
        }

        private void StandardValuesNextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (
                companyName.Text == "" |
                companyAdress.Text == "" |
                companyCity.Text == "" |
                companyZIP.Text == "" |
                companyPhone.Text == "" |
                companyMail.Text == "" |
                wrongLocationMail.Text == "" |
                orderMail.Text == "" |
                CurrencyCombobox.SelectedIndex == -1
                )
            {
                ErrorHandlerModel.ErrorText = "Es müssen alle Felder ausgefüllt sein, damit Sie mit der Installation fortsetzen können.";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }
            else
            {
                InstallationSave.companyName = companyName.Text;
                InstallationSave.companyAdress = companyAdress.Text;
                InstallationSave.companyCity = companyCity.Text;
                InstallationSave.companyZip = companyZIP.Text;
                InstallationSave.companyPhone = companyPhone.Text;
                InstallationSave.companyMail = companyMail.Text;
                InstallationSave.errorEmail = wrongLocationMail.Text;
                InstallationSave.orderEmail = orderMail.Text;
                InstallationSave.standardCurrencyID = CurrencyCombobox.SelectedIndex + 1;
                InstallationSave.standardCurrencyName = CurrencyCombobox.SelectedItem.ToString();
                currentStep++;
                InstallationTabs.SelectedIndex = currentStep;
            }

        }

        private void PrintValuesNextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (marginLeft.Text == "" | marginRight.Text == "" | marginTop.Text == "" | marginBottom.Text == "")
            {
                ErrorHandlerModel.ErrorText = "Bitte füllen alle Randabstände aus um fortzufahren!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }
            else
            {
                if (NoBackletterCheckbox.IsChecked == false && PDFFilePath.Text == "")
                {
                    ErrorHandlerModel.ErrorText = "Bitte geben Sie ein Dokument als Briefpapier an oder klicken Sie auf ,,Kein BriefPaper''!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow openError = new ErrorWindow();
                    openError.ShowDialog();
                }
                else
                {
                    InstallationSave.leftMargin = marginLeft.Text;
                    InstallationSave.rightMargin = marginRight.Text;
                    InstallationSave.topMargin = marginTop.Text;
                    InstallationSave.bottomMargin = marginBottom.Text;
                    InstallationSave.letterPath = PDFFilePath.Text;
                    currentStep++;
                    InstallationTabs.SelectedIndex = currentStep;
                }
            }
        }

        private void ExitInstallationBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            SaveValues.IsEnabled = false;
            SuccessText.Visibility = Visibility.Hidden;
            SuccessIcon.Visibility = Visibility.Hidden;
            MySqlConnection connNew = new MySqlConnection($"Server={ServerAdress.Text};userid={DatabaseUser.Text};password={DatabasePassword.Password}");
            MySqlCommand cmd = new MySqlCommand();
            //Test 1 DB Connection

            bool check2 = false;
            bool check3 = false;
            try
            {
                connNew.Open();
                cmd = new MySqlCommand("SELECT 1", connNew);
                cmd.ExecuteNonQuery();
                DatabaseConnectionStatus.Text = "Status: Verbindung zur Datenbank war erfolgreich!";
                ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                check2 = true;
            }
            catch (MySqlException ex)
            {
                //   ErrorLogger.LogSqlError(ex);
                if (ex.Number == (int)MySqlErrorCode.AccessDenied)
                {
                    DatabaseConnectionStatus.Text = "Status: Die Logindaten sind ungültig!";
                    ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                    SaveValues.IsEnabled = false;
                }
                else
                {
                    DatabaseConnectionStatus.Text = "Status: Die Serveradresse ist ungültig!";
                    ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                    SaveValues.IsEnabled = false;
                }
            }
            finally
            {
                connNew.Close();
            }





            if (check2 == true)
            {
                try
                {
                    connNew.Open();
                    cmd = new MySqlCommand("SHOW DATABASES", connNew);
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adp.Fill(ds);
                    List<string> strDetailIDList = new List<string>();
                    bool check = false;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row[0].ToString() == ServerSchema.Text)
                        {
                            check = true; break;
                        }
                    }
                    if (!check)
                    {
                        DatabaseSchemaStatus.Text = $"Status: Das Schema {ServerSchema.Text} konnte nicht gefunden werden!";
                        ValidationSchemaBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                        SaveValues.IsEnabled = false;
                    }
                    else
                    {
                        DatabaseSchemaStatus.Text = $"Status: Das Schema {ServerSchema.Text} existiert in der Datenbank!";
                        ValidationSchemaBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                        check3 = true;
                    }
                }
                catch (MySqlException ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
                finally
                {
                    connNew.Close();
                }

                if (check3 == true)
                {
                    try
                    {
                        connNew = new MySqlConnection($"Server={ServerAdress.Text};userid={DatabaseUser.Text};password={DatabasePassword.Password};Database={ServerSchema.Text}");
                        connNew.Open();
                        cmd = new MySqlCommand("SHOW TABLES", connNew);
                        MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        List<string> strDetailIDList = new List<string>();

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            strDetailIDList.Add(row[0].ToString());
                        }

                        String[] tmpArr = new string[strDetailIDList.Count];
                        for (int i = 0; i < strDetailIDList.Count; i++)
                        {
                            tmpArr[i] = strDetailIDList[i].ToString();
                        }

                        string neededTables = Database.Default.dbTablesDefault.ToString();
                        neededTables = neededTables.Replace(" ", "");
                        string[] neededTablesArr = neededTables.Split(';');

                        int counter = 0;

                        for (int i = 0; i < tmpArr.Length; i++)
                        {
                            for (int j = 0; j < neededTablesArr.Length; j++)
                            {
                                if (tmpArr[i].Equals(neededTablesArr[j]))
                                {
                                    counter++;
                                }
                            }
                        }

                        if (counter < 0)
                        {
                            ConfigureDB.Visibility = Visibility.Visible;


                            DatabaseTablesStatus.Text = $"Status: Die Tabellen des Schemas {ServerSchema.Text} sind nicht korrekt oder vollständig!";
                            ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                            SaveValues.IsEnabled = false;

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                InfoAlertText.Text = "Bitte löschen Sie erst alle Tabellen um eine neue Datenbank anzulegen!";
                                ConfigureDB.IsEnabled = false;

                            }
                            else
                            {
                                InfoAlertText.Text = "";
                                ConfigureDB.IsEnabled = true;
                            }

                            SuccessInfo.Visibility = Visibility.Hidden;
                            NewDatabaseInfo.Visibility = Visibility.Visible;


                        }
                        else
                        {
                            SuccessInfo.Visibility = Visibility.Visible;
                            NewDatabaseInfo.Visibility = Visibility.Hidden;
                            ConfigureDB.IsEnabled = false;
                            cmd = new MySqlCommand("SELECT * FROM dbinfo", connNew);
                            adp = new MySqlDataAdapter(cmd);
                            ds = new DataSet();
                            adp.Fill(ds);
                            if (dbUpdate.IsCurrentDBVersionValidInstallation(ds.Tables[0].Rows[0][2].ToString()))
                            {
                                DatabaseTablesStatus.Text = $"Status: Das Schema {ServerSchema.Text} besitzt alle Tabellen und hat die richtige Datenbankversion!";
                                ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                                SaveValues.IsEnabled = true;


                                cmd = new MySqlCommand("SELECT * FROM company_settings", connNew);
                                MySqlDataAdapter adp2 = new MySqlDataAdapter(cmd);
                                DataSet CompanyInformation = new DataSet();

                                adp2.Fill(CompanyInformation);

                                for (int i = 0; i < 4; i++)
                                {
                                    if (CompanyInformation.Tables[0].Rows[i][1].ToString() == "") { InstallationSave.companyInformaitonsSet = false; }
                                }

                            }
                            else
                            {
                                DatabaseTablesStatus.Text = $"Status: Das Schema {ServerSchema.Text} hat die falsche Datenbankversion!";
                                ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                                SaveValues.IsEnabled = false;
                            }

                        }





                    }
                    catch (MySqlException)
                    {

                        DatabaseTablesStatus.Text = $"Status: Es konnte keine Verbindung zur Datenbank hergestellt werden!";
                        ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");

                    }
                    finally
                    {
                        connNew.Close();
                    }

                }
                else
                {
                    DatabaseTablesStatus.Text = "Status: Das angegebene Schema konnte nicht gefunden werden!";
                    ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#8C8C8C");
                }


            }
            else
            {
                DatabaseSchemaStatus.Text = "Status: Es konnte keine Verbindung zur Datenbank hergestellt werden!";
                ValidationSchemaBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#8C8C8C");
                DatabaseTablesStatus.Text = "Status: Es konnte keine Verbindung zur Datenbank hergestellt werden!";
                ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#8C8C8C");
            }




        }

        private void SaveValues_Click(object sender, RoutedEventArgs e)
        {
            //SqlConn.SetConnectionString(ServerAdress.Text, DatabaseUser.Text, DatabasePassword.Password, ServerSchema.Text);
            InstallationSave.dbServer = ServerAdress.Text;
            InstallationSave.dbUser = DatabaseUser.Text;
            InstallationSave.dbPassword = DatabasePassword.Password;
            InstallationSave.dbSheme = ServerSchema.Text;

            SuccessText.Visibility = Visibility.Visible;
            SuccessIcon.Visibility = Visibility.Visible;

            MySqlConnection connNew = new MySqlConnection($"Server={ServerAdress.Text};userid={DatabaseUser.Text};password={DatabasePassword.Password};Database={ServerSchema.Text}");
            connNew.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM company_settings", connNew);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i][2].ToString() == "")
                {
                    InstallationSave.jumpToFinish = false;
                }
            }

            cmd = new MySqlCommand("SELECT * FROM currency", connNew);
            adp = new MySqlDataAdapter(cmd);
            ds = new DataSet();
            adp.Fill(ds);
            CurrencyCombobox.Items.Clear();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                CurrencyCombobox.Items.Add(ds.Tables[0].Rows[i]["currency_code"].ToString());
            }

            connNew.Close();
            DatabaseNextBtn.IsEnabled = true;
        }

        private void selectImageFolderPath(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    InstallationSave.imagePath = dialog.SelectedPath;
                    imageFolderPath.Text = dialog.SelectedPath;
                }
            }
        }
        private void selectOrderPath(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    InstallationSave.orderPath = dialog.SelectedPath;
                    orderPathText.Text = dialog.SelectedPath;
                }
            }
        }

        private void selectHistoryPath(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    InstallationSave.historyPath = dialog.SelectedPath;
                    historyPathText.Text = dialog.SelectedPath;
                }
            }
        }

        private string ValidateString(string margin)
        {
            string result = "";
            char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' }; // these are ok

            bool isComma = false;


            if (margin[0] == ',' | margin.Length == 0)
            {
                return "0";
            }

            foreach (char c in margin) // check each character in the user's input
            {
                if (c == ',' && isComma == true)
                {
                    return margin.Remove(margin.Length - 1);
                }
                if (c == ',')
                {
                    isComma = true;
                }

                if (Array.IndexOf(validChars, c) != -1)
                {
                    result += c; // if this is ok, then add it to the result
                }

            }

            return result; // change the text to the "clean" version where illegal chars have been removed.

        }

        private void marginTop_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            if (marginTop.Text.Length == 0)
            {
                marginTop.Text = "0";
            }
            else
            {
                StringBuilder sb = new StringBuilder(ValidateString(marginTop.Text));
                marginTop.Text = sb.ToString();
                for (int i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == ',')
                    {
                        sb[i] = '.';
                    }
                }
                ChangeMargin(sb.ToString(), "top");
            }

        }



        private void marginLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (marginLeft.Text.Length == 0)
            {
                marginLeft.Text = "0";
            }
            else
            {
                StringBuilder sb = new StringBuilder(ValidateString(marginLeft.Text));
                marginLeft.Text = sb.ToString();
                for (int i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == ',')
                    {
                        sb[i] = '.';
                    }
                }
                ChangeMargin(sb.ToString(), "left");
            }
        }
        private void marginRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (marginRight.Text.Length == 0)
            {
                marginRight.Text = "0";
            }
            else
            {
                StringBuilder sb = new StringBuilder(ValidateString(marginRight.Text));
                marginRight.Text = sb.ToString();
                for (int i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == ',')
                    {
                        sb[i] = '.';
                    }
                }
                ChangeMargin(sb.ToString(), "right");
            }
        }

        private void marginBottom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (marginBottom.Text.Length == 0)
            {
                marginBottom.Text = "0";
            }
            else
            {
                StringBuilder sb = new StringBuilder(ValidateString(marginBottom.Text));
                marginBottom.Text = sb.ToString();
                for (int i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == ',')
                    {
                        sb[i] = '.';
                    }
                }
                ChangeMargin(sb.ToString(), "bottom");
            }
        }


        private void FinishInstalaltionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StartApplicationNowCheckbox.IsChecked == true)
            {
                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStep--;
            InstallationTabs.SelectedIndex = currentStep;
        }

        private void FinishInstallationBtn_Click(object sender, RoutedEventArgs e)
        {

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            //Company Data

            key.SetValue("CompanyName", InstallationSave.companyName.ToString());
            key.SetValue("CompanyAdress", InstallationSave.companyAdress.ToString());
            key.SetValue("CompanyCity", InstallationSave.companyCity.ToString());
            key.SetValue("CompanyZIP", InstallationSave.companyZip.ToString());
            key.SetValue("CompanyPhone", InstallationSave.companyPhone.ToString());
            key.SetValue("CompanyMail", InstallationSave.companyMail.ToString());
            key.SetValue("RemindMeLaterDate", "");

            //Automated Reports Mails
            key.SetValue("ErrorLocationMail", InstallationSave.errorEmail.ToString());
            key.SetValue("OrderMail", InstallationSave.orderEmail.ToString());

            //Save Locations
            key.SetValue("HistoryLogsPath", InstallationSave.historyPath.ToString());
            key.SetValue("OrderOverviewPath", InstallationSave.orderPath.ToString());

            //Settings Locations
            key.SetValue("LetterPaperPath", InstallationSave.letterPath.ToString());

            //General Settings
            key.SetValue("DecimalSign", InstallationSave.standardDecimal.ToString());
            key.SetValue("StandardCurrency", InstallationSave.standardCurrencyName.ToString());
            key.SetValue("IsTouch", bool.Parse(InstallationSave.isTouch.ToString()));

            //PDF Letter Settings
            key.SetValue("MarginTop", InstallationSave.topMargin.ToString());
            key.SetValue("MarginBottom", InstallationSave.bottomMargin.ToString());
            key.SetValue("MarginLeft", InstallationSave.leftMargin.ToString());
            key.SetValue("MarginRight", InstallationSave.rightMargin.ToString());

            //Database Settings
            key.SetValue("DBServer", InstallationSave.dbServer.ToString());
            key.SetValue("DBUser", InstallationSave.dbUser.ToString());
            key.SetValue("DBPassword", InstallationSave.dbPassword.ToString());
            key.SetValue("DBSchema", InstallationSave.dbSheme.ToString());

            //Programm Parameters
            key.SetValue("CH", SystemHardwareReader.GetSystemHash());
            key.SetValue("checkIn", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            key.SetValue("hasLicense", true);
            key.Close();
            SqlConn.SetConnectionString(InstallationSave.dbServer.ToString(), InstallationSave.dbUser.ToString(), InstallationSave.dbPassword.ToString(), InstallationSave.dbSheme.ToString());

            if (!InstallationSave.jumpToFinish)
            {
                if (!InstallationSave.companyInformaitonsSet)
                {
                    AdministrationQueries.RunSqlExec($"UPDATE currency SET currency_isDefault = 0");
                    AdministrationQueries.RunSqlExec($"UPDATE currency SET currency_isDefault = 1 WHERE currency_id = {InstallationSave.standardCurrencyID}");


                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.companyName}' WHERE settings_name = 'company_name'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.companyAdress}' WHERE settings_name = 'company_adress'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.companyCity}' WHERE settings_name = 'company_city'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.companyZip}' WHERE settings_name = 'company_postcode'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.companyPhone}' WHERE settings_name = 'company_phone'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.companyMail}' WHERE settings_name = 'company_mail'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.smtpPassword}' WHERE settings_name = 'company_mail_password'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.smtpPort}' WHERE settings_name = 'company_mail_port'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.smtpURL}' WHERE settings_name = 'company_mail_host'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = 'ASPOK2139!9asdj?' WHERE settings_name = 'master_password'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '30' WHERE settings_name = 'max_rents'");




                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.errorEmail}' WHERE settings_name = 'global_stockerror_mail'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.orderEmail}' WHERE settings_name = 'global_order_mail'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.standardDecimal}' WHERE settings_name = 'global_decimal'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.standardCurrencyName}' WHERE settings_name = 'global_currency'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.historyPath.Replace("\\", "\\\\")}' WHERE settings_name = 'global_history_path'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.letterPath.Replace("\\", "\\\\")}' WHERE settings_name = 'global_letterpaper_path'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.orderPath.Replace("\\", "\\\\")}' WHERE settings_name = 'global_orderdoc_path'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.ControlDocPath.Replace("\\", "\\\\")}' WHERE settings_name = 'global_measure_equip_protocol_path'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.imagePath.Replace("\\", "\\\\")}' WHERE settings_name = 'global_image_path'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '4' WHERE settings_name = 'global_culture_id'");

                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.topMargin}' WHERE settings_name = 'margin_top'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.bottomMargin}' WHERE settings_name = 'margin_bottom'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.leftMargin}' WHERE settings_name = 'margin_left'");
                    AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{InstallationSave.rightMargin}' WHERE settings_name = 'margin_right'");
                }
            }
            currentStep++;
            CurrentUserAdministrationModel.ShowSendReportBtn = true;
            InstallationTabs.SelectedIndex = currentStep;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InstallationSave.isTouch = true;
            Color color = (Color)ColorConverter.ConvertFromString("#A02619"); // Replace with your desired hex color
            Brush brush = new SolidColorBrush(color);
            SelectTouchBorder.BorderBrush = brush;
            SelectTouchBorder.BorderThickness = new Thickness(2);

            color = (Color)ColorConverter.ConvertFromString("#A7A7A7"); // Replace with your desired hex color
            brush = new SolidColorBrush(color);
            SelectMandKInputBorder.BorderBrush = brush;
            SelectMandKInputBorder.BorderThickness = new Thickness(2);

        }

        private void SelectMandKInputBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InstallationSave.isTouch = false;
            Color color = (Color)ColorConverter.ConvertFromString("#A02619"); // Replace with your desired hex color
            Brush brush = new SolidColorBrush(color);
            SelectMandKInputBorder.BorderBrush = brush;
            SelectMandKInputBorder.BorderThickness = new Thickness(2);

            color = (Color)ColorConverter.ConvertFromString("#A7A7A7"); // Replace with your desired hex color
            brush = new SolidColorBrush(color);
            SelectTouchBorder.BorderBrush = brush;
            SelectTouchBorder.BorderThickness = new Thickness(2);

        }

        private void selectPdfDocument(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "PDF Dokument|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {

                PDFFilePath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void CurrencyCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InstallationSave.standardCurrencyID = CurrencyCombobox.SelectedIndex;
            InstallationSave.standardCurrencyName = CurrencyCombobox.Text;
        }

        private void isComma_Checked(object sender, RoutedEventArgs e)
        {
            InstallationSave.standardDecimal = ",";
        }


        private void isPoint_Checked(object sender, RoutedEventArgs e)
        {
            InstallationSave.standardDecimal = ".";
        }

        private void NoBackletterCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SelectFileBtn.IsEnabled = false;
        }

        private void NoBackletterCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectFileBtn.IsEnabled = true;
        }

        private void licenseField1_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void licenseField1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField1.Text.Length == 5)
            {
                licenseField2.Focus();
            }
        }

        private void licenseField2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField2.Text.Length == 5)
            {
                licenseField3.Focus();
            }
        }

        private void licenseField3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField3.Text.Length == 5)
            {
                licenseField4.Focus();
            }
        }

        private void licenseField4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField4.Text.Length == 5)
            {
                licenseField5.Focus();
            }
        }

        private void companyMail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex emailRegex = new Regex(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$");
            string newText = companyMail.Text + e.Text;

            // Check if the new text matches the email pattern or if the added character is a backspace
            if (!emailRegex.IsMatch(newText) && e.Text != "\b")
                e.Handled = true;
        }

        private void companyZIP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex numericRegex = new Regex(@"^\d+$");

            // Check if the new text is numeric or if the added character is a backspace
            if (!numericRegex.IsMatch(e.Text) && e.Text != "\b")
                e.Handled = true;
        }





        private void marginLeft_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex numericRegex = new Regex(@"^\d+$");

            // Check if the new text is numeric or if the added character is a backspace
            if (!numericRegex.IsMatch(e.Text) && e.Text != "\b")
                e.Handled = true;
        }
        private void ChangeMargin(string input, string position)
        {
            if (input != "")
            {
                double scaleFactor = 2.052675862068966;
                int originMarginTopBottom = 11;
                int originMarginSide = 12;
                if (position == "top")
                {
                    double MarginTop = double.Parse(input) / scaleFactor;
                    double MarginTopPt = XUnit.FromMillimeter(MarginTop);
                    TopMarginBar.Margin = new Thickness(0, originMarginTopBottom + MarginTopPt, 0, 0);
                }
                else if (position == "left")
                {
                    double MarginLeft = double.Parse(input) / scaleFactor;
                    double MarginLeftPt = XUnit.FromMillimeter(MarginLeft);
                    LeftMarginBar.Margin = new Thickness(originMarginSide + MarginLeftPt, 0, 0, 0);
                }
                else if (position == "right")
                {
                    double MarginRight = double.Parse(input) / scaleFactor;
                    double MarginRightPt = XUnit.FromMillimeter(MarginRight);
                    RightMarginBar.Margin = new Thickness(0, 0, originMarginSide + MarginRightPt, 0);
                }
                else if (position == "bottom")
                {
                    double MarginBottom = double.Parse(input) / scaleFactor;
                    double MarginBottomPt = XUnit.FromMillimeter(MarginBottom);
                    BottomMarginBar.Margin = new Thickness(0, 0, 0, originMarginTopBottom + MarginBottomPt);
                }
            }
        }

        private double GetMargin(string input, string position)
        {
            double scaleFactor = 2.052675862068966;
            int originMarginTopBottom = 11;
            int originMarginSide = 12;
            if (position == "top")
            {
                double MarginTop = double.Parse(input) / scaleFactor;
                double MarginTopPt = XUnit.FromMillimeter(MarginTop);
                return originMarginTopBottom + MarginTopPt;
            }
            else if (position == "left")
            {
                double MarginLeft = double.Parse(input) / scaleFactor;
                double MarginLeftPt = XUnit.FromMillimeter(MarginLeft);
                return originMarginSide + MarginLeftPt;
            }
            else if (position == "right")
            {
                double MarginRight = double.Parse(input) / scaleFactor;
                double MarginRightPt = XUnit.FromMillimeter(MarginRight);
                return originMarginSide + MarginRightPt;
            }
            else if (position == "bottom")
            {
                double MarginBottom = double.Parse(input) / scaleFactor;
                double MarginBottomPt = XUnit.FromMillimeter(MarginBottom);
                return originMarginTopBottom + MarginBottomPt;
            }
            else
            {
                return 0;
            }
        }

        private void TestSMTPConn_Click(object sender, RoutedEventArgs e)
        {
            if (companyMailPassword.Password != "")
            {
                string emailAddress = companyMail.Text;
                string password = companyMailPassword.Password;
                string host = smtpHost.Text;
                int port = int.Parse(smtpPort.Text); // Change the port number if necessary

                try
                {
                    using (var client = new SmtpClient(host, port))
                    {
                        client.Credentials = new NetworkCredential(emailAddress, password);
                        client.EnableSsl = true; // Enable SSL/TLS encryption if required

                        client.Send(new MailMessage(emailAddress, emailAddress, "Connection Test", "This is a connection test."));
                        SuccessInfoMail.Visibility = Visibility.Visible;
                        ErrorInfoMail.Visibility = Visibility.Hidden;
                        StandardValuesNextBtn.IsEnabled = true;
                        InstallationSave.smtpURL = host;
                        InstallationSave.smtpPassword = password;
                        InstallationSave.smtpPort = port.ToString();

                    }
                }
                catch (Exception)
                {
                    SuccessInfoMail.Visibility = Visibility.Hidden;
                    ErrorInfoMail.Visibility = Visibility.Visible;
                    StandardValuesNextBtn.IsEnabled = false;
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie ein Passwort ein!!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }
        }
        private void SaveSMTPConn_Click(object sender, RoutedEventArgs e)
        {

            AdministrationQueries.RunSql($"UPDATE company_settings SET settings_value = '{smtpHost.Text}' WHERE settings_name = 'company_mail_host'");
            AdministrationQueries.RunSql($"UPDATE company_settings SET settings_value = '{smtpPort.Text}' WHERE settings_name = 'company_mail_port'");
            AdministrationQueries.RunSql($"UPDATE company_settings SET settings_value = '{companyMailPassword.Password}' WHERE settings_name = 'company_mail_password'");

            ErrorHandlerModel.ErrorText = "Die Einstellungen wurden erfolgreich in der Datenbank gespeichert!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();

            StandardValuesNextBtn.IsEnabled = true;



        }

        private void ConfigureDB_Click(object sender, RoutedEventArgs e)
        {
            InstallationStore.ServerSchema = ServerSchema.Text;
            InstallationStore.ServerAdress = ServerAdress.Text;
            InstallationStore.DatabaseUser = DatabaseUser.Text;
            InstallationStore.DatabasePassword = DatabasePassword.Password;
            if (dbSetup.InitNewDBInstallation())
            {
                ConfigureDB.IsEnabled = false;
                InfoAlertText.Visibility = Visibility.Hidden;
                InfoText.Text = "Bitte klicken Sie erneut auf Verbindung testen und dann auf Werte speichern!";
            }
            else
            {
                InfoAlertText.Visibility = Visibility.Hidden;
                InfoText.Text = "Es ist ein Fehler aufgetreten!";
            }



        }

        private void ControlBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    InstallationSave.ControlDocPath = dialog.SelectedPath;
                    ControlText.Text = dialog.SelectedPath;
                }
            }
        }
    }
}


