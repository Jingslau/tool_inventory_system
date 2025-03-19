namespace waerp_toolpilot.store
{
    internal class ContactCardModel
    {
        static ContactCardModel()
        {
            CompanyName = "";
            CompanyAdress = "";
            CompanyCity = "";
            CompanyPostcode = "";
            CompanyCountry = "";
            CompanyWebsite = "";
            CompanyMail = "";
            CompanyPhone = "";
        }
        public static string CompanyName { get; set; }
        public static string CompanyAdress { get; set; }
        public static string CompanyCity { get; set; }
        public static string CompanyPostcode { get; set; }
        public static string CompanyCountry { get; set; }
        public static string CompanyWebsite { get; set; }
        public static string CompanyMail { get; set; }
        public static string CompanyPhone { get; set; }
    }
}
