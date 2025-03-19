using System.Windows;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.TempLocations
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteFromGroupWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteFromGroupWindow : Window
    {
        public ConfirmDeleteFromGroupWindow()
        {
            InitializeComponent();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            TempLocationsQueries.DeleteItemFromGroup();

            DialogResult = false;
        }
    }
}
