using System.Data;

namespace waerp_toolpilot.store.Administration
{
    internal class CurrentItemAdministrationModel
    {
        static CurrentItemAdministrationModel()
        {
            currentSelectedTableIndex = -1;
            NewItemIdentStr = "";
            NewItemDescription = "";
            NewItemFilter1 = "";
            NewItemFilter2 = "";
            NewItemFilter3 = "";
            NewItemFilter4 = "";
            NewItemFilter5 = "";
            NewItemIsOrderable = false;
            NewItemMinQuant = "0";
            NewItemMinOrder = "0";
            NewItemPrice = "0";
            NewItemCurrencyID = 0;
            NewItemVendorID = 0;

        }

        public static DataRow SelectedItem { get; set; }
        public static DataRow SelectedItemSave { get; set; }

        public static int currentSelectedTableIndex { get; set; }
        public static string NewItemIdentStr { get; set; }
        public static string NewItemDescription { get; set; }
        public static string NewItemFilter1 { get; set; }
        public static string NewItemFilter2 { get; set; }
        public static string NewItemFilter3 { get; set; }
        public static string NewItemFilter4 { get; set; }
        public static string NewItemFilter5 { get; set; }
        public static bool NewItemIsOrderable { get; set; }
        public static string NewItemMinQuant { get; set; }
        public static string NewItemMinOrder { get; set; }
        public static string NewItemPrice { get; set; }
        public static int NewItemCurrencyID { get; set; }
        public static int NewItemVendorID { get; set; }

    }
}
