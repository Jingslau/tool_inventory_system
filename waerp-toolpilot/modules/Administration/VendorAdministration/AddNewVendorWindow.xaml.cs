using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.VendorAdministration
{
    /// <summary>
    /// Interaction logic for AddNewVendorWindow.xaml
    /// </summary>
    public partial class AddNewVendorWindow : Window
    {
        public AddNewVendorWindow()
        {
            InitializeComponent();
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CreateVendor_Click(object sender, RoutedEventArgs e)
        {
            if (VendorName.Text == "" ||
                VendorName.Text == "" ||
                VendorAdress.Text == "" ||
                VendorPostcode.Text == "" ||
                VendorCity.Text == "" ||
                VendorCountry.Text == "" ||
                VendorWebsite.Text == "" ||
                VendorPhone.Text == "" ||
                VendorMail.Text == "" ||
                VendorContact.Text == ""
                )
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText31");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                CurrentCustomerModel.SelfCustomerID = CustomerID.Text;
                CurrentCustomerModel.CustomerName = VendorName.Text;
                CurrentCustomerModel.CustomerAdress = VendorAdress.Text;
                CurrentCustomerModel.CustomerPostcode = VendorPostcode.Text;
                CurrentCustomerModel.CustomerCity = VendorCity.Text;
                CurrentCustomerModel.CustomerCountry = VendorCountry.Text;
                CurrentCustomerModel.CustomerWebsite = VendorWebsite.Text;
                CurrentCustomerModel.CustomerPhone = VendorPhone.Text;
                CurrentCustomerModel.CustomerMail = VendorMail.Text;
                CurrentCustomerModel.CustomerContact = VendorContact.Text;

                AdministrationQueries.CreateVendor();
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText32");
                ErrorHandlerModel.ErrorType = "SUCCESS";

                ErrorWindow showSuccess = new ErrorWindow();
                showSuccess.ShowDialog();
                DialogResult = false;
            }
        }

        private void automatedOrder_Click(object sender, RoutedEventArgs e)
        {
            if (automatedOrder.IsChecked == true)
            {
                CurrentCustomerModel.automatedOrder = "1";
            }
            else
            {
                CurrentCustomerModel.automatedOrder = "0";
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
