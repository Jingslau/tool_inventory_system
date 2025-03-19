using System.Data;
using System.Windows.Controls;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.OrderSystem.ManageOldOrders
{
    /// <summary>
    /// Interaction logic for OldOrdersOverviewView.xaml
    /// </summary>
    public partial class OldOrdersOverviewView : UserControl
    {
        public static DataSet allOrderIdents;
        public static DataSet selectedOrderItems;

        public OldOrdersOverviewView()
        {
            InitializeComponent();
            allOrderIdents = AdministrationQueries.RunSql("SELECT * FROM order_objects WHERE order_status = 0");
            if (allOrderIdents.Tables[0].Rows.Count > 0)
            {
                orderData.DataContext = allOrderIdents;
                orderData.ItemsSource = new DataView(allOrderIdents.Tables[0]);
                orderData.SelectedIndex = -1;
                searchBoxOrder.IsEnabled = true;
                searchBox.IsEnabled = true;
            }
            else
            {
                searchBoxOrder.IsEnabled = false;
                searchBox.IsEnabled = false;

            }
            searchBoxOrder.IsEnabled = false;
            searchBox.IsEnabled = false;
        }



        private void orderData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            selectedOrderItems = AdministrationQueries.RunSql($"SELECT * FROM item_objects where item_id = 0");
            selectedOrderItems.Tables[0].Rows.Clear();
            selectedOrderItems.Tables[0].Columns.Add("order_quantity_org");
            selectedOrderItems.Tables[0].Columns.Add("createdAt");


            if (row_selected != null)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM order_item_relations WHERE order_ident = '{row_selected["order_ident"]}'");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}");
                        if (tmp.Tables[0].Rows.Count > 0)
                        {
                            selectedOrderItems.Tables[0].ImportRow(tmp.Tables[0].Rows[0]);
                            selectedOrderItems.Tables[0].Rows[i]["order_quantity_org"] = ds.Tables[0].Rows[i]["order_quantity_org"];
                            selectedOrderItems.Tables[0].Rows[i]["createdAt"] = ds.Tables[0].Rows[i]["createdAt"];
                        }
                    }
                    dataGridItems.DataContext = selectedOrderItems;
                    dataGridItems.ItemsSource = new DataView(selectedOrderItems.Tables[0]);
                    searchBoxOrder.IsEnabled = true;
                    searchBox.IsEnabled = true;
                }
            }
        }

        private void searchBoxOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBoxOrder.Text != "")
            {
                DataSet output = allOrderIdents.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in allOrderIdents.Tables[0].Rows)
                {
                    if (row["order_ident"].ToString().Contains(searchBoxOrder.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                orderData.DataContext = output;
                orderData.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                orderData.DataContext = allOrderIdents;

                orderData.ItemsSource = new DataView(allOrderIdents.Tables[0]);
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = selectedOrderItems.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in selectedOrderItems.Tables[0].Rows)
                {

                    if (row["item_ident"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description_2"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_diameter"].ToString().ToLower().Contains(searchBox.Text.ToLower()))

                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                dataGridItems.DataContext = output;
                dataGridItems.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                dataGridItems.DataContext = selectedOrderItems;

                dataGridItems.ItemsSource = new DataView(selectedOrderItems.Tables[0]);
            }
        }

        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
