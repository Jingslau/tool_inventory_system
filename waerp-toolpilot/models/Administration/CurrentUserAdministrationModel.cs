namespace waerp_toolpilot.store.Administration
{
    internal class CurrentUserAdministrationModel
    {
        static CurrentUserAdministrationModel()
        {

            UserID = 0;
            ShowSendReportBtn = true;
        }
        public static int UserID { get; set; }
        public static bool ShowSendReportBtn { get; set; }

    }


}
