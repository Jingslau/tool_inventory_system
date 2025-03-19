using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace waerp_toolpilot.store
{
    internal class CurrentReturnModel
    {
        static CurrentReturnModel()
        {
            ItemIdent = "1";
            IsSelectedUser = false;
            CurrentUserId = "";
            ItemIsConstructed = false;
            ItemDescription = "asd";
            ItemDescription2 = "asd";
            currentReturnItem = null;
            ItemIdentStr = "";
            ItemTotalQuantity = "";
            ReturnLocation = "";
            ReturnQuantity = "";
            SearchIdent = "";
            MachineID = "";
            RentID = "";
            ReturnLocationID = "";
            ItemImagePath = "";
            RentedQuantity = "";
            ReturnIsUsed = false;
            MachineName = "";


        }

        public static string MachineName { get; set; }

        public static string ItemImagePath { get; set; }
        public static DataRowView currentReturnItem { get; set; }

        public static bool IsSelectedUser { get; set; }

        public static string CurrentUserId { get; set; }
        public static bool ItemIsConstructed { get; set; }


        public static string ItemIdentStr { get; set; }
        public static string ReturnLocationID { get; set; }
        public static string SearchIdent { get; set; }
        public static string ReturnLocation { get; set; }
        public static bool ReturnIsUsed { get; set; }
        public static string RentID { get; set; }
        public static string ReturnQuantity { get; set; }
        public static string ItemTotalQuantity { get; set; }
        public static string MachineID { get; set; }
        public static string RentedQuantity { get; set; }

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
    }
}
