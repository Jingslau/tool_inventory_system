using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MeasureEquipAdministration
{
    /// <summary>
    /// Interaction logic for MeasureEquipAuditorOverview.xaml
    /// </summary>
    public partial class MeasureEquipAuditorOverview : UserControl
    {
        public DataSet allItems = new DataSet();
        public MeasureEquipAuditorOverview()
        {
            InitializeComponent();

            loadData();
        }
        private void loadData()
        {
            allItems = AdministrationQueries.RunSql("SELECT * FROM measuring_equip_auditor_objects");
            if (allItems.Tables.Count > 0)
            {
                dataGridItems.DataContext = allItems;
                dataGridItems.ItemsSource = new DataView(allItems.Tables[0]);
            }
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddMeasureItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                MeasureEquipModels.CurrentMeasureEquipAuditor = row_selected.Row;
            }
        }
        private void AddAuditor_Click(object sender, RoutedEventArgs e)
        {
            AddMeasureAuditorWindow openAddAuditor = new AddMeasureAuditorWindow();
            openAddAuditor.ShowDialog();
            loadData();
        }

        private void RemoveAuditor_Click(object sender, RoutedEventArgs e)
        {
            DataSet auditorItemRelation = AdministrationQueries.RunSql($"SELECT * FROM measuring_equip_objects WHERE measuring_equip_auditor_id = {MeasureEquipModels.CurrentMeasureEquipAuditor["auditor_id"]}");

            if (auditorItemRelation.Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = "Messmittelprüfer kann nicht gelöscht werden! Es ist noch für ein Messmittel dieser Prüfer eingetragen. Bitte ändern Sie zuerst den Messmittelprüfer für den Artikel, bevor Sie den Prüfer löschen";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                DeleteMeasureEquipAuditorWindow showDelete = new DeleteMeasureEquipAuditorWindow();
                showDelete.ShowDialog();
                loadData();
            }
        }

        private void EditAuditor_Click(object sender, RoutedEventArgs e)
        {
            EditMeasureEquipAuditor openEditAuditor = new EditMeasureEquipAuditor();
            openEditAuditor.ShowDialog();
            loadData();
        }
    }
}
