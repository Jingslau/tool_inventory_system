namespace waerp_toolpilot.models
{
    internal class OrderStore
    {
        static OrderStore()
        {
            VendorID = "";
            OrderIdent = "";
            item_id = "";
        }


        public static string VendorID { get; set; }
        public static string OrderIdent { get; set; }
        public static string item_id { get; set; }
    }
}
