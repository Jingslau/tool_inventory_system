using System.Windows;
using waerp_toolpilot.models;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for ContinueAddingItemWindow.xaml
    /// </summary>
    public partial class ContinueAddingItemWindow : Window
    {
        public ContinueAddingItemWindow()
        {
            InitializeComponent();
        }


        private void Decline_Click(object sender, RoutedEventArgs e)
        {
            NewItemInInputModel.createSameItem = false;
            DialogResult = false;

        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            NewItemInInputModel.createSameItem = true;
            DialogResult = true;
        }
    }
}
