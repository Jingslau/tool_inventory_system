using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.application.rentItem;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.RentItem
{
    /// <summary>
    /// Interaction logic for SelectMachineWindow.xaml
    /// </summary>
    public partial class SelectMachineWindow : Window
    {
        public SelectMachineWindow()
        {
            InitializeComponent();
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM machines");
            ds.Tables[0].Columns.Add("tool_quantity");

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE machine_id = {ds.Tables[0].Rows[i]["machine_id"]}");

                ds.Tables[0].Rows[i]["tool_quantity"] = tmp.Tables[0].Rows.Count;
            }

            if (ds.Tables[0].Rows.Count > 0)
            {

                machineData.SelectedIndex = 0;
            }


            quantity.Text = CurrentRentModel.RentQuantity.ToString();
            machineData.DataContext = ds;

            machineData.ItemsSource = new DataView(ds.Tables[0]);
        }

        private void machineData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                if (CurrentRentModel.ItemIdent != "" && int.Parse(row_selected["tool_quantity"].ToString()) < int.Parse(row_selected["machine_max_items"].ToString()) && (int.Parse(row_selected["tool_quantity"].ToString()) + int.Parse(CurrentRentModel.RentQuantity)) <= int.Parse(row_selected["machine_max_items"].ToString()))
                {
                    saveSelection.IsEnabled = true;
                }
                else
                {
                    saveSelection.IsEnabled = false;
                }
                SearchParams.machine = row_selected["name"].ToString();
                CurrentRentModel.MachineID = row_selected["machine_id"].ToString();
            }
        }

        private void getLastStage(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentRentModel.closeWindow = false;
            DialogResult = false;
        }

        private void rentItem(object sender, System.Windows.RoutedEventArgs e)
        {
            if (RentItemQueries.RentItemExec(CurrentRentModel.newQuant, CurrentRentModel.RentQuantity, CurrentRentModel.locationID, CurrentRentModel.locationQuantity, CurrentRentModel.zoneName, CurrentRentModel.isUsed))
            {
                SuccessRentView successDialog = new SuccessRentView();
                Nullable<bool> dialogResult = successDialog.ShowDialog();
                CurrentRentModel.closeWindow = true;
                DialogResult = false;
            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText58");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow notAllowed = new ErrorWindow();
                notAllowed.ShowDialog();

            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
