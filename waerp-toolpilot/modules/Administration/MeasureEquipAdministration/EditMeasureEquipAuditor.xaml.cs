using System.Data;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for EditMeasureEquipAuditor.xaml
    /// </summary>
    public partial class EditMeasureEquipAuditor : Window
    {
        public EditMeasureEquipAuditor()
        {
            InitializeComponent();

            AuditorName.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_name"].ToString();
            AuditorAddress.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_address"].ToString();
            AuditorCity.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_city"].ToString();
            AuditorPostcode.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_postcode"].ToString();
            AuditorPhone.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_phone"].ToString();
            AuditorMail.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_mail"].ToString();
            AuditorWebsite.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_website"].ToString();
            AuditorPerson.Text = MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_person"].ToString();


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

                if (searchResults.Tables[0].Rows.Count <= 1)
                {
                    string query = $"UPDATE measuring_equip_auditor_objects SET " +
                        $"auditor_name = '{AuditorName.Text}', " +
                        $"auditor_address = '{AuditorAddress.Text}', " +
                        $"auditor_city = '{AuditorCity.Text}', " +
                        $"auditor_postcode = '{AuditorPostcode.Text}', " +
                        $"auditor_phone = '{AuditorPhone.Text}', " +
                        $"auditor_mail = '{AuditorMail.Text}', " +
                        $"auditor_website = '{AuditorWebsite.Text}', " +
                        $"auditor_person = '{AuditorPerson.Text}' " +
                        $"WHERE auditor_id = '{MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_id"]}'";

                    AdministrationQueries.RunSql(query);


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
