using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.VendorAdministration
{
    /// <summary>
    /// Interaction logic for EditVendorWindow.xaml
    /// </summary>
    public partial class EditVendorWindow : Window
    {
        public EditVendorWindow()
        {
            InitializeComponent();
            if (CurrentCustomerModel.automatedOrder == "1")
            {
                automatedOrder.IsChecked = true;
            }
            else
            {
                automatedOrder.IsChecked = false;
            }

            CustomerID.Text = CurrentCustomerModel.SelfCustomerID;
            VendorName.Text = CurrentCustomerModel.CustomerName;
            VendorAdress.Text = CurrentCustomerModel.CustomerAdress;
            VendorPostcode.Text = CurrentCustomerModel.CustomerPostcode;
            VendorCity.Text = CurrentCustomerModel.CustomerCity;
            VendorCountry.Text = CurrentCustomerModel.CustomerCountry;
            VendorWebsite.Text = CurrentCustomerModel.CustomerWebsite;
            VendorPhone.Text = CurrentCustomerModel.CustomerPhone;
            VendorMail.Text = CurrentCustomerModel.CustomerMail;
            VendorContact.Text = CurrentCustomerModel.CustomerContact;
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
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

                if (automatedOrder.IsChecked == true)
                {
                    CurrentCustomerModel.automatedOrder = "1";
                }
                else
                {
                    CurrentCustomerModel.automatedOrder = "0";

                }

                AdministrationQueries.UpdateVendor();
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
