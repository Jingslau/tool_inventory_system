using System;
using System.Data;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.errorHandling;

namespace waerp_toolpilot.License
{
    internal class LicenseServerConnector
    {
        public static MySqlConnection conn = new MySqlConnection(GetConnectionString());
        public static string GetConnectionString()
        {

            return $"Server={Database.Default.dbLicenseServer};userid={Database.Default.dbLicenseUser};password={Database.Default.dbLicensePassword};Database={Database.Default.dbLicenseSchema};Convert Zero Datetime=True";
        }



        public static bool ActivateLicense(string licenseKey)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);

            key.SetValue("hasLicense", true);
            key.Close();
            ErrorHandlerModel.ErrorText = "Der eingegebene Lizenzschlüssel wurde erfolgreich aktiviert!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            return true;

            //conn.Open();
            //MySqlCommand cmd = new MySqlCommand($"SELECT * FROM licensekeys WHERE product_key = '{licenseKey}'", conn);
            //MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //adp.Fill(ds);

            //if (ds.Tables[0].Rows.Count == 1)
            //{
            //    if (ds.Tables[0].Rows[0]["isActivated"].ToString() == "False")
            //    {
            //        try
            //        {
            //            cmd = new MySqlCommand($"UPDATE licensekeys SET " +
            //                $"component_hash = '{SystemHardwareReader.GetSystemHash()}'" +
            //                $", " +
            //                $"isActivated = 1" +
            //                $", " +
            //                $"activation_date = '{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}'" +
            //                $", " +
            //                $"last_checkin_date = '{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}' " +
            //                $"WHERE " +
            //                $"product_key = '{licenseKey}'", conn);
            //            cmd.ExecuteNonQuery();
            //            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);

            //            key.SetValue("hasLicense", true);
            //            key.Close();
            //            ErrorHandlerModel.ErrorText = "Der eingegebene Lizenzschlüssel wurde erfolgreich aktiviert!";
            //            ErrorHandlerModel.ErrorType = "SUCCESS";
            //            ErrorWindow showSuccess = new ErrorWindow();
            //            showSuccess.ShowDialog();
            //            conn.Close();
            //            return true;
            //        }
            //        catch (MySqlException ex)
            //        {
            //            ErrorHandlerModel.SQLQuery = "Es konnte keine Verbindung zum Lizenzserver aufgebaut werden.";
            //            ErrorHandlerModel.ErrorType = "NOTALLOWED";
            //            ErrorLogger.LogSqlError(ex);
            //            conn.Close();
            //            return false;

            //        }
            //        finally
            //        {
            //            conn.Close();
            //        }

            //    }
            //    else
            //    {
            //        ErrorHandlerModel.ErrorText = "Der eingegebene Lizenzschlüssel ist bereits aktiviert!";
            //        ErrorHandlerModel.ErrorType = "NOTALLOWED";
            //        ErrorWindow showError = new ErrorWindow();
            //        showError.ShowDialog();
            //        conn.Close();
            //        return false;
            //    }
            //}
            //else
            //{
            //    ErrorHandlerModel.ErrorText = "Der eingegebene Lizenzschlüssel ist ungültig!";
            //    ErrorHandlerModel.ErrorType = "NOTALLOWED";
            //    ErrorWindow showError = new ErrorWindow();
            //    showError.ShowDialog();
            //    conn.Close();
            //    return false;
            //}

        }


        public static bool CheckLicense()
        {



            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM licensekeys WHERE component_hash = '{SystemHardwareReader.GetSystemHash()}'", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);


                //Verifying Respnse from Licenseserver
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //boolean to keep track if something is invalid
                    bool licenseIsValid = true;


                    var lastCheckInDate = DateTime.Parse(ds.Tables[0].Rows[0]["last_checkin_date"].ToString());
                    var expiryDate = DateTime.Parse(ds.Tables[0].Rows[0]["expiry_date"].ToString());
                    DateTime thisDay = DateTime.Now;


                    //Checks if Product for this SystemHash is already activated
                    if (ds.Tables[0].Rows[0]["isActivated"].ToString() == "0")
                    {
                        licenseIsValid = false;
                    }



                    //Checks if last Checkin from Database lays behind or infront of current PC Datetime or if product is expiered. 
                    if (DateTime.Compare(thisDay, lastCheckInDate) < 0 | DateTime.Compare(thisDay, expiryDate) > 0)
                    {

                        licenseIsValid = false;
                    }

                    //if nothing triggers the licenseIsValid boolean, a new last checkin entry is made for the next comparision. It will be save in registry and database
                    if (licenseIsValid)
                    {
                        cmd = new MySqlCommand($"UPDATE licensekeys SET last_checkin_date = '{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}' WHERE product = 'toolpilot' AND component_hash = '{SystemHardwareReader.GetSystemHash()}'", conn);
                        cmd.ExecuteNonQuery();
                        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                        key.SetValue("checkIn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        key.Close();
                        conn.Close();
                        return true;
                    }
                    else
                    {
                        conn.Close();
                        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
                        key.SetValue("hasLicense", false);
                        key.Close();
                        ErrorHandlerModel.ErrorText = "Es ist keine gültige Lizenz für dieses Gerät hinterlegt! Bitte stellen Sie sicher dass Sie eine aktive Internetverbindung haben und geben Sie den Lizenzschlüssel erneut ein!";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();

                        return false;
                    }



                }
                else
                {
                    conn.Close();
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                ErrorHandlerModel.SQLQuery = "License confirmation SQL Error";
                ErrorLogger.LogSqlError(ex);
                return false;

            }
            finally
            {
                conn.Close();
            }
        }

        public static int CheckLicenseStartUp()
        {
            if (Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot") != null)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);

                if (key.GetValue("hasLicense").ToString() == "False")
                {
                    return 1;
                }
                else
                {
                    if (!SystemHardwareReader.CompareSavedHash())
                    {
                        return 2;
                    }
                    else
                    {
                        if (!LicenseServerConnector.CheckLicense())
                        {
                            return 3;

                        }

                    }
                }
                return 0;
            }
            else
            {
                return 4;

            }
        }
    }
}

