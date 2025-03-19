using System.Data;

namespace waerp_toolpilot.models
{
    internal class MeasureEquipModels
    {
        static MeasureEquipModels()
        {
            _ = CurrentMeasureEquipItem;
            _ = CurrentMeasureEquipHistoryEntry;
            _ = CurrentMeasureEquipAuditor;



        }

        public static DataRow CurrentMeasureEquipItem { get; set; }
        public static DataRow CurrentMeasureEquipHistoryEntry { get; set; }
        public static DataRow CurrentMeasureEquipAuditor { get; set; }
    }
}
