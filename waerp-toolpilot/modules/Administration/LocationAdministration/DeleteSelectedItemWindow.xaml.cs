using System;
using System.Data;
using System.Windows;
using System.Windows.Media;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for DeleteSelectedItemWindow.xaml
    /// </summary>
    public partial class DeleteSelectedItemWindow : Window
    {
        public DeleteSelectedItemWindow()
        {
            InitializeComponent();
            if (TempLocationsModel.SelectedAction == 1)
            {
                MessageTitle.Foreground = GetColorFromHexa("#FFB752");
                ErrorWindowBorder.Background = GetColorFromHexa("#FFB752");
                ErrorWindowIcon.Foreground = GetColorFromHexa("#FFB752");
                DeleteItem.Background = GetColorFromHexa("#FFB752");
                MessageTitle.Text = (string)FindResource("locationAdminTitle1");
                ErrorWindowText.Text = (string)FindResource("locationAdminText1a") + TempLocationsModel.ContainerName + (string)FindResource("locationAdminText1b");
                ErrorWindowText2.Text = (string)FindResource("locationAdminText1c");
            }
            else if (TempLocationsModel.SelectedAction == 2)
            {
                MessageTitle.Foreground = GetColorFromHexa("#FF5297");
                ErrorWindowBorder.Background = GetColorFromHexa("#FF5297");
                ErrorWindowIcon.Foreground = GetColorFromHexa("#FF5297");
                DeleteItem.Background = GetColorFromHexa("#FF5297");
                MessageTitle.Text = (string)FindResource("locationAdminTitle2");
                ErrorWindowText.Text = (string)FindResource("locationAdminText2a") + TempLocationsModel.DrawerName +
                    (string)FindResource("locationAdminText2b") +
                    TempLocationsModel.ContainerName + (string)FindResource("locationAdminText2c");


                ErrorWindowText2.Text = (string)FindResource("locationAdminText2d");
            }
            else if (TempLocationsModel.SelectedAction == 3)
            {
                MessageTitle.Foreground = GetColorFromHexa("#FF5252");
                ErrorWindowBorder.Background = GetColorFromHexa("#FF5252");
                ErrorWindowIcon.Foreground = GetColorFromHexa("#FF5252");
                DeleteItem.Background = GetColorFromHexa("#FF5252");
                MessageTitle.Text = (string)FindResource("locationAdminTitle3");
                ErrorWindowText.Text = (string)FindResource("locationAdminText3a") +
                    TempLocationsModel.CompartmentName +
                    (string)FindResource("locationAdminText3b") + TempLocationsModel.DrawerName +
                    (string)FindResource("locationAdminText3c") + TempLocationsModel.ContainerName +
                    (string)FindResource("locationAdminText3d");

                ErrorWindowText2.Text = (string)FindResource("locationAdminText3e");
            }
            else
            {
                DialogResult = false;
            }

        }
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            byte r = Convert.ToByte(hexaColor.Substring(1, 2), 16);
            byte g = Convert.ToByte(hexaColor.Substring(3, 2), 16);
            byte b = Convert.ToByte(hexaColor.Substring(5, 2), 16);
            SolidColorBrush soliColorBrush = new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
            return soliColorBrush;
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (TempLocationsModel.SelectedAction == 1)
            {
                DataSet relatedDrawers = AdministrationQueries.RunSql($"SELECT * FROM drawer_objects WHERE container_id = {TempLocationsModel.ContainerID}");
                if (relatedDrawers.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < relatedDrawers.Tables.Count; i++)
                    {
                        DataSet tmpCompartments = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE drawer_id = {relatedDrawers.Tables[0].Rows[i]["drawer_id"]}");
                        if (tmpCompartments.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < tmpCompartments.Tables[0].Rows.Count; j++)
                            {
                                DeleteCompartment(tmpCompartments.Tables[0].Rows[j]["compartment_id"].ToString());
                            }
                            // AdministrationQueries.RunSqlExec($"DELETE FROM compartment_objects WHERE drawer_id = {relatedDrawers.Tables[0].Rows[i]["drawer_id"]}");
                        }

                        AdministrationQueries.RunSqlExec($"DELETE FROM drawer_objects WHERE drawer_id = {relatedDrawers.Tables[0].Rows[i]["drawer_id"]}");
                    }

                }
                AdministrationQueries.RunSqlExec($"DELETE FROM container_objects WHERE container_id = {TempLocationsModel.ContainerID}");

                ErrorHandlerModel.ErrorText = "Der Schrank wurde mit allen Schubladen und Lagerfächern erfolgreich gelöscht!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSuccess = new ErrorWindow();
                showSuccess.ShowDialog();
                DialogResult = false;
            }
            else if (TempLocationsModel.SelectedAction == 2)
            {
                DataSet relatedCompartments = AdministrationQueries.RunSql($"SELECT * FROM compartment_objects WHERE drawer_id = {TempLocationsModel.DrawerID}");


                if (relatedCompartments.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < relatedCompartments.Tables[0].Rows.Count; i++)
                    {
                        DeleteCompartment(relatedCompartments.Tables[0].Rows[i]["compartment_id"].ToString());
                    }
                }
                AdministrationQueries.RunSqlExec($"DELETE FROM drawer_objects WHERE drawer_id = {TempLocationsModel.DrawerID}");

                ErrorHandlerModel.ErrorText = "Die Schublade wurde mit allen beinhaltenden Lagerfächern erfolgreich gelöscht!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSuccess = new ErrorWindow();
                showSuccess.ShowDialog();
                DialogResult = false;
            }
            else if (TempLocationsModel.SelectedAction == 3)
            {

                DeleteCompartment(TempLocationsModel.CompartmentID);


                //AdministrationQueries.RunSqlExec($"DELETE FROM compartment_item_relations WHERE compartment_id = {TempLocationsModel.CompartmentID}");
                //AdministrationQueries.RunSqlExec($"DELETE FROM compartment_objects WHERE compartment_id = {TempLocationsModel.CompartmentID}");

                ErrorHandlerModel.ErrorText = "Das Lagerfach wurde erfolgreich gelöscht!";
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showSuccess = new ErrorWindow();
                showSuccess.ShowDialog();
                DialogResult = false;
            }
        }
        private void DeleteCompartment(string compartmentId)
        {
            DataSet compartments = AdministrationQueries.RunSql($"SELECT * FROM compartment_item_relations WHERE compartment_id = {compartmentId}");
            if (compartments.Tables[0].Rows.Count > 0)
            {
                AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total = item_quantity_total - {compartments.Tables[0].Rows[0]["item_quantity"]} WHERE item_id = {compartments.Tables[0].Rows[0]["item_id"]}");
                if (compartments.Tables[0].Rows[0]["item_used"].ToString() == "0")
                {
                    AdministrationQueries.RunSqlExec($"UPDATE item_objects SET item_quantity_total_new = item_quantity_total_new - {compartments.Tables[0].Rows[0]["item_quantity"]} WHERE item_id = {compartments.Tables[0].Rows[0]["item_id"]}");
                }
            }
            AdministrationQueries.RunSqlExec($"DELETE FROM compartment_item_relations WHERE compartment_id = {compartmentId}");
            AdministrationQueries.RunSqlExec($"DELETE FROM compartment_objects WHERE compartment_id = {compartmentId}");
        }

    }
}
