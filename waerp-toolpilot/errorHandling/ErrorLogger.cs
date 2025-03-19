using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MySqlConnector;
using waerp_toolpilot.store;

namespace waerp_toolpilot.errorHandling
{
    internal class ErrorLogger
    {
        private static string _sqlErrorFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "toolpilot\\logs\\sql-error.log");
        private static string _sysErrorFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "toolpilot\\logs\\system-Error.log");





        internal static void LogSqlError(Exception e)
        {
            bool isRealError = true;

            // not every error is an error - some should be only warnings
            if (e is MySqlException sqlex)
            {
                switch (sqlex.Number)
                {
                    case 0:
                    case 1040: // Too Many Connections
                    case 1042: // Server not available
                    case 1044: // User not allowed
                    case 1045: // Username or password wrong
                        break;
                    case 1217: // Foreign Key Checks Failed
                        //ShowMessage.ShowWarning(
                        //    LanguageReader.GetWarningString("title.ForeignKeyChecksFailed"),
                        //    LanguageReader.GetWarningString("message.ForeignKeyChecksFailed"));
                        isRealError = false;
                        break;
                    default:
                        break;
                }
            }

            if (isRealError)
            {
                FileStream traceStream;
                Trace.Listeners.Clear();
                try
                {
                    if (File.Exists(_sqlErrorFile))
                    {
                        FileInfo fileInfo = new FileInfo(_sqlErrorFile);
                        if (fileInfo.CreationTime < DateTime.Now.AddDays(-30))
                        {
                            if (File.Exists(_sqlErrorFile + ".old"))
                                File.Delete(_sqlErrorFile + ".old");

                            fileInfo.MoveTo(_sqlErrorFile + ".old");
                        }
                    }

                    traceStream = new FileStream(_sqlErrorFile, FileMode.Append, FileAccess.Write);

                    DateTime timestamp = DateTime.Now;
                    string time = timestamp.GetDateTimeFormats()[39];

                    TextWriterTraceListener textListener;
                    textListener = new TextWriterTraceListener(traceStream);

                    _ = Trace.Listeners.Add(textListener);

                    string message = "\n\n" + time + " :: " + e.GetType().Name;
                    string messageEmail = "<br><br>" + time + " :: " + e.GetType().Name;
                    message += "\n" + e.Message;
                    messageEmail += "<br>" + e.Message;

                    if (PrgrammParameter.DebugLevel == DebugLevelType.Deep)
                    {
                        if (!string.IsNullOrWhiteSpace(e.StackTrace))
                        {

                            message += "\nStacktrace:\n" + e.StackTrace;
                            messageEmail += "<br>Stacktrace:<br>" + e.StackTrace;
                        }

                        if (e.InnerException != null)
                        {
                            messageEmail += "<br>InnerException :: " + e.InnerException.GetType().Name;
                            message += "\nInnerException :: " + e.InnerException.GetType().Name;
                            if (!string.IsNullOrWhiteSpace(e.InnerException.StackTrace))
                            {
                                messageEmail += "<br> InnerExecption Stacktrace:<br>" + e.InnerException.StackTrace;
                                message += "\nInnerException Stacktrace:\n" + e.InnerException.StackTrace;
                            }
                        }
                        if (e.Data.Contains("UserDetails"))
                        {
                            message += "\n\nDetails: " + e.Data["UserDetails"].ToString();
                            messageEmail += "<br><br>Details: " + e.Data["UserDetails"].ToString();
                        }
                        if (e.Data.Contains("UserDetailsAddon"))
                        {
                            messageEmail += "<br><br>Zusatzinfo: " + e.Data["UserDetailsAddon"].ToString();
                            message += "\n\nZusatzinfo: " + e.Data["UserDetailsAddon"].ToString();
                        }
                    }

                    // write Message to Logfile
                    Trace.WriteLine(message);
                    Trace.Flush();
                    textListener.Flush();

                    textListener.Close();
                    traceStream.Close();

                    Trace.Listeners.Clear();
                    ErrorHandlerModel.ErrorType = "ERROR";
                    ErrorHandlerModel.ErrorText = $"Es ist ein SQL Fehler aufgetreten. \n \n {e.Message}";
                    ErrorHandlerModel.ErrorMessage = messageEmail;
                    ErrorHandlerModel.ErrorTime = DateTime.Now;

                    ErrorWindow showError = new ErrorWindow();
                    Nullable<bool> dialogResult = showError.ShowDialog();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    // ShowMessage.ShowAlert(ex, sendReport);
                }
            }
        }

        internal static void LogSysError(Exception e)
        {
            FileStream traceStream;
            Trace.Listeners.Clear();
            try
            {
                if (File.Exists(_sysErrorFile))
                {
                    FileInfo fileInfo = new FileInfo(_sysErrorFile);
                    if (fileInfo.CreationTime < DateTime.Now.AddDays(-30))
                    {
                        if (File.Exists(_sysErrorFile + ".old"))
                            File.Delete(_sysErrorFile + ".old");

                        fileInfo.MoveTo(_sysErrorFile + ".old");
                    }
                }

                traceStream = new FileStream(_sysErrorFile, FileMode.Append, FileAccess.Write);

                DateTime timestamp = DateTime.Now;
                string time = timestamp.GetDateTimeFormats()[39];

                TextWriterTraceListener textListener;
                textListener = new TextWriterTraceListener(traceStream);

                _ = Trace.Listeners.Add(textListener);

                string message;

                if (e is null)
                {
                    message = "\n\n" + time + " :: Unknown - Null";

                    message += "\nThere is no given error";
                }
                else
                {
                    message = "\n\n" + time + " :: " + e.GetType().Name;

                    message += "\n" + e.Message;

                    if (PrgrammParameter.DebugLevel == DebugLevelType.Deep)
                    {
                        if (!string.IsNullOrWhiteSpace(e.StackTrace))
                            message += "\nStacktrace:\n" + e.StackTrace;

                        if (e.InnerException != null)
                        {
                            message += "\nInnerException :: " + e.InnerException.GetType().Name;
                            if (!string.IsNullOrWhiteSpace(e.InnerException.StackTrace))
                                message += "\nInnerException Stacktrace:\n" + e.InnerException.StackTrace;
                        }
                        if (e.Data.Contains("UserDetails"))
                            message += "\n\nDetails: " + e.Data["UserDetails"].ToString();
                    }
                }

                // write Message to Logfile
                Trace.WriteLine(message);
                Trace.Flush();
                textListener.Flush();

                textListener.Close();
                traceStream.Close();

                Trace.Listeners.Clear();

                ErrorHandlerModel.ErrorText = e.Message;
                ErrorHandlerModel.ErrorType = "ERROR";
                ErrorWindow errorWindow = new ErrorWindow();
                errorWindow.ShowDialog();
                //    ShowMessage.ShowAlert(e, true);
            }
            catch (Exception ex)
            {

                ErrorHandlerModel.ErrorText = ex.Message;
                ErrorHandlerModel.ErrorType = "ERROR";
                ErrorWindow errorWindow = new ErrorWindow();
                errorWindow.ShowDialog();
                //MessageBox.Show(ex.Message);
                //  ShowMessage.ShowAlert(ex, true);
            }
        }
    }
}
