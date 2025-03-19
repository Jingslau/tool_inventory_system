using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Win32;
using MySqlConnector;
using PdfSharp.Drawing;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.Installation;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.config
{
    /// <summary>
    /// Interaction logic for UserSettingsWindow.xaml
    /// </summary>
    public partial class UserSettingsWindow : Window
    {
        public static bool touchSelected = false;
        public static string selectedDecimalSign = "";
        public UserSettingsWindow()
        {
            InitializeComponent();
            //  CurrentVersion.Text = "wærp.toolpilot Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            LoadSettings();
            //   DataSet settings = LoadGlobalSettings();





        }

        private void LoadSettings()
        {
            DataSet ds = ItemAdministrationQueries.RunSql("SELECT * FROM currency");
            int defaultIndexCurrency = -1;
            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
            {
                CurrencyCombobox.Items.Add(ds.Tables[0].Rows[j]["currency_code"].ToString());
                if (ds.Tables[0].Rows[j]["currency_isDefault"].ToString() == "True")
                {
                    defaultIndexCurrency = j;
                }
            }

            DataSet companySettings = AdministrationQueries.RunSql("SELECT * FROM company_settings");
            DataSet languages = AdministrationQueries.RunSql("SELECT * FROM culture_objects");

            for (int i = 0; i < languages.Tables[0].Rows.Count; i++)
            {
                //   languageCombobox.Items.Add(languages.Tables[0].Rows[i][2].ToString() + " - " + languages.Tables[0].Rows[i][1].ToString());
            }

            // languageCombobox.SelectedIndex = int.Parse(companySettings.Tables[0].Rows[22][2].ToString()) - 1;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            //Company Data
            companyName.Text = companySettings.Tables[0].Rows[0][2].ToString();
            companyAdress.Text = companySettings.Tables[0].Rows[1][2].ToString();
            companyCity.Text = companySettings.Tables[0].Rows[2][2].ToString();
            companyZIP.Text = companySettings.Tables[0].Rows[3][2].ToString();
            companyPhone.Text = companySettings.Tables[0].Rows[4][2].ToString();
            companyMail.Text = companySettings.Tables[0].Rows[5][2].ToString();
            wrongLocationMail.Text = companySettings.Tables[0].Rows[7][2].ToString();
            orderMail.Text = companySettings.Tables[0].Rows[8][2].ToString();
            //maxRentNumber.Text = companySettings.Tables[0].Rows[24][2].ToString();
            DocumentHistory.Text = companySettings.Tables[0].Rows[10][2].ToString();
            DocumentOrder.Text = companySettings.Tables[0].Rows[16][2].ToString();
            ImageFolder.Text = companySettings.Tables[0].Rows[21][2].ToString();
            smtpHost.Text = companySettings.Tables[0].Rows[19][2].ToString();
            smtpPort.Text = companySettings.Tables[0].Rows[18][2].ToString();


            string decimalSign = companySettings.Tables[0].Rows[6][2].ToString();
            if (decimalSign == ",")
            {
                isComma.IsChecked = true;
                isPoint.IsChecked = false;
            }
            else
            {
                isComma.IsChecked = false;
                isPoint.IsChecked = true;
            }




            LoadInputSettings(bool.Parse(key.GetValue("isTouch").ToString()));


            PDFFilePath.Text = companySettings.Tables[0].Rows[11][2].ToString();
            marginTop.Text = companySettings.Tables[0].Rows[13][2].ToString();
            marginBottom.Text = companySettings.Tables[0].Rows[12][2].ToString();
            marginLeft.Text = companySettings.Tables[0].Rows[14][2].ToString();
            marginRight.Text = companySettings.Tables[0].Rows[15][2].ToString();

            ServerAdress.Text = key.GetValue("DBServer").ToString();
            DatabaseUser.Text = key.GetValue("DBUser").ToString();
            DatabasePassword.Password = key.GetValue("DBPassword").ToString();
            ServerSchema.Text = key.GetValue("DBSchema").ToString();

            string standardCurrencyID = AdministrationQueries.RunSql("SELECT * FROM currency WHERE currency_isDefault = 1").Tables[0].Rows[0][0].ToString();
            CurrencyCombobox.SelectedIndex = int.Parse(standardCurrencyID) - 1;

            key.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
        }
        public DataSet LoadGlobalSettings()
        {
            DataSet settings = AdministrationQueries.RunSql("SELECT * FROM company_settings");
            companyName.Text = settings.Tables[0].Rows[0][2].ToString();
            companyAdress.Text = settings.Tables[0].Rows[1][2].ToString();
            companyCity.Text = settings.Tables[0].Rows[2][2].ToString();
            companyZIP.Text = settings.Tables[0].Rows[3][2].ToString();
            companyPhone.Text = settings.Tables[0].Rows[4][2].ToString();
            companyMail.Text = settings.Tables[0].Rows[5][2].ToString();
            if (settings.Tables[0].Rows[6][2].ToString() == ",")
            {
                isComma.IsChecked = true;
                isPoint.IsChecked = false;
            }
            else
            {
                isComma.IsChecked = false;
                isPoint.IsChecked = true;
            }
            DataSet ds = ItemAdministrationQueries.RunSql("SELECT * FROM currency");
            int defaultIndexCurrency = -1;
            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
            {
                CurrencyCombobox.Items.Add(ds.Tables[0].Rows[j]["currency_code"].ToString());
                if (ds.Tables[0].Rows[j]["currency_isDefault"].ToString() == "True")
                {
                    defaultIndexCurrency = j;
                }
            }
            wrongLocationMail.Text = settings.Tables[0].Rows[7][2].ToString();
            orderMail.Text = settings.Tables[0].Rows[8][2].ToString();

            CurrencyCombobox.SelectedIndex = defaultIndexCurrency;
            return settings;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {


            // TempLocationsQueries.SyncLocations();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            //Company Data
            key.SetValue("CompanyName", companyName.Text);
            key.SetValue("CompanyAdress", companyAdress.Text);
            key.SetValue("CompanyCity", companyCity.Text);
            key.SetValue("CompanyZIP", companyZIP.Text);
            key.SetValue("CompanyPhone", companyPhone.Text);
            key.SetValue("CompanyMail", companyMail.Text);

            //Automated Reports Mails
            key.SetValue("ErrorLocationMail", wrongLocationMail.Text);
            key.SetValue("OrderMail", orderMail.Text);

            //Save Locations
            key.SetValue("HistoryLogsPath", DocumentHistory.Text);
            key.SetValue("OrderOverviewPath", DocumentOrder.Text);

            //Settings Locations
            key.SetValue("LetterPaperPath", PDFFilePath.Text);

            //General Settings
            //key.SetValue("DecimalSign", InstallationSave.standardDecimal.ToString());
            //key.SetValue("StandardCurrency", InstallationSave.standardCurrencyName.ToString());
            key.SetValue("IsTouch", touchSelected);

            //PDF Letter Settings
            key.SetValue("MarginTop", marginTop.Text);
            key.SetValue("MarginBottom", marginBottom.Text);
            key.SetValue("MarginLeft", marginLeft.Text);
            key.SetValue("MarginRight", marginRight.Text);

            // int selectedLanguage = languageCombobox.SelectedIndex + 1;

            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{companyName.Text}' WHERE settings_name = 'company_name'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{companyAdress.Text}' WHERE settings_name = 'company_adress'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{companyCity.Text}' WHERE settings_name = 'company_city'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{companyZIP.Text}' WHERE settings_name = 'company_postcode'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{companyPhone.Text}' WHERE settings_name = 'company_phone'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{companyMail.Text}' WHERE settings_name = 'company_mail'");

            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{wrongLocationMail.Text}' WHERE settings_name = 'global_stockerror_mail'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{orderMail.Text}' WHERE settings_name = 'global_order_mail'");
            // AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{selectedLanguage}' WHERE settings_name = 'global_culture_id'");
            //AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{maxRentNumber.Text}' WHERE settings_name = 'max_rents'");




            if (isComma.IsChecked == true)
            {
                AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = ',' WHERE settings_name = 'global_decimal'");
            }
            else
            {
                AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '.' WHERE settings_name = 'global_decimal'");
            }

            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{CurrencyCombobox.SelectedItem.ToString()}' WHERE settings_name = 'global_currency'");

            AdministrationQueries.RunSqlExec($"UPDATE currency SET currency_isDefault = 0");
            int currencyID = CurrencyCombobox.SelectedIndex + 1;
            AdministrationQueries.RunSqlExec($"UPDATE currency SET currency_isDefault = 1 WHERE currency_id = {currencyID}");


            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{DocumentHistory.Text.Replace("\\", "\\\\")}' WHERE settings_name = 'global_history_path'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{PDFFilePath.Text.Replace("\\", "\\\\")}' WHERE settings_name = 'global_letterpaper_path'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{DocumentOrder.Text.Replace("\\", "\\\\")}' WHERE settings_name = 'global_orderdoc_path'");

            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{marginTop.Text}' WHERE settings_name = 'margin_top'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{marginBottom.Text}' WHERE settings_name = 'margin_bottom'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{marginLeft.Text}' WHERE settings_name = 'margin_left'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{marginRight.Text}' WHERE settings_name = 'margin_right'");


            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{ControlPath.Text.Replace("\\", "\\\\")}' WHERE settings_name = 'global_measure_equip_protocol_path'");
            AdministrationQueries.RunSqlExec($"UPDATE company_settings SET settings_value = '{ImageFolder.Text.Replace("\\", "\\\\")}' WHERE settings_name = 'global_image_path'");


            key.Close();
            Process.Start(System.Windows.Application.ResourceAssembly.Location);
            System.Windows.Application.Current.Shutdown();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        //private void LoadSettings()
        //{
        //    marginTop.Text = pdfDocument.Default.printSpaceTop.ToString();
        //    marginBottom.Text = pdfDocument.Default.printSpaceBottom.ToString();
        //    marginLeft.Text = pdfDocument.Default.printSpaceLeft.ToString();
        //    marginRight.Text = pdfDocument.Default.printSpaceRight.ToString();
        //    TopMarginBarOrigin.Margin = new Thickness(0, GetMargin(pdfDocument.Default.printSpaceTop.ToString(), "top"), 0, 0);
        //    BottomMarginBarOrigin.Margin = new Thickness(0, 0, 0, GetMargin(pdfDocument.Default.printSpaceBottom.ToString(), "bottom"));
        //    LeftMarginBarOrigin.Margin = new Thickness(GetMargin(pdfDocument.Default.printSpaceLeft.ToString(), "left"), 0, 0, 0);
        //    RightMarginBarOrigin.Margin = new Thickness(0, 0, GetMargin(pdfDocument.Default.printSpaceRight.ToString(), "right"), 0);
        //}



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

        public class StringValidationRule : ValidationRule
        {
            private string _errorMessage;
            public string ErrorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string input = value.ToString();
                bool rt = Regex.IsMatch(input, "^[0-9.,\\s]*$");
                if (!rt)
                {
                    return new ValidationResult(false, ErrorMessage);
                }
                else
                {
                    return new ValidationResult(true, null);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {

                    DocumentOrder.Text = dialog.SelectedPath;
                }
            }
        }

        private void SyncDatabase_Click(object sender, RoutedEventArgs e)
        {
            SettingsQueries.SyncDataLocations();
        }



        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            touchSelected = true;
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
            touchSelected = false;
            Color color = (Color)ColorConverter.ConvertFromString("#A02619"); // Replace with your desired hex color
            Brush brush = new SolidColorBrush(color);
            SelectMandKInputBorder.BorderBrush = brush;
            SelectMandKInputBorder.BorderThickness = new Thickness(2);

            color = (Color)ColorConverter.ConvertFromString("#A7A7A7"); // Replace with your desired hex color
            brush = new SolidColorBrush(color);
            SelectTouchBorder.BorderBrush = brush;
            SelectTouchBorder.BorderThickness = new Thickness(2);
        }

        private void LoadInputSettings(bool Value)
        {
            if (Value)
            {
                touchSelected = true;
                Color color = (Color)ColorConverter.ConvertFromString("#A02619"); // Replace with your desired hex color
                Brush brush = new SolidColorBrush(color);
                SelectTouchBorder.BorderBrush = brush;
                SelectTouchBorder.BorderThickness = new Thickness(2);

                color = (Color)ColorConverter.ConvertFromString("#A7A7A7"); // Replace with your desired hex color
                brush = new SolidColorBrush(color);
                SelectMandKInputBorder.BorderBrush = brush;
                SelectMandKInputBorder.BorderThickness = new Thickness(2);
            }
            else
            {
                touchSelected = false;
                Color color = (Color)ColorConverter.ConvertFromString("#A02619"); // Replace with your desired hex color
                Brush brush = new SolidColorBrush(color);
                SelectMandKInputBorder.BorderBrush = brush;
                SelectMandKInputBorder.BorderThickness = new Thickness(2);

                color = (Color)ColorConverter.ConvertFromString("#A7A7A7"); // Replace with your desired hex color
                brush = new SolidColorBrush(color);
                SelectTouchBorder.BorderBrush = brush;
                SelectTouchBorder.BorderThickness = new Thickness(2);
            }
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

                        if (counter != neededTablesArr.Length)
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
            SqlConn.SetConnectionString(ServerAdress.Text, DatabaseUser.Text, DatabasePassword.Password, ServerSchema.Text);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            key.SetValue("DBServer", ServerAdress.Text);
            key.SetValue("DBUser", DatabaseUser.Text);
            key.SetValue("DBPassword", DatabasePassword.Password);
            key.SetValue("DBSchema", ServerSchema.Text);
            key.SetValue("LetterPaperPath", PDFFilePath.Text);
            key.SetValue("HistoryLogsPath", DocumentHistory.Text);
            key.SetValue("OrderOverviewPath", DocumentOrder.Text);






            SuccessText.Visibility = Visibility.Visible;
            SuccessIcon.Visibility = Visibility.Visible;
        }

        private void OpenDialogHistoryPath_Click(object sender, RoutedEventArgs e)
        {

            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {

                    DocumentHistory.Text = dialog.SelectedPath;
                }
            }
        }

        private void LetterPaperPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "PDF Dokument|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {

                PDFFilePath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);
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
                        SaveSMTPConn.IsEnabled = true;

                    }
                }
                catch (Exception)
                {
                    SuccessInfoMail.Visibility = Visibility.Hidden;
                    ErrorInfoMail.Visibility = Visibility.Visible;
                    SaveSMTPConn.IsEnabled = false;
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
            companyMailPassword.Password = "";
            SaveSMTPConn.IsEnabled = false;




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
                SaveValues.IsEnabled = true;
                InfoAlertText.Visibility = Visibility.Hidden;
                InfoText.Text = "Bitte klicken Sie erneut auf Verbindung testen und dann auf Werte speichern!";
            }
            else
            {
                InfoAlertText.Visibility = Visibility.Hidden;
                InfoText.Text = "Es ist ein Fehler aufgetreten!";
            }
        }

        private void OpenControlPath_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {

                    ControlPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void changeImageFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {

                    ImageFolder.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
