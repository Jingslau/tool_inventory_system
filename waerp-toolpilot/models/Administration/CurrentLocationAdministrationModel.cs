namespace waerp_toolpilot.store.Administration
{
    internal class CurrentLocationAdministrationModel
    {
        static CurrentLocationAdministrationModel()
        {
            LocationName = "";
            SelectedLocationId = "";
            SelectedLocationName = "";



        }
        public static string LocationName { get; set; }
        public static string SelectedLocationId { get; set; }
        public static string SelectedLocationName { get; set; }
    }
}
