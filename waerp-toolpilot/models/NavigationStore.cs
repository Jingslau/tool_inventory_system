namespace waerp_toolpilot.store
{
    internal class NavigationStore
    {
        static NavigationStore()
        {
            SideBarReookIndex = -1;
            SideBarMngtIndex = -1;
            SideBarShoppingIndex = -1;
            SideBarMainIndex = -1;

        }
        public static int SideBarReookIndex { get; set; }
        public static int SideBarMngtIndex { get; set; }
        public static int SideBarShoppingIndex { get; set; }
        public static int SideBarMainIndex { get; set; }
    }
}
