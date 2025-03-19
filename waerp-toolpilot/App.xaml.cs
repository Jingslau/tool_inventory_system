using System;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.FirstTimeStartup;
using waerp_toolpilot.loginHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.ViewModels;

namespace waerp_toolpilot
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            CheckDatabaseTables();

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            if (key != null)
            {
                Thread databaseThread = new Thread(CheckDatabaseReachability);
                ResourceDictionary languageDictionary;
                if (checkDBConnect())
                {
                    //int globalLanguageID = int.Parse(AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_culture_id'").Tables[0].Rows[0][2].ToString());
                    int globalLanguageID = 4;
                    string selectedLanguage = AdministrationQueries.RunSql($"SELECT * FROM culture_objects WHERE culture_id = {globalLanguageID}").Tables[0].Rows[0]["culture_code"].ToString();
                    languageDictionary = new ResourceDictionary
                    {
                        Source = new Uri($"/Language/{selectedLanguage}.xaml", UriKind.Relative)
                    };


                    Resources.MergedDictionaries.Add(languageDictionary);
                }

                // Set the thread as a background thread
                databaseThread.IsBackground = true;

                // Start the thread
                databaseThread.Start();
                loginView mainWindow = new loginView
                {
                    DataContext = new MainViewModel()
                };
            }
            else
            {
                FirstTimeStartUpWindow openInstallation = new FirstTimeStartUpWindow();
                openInstallation.Show();
            }












            // Create an instance of the StartupWindow and show it
            // The MainWindow will be automatically created and displayed



        }


        public static void CheckDatabaseTables()
        {
            using (var connection = new MySqlConnection(SqlConn.GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    // Check if column 'item_documentation' exists in the 'item_objects' table
                    string checkColumnQuery = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = DATABASE() 
                AND TABLE_NAME = 'item_objects' 
                AND COLUMN_NAME = 'item_documentation';
            ";

                    using (var checkCmd = new MySqlCommand(checkColumnQuery, connection))
                    {
                        var columnExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                        if (!columnExists)
                        {
                            // Add the column if it does not exist
                            string addColumnQuery = @"
                        ALTER TABLE item_objects 
                        ADD COLUMN item_documentation VARCHAR(255) DEFAULT '';
                    ";

                            using (var addColumnCmd = new MySqlCommand(addColumnQuery, connection))
                            {
                                addColumnCmd.ExecuteNonQuery();
                                Console.WriteLine("Column 'item_documentation' has been added to the 'item_objects' table.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Column 'item_documentation' already exists in the 'item_objects' table.");
                        }
                    }

                    // Check and add entries to user_privileges table
                    string checkPrivilegesQuery = @"
                SELECT COUNT(*) 
                FROM user_privileges 
                WHERE privileges_id = 5 
                AND privileges_name = 'measuring_equipment' 
                AND privileges_name_DE = 'Messmittelverwaltung';
            ";

                    using (var checkPrivilegesCmd = new MySqlCommand(checkPrivilegesQuery, connection))
                    {
                        var privilegeExists = Convert.ToInt32(checkPrivilegesCmd.ExecuteScalar()) > 0;

                        if (!privilegeExists)
                        {
                            // Add the privileges entry if it does not exist
                            string addPrivilegesQuery = @"
                        INSERT INTO user_privileges (privileges_id, privileges_name, privileges_name_DE) 
                        VALUES (5, 'measuring_equipment', 'Messmittelverwaltung');
                    ";

                            using (var addPrivilegesCmd = new MySqlCommand(addPrivilegesQuery, connection))
                            {
                                addPrivilegesCmd.ExecuteNonQuery();
                                Console.WriteLine("Privilege 'measuring_equipment' has been added to the 'user_privileges' table.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Privilege 'measuring_equipment' already exists in the 'user_privileges' table.");
                        }
                    }

                    // Check if column 'isDeactivated' exists in the 'users' table
                    string checkIsDeactivatedQuery = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = DATABASE() 
                AND TABLE_NAME = 'users' 
                AND COLUMN_NAME = 'isDeactivated';
            ";

                    using (var checkCmd = new MySqlCommand(checkIsDeactivatedQuery, connection))
                    {
                        var columnExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                        if (!columnExists)
                        {
                            // Add the 'isDeactivated' column if it does not exist
                            string addColumnQuery = @"
                        ALTER TABLE users 
                        ADD COLUMN isDeactivated TINYINT(1) DEFAULT 0;
                    ";

                            using (var addColumnCmd = new MySqlCommand(addColumnQuery, connection))
                            {
                                addColumnCmd.ExecuteNonQuery();
                                Console.WriteLine("Column 'isDeactivated' has been added to the 'users' table with default value TRUE.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Column 'isDeactivated' already exists in the 'users' table.");
                        }
                    }

                    // Check if 'auditor_id' in 'measuring_equip_auditor_objects' is set to auto-increment
                    string checkAutoIncrementQuery = @"
                SELECT EXTRA 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = DATABASE() 
                AND TABLE_NAME = 'measuring_equip_auditor_objects' 
                AND COLUMN_NAME = 'auditor_id';
            ";

                    using (var checkCmd = new MySqlCommand(checkAutoIncrementQuery, connection))
                    {
                        var extraValue = checkCmd.ExecuteScalar()?.ToString();

                        if (extraValue == null || extraValue.IndexOf("auto_increment", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            // Modify the column to be auto-increment
                            string modifyColumnQuery = @"
                        ALTER TABLE measuring_equip_auditor_objects 
                        MODIFY COLUMN auditor_id INT NOT NULL AUTO_INCREMENT;
                    ";

                            using (var modifyCmd = new MySqlCommand(modifyColumnQuery, connection))
                            {
                                modifyCmd.ExecuteNonQuery();
                                Console.WriteLine("Column 'auditor_id' in 'measuring_equip_auditor_objects' has been set to auto-increment.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Column 'auditor_id' in 'measuring_equip_auditor_objects' is already set to auto-increment.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        private void CheckDatabaseReachability()
        {
            while (true)
            {
                // Perform the database reachability check here
                // You can use a try-catch block to catch any exceptions

                try
                {
                    // Attempt to connect to the MySQL database
                    using (var connection = new MySqlConnection(SqlConn.GetConnectionString()))
                    {
                        connection.Open();
                    }

                }
                catch (Exception ex)
                {

                    ErrorHandlerModel.ErrorText = "Die Datenbank ist nicht erreichbar. Bitte prüfen Sie Ihre Netzwerkverbindung oder wenden Sie sich an den Systemadministrator!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow openError = new ErrorWindow();
                    openError.ShowDialog();
                    ErrorLogger.LogSqlError(ex);
                }

                // Wait for 10 seconds before performing the next reachability check
                Thread.Sleep(10000);
            }
        }
        private bool checkDBConnect()
        {
            try
            {
                // Attempt to connect to the MySQL database
                using (var connection = new MySqlConnection(SqlConn.GetConnectionString()))
                {
                    connection.Open();
                }
                return true;

            }
            catch (Exception ex)
            {

                ErrorHandlerModel.ErrorText = "Die Datenbank ist nicht erreichbar. Bitte prüfen Sie Ihre Netzwerkverbindung oder wenden Sie sich an den Systemadministrator!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
                ErrorLogger.LogSqlError(ex);
                return false;
            }
        }
    }
}
