using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.modules.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteItemWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteItemWindow : Window
    {
        public ConfirmDeleteItemWindow()
        {
            InitializeComponent();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {



            AdministrationQueries.RunSqlExec($"DELETE FROM item_objects WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            //AdministrationQueries.RunSqlExec($"DELETE FROM item_location_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            //AdministrationQueries.RunSqlExec($"DELETE FROM floor_group_item_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM item_filter_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM item_rents WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM item_subitem_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM item_vendor_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");
            AdministrationQueries.RunSqlExec($"DELETE FROM order_item_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");


            AdministrationQueries.RunSqlExec($"DELETE FROM compartment_item_relations WHERE item_id = {CurrentItemAdministrationModel.SelectedItem["item_id"]}");

            ErrorHandlerModel.ErrorText = (string)FindResource("errorText15");
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            DialogResult = false;



        }
    }
}
