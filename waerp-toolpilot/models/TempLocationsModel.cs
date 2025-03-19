namespace waerp_toolpilot.store
{
    internal class TempLocationsModel
    {
        static TempLocationsModel()
        {
            FloorID = "";
            GroupID = "";
            ItemQuant = "";
            ItemName = "";
            ContainerID = "";
            DrawerID = "";
            CompartmentID = "";
            MeasureEquipName = "";
            MeasureEquipID = "";
            AuditorID = "";
            SerialNumber = "";
            SelectedAction = 0;
            ContainerName = "";
            DrawerName = "";
            CompartmentName = "";
            filterNameNew = "";
            filterNameOld = "";
            itemsMerged = false;
            filterStage = "";
            currentItemIdEdit = "";
        }
        public static string FloorID { get; set; }
        public static string ContainerName { get; set; }
        public static string SerialNumber { get; set; }
        public static string MeasureEquipID { get; set; }

        public static string currentItemIdEdit { get; set; }

        public static string MeasureEquipName { get; set; }
        public static string AuditorID { get; set; }
        public static string filterNameNew { get; set; }
        public static string filterNameOld { get; set; }

        public static bool itemsMerged { get; set; }

        public static string filterStage { get; set; }

        public static string DrawerName { get; set; }

        public static string CompartmentName { get; set; }

        public static int SelectedAction { get; set; }

        public static string DrawerID { get; set; }
        public static string CompartmentID { get; set; }


        public static string ContainerID { get; set; }
        public static string ItemName { get; set; }
        public static string ItemQuant { get; set; }
        public static string GroupID { get; set; }
    }
}
