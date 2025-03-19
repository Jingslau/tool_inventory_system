using System.Windows;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteLocationWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteLocationWindow : Window
    {
        public ConfirmDeleteLocationWindow()
        {
            InitializeComponent();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.DeleteLocation();

            DialogResult = false;
        }
    }
}
