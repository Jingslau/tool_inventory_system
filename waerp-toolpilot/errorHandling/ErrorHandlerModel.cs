using System;

namespace waerp_toolpilot.errorHandling
{
    internal class ErrorHandlerModel
    {
        public ErrorHandlerModel()
        {
            ErrorType = "";
            ErrorText = "";
            ErrorMessage = "";
            ErrorTime = new DateTime();
            SQLQuery = "";

        }

        public static string ErrorType { get; set; }
        public static string ErrorText { get; set; }
        public static string ErrorMessage { get; set; }
        public static string SQLQuery { get; set; }
        public static DateTime ErrorTime { get; set; }

    }
}
