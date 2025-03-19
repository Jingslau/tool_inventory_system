using System.Data;

namespace waerp_toolpilot.store
{
    internal class RebookGroupModel
    {
        static RebookGroupModel()
        {
            CurrentGroupId = "";
            CurrentGroupName = "";
            CurrentGroupItems = new DataSet();
            NewGroupId = "";
            CurrentGroupQuantity = 0;
            RebookGroupText1 = "";
            RebookGroupText2 = "";
            NewLocationName = "";
            QuantityNewLocation = 0;
            FloorID = "";
            IsEmpty = true;
            SelectedFloorId = "";

        }


        public static string CurrentGroupId { get; set; }
        public static string CurrentGroupName { get; set; }
        public static string RebookGroupText1 { get; set; }
        public static string NewLocationName { get; set; }
        public static string SelectedFloorId { get; set; }
        public static string RebookGroupText2 { get; set; }
        public static string NewGroupId { get; set; }
        public static string FloorID { get; set; }
        public static bool IsEmpty { get; set; }
        public static int QuantityNewLocation { get; set; }
        public static int CurrentGroupQuantity { get; set; }
        public static DataSet CurrentGroupItems { get; set; }
    }
}
