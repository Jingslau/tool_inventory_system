using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;

namespace waerp_toolpilot.config.DatabaseSettingsView
{
    /// <summary>
    /// Interaction logic for DatabaseSettingsView.xaml
    /// </summary>
    /// 
    public partial class DatabaseSettingsView : Window
    {


        public DatabaseSettingsView()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);

            InitializeComponent();
            SuccessText.Visibility = Visibility.Hidden;
            SuccessIcon.Visibility = Visibility.Hidden;
            ServerAdress.Text = key.GetValue("DBServer").ToString();
            DatabaseUser.Text = key.GetValue("DBUser").ToString();
            DatabasePassword.Password = key.GetValue("DBPassword").ToString();
            ServerSchema.Text = key.GetValue("DBSchema").ToString();
            // TestDBConnection();

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
                ErrorLogger.LogSqlError(ex);
                //if (ex.Number == (int)MySqlErrorCode.AccessDenied)
                //{
                //    DatabaseConnectionStatus.Text = "Status: Die Logindaten sind ungültig!";
                //    ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                //    SaveValues.IsEnabled = false;
                //}
                //else
                //{
                //    DatabaseConnectionStatus.Text = "Status: Die Serveradresse ist ungültig!";
                //    ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                //    SaveValues.IsEnabled = false;
                //}
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
                catch (MySqlException)
                {
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
                            DatabaseTablesStatus.Text = $"Status: Die Tabellen des Schemas {ServerSchema.Text} sind nicht korrekt oder vollständig!";
                            ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                            SaveValues.IsEnabled = false;
                        }
                        else
                        {
                            DatabaseTablesStatus.Text = $"Status: Das Schema {ServerSchema.Text} besitzt alle Tabellen!";
                            ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                            SaveValues.IsEnabled = true;
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

        private void TestDBConnection()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);


            ServerAdress.Text = key.GetValue("DBServer").ToString();
            DatabaseUser.Text = key.GetValue("DBUser").ToString();
            DatabasePassword.Password = key.GetValue("DBPassword").ToString();
            ServerSchema.Text = key.GetValue("DBSchema").ToString();


            //Test 1 DB Connection
            MySqlConnection conn = new MySqlConnection($"Server={key.GetValue("DBServer").ToString()};" +
                $"userid={key.GetValue("DBUser").ToString()};" +
                $"password={key.GetValue("DBServer").ToString()}");
            MySqlCommand cmd = new MySqlCommand();
            //Test 1 DB Connection

            bool check2 = false;
            bool check3 = false;
            try
            {
                conn.Open();
                cmd = new MySqlCommand("SELECT 1", conn);
                cmd.ExecuteNonQuery();
                DatabaseConnectionStatus.Text = "Status: Verbindung zur Datenbank war erfolgreich!";
                ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                check2 = true;
            }
            catch (MySqlException ex)
            {
                ErrorLogger.LogSqlError(ex);
                //if (ex.Number == (int)MySqlErrorCode.AccessDenied)
                //{
                //    DatabaseConnectionStatus.Text = "Status: Die Logindaten sind ungültig!";
                //    ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                //}
                //else
                //{
                //    DatabaseConnectionStatus.Text = "Status: Die Serveradresse ist ungültig!";
                //    ValidationDatabaseBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");

                //}
            }
            finally
            {
                conn.Close();
            }





            if (check2 == true)
            {
                try
                {
                    conn.Open();
                    cmd = new MySqlCommand("SHOW DATABASES", conn);
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
                    }
                    else
                    {
                        DatabaseSchemaStatus.Text = $"Status: Das Schema {ServerSchema.Text} existiert in der Datenbank!";
                        ValidationSchemaBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                        check3 = true;
                    }
                }
                catch (MySqlException)
                {
                }
                finally
                {
                    conn.Close();
                }
                if (check3 == true)
                {
                    try
                    {
                        conn = new MySqlConnection(SqlConn.GetConnectionString());
                        conn.Open();
                        cmd = new MySqlCommand("SHOW TABLES", conn);
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
                            DatabaseTablesStatus.Text = $"Status: Die Tabellen des Schemas {ServerSchema.Text} sind nicht korrekt oder vollständig!";
                            ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");
                        }
                        else
                        {
                            DatabaseTablesStatus.Text = $"Status: Das Schema {ServerSchema.Text} besitzt alle Tabellen!";
                            ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#52CB74");
                        }





                    }
                    catch (MySqlException)
                    {

                        DatabaseTablesStatus.Text = $"Status: Es konnte keine Verbindung zur Datenbank hergestellt werden!";
                        ValidationTablesBar.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#A94C42");

                    }
                    finally
                    {
                        conn.Close();
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

        private void CloseConfig_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveValues_Click(object sender, RoutedEventArgs e)
        {
            SqlConn.SetConnectionString(ServerAdress.Text, DatabaseUser.Text, DatabasePassword.Password, ServerSchema.Text);
            SuccessText.Visibility = Visibility.Visible;
            SuccessIcon.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
