using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.Administration.CustomerAdministration
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteWindow : Window
    {
        public ConfirmDeleteWindow()
        {
            InitializeComponent();
            ConfirmText.Text = (string)FindResource("errorText5");
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            AdministrationQueries.DeleteCustomer();
            ErrorHandlerModel.ErrorText = (string)FindResource("errorText6");
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();
            DialogResult = false;
        }


    }
}
