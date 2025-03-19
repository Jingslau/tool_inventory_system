using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace waerp_toolpilot.store
{
    internal class CurrentRebookModel
    {
        static CurrentRebookModel()
        {
            ItemIdent = "";
            ItemDescription = "asd";
            ItemDescription2 = "asd";

            ItemIdentStr = "";
            ItemTotalQuantity = "";
            RebookLocation = "";
            OldLocationName = "";
            NewLocationName = "";
            RebookQuantity = "";
            MachineID = "";
            ItemImagePath = "";
            ItemDiameter = "";
        }
        public static string OldLocationName { get; set; }
        public static string ItemDiameter { get; set; }

        public static string NewLocationName { get; set; }
        public static string ItemImagePath { get; set; }
        public static string ItemIdentStr { get; set; }
        public static string RebookLocation { get; set; }
        public static string RebookQuantity { get; set; }
        public static string ItemTotalQuantity { get; set; }
        public static string MachineID { get; set; }

        public static string ItemIdent { get; set; }
        public static string ItemDescription { get; set; }
        public static string ItemDescription2 { get; set; }


        public static event PropertyChangedEventHandler PropertyChanged;
        private static void RaisePorpertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static void ResetParams()
        {
            ItemIdent = "";
            ItemDescription = "asd";
            ItemIdentStr = "";
            ItemTotalQuantity = "";
            RebookLocation = "";
            RebookQuantity = "";
            ItemImagePath = "";
        }
    }
}
