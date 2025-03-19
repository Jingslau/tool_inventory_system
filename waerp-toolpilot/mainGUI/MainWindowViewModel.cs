using System.Windows;

namespace waerp_toolpilot.main
{
    internal class MainWindowViewModel
    {

        static MainWindowViewModel()
        {
            Firstname = "Max";
            Lastname = "Mustermann";
            UserID = "";
            username = "";
            RoleID = 0;
            RoleStr = "";
            CurrentBreadcumb = "";
            globalImagePath = "";
            showOrdersystem = false;
            showRebook = false;
            showAdministration = false;
            showSettings = false;
            openApplication = false;
            originLanguageDic = null;
            currentLanguageDic = null;
            loginSuccesful = false;
            ItemOnMin = 0;
            ItemsToOrder = 0;
        }

        public static string Firstname { get; set; }
        public static string CurrentBreadcumb { get; set; }
        public static ResourceDictionary originLanguageDic { get; set; }
        public static ResourceDictionary currentLanguageDic { get; set; }

        public static string RoleStr { get; set; }
        public static string globalImagePath { get; set; }
        public static int ItemOnMin { get; set; }
        public static int ItemsToOrder { get; set; }
        public static bool showOrdersystem { get; set; }
        public static bool loginSuccesful { get; set; }
        public static bool openApplication { get; set; }
        public static bool showRebook { get; set; }
        public static bool showAdministration { get; set; }
        public static bool showSettings { get; set; }

        public static string UserID { get; set; }
        public static string Lastname { get; set; }
        public static string username { get; set; }
        public static int RoleID { get; set; }

        public static string Fullname => $"{Firstname} {Lastname}";



    }
}
