using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySqlConnector;
using waerp_toolpilot.dbtools;

namespace waerp_toolpilot.License
{
    /// <summary>
    /// Interaction logic for LicenseActivationWindw.xaml
    /// </summary>
    public partial class LicenseActivationWindow : Window
    {
        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetLicenseServerConnectionString());
        public LicenseActivationWindow()
        {
            InitializeComponent();
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ActivateLicense_Click(object sender, RoutedEventArgs e)
        {
            string licenseKey = licenseField1.Text + "-" + licenseField2.Text + "-" + licenseField3.Text + "-" + licenseField4.Text + "-" + licenseField5.Text;
            if (LicenseServerConnector.ActivateLicense(licenseKey))
            {
                DialogResult = false;
            }


        }
        private void licenseField1_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private void licenseField1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField1.Text.Length == 5)
            {
                licenseField2.Focus();
            }
        }

        private void licenseField2_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private void licenseField2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField2.Text.Length == 5)
            {
                licenseField3.Focus();
            }
        }
        private void licenseField3_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private void licenseField3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField3.Text.Length == 5)
            {
                licenseField4.Focus();
            }
        }
        private void licenseField4_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private void licenseField4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (licenseField4.Text.Length == 5)
            {
                licenseField5.Focus();
            }
        }
        private void licenseField5_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
