using System;
using System.Data;
using System.Windows;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.RentItem
{
    /// <summary>
    /// Interaction logic for ConfirmRevertLastRentWindow.xaml
    /// </summary>
    public partial class ConfirmRevertLastRentWindow : Window
    {
        public DataSet LastUserRent = new DataSet();
        public ConfirmRevertLastRentWindow()
        {
            InitializeComponent();

            LastUserRent = AdministrationQueries.RunSql($"SELECT * FROM item_rents t1 WHERE t1.user_id = {MainWindowViewModel.UserID} AND t1.createdAt = (SELECT MAX(createdAt) FROM item_rents t2 WHERE t2.user_id = {MainWindowViewModel.UserID} )");

            if (LastUserRent.Tables[0].Rows.Count > 0)
            {


                DataSet itemInfo = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {LastUserRent.Tables[0].Rows[0]["item_id"]}");


                DataSet compartmentInfo = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE compartment_id = {LastUserRent.Tables[0].Rows[0]["location_id"]}");
                DataSet drawerInfo = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE drawer_id = {compartmentInfo.Tables[0].Rows[0]["drawer_id"]}");
                DataSet containerInfo = AdministrationQueries.RunSql($"SELECT * FROM container_objects WHERE container_id = {drawerInfo.Tables[0].Rows[0]["container_id"]}");

                string locationName = containerInfo.Tables[0].Rows[0]["container_name_short"].ToString() + "-" + drawerInfo.Tables[0].Rows[0]["drawer_name"].ToString() + "-" + compartmentInfo.Tables[0].Rows[0]["compartment_name"].ToString();

                itemIdent.Text = itemInfo.Tables[0].Rows[0]["item_ident"].ToString();
                itemDescription.Text = itemInfo.Tables[0].Rows[0]["item_description"].ToString() + " " + itemInfo.Tables[0].Rows[0]["item_description_2"].ToString();
                itemDiameter.Text = itemInfo.Tables[0].Rows[0]["item_diameter"].ToString() + " mm";
                quantity.Text = LastUserRent.Tables[0].Rows[0]["rent_quantity"].ToString();
                rentDate.Text = LastUserRent.Tables[0].Rows[0]["createdAt"].ToString();
                location.Text = locationName;


            }
        }

        private bool CheckIfDateValid(string date)
        {
            // Parse the datetime string
            DateTime dateTime = DateTime.Parse(date);

            // Get the current datetime
            DateTime currentDateTime = DateTime.Now;

            // Calculate the threshold datetime (one hour ago)
            DateTime oneHourAgo = currentDateTime.Subtract(TimeSpan.FromHours(1));

            // Check if the parsed datetime is within the past hour
            return oneHourAgo <= dateTime && dateTime < currentDateTime;
        }

        private void RebookItem_Click(object sender, RoutedEventArgs e)
        {
            DataSet FoundCompartment = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {LastUserRent.Tables[0].Rows[0]["location_id"]} AND {LastUserRent.Tables[0].Rows[0]["item_id"]}");

            if (FoundCompartment.Tables[0].Rows.Count > 0)
            {
                AdministrationQueries.RunSqlExec($"UPDATE compartment_item_relations SET item_quantity = item_quantity + {LastUserRent.Tables[0].Rows[0]["rent_quantity"]} WHERE compartment_id = {LastUserRent.Tables[0].Rows[0]["location_id"]}");
            }
            else
            {
                AdministrationQueries.RunSqlExec($"INSERT INTO compartment_item_relations (compartment_id, item_id, item_quantity, item_constructed, item_used) VALUES ({LastUserRent.Tables[0].Rows[0]["location_id"]}, {LastUserRent.Tables[0].Rows[0]["item_id"]}, {LastUserRent.Tables[0].Rows[0]["rent_quantity"]}, 0, {LastUserRent.Tables[0].Rows[0]["item_used"]})");
            }

            AdministrationQueries.RunSqlExec($"DELETE FROM item_rents WHERE rent_id = {LastUserRent.Tables[0].Rows[0]["rent_id"]}");
            AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total + {LastUserRent.Tables[0].Rows[0]["rent_quantity"]}");

            if (LastUserRent.Tables[0].Rows[0]["item_used"].ToString() == "0")
            {
                AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new+ {LastUserRent.Tables[0].Rows[0]["rent_quantity"]}");

            }

            ErrorHandlerModel.ErrorText = "Der Artikel wurde erfolgreich zurückgebucht!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            HistoryLogger.CreateHistory(LastUserRent.Tables[0].Rows[0]["item_id"].ToString(), LastUserRent.Tables[0].Rows[0]["rent_quantity"].ToString(), LastUserRent.Tables[0].Rows[0]["location_id"].ToString(), LastUserRent.Tables[0].Rows[0]["location_id"].ToString(), "0", "0", "10");

            DialogResult = false;

        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
