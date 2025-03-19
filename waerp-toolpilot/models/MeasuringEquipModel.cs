using System;

namespace waerp_toolpilot.models
{
    internal class MeasuringEquipModel
    {
        static MeasuringEquipModel()
        {
            MeasuringEquipID = "";
            MeasuringEquipName = "";
            MeasuringEquipVendor = "";
            CurrentSelectedHistory_DocPath = "";
            CurrentSelectedHistory_CheckDate = DateTime.MinValue;

        }


        public static string MeasuringEquipID { get; set; }
        public static string MeasuringEquipName { get; set; }
        public static string MeasuringEquipVendor { get; set; }
        public static string CurrentSelectedHistory_DocPath { get; set; }

        public static DateTime CurrentSelectedHistory_CheckDate { get; set; }

    }
}
