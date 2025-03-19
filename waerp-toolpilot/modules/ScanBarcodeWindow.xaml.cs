using System;
using System.Data;
using System.Windows;
using MySqlConnector;
using waerp_toolpilot.application.SearchItem;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.modules.SearchItem;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application
{
    /// <summary>
    /// Interaktionslogik für ScanBarcodeWindow.xaml
    /// </summary>
    public partial class ScanBarcodeWindow : Window
    {
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public ScanBarcodeWindow()
        {
            InitializeComponent();
            ItemIdentInput.Focus();
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void SearchItem_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand($"Select * from item_objects WHERE item_ident = '{ItemIdentInput.Text}'", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            ItemIdentInput.Text = "";
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        CurrentRentModel.ItemIdent = row["item_id"].ToString();
                        CurrentRentModel.ItemDescription = row["item_description"].ToString();
                        CurrentRentModel.ItemIdentStr = row["item_ident"].ToString();
                        CurrentRentModel.ItemTotalQuantity = row["item_quantity_total"].ToString();
                        CurrentRentModel.ItemImagePath = row["item_image_path"].ToString();
                    }

                    SearchSelectionWindow openSearchResult = new SearchSelectionWindow();
                    openSearchResult.ShowDialog();
                }
                else
                {
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText69");
                    ErrorWindow ErrBox = new ErrorWindow();
                    Nullable<bool> DialogResult = ErrBox.ShowDialog();
                }
            }
            else
            {
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText69");
                ErrorWindow ErrBox = new ErrorWindow();
                Nullable<bool> DialogResult = ErrBox.ShowDialog();

            }
            conn.Close();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ItemIdentInputBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentReturnModel.SearchIdent = ItemIdentInput.Text;
            ItemIdentInputWindow openInput = new ItemIdentInputWindow();
            openInput.ShowDialog();
            ItemIdentInput.Text = CurrentReturnModel.SearchIdent;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
