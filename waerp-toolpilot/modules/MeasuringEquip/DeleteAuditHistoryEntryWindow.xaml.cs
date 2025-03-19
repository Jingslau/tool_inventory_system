using System.Data;
using System.IO;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.MeasuringEquip
{
    /// <summary>
    /// Interaction logic for DeleteAuditHistoryEntryWindow.xaml
    /// </summary>
    public partial class DeleteAuditHistoryEntryWindow : Window
    {
        public DeleteAuditHistoryEntryWindow()
        {
            InitializeComponent();
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            string query = $"DELETE FROM measuring_equip_history WHERE measuring_equip_history_id = {MeasureEquipModels.CurrentMeasureEquipHistoryEntry["measuring_equip_history_id"]}";
            AdministrationQueries.RunSqlExec(query);
            query = $"SELECT * FROM measuring_equip_history WHERE measuring_equip_history_itemID = {MeasureEquipModels.CurrentMeasureEquipItem["measuring_equip_id"]} ORDER BY measuring_equip_history_auditDate DESC";
            DataSet newestAudit = AdministrationQueries.RunSql(query);



            string documentationPathGlobal = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'").Tables[0].Rows[0][2].ToString();
            documentationPathGlobal = Path.Combine(documentationPathGlobal, MeasureEquipModels.CurrentMeasureEquipHistoryEntry["measuring_equip_history_protocol"].ToString());

            File.Delete(documentationPathGlobal);

            ErrorHandlerModel.ErrorText = "Der Eintrag wurde erfolgreich gelöscht!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();

            DialogResult = false;
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
