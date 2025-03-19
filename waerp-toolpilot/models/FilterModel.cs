namespace waerp_toolpilot.models
{
    internal class FilterModel
    {
        static FilterModel()
        {
            filterNumber = 0;
            selectedFilterID = -1;
            selectedFilterName = "";
        }
        public static int filterNumber { get; set; }
        public static int selectedFilterID { get; set; }
        public static string selectedFilterName { get; set; }


    }

}
