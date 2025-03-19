using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using waerp_toolpilot.application.RentItem;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.returnItem
{
    /// <summary>
    /// Interaction logic for MachineSelectionRebookWindow.xaml
    /// </summary>
    public partial class MachineSelectionRebookWindow : Window
    {
        String currentmachineId = "";
        String currentMachineStr = null;
        DataRowView current_row = null;
        public MachineSelectionRebookWindow()
        {
            InitializeComponent();

            DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM machines WHERE machine_id != {CurrentReturnModel.currentReturnItem["machine_id"]}");

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



            machineData.DataContext = ds;

            machineData.ItemsSource = new DataView(ds.Tables[0]);
        }

        private void MinusQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) > 0)
            {
                int quant = int.Parse(QuantityInput.Text);
                quant--;
                QuantityInput.Text = quant.ToString();

            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PlusQuantity_Click(object sender, RoutedEventArgs e)
        {
            int quant = int.Parse(QuantityInput.Text);
            quant++;
            QuantityInput.Text = quant.ToString();
        }

        private void QuantityNumInput_Click(object sender, RoutedEventArgs e)
        {
            CurrentRentModel.RentQuantity = QuantityInput.Text.ToString();
            RentNumberInputView numberInput = new RentNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = CurrentRentModel.RentQuantity.ToString();
        }



        private void rebookItem(object sender, RoutedEventArgs e)
        {
            DateTime rentDateTime = DateTime.Now;
            string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            if (QuantityInput.Text != "")
            {
                if (int.Parse(QuantityInput.Text) <= int.Parse(CurrentReturnModel.currentReturnItem["rent_quantity"].ToString()) && int.Parse(QuantityInput.Text) > 0)
                {
                    if ((int.Parse(current_row["tool_quantity"].ToString()) < int.Parse(current_row["machine_max_items"].ToString()) && (int.Parse(current_row["tool_quantity"].ToString()) + int.Parse(QuantityInput.Text)) <= int.Parse(current_row["machine_max_items"].ToString())))
                    {
                        if (int.Parse(QuantityInput.Text) == int.Parse(CurrentReturnModel.currentReturnItem["rent_quantity"].ToString()))
                        {
                            AdministrationQueries.RunSqlExec($"UPDATE item_rents SET machine_id = {currentmachineId} WHERE rent_id = {CurrentReturnModel.currentReturnItem["rent_id"].ToString()}");

                            ErrorHandlerModel.ErrorText = $"Der Artikel wurde erfolgreich auf die Maschine {currentmachineId} umgebucht! (Anzahl: {QuantityInput.Text})";
                            ErrorHandlerModel.ErrorType = "SUCCESS";
                            ErrorWindow showSuccess = new ErrorWindow();
                            showSuccess.ShowDialog();
                            DialogResult = false;
                        }
                        else
                        {
                            AdministrationQueries.RunSqlExec($"UPDATE item_rents SET rent_quantity = {int.Parse(CurrentReturnModel.currentReturnItem["rent_quantity"].ToString()) - int.Parse(QuantityInput.Text)} WHERE rent_id = {CurrentReturnModel.currentReturnItem["rent_id"].ToString()}");
                            AdministrationQueries.RunSqlExec($"INSERT INTO item_rents (item_id, user_id, location_id, rent_quantity, machine_id, item_used, createdAt) VALUES (" +
                                $"{CurrentReturnModel.currentReturnItem["item_id"]}, " +
                                $"{MainWindowViewModel.UserID}," +
                                $"{CurrentReturnModel.currentReturnItem["location_id"]}," +
                                $"{QuantityInput.Text}," +
                                $"{currentmachineId}, " +
                                $"{CurrentReturnModel.currentReturnItem["item_used"]}," +
                                $"'{sqlFormattedDate}'" +
                                $")");
                            ErrorHandlerModel.ErrorText = $"Der Artikel wurde erfolgreich auf die Maschine {currentmachineId} umgebucht! (Anzahl: {QuantityInput.Text})";
                            ErrorHandlerModel.ErrorType = "SUCCESS";

                            string item_ident_str = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {CurrentReturnModel.currentReturnItem["item_id"].ToString()}").Tables[0].Rows[0]["item_ident"].ToString();
                            AdministrationQueries.RunSqlExec($"INSERT INTO history_log (history_id, item_id, item_quantity,item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES ({GetMaxId(AdministrationQueries.RunSql($"SELECT * FROM history_log"), "history_id")}, '{item_ident_str}', {CurrentReturnModel.currentReturnItem["rent_quantity"].ToString()}, '{CurrentReturnModel.MachineName}', '{currentMachineStr}', '0', '0', 11, '{MainWindowViewModel.username}', '{sqlFormattedDate}','{sqlFormattedDate}',1)");
                            ErrorWindow showSuccess = new ErrorWindow();
                            showSuccess.ShowDialog();
                            DialogResult = false;
                        }
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = "Die maximale Anzahl an zu lagernden Artikel für diese Maschine wird überschritten! Bitte wählen Sie einen anderen Wert!";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }
                }
                else
                {
                    ErrorHandlerModel.ErrorText = "Bitte geben Sie eine gültige Menge an! Die Menge muss größer als 0 sein und darf nicht größer als der aktuell ausgeliehene Wert sein!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie eine gültige Menge an! Die Menge muss größer als 0 sein und darf nicht größer als der aktuell ausgeliehene Wert sein!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
        }
        public static string GetMaxId(DataSet ds, string Prompt)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                return "0";
            }
            else
            {
                int maxID = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (int.Parse(row[Prompt].ToString()) > maxID)
                    {
                        maxID = int.Parse(row[Prompt].ToString());
                    }
                }
                maxID++;
                return maxID.ToString();
            }

        }
        private void machineData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                if (int.Parse(row_selected["tool_quantity"].ToString()) < int.Parse(row_selected["machine_max_items"].ToString()))
                {
                    currentmachineId = row_selected["machine_id"].ToString();
                    currentMachineStr = row_selected["name"].ToString();
                    current_row = row_selected;
                    runRebook.IsEnabled = true;
                }
                else
                {
                    runRebook.IsEnabled = false;
                }
            }
        }
        private void closeWindow_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
