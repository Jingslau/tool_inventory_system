using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.Administration.VendorAdministration
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteWindow : Window
    {
        public ConfirmDeleteWindow()
        {
            InitializeComponent();
            ConfirmText.Text = (string)FindResource("errorText33");
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }

        private void DeleteVendor_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.DeleteVendor();
            ErrorHandlerModel.ErrorText = (string)FindResource("errorText34");
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();
            DialogResult = false;
        }
    }
}
