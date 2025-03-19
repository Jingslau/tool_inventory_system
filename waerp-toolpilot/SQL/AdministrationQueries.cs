using System.Data;
using System.Windows;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.sql
{
    internal class AdministrationQueries
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());


        public static DataSet GetAllInfo(string TableName)
        {
            return RunSql($"SELECT * FROM {TableName}");

        }

        public static void CreateCustomer()
        {
            string MaxIDStr = GetMaxId(RunSql($"SELECT * FROM customer_objects"), "customer_id");
            string MaxIDStr2 = GetMaxId(RunSql($"SELECT * FROM filter1_names"), "filter_id");
            RunSqlExec($"INSERT INTO filter1_names (filter_id, name) VALUES ({MaxIDStr2}, '{CurrentCustomerModel.CustomerName}')");
            RunSqlExec($"INSERT INTO customer_objects (customer_company_id, customer_id, customer_name, customer_adress, customer_postcode, customer_city, customer_country, customer_website, customer_phone, customer_mail, customer_contact) VALUES ({CurrentCustomerModel.CustomerIDNumber}, {MaxIDStr}, '{CurrentCustomerModel.CustomerName}', '{CurrentCustomerModel.CustomerAdress}','{CurrentCustomerModel.CustomerPostcode}','{CurrentCustomerModel.CustomerCity}', '{CurrentCustomerModel.CustomerCountry}','{CurrentCustomerModel.CustomerWebsite}','{CurrentCustomerModel.CustomerPhone}','{CurrentCustomerModel.CustomerMail}','{CurrentCustomerModel.CustomerContact}')");
        }

        public static bool CheckCustomerID(string customerID)
        {
            DataSet ds = RunSql($"SELECT * FROM customer_objects WHERE customer_company_id = {customerID}");
            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            else
            { return true; }
        }
        public static void UpdateCustomer()
        {
            RunSqlExec($"UPDATE filter1_names SET name = '{CurrentCustomerModel.CustomerName}' WHERE name = '{CurrentCustomerModel.SelectedCustomerName}'");
            RunSqlExec($"UPDATE customer_objects SET customer_company_id='{CurrentCustomerModel.CustomerIDNumber}', customer_name = '{CurrentCustomerModel.CustomerName}', customer_adress = '{CurrentCustomerModel.CustomerAdress}', customer_postcode = '{CurrentCustomerModel.CustomerPostcode}', customer_city = '{CurrentCustomerModel.CustomerCity}', customer_country = '{CurrentCustomerModel.CustomerCountry}', customer_website = '{CurrentCustomerModel.CustomerWebsite}', customer_phone = '{CurrentCustomerModel.CustomerPhone}', customer_mail = '{CurrentCustomerModel.CustomerMail}', customer_contact = '{CurrentCustomerModel.CustomerContact}' WHERE customer_id = {CurrentCustomerModel.CustomerID}");
        }

        public static void DeleteCustomer()
        {
            RunSqlExec($"DELETE FROM customer_objects WHERE customer_id = {CurrentCustomerModel.CustomerID}");
        }
        public static void CreateVendor()
        {
            string MaxIDStr = GetMaxId(RunSql($"SELECT * FROM vendor_objects"), "vendor_id");
            RunSqlExec($"INSERT INTO vendor_objects (vendor_id, vendor_name, vendor_adress, vendor_postcode, vendor_city, vendor_country, vendor_website, vendor_phone, vendor_mail, vendor_contact, vendor_auto_order, vendor_customerid) VALUES ({MaxIDStr}, '{CurrentCustomerModel.CustomerName}', '{CurrentCustomerModel.CustomerAdress}','{CurrentCustomerModel.CustomerPostcode}','{CurrentCustomerModel.CustomerCity}', '{CurrentCustomerModel.CustomerCountry}','{CurrentCustomerModel.CustomerWebsite}','{CurrentCustomerModel.CustomerPhone}','{CurrentCustomerModel.CustomerMail}','{CurrentCustomerModel.CustomerContact}', {CurrentCustomerModel.automatedOrder}, '{CurrentCustomerModel.SelfCustomerID}')");
        }
        public static void UpdateVendor()
        {
            RunSqlExec($"UPDATE vendor_objects SET vendor_name = '{CurrentCustomerModel.CustomerName}', vendor_adress = '{CurrentCustomerModel.CustomerAdress}', vendor_postcode = '{CurrentCustomerModel.CustomerPostcode}', vendor_city = '{CurrentCustomerModel.CustomerCity}', vendor_country = '{CurrentCustomerModel.CustomerCountry}', vendor_website = '{CurrentCustomerModel.CustomerWebsite}', vendor_phone = '{CurrentCustomerModel.CustomerPhone}', vendor_mail = '{CurrentCustomerModel.CustomerMail}', vendor_contact = '{CurrentCustomerModel.CustomerContact}', vendor_auto_order = {CurrentCustomerModel.automatedOrder}, vendor_customerid = '{CurrentCustomerModel.SelfCustomerID}' WHERE vendor_id = {CurrentCustomerModel.CustomerID}");
        }

        public static void DeleteVendor()
        {
            RunSqlExec($"DELETE FROM vendor_objects WHERE vendor_id = {CurrentCustomerModel.CustomerID}");
        }


        public static bool AddNewUser(string user_ident, string username, string password, string name, string surname, string email, string roleStr, string[] userPrivilegesArr)
        {

            DataSet ds2 = RunSql($"SELECT * FROM user_roles WHERE name = '{roleStr}'");
            string roleID = "0";
            if (ds2.Tables[0].Rows.Count == 0)
            {
                roleID = GetMaxId(RunSql("SELECT * FROM user_roles"), "role_id");
                RunSqlExec($"INSERT INTO user_roles (role_id, name) VALUES ({roleID}, '{roleStr}')");
            }
            else
            {
                roleID = RunSql($"SELECT * FROM user_roles WHERE name = '{roleStr}'").Tables[0].Rows[0]["role_id"].ToString();
            }
            DataSet ds = GetAllInfo("user_privileges");
            string[] priviIDs = new string[userPrivilegesArr.Length];
            int index = 0;
            string MaxIDStr = GetMaxId(RunSql("SELECT * FROM users"), "user_id");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < userPrivilegesArr.Length; j++)
                {

                    if (ds.Tables[0].Rows[i]["privileges_name"].ToString() == userPrivilegesArr[j] | ds.Tables[0].Rows[i]["privileges_name_DE"].ToString() == userPrivilegesArr[j])
                    {
                        priviIDs[index] = ds.Tables[0].Rows[i]["privileges_id"].ToString();
                        index++;
                    }
                }
            }

            for (int i = 0; i < priviIDs.Length; i++)
            {
                RunSqlExec($"INSERT INTO user_privilege_relations (id, user_id, privilege_id) VALUES ({GetMaxId(RunSql("SELECT * FROM user_privilege_relations"), "id")}, {MaxIDStr}, {priviIDs[i]})");
            }

            RunSqlExec($"INSERT INTO users (user_id, user_ident, username, user_password, name, surname, email, role_id, culture_id) VALUES ({MaxIDStr}, '{user_ident}','{username}', '{password}', '{name}', '{surname}', '{email}', {roleID}, '4')");
            return true;
        }


        public static bool EditUser(string user_ident, string username, string password, string name, string surname, string email, string roleStr, string[] userPrivilegesArr)
        {
            DataSet ds2 = RunSql($"SELECT * FROM user_roles WHERE name = '{roleStr}'");
            string roleID = "0";
            if (ds2.Tables[0].Rows.Count == 0)
            {
                roleID = GetMaxId(RunSql("SELECT * FROM user_roles"), "role_id");
                RunSqlExec($"INSERT INTO user_roles (role_id, name) VALUES ({roleID}, '{roleStr}')");
            }
            else
            {
                roleID = RunSql($"SELECT * FROM user_roles WHERE name = '{roleStr}'").Tables[0].Rows[0]["role_id"].ToString();
            }
            RunSqlExec($"DELETE FROM user_privilege_relations WHERE user_id = {CurrentUserAdministrationModel.UserID}");
            DataSet ds = GetAllInfo("user_privileges");
            string[] priviIDs = new string[userPrivilegesArr.Length];
            int index = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < userPrivilegesArr.Length; j++)
                {

                    if (ds.Tables[0].Rows[i]["privileges_name"].ToString() == userPrivilegesArr[j] | ds.Tables[0].Rows[i]["privileges_name_DE"].ToString() == userPrivilegesArr[j])
                    {
                        priviIDs[index] = ds.Tables[0].Rows[i]["privileges_id"].ToString();
                        index++;
                    }
                }
            }

            for (int i = 0; i < priviIDs.Length; i++)
            {
                RunSqlExec($"INSERT INTO user_privilege_relations (id, user_id, privilege_id) VALUES ({GetMaxId(RunSql("SELECT * FROM user_privilege_relations"), "id")}, {CurrentUserAdministrationModel.UserID}, {priviIDs[i]})");
            }


            RunSqlExec($"UPDATE users SET user_ident = '{user_ident}', username = '{username}', user_password = '{password}', name = '{name}', surname = '{surname}', email = '{email}', role_id = {roleID} WHERE user_id = {CurrentUserAdministrationModel.UserID}");
            return true;
        }
        public static bool EditPassword(string NewPassword, string UserID)
        {
            RunSqlExec($"UPDATE users SET user_password = '{NewPassword}' WHERE user_id = '{UserID}'");
            return true;
        }

        public static bool DeleteUser()
        {
            RunSqlExec($"DELETE FROM user_privilege_relations WHERE user_id = {CurrentUserAdministrationModel.UserID}");
            RunSqlExec($"UPDATE users SET isDeactivated = 1 WHERE user_id = {CurrentUserAdministrationModel.UserID}");
            return true;
        }
        //public static void EditItem()
        //{

        //    RunSqlExec($"UPDATE item_objects SET item_ident = {}, item_description = {} WHERE item_id = {}");
        //}



        public static DataSet GetFilterList(int index)
        {
            return RunSql($"SELECT * FROM filter{index}_names ORDER BY name ASC");
        }


        public static bool CreateLocation()
        {

            string MaxIDStr = GetMaxId(RunSql("SELECT * FROM location_objects"), "location_id");
            if (RunSql($"SELECT * FROM location_objects WHERE location_name = '{CurrentLocationAdministrationModel.LocationName}'").Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = (string)Application.Current.FindResource("errorText70");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openNotallowed = new ErrorWindow();
                openNotallowed.ShowDialog();
                return false;
            }
            else
            {
                RunSqlExec($"INSERT INTO location_objects (location_id, location_name, location_size, location_quantity, item_used, item_constructed) VALUES ({MaxIDStr}, '{CurrentLocationAdministrationModel.LocationName}', '', 0, 0, 0)");
                ErrorHandlerModel.ErrorText = (string)Application.Current.FindResource("errorText71");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();
                return true;
            }
        }

        public static bool DeleteLocation()
        {
            if (int.Parse(RunSql($"SELECT * FROM location_objects WHERE location_id = {CurrentLocationAdministrationModel.SelectedLocationId}").Tables[0].Rows[0]["location_quantity"].ToString()) == 0)
            {
                RunSqlExec($"DELETE FROM location_objects WHERE location_id = {CurrentLocationAdministrationModel.SelectedLocationId}");
                ErrorHandlerModel.ErrorText = (string)Application.Current.FindResource("errorText72");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();
                return true;
            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)Application.Current.FindResource("errorText73");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openNotallowed = new ErrorWindow();
                openNotallowed.ShowDialog();
                return false;
            }

        }

        public static bool EditLocation()
        {
            DataSet ds = RunSql($"SELECT * from location_objects WHERE location_name = '{CurrentLocationAdministrationModel.LocationName}'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = (string)Application.Current.FindResource("errorText70");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
            else
            {
                RunSqlExec($"UPDATE location_objects SET location_name = '{CurrentLocationAdministrationModel.LocationName}' WHERE location_id = {CurrentLocationAdministrationModel.SelectedLocationId}");
                ErrorHandlerModel.ErrorText = (string)Application.Current.FindResource("errorText74");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();
                return true;
            }

        }

        public static bool CreateTempLocation()
        {
            string MaxIDStr = GetMaxId(RunSql("SELECT * FROM floor_objects"), "floor_id");
            if (RunSql($"SELECT * FROM floor_objects WHERE floor_name = '{CurrentLocationAdministrationModel.LocationName}'").Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = "Es besteht bereits ein Zwischenlager mit diesem Namen!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openNotallowed = new ErrorWindow();
                openNotallowed.ShowDialog();
                return false;
            }
            else
            {
                RunSqlExec($"INSERT INTO floor_objects (floor_id, floor_name, floor_quantity) VALUES ({MaxIDStr}, '{CurrentLocationAdministrationModel.LocationName}', 0)");
                ErrorHandlerModel.ErrorText = "Der Lagerort wurde erfolgreich angelegt!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();
                return true;
            }
        }

        public static bool EditTempLocation()
        {
            DataSet ds = RunSql($"SELECT * from floor_objects WHERE floor_name = '{CurrentLocationAdministrationModel.LocationName}'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = $"Es existiert bereits ein anderes Zwischenlager mit dem Namen {CurrentLocationAdministrationModel.LocationName}!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
            else
            {
                RunSqlExec($"UPDATE floor_objects SET floor_name = '{CurrentLocationAdministrationModel.LocationName}' WHERE floor_id = {CurrentLocationAdministrationModel.SelectedLocationId}");
                ErrorHandlerModel.ErrorText = "Der Lagerort wurde erfolgreich bearbeitet!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();
                return true;
            }
        }

        public static bool DeleteTempLocation()
        {
            DataSet ds = RunSql($"SELECT * FROM floor_objects WHERE floor_id = {CurrentLocationAdministrationModel.SelectedLocationId}");
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (int.Parse(ds.Tables[0].Rows[0]["floor_quantity"].ToString()) > 0)
                {
                    ErrorHandlerModel.ErrorText = "Es sind aktuell noch Artikel im Zwischenlager! Bitte leeren Sie das Zwischenlager, damit Sie es löschen können!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow openError = new ErrorWindow();
                    openError.ShowDialog();
                    return false;
                }
                else
                {
                    RunSqlExec($"DELETE FROM floor_objects WHERE floor_id = {CurrentLocationAdministrationModel.SelectedLocationId}");
                    ErrorHandlerModel.ErrorText = "Das Zwischenlager wurde erfolgreich gelöscht!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow openSuccess = new ErrorWindow();
                    openSuccess.ShowDialog();
                    return true;
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Das Zwischenlager konnte nicht gefunden werden!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
                return false;
            }

        }
        public static string GetMaxId(DataSet ds, string Prompt)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                return "0";
            }
            else
            {
                int maxID = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (int.Parse(row[Prompt].ToString()) > maxID)
                    {
                        maxID = int.Parse(row[Prompt].ToString());
                    }
                }
                maxID++;
                return maxID.ToString();
            }

        }
        public static DataSet RunSql(string query)
        {
            try
            {
                conn.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter();
                DataSet ReturnDataSet = new DataSet();
                adp = new MySqlDataAdapter(new MySqlCommand(query, conn));
                adp.Fill(ReturnDataSet);
                conn.Close();
                return ReturnDataSet;
            }
            catch (MySqlException ex)
            {
                ErrorHandlerModel.SQLQuery = query;
                ErrorLogger.LogSqlError(ex); conn.Close();
                return null;

            }
            finally { conn.Close(); }
        }
        public static bool RunSqlExec(string query)
        {
            try
            {
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();
                return true;

            }
            catch (MySqlException ex)
            {
                ErrorHandlerModel.SQLQuery = query;
                ErrorLogger.LogSqlError(ex); conn.Close();
                return false;

            }
            finally { conn.Close(); }
        }
    }

}
