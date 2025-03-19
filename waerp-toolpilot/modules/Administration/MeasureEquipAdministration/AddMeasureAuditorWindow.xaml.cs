using System.Data;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for AddMeasureAuditorWindow.xaml
    /// </summary>
    public partial class AddMeasureAuditorWindow : Window
    {
        public AddMeasureAuditorWindow()
        {
            InitializeComponent();
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CreateItem_Click(object sender, RoutedEventArgs e)
        {
            if (AuditorAddress.Text.Length > 0 && AuditorMail.Text.Length > 0 && AuditorName.Text.Length > 0 && AuditorPerson.Text.Length > 0 && AuditorPhone.Text.Length > 0 && AuditorPostcode.Text.Length > 0)
            {
                DataSet searchResults = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_auditor_objects WHERE auditor_name = '{AuditorName.Text}'");

                if (searchResults.Tables[0].Rows.Count == 0)
                {
                    AdministrationQueries.RunSql($"INSERT INTO measuring_equip_auditor_objects(auditor_name, auditor_address, auditor_city, auditor_postcode, auditor_phone, auditor_mail, auditor_website, auditor_person) VALUES('{AuditorName.Text}', " +
                        $"'{AuditorAddress.Text}', " +
                        $"'{AuditorCity.Text}', " +
                        $"'{AuditorPostcode.Text}', " +
                        $"'{AuditorPhone.Text}', " +
                        $"'{AuditorMail.Text}'," +
                        $"'{AuditorWebsite.Text}', " +
                        $"'{AuditorPerson.Text}')");
                    ErrorHandlerModel.ErrorText = "Messmittelprüfer wurde erfolgreich angelegt!";
                    ErrorHandlerModel.ErrorType = "SUCCESS";
                    ErrorWindow showSuccess = new ErrorWindow();
                    showSuccess.ShowDialog();
                    DialogResult = false;
                }
                else
                {
                    ErrorHandlerModel.ErrorText = "Es existiert bereits ein Messmittelprüfer mit diesem Namen!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte füllen Sie alle Pflichtfelder aus!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
