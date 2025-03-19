using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.CustomerAdministration
{
    /// <summary>
    /// Interaction logic for EditCustomerWindow.xaml
    /// </summary>
    public partial class EditCustomerWindow : Window
    {
        public EditCustomerWindow()
        {
            InitializeComponent();
            VendorNumber.Text = CurrentCustomerModel.CustomerIDNumber;
            CustomerName.Text = CurrentCustomerModel.CustomerName;
            CustomerAdress.Text = CurrentCustomerModel.CustomerAdress;
            CustomerPostcode.Text = CurrentCustomerModel.CustomerPostcode;
            CustomerCity.Text = CurrentCustomerModel.CustomerCity;
            CustomerCountry.Text = CurrentCustomerModel.CustomerCountry;
            CustomerWebsite.Text = CurrentCustomerModel.CustomerWebsite;
            CustomerPhone.Text = CurrentCustomerModel.CustomerPhone;
            CustomerMail.Text = CurrentCustomerModel.CustomerMail;
            CustomerContact.Text = CurrentCustomerModel.CustomerContact;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
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
            AdministrationQueries.UpdateCustomer();
            ErrorHandlerModel.ErrorText = (string)FindResource("errorText7");
            ErrorHandlerModel.ErrorType = "SUCCESS";

            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            DialogResult = false;
        }
    }
}
