using System;
using System.IO;
using MySqlConnector;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.Installation;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.dbtools
{
    internal class dbSetup
    {


        public static bool InitNewDBInstallation()
        {

            MySqlConnection conn = new MySqlConnection($"Server={InstallationStore.ServerAdress};userid={InstallationStore.DatabaseUser};password={InstallationStore.DatabasePassword};Database={InstallationStore.ServerSchema}");
            try
            {


                using (conn)
                {
                    conn.Open();
                    string script = File.ReadAllText("update\\0\\initEmptyDB0.sql");

                    new MySqlCommand(script, conn).ExecuteNonQuery();
                    script = File.ReadAllText("update\\0\\initStandardValues.sql");
                    new MySqlCommand(script, conn).ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandlerModel.ErrorText = "Es ist ein Problem beim Update der Datenbank aufgetreten. Bitte wendne Sie sich an den Administrator! <br><br>" + ex;
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool DropAllTables()
        {
            MySqlConnection conn = new MySqlConnection($"Server={InstallationStore.ServerAdress};userid={InstallationStore.DatabaseUser};password={InstallationStore.DatabasePassword};Database={InstallationStore.ServerSchema}");
            conn.Open();
            string query = $@"
                SELECT CONCAT('DROP TABLE IF EXISTS ', table_name, ';')
                FROM information_schema.tables
                WHERE table_schema = '{InstallationStore.ServerSchema}';
                ";

            MySqlCommand command = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                // Iterate through the result set and execute the DROP TABLE statements
                while (reader.Read())
                {
                    string dropStatement = reader.GetString(0);
                    MySqlCommand dropCommand = new MySqlCommand(dropStatement, conn);
                    dropCommand.ExecuteNonQuery();
                }
            }
            conn.Close();
            return true;
        }

        public static bool SetupNewDB()
        {
            MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
            try
            {
                string script = File.ReadAllText("update\\0\\initEmptyDB0.sql");

                using (conn)
                {

                    if (AdministrationQueries.RunSqlExec(script))
                    {

                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandlerModel.ErrorText = "Es ist ein Problem beim Update der Datenbank aufgetreten. Bitte wendne Sie sich an den Administrator! <br><br>" + ex;
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool InitStandardValues()
        {
            MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
            try
            {
                string script = File.ReadAllText("update\\0\\initStandardValues.sql");

                using (conn)
                {

                    if (AdministrationQueries.RunSqlExec(script))
                    {

                        return true;

                    }
                    else
                    {
                        return false;
                    }




                }
            }
            catch (Exception ex)
            {
                ErrorHandlerModel.ErrorText = "Es ist ein Problem beim Update der Datenbank aufgetreten. Bitte wendne Sie sich an den Administrator! <br><br>" + ex;
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
