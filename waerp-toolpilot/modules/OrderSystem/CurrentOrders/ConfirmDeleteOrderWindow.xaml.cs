using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.OrderSystem.CurrentOrders
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteOrderWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteOrderWindow : Window
    {
        public ConfirmDeleteOrderWindow()
        {
            InitializeComponent();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentOrdersQueries.DeleteOrder())
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText43");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSuccess = new ErrorWindow();
                showSuccess.ShowDialog();
                DialogResult = false;
            }
        }
    }
}
