using System;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for FilterAlreadyExistsWindow.xaml
    /// </summary>
    public partial class FilterAlreadyExistsWindow : Window
    {
        public FilterAlreadyExistsWindow()
        {
            InitializeComponent();

            ErrorWindowText.Text = $"Es existiert bereits ein Filter mit dem Namen {TempLocationsModel.filterNameNew} in Suchstufe {TempLocationsModel.filterStage}! Wollen Sie diese Filter zusammenführen?";
        }

        private void MergeFilter_Click(object sender, RoutedEventArgs e)
        {
            String filter_id_old = AdministrationQueries.RunSql($"SELECT * FROM filter{TempLocationsModel.filterStage}_names WHERE name = '{TempLocationsModel.filterNameOld}'").Tables[0].Rows[0]["filter_id"].ToString();
            String filter_id_new = AdministrationQueries.RunSql($"SELECT * FROM filter{TempLocationsModel.filterStage}_names WHERE name = '{TempLocationsModel.filterNameNew}'").Tables[0].Rows[0]["filter_id"].ToString();

            AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter{TempLocationsModel.filterStage}_id = '{filter_id_new}' WHERE filter{TempLocationsModel.filterStage}_id = '{filter_id_old}' ");

            ErrorHandlerModel.ErrorText = "Die Filter wurden erfolgreich zusammengeführ!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            TempLocationsModel.itemsMerged = true;
            DialogResult = false;

        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            TempLocationsModel.itemsMerged = false;
            DialogResult = false;
        }
    }
}
