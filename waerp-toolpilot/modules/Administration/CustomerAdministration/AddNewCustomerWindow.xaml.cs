using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.CustomerAdministration
{
    /// <summary>
    /// Interaction logic for AddNewCustomerWindow.xaml
    /// </summary>
    public partial class AddNewCustomerWindow : Window
    {
        public AddNewCustomerWindow()
        {
            InitializeComponent();
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void CreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerName.Text == "" | VendorNumber.Text == "" | CustomerAdress.Text == "" | CustomerMail.Text == "" | CustomerCity.Text == "" | CustomerCountry.Text == "" | CustomerPostcode.Text == "")
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText2");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                if (!AdministrationQueries.CheckCustomerID(VendorNumber.Text))
                {
                    CurrentCustomerModel.CustomerIDNumber = VendorNumber.Text;
                    CurrentCustomerModel.CustomerName = CustomerName.Text;
                    CurrentCustomerModel.CustomerAdress = CustomerAdress.Text;
                    CurrentCustomerModel.CustomerPostcode = CustomerPostcode.Text;
                    CurrentCustomerModel.CustomerCity = CustomerCity.Text;
                    CurrentCustomerModel.CustomerCountry = CustomerCountry.Text;
                    CurrentCustomerModel.CustomerWebsite = CustomerWebsite.Text;
                    CurrentCustomerModel.CustomerPhone = CustomerPhone.Text;
                    CurrentCustomerModel.CustomerMail = CustomerMail.Text;
                    CurrentCustomerModel.CustomerContact = CustomerContact.Text;
                    AdministrationQueries.CreateCustomer();
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText3");
                    ErrorHandlerModel.ErrorType = "SUCCESS";

                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    DialogResult = false;
                }
                else
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText4");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }

            }
        }
    }
}
