using System;
using System.IO;
using MySqlConnector;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.dbtools
{
    internal class dbUpdate
    {


        public static int GetCurrentDBVersion()
        {
            return int.Parse(AdministrationQueries.GetAllInfo("dbinfo").Tables[0].Rows[0][2].ToString());
        }

        public static string GetUpdateFilePath()
        {
            string[] subfolderPaths = Directory.GetDirectories("update");
            string[] subfolderNames = new string[subfolderPaths.Length];
            for (int i = 0; i < subfolderPaths.Length; i++)
            {
                subfolderNames[i] = Path.GetFileName(subfolderPaths[i]);
            }
            return Path.Combine("update", subfolderNames[subfolderNames.Length - 1], "updateDB.sql");
        }

        public static bool IsCurrentDBVersionValidInstallation(string version)
        {
            try
            {
                // Get the subfolder paths
                string[] subfolderPaths = Directory.GetDirectories("update");

                // Extract only the folder names from the paths
                string[] subfolderNames = new string[subfolderPaths.Length];
                for (int i = 0; i < subfolderPaths.Length; i++)
                {
                    subfolderNames[i] = Path.GetFileName(subfolderPaths[i]);
                }

                int newVersion = subfolderNames.Length;


                if (int.Parse(version) == subfolderNames.Length - 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandlerModel.ErrorText = "Es ist ein Problem beim Abgleich der Datenbankversion aufgetreten. Bitte wendne Sie sich an den Administrator! \n\n" + ex;
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
        }

        public static bool IsCurrentDBVersionValid()
        {
            try
            {
                // Get the subfolder paths
                string[] subfolderPaths = Directory.GetDirectories("update");

                // Extract only the folder names from the paths
                string[] subfolderNames = new string[subfolderPaths.Length];
                for (int i = 0; i < subfolderPaths.Length; i++)
                {
                    subfolderNames[i] = Path.GetFileName(subfolderPaths[i]);
                }

                int newVersion = subfolderNames.Length;



                if (GetCurrentDBVersion() == subfolderNames.Length - 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandlerModel.ErrorText = "Es ist ein Problem beim Abgleich der Datenbankversion aufgetreten. Bitte wendne Sie sich an den Administrator! \n\n" + ex;
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
                return false;
            }
        }

        public static bool UpdateDatabase()
        {
            MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
            try
            {

                string script = File.ReadAllText(GetUpdateFilePath());

                using (conn)
                {

                    if (AdministrationQueries.RunSqlExec(script))
                    {
                        int newDBVer = GetCurrentDBVersion() + 1;

                        AdministrationQueries.RunSql($"UPDATE dbinfo SET dbinfo_value = {newDBVer} WHERE dbinfo_key = 'version'");
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
                ErrorHandlerModel.ErrorText = "Es ist ein Problem beim Update der Datenbank aufgetreten. Bitte wendne Sie sich an den Administrator! \n\n" + ex;
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
