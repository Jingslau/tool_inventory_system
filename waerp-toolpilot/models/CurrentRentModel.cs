namespace waerp_toolpilot.store
{
    internal class CurrentRentModel
    {
        static CurrentRentModel()
        {
            rentId = "";
            ItemIdent = "";
            ItemDescription = "asd";
            ItemDescription2 = "asd";
            compartmentID = "";
            ItemIdentStr = "";
            ItemTotalQuantity = "";
            RentLocation = "";
            IsGroup = false;
            RentQuantity = "";
            MachineID = "";
            ItemImagePath = "";
            newQuant = "";
            locationID = "";
            locationQuantity = "";
            zoneName = "";
            isUsed = "";
            closeWindow = false;
            is_dynamic = false;
        }

        public static string rentId { get; set; }
        public static string compartmentID { get; set; }


        public static string ItemImagePath { get; set; }
        public static bool is_dynamic { get; set; }

        public static string ItemIdentStr { get; set; }
        public static string RentLocation { get; set; }
        public static string RentQuantity { get; set; }
        public static string ItemTotalQuantity { get; set; }
        public static string MachineID { get; set; }
        public static bool IsGroup { get; set; }

        public static string ItemIdent { get; set; }
        public static string ItemDescription { get; set; }
        public static string ItemDescription2 { get; set; }


        public static string newQuant { get; set; }
        public static string locationID { get; set; }
        public static string locationQuantity { get; set; }

        public static string zoneName { get; set; }
        public static bool closeWindow { get; set; }
        public static string isUsed { get; set; }
        public static void ResetParams()
        {
            ItemIdent = "";
            ItemDescription = "asd";
            ItemIdentStr = "";
            ItemTotalQuantity = "";
            RentLocation = "";
            RentQuantity = "";
            closeWindow = false;
            ItemImagePath = "";
        }
    }





}
