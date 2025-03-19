using System.Windows;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.BookItem
{
    /// <summary>
    /// Interaction logic for SuccessBookWindow.xaml
    /// </summary>
    public partial class SuccessBookWindow : Window
    {
        public SuccessBookWindow()
        {
            InitializeComponent();
            ItemIdent.Text = (string)FindResource("errorText38a") + "\n\n" + CurrentRentModel.ItemIdentStr + " \n " + (string)FindResource("errorText38b");
            LocationName.Text = CurrentReturnModel.ReturnLocation;
        }
        private void CloseCurrentDialog(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
