namespace waerp_toolpilot.store
{
    internal class ActiveOrderModel
    {
        static ActiveOrderModel()
        {
            ItemQuantity = "";
            BookQuantity = "";
            check = false;
            CurrentItemId = "";
            ItemIdentStr = "";
            ItemDescription = "";
            Order_Ident = "";
            ItemImagePath = "";


        }
        public static string ItemQuantity { get; set; }
        public static bool check { get; set; }

        public static string CurrentItemId { get; set; }
        public static string Order_Ident { get; set; }
        public static string ItemIdentStr { get; set; }
        public static string ItemDescription { get; set; }
        public static string ItemImagePath { get; set; }

        public static string BookQuantity { get; set; }
    }
}
