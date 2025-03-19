using System;
using System.Data;
using System.Windows;
using waerp_toolpilot.application.returnItem;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.main;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.returnItem
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteReturn.xaml
    /// </summary>
    public partial class ConfirmDeleteReturn : Window
    {
        public ConfirmDeleteReturn()
        {
            InitializeComponent();
            QuantityInput.Text = CurrentReturnModel.currentReturnItem["rent_quantity"].ToString();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) <= int.Parse(CurrentReturnModel.currentReturnItem["rent_quantity"].ToString()))
            {
                CurrentReturnModel.ReturnQuantity = QuantityInput.Text;
                if (int.Parse(QuantityInput.Text) == int.Parse(CurrentReturnModel.currentReturnItem["rent_quantity"].ToString())) { ReturnItemQueries.DeleteRent(); }
                else
                {
                    ReturnItemQueries.UpdateRent();
                }
                DateTime rentDateTime = DateTime.Now;
                string sqlFormattedDate = rentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string item_ident_str = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {CurrentReturnModel.currentReturnItem["item_id"]}").Tables[0].Rows[0]["item_ident"].ToString();
                string machineName = AdministrationQueries.RunSql($"SELECT * FROM machines WHERE machine_id = {CurrentReturnModel.currentReturnItem["machine_id"]}").Tables[0].Rows[0]["name"].ToString();

                AdministrationQueries.RunSqlExec($"INSERT INTO history_log (history_id, item_id, item_quantity,item_location_old, item_location_new, old_zone, new_zone, action_id, user_id, createdAt, updatedAt, show_trigger) VALUES ({GetMaxId(AdministrationQueries.RunSql($"SELECT * FROM history_log"), "history_id")}, " +
                    $"'{item_ident_str}', " +
                    $"{QuantityInput.Text}, " +
                    $"'{machineName}', " +
                    $"'', 0, 0, 6, " +
                    $"'{MainWindowViewModel.username}', " +
                    $"'{sqlFormattedDate}','{sqlFormattedDate}',{1})");
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText66");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();
                DialogResult = false;
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Die Menge darf nicht größer als die ausgeliehene Anzahl an Artikel sein!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
        }

        private void PlusQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) <= int.Parse(CurrentReturnModel.currentReturnItem["rent_quantity"].ToString()))
            {
                int quant = int.Parse(QuantityInput.Text);
                quant++;
                QuantityInput.Text = quant.ToString();
            }

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

        private void QuantityNumInput_Click(object sender, RoutedEventArgs e)
        {
            CurrentReturnModel.ReturnQuantity = QuantityInput.Text.ToString();
            ReturnNumberInputView numberInput = new ReturnNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = CurrentReturnModel.ReturnQuantity.ToString();
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            CurrentReturnModel.ReturnQuantity = QuantityInput.Text.ToString();
            ReturnNumberInputView numberInput = new ReturnNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = CurrentReturnModel.ReturnQuantity.ToString();
        }
    }
}
