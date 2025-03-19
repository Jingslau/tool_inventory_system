using System;

namespace waerp_toolpilot.store
{
    internal class PrgrammParameter
    {
        public static bool UserTracking = config.SystemConfig.UserTracking.ToLower().Equals("true");
        public static bool Debug = config.SystemConfig.Debug.ToLower().Equals("true");

        public static DebugLevelType DebugLevel = (DebugLevelType)Enum.Parse(typeof(DebugLevelType), config.SystemConfig.DebugLevel);

        public static AccountStore username { get; set; }
    }

    internal enum DebugLevelType
    {
        Deep,
        Normal
    }
}
