namespace waerp_toolpilot.store.Administration
{
    internal class CurrentCustomerModel
    {
        static CurrentCustomerModel()
        {
            SelfCustomerID = "";
            CustomerIDNumber = "";
            SelectedCustomerName = "";
            CustomerID = "";
            CustomerName = "";
            CustomerAdress = "";
            CustomerPostcode = "";
            CustomerCity = "";
            CustomerCountry = "";
            CustomerWebsite = "";
            CustomerPhone = "";
            CustomerMail = "";
            CustomerContact = "";
            automatedOrder = "0";


        }

        public static string SelfCustomerID { get; set; }
        public static string CustomerIDNumber { get; set; }

        public static string CustomerID { get; set; }
        public static string SelectedCustomerName { get; set; }
        public static string CustomerName { get; set; }
        public static string CustomerAdress { get; set; }
        public static string CustomerPostcode { get; set; }
        public static string CustomerCity { get; set; }
        public static string CustomerCountry { get; set; }

        public static string CustomerWebsite { get; set; }
        public static string CustomerPhone { get; set; }
        public static string CustomerMail { get; set; }
        public static string CustomerContact { get; set; }
        public static string automatedOrder { get; set; }

    }
}
