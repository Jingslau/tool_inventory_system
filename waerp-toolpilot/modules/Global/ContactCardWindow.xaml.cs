using System.Windows;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.Global
{
    /// <summary>
    /// Interaction logic for ContactCardWindow.xaml
    /// </summary>
    public partial class ContactCardWindow : Window
    {
        public ContactCardWindow()
        {
            InitializeComponent();

            CompanyAdress.Text = ContactCardModel.CompanyAdress;
            CompanyCity.Text = ContactCardModel.CompanyCity;
            CompanyCountry.Text = ContactCardModel.CompanyCountry;
            CompanyMail.Text = ContactCardModel.CompanyMail;
            CompanyName.Text = ContactCardModel.CompanyName;
            CompanyPhone.Text = ContactCardModel.CompanyPhone;
            CompanyPostcode.Text = ContactCardModel.CompanyPostcode;
            CompanyWebsite.Text = ContactCardModel.CompanyWebsite;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
