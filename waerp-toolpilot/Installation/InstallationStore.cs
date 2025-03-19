namespace waerp_toolpilot.Installation
{
    internal class InstallationStore
    {
        static InstallationStore()
        {
            ServerAdress = "";
            DatabaseUser = "";
            DatabasePassword = "";
            ServerSchema = "";
        }
        public static string ServerAdress { get; set; }
        public static string DatabaseUser { get; set; }
        public static string DatabasePassword { get; set; }
        public static string ServerSchema { get; set; }
    }
}
