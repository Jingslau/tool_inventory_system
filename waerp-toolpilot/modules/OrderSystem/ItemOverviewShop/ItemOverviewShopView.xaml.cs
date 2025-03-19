using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.application.OrderSystem.ItemOverviewShop;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.OrderSystem
{
    static class ItemsDataSets
    {
        public static DataSet AllItems = new DataSet();
        public static DataSet NeededItems = new DataSet();
        public static DataSet MinItems = new DataSet();
        public static DataSet OnOrderItems = new DataSet();
        public static DataSet ShoppingCart = new DataSet();
        public static DataRow CurrentSelectedRow;
        public static DataRow CurrentSelectedCartItem;
        public static DataSet CurrentDataSet = new DataSet();
    }
    /// <summary>
    /// Interaction logic for ItemOverviewShopView.xaml
    /// </summary>
    public partial class ItemOverviewShopView : UserControl
    {
        public ItemOverviewShopView()
        {
            InitializeComponent();


            LoadData();

        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public async void LoadData()
        {
            // Show buffer loader
            LoadingPanel.Visibility = Visibility.Visible;
            NoInternetPanel.Visibility = Visibility.Hidden;
            // Hide main content
            mainGrid.Visibility = Visibility.Hidden;

            if (!CheckForInternetConnection())
            {
                mainGrid.Visibility = Visibility.Hidden;
                NoInternetPanel.Visibility = Visibility.Visible;
                LoadingPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                // Start the data loading process asynchronously
                await Task.Run(() => DataLoader());

                // Data loading completed
                // Update the UI with the loaded data

                // Hide buffer loader
                NoInternetPanel.Visibility = Visibility.Hidden;
                LoadingPanel.Visibility = Visibility.Hidden;

                // Show main content
                mainGrid.Visibility = Visibility.Visible;

                // Update your UI elements with the loaded data
            }


        }

        public void DataLoader()
        {
            ItemsDataSets.AllItems = OrderItemOverviewQueries.GetAllItems();

            Dispatcher.Invoke(() =>
            {
                GridItems.DataContext = ItemsDataSets.AllItems;
                GridItems.ItemsSource = new DataView(ItemsDataSets.AllItems.Tables[0]);
            });

            ItemsDataSets.NeededItems = OrderItemOverviewQueries.GetAllItemsNeeded();



            ItemsDataSets.MinItems = OrderItemOverviewQueries.GetAllItemsMin();

            ItemsDataSets.OnOrderItems = OrderItemOverviewQueries.GetAllItemsOrdered();

            ItemsDataSets.ShoppingCart = ItemsDataSets.AllItems.Copy();
            ItemsDataSets.ShoppingCart.Tables[0].Clear();
            Dispatcher.Invoke(() =>
            {
                if (ItemsDataSets.NeededItems.Tables[0].Rows.Count > 0)
                {
                    AddAllNeededBtn.IsEnabled = true;
                }
                else
                {
                    AddAllNeededBtn.IsEnabled = false;
                }
                ShoppingCartGrid.DataContext = ItemsDataSets.ShoppingCart;
                ShoppingCartGrid.ItemsSource = new DataView(ItemsDataSets.ShoppingCart.Tables[0]);
            });

        }





        private void allItems_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GridItems.DataContext = ItemsDataSets.AllItems;
            GridItems.ItemsSource = new DataView(ItemsDataSets.AllItems.Tables[0]);
        }

        private void neededItems_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GridItems.DataContext = ItemsDataSets.NeededItems;
            GridItems.ItemsSource = new DataView(ItemsDataSets.NeededItems.Tables[0]);
        }

        private void AlmostItems_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GridItems.DataContext = ItemsDataSets.MinItems;
            GridItems.ItemsSource = new DataView(ItemsDataSets.MinItems.Tables[0]);
        }

        private void OnOrderItems_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GridItems.DataContext = ItemsDataSets.OnOrderItems;
            GridItems.ItemsSource = new DataView(ItemsDataSets.OnOrderItems.Tables[0]);
        }

        private void GridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                ItemsDataSets.CurrentSelectedRow = row_selected.Row;
            }
        }

        private void AddToCart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool check = false;
            for (var i = 0; i < ItemsDataSets.ShoppingCart.Tables[0].Rows.Count; i++)
            {
                if (ItemsDataSets.ShoppingCart.Tables[0].Rows[i][0].ToString() == ItemsDataSets.CurrentSelectedRow[0].ToString())
                {
                    check = true; break;
                }
            }
            if (check == false)
            {
                ItemOverviewQuantSelection numberInput = new ItemOverviewQuantSelection();
                Nullable<bool> dialogResult = numberInput.ShowDialog();
                if (ShoppingCartModel.check == false)
                {
                    ItemsDataSets.CurrentSelectedRow["order_quantity"] = ShoppingCartModel.ItemQuantity;
                    ItemsDataSets.ShoppingCart.Tables[0].ImportRow(ItemsDataSets.CurrentSelectedRow);
                }
                ShoppingCartModel.check = false;

            }

            else if (ShoppingCartModel.check == false)

            {
                MessageBox.Show((string)FindResource("errorText48"));
            }
            if (ItemsDataSets.ShoppingCart.Tables[0].Rows.Count > 0)
            {
                CreateOrder.IsEnabled = true;

                EmptyCartBtn.IsEnabled = true;
            }
            else
            {
                CreateOrder.IsEnabled = false;

                EmptyCartBtn.IsEnabled = false;
            }

        }

        private void CheckOut(object sender, RoutedEventArgs e)
        {
            ShoppingCartModel.ShoppingCartInput = ItemsDataSets.ShoppingCart;

            CheckOutView openCheckOut = new CheckOutView();
            Nullable<bool> dialogResult = openCheckOut.ShowDialog();
            if (ShoppingCartModel.check == true)
            {
                ItemsDataSets.ShoppingCart = ItemsDataSets.AllItems.Copy();
                ItemsDataSets.ShoppingCart.Tables[0].Clear();
                ShoppingCartGrid.DataContext = ItemsDataSets.ShoppingCart;
                ShoppingCartGrid.ItemsSource = new DataView(ItemsDataSets.ShoppingCart.Tables[0]);
                AddAllNeededBtn.IsEnabled = true;
                EmptyCartBtn.IsEnabled = false;
                ShoppingCartModel.check = false;
                loadingText.Text = (string)FindResource("errorText49");
                LoadData();
            }
        }
        private void EmptyCartBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemsDataSets.ShoppingCart = ItemsDataSets.AllItems.Copy();
            ItemsDataSets.ShoppingCart.Tables[0].Clear();
            ShoppingCartGrid.DataContext = ItemsDataSets.ShoppingCart;
            ShoppingCartGrid.ItemsSource = new DataView(ItemsDataSets.ShoppingCart.Tables[0]);

            EmptyCartBtn.IsEnabled = false;
            EditItemBtn.IsEnabled = false;
            CreateOrder.IsEnabled = false;
            DeleteItemBtn.IsEnabled = false;
            AddAllNeededBtn.IsEnabled = true;
        }
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            ItemsDataSets.ShoppingCart.Tables[0].Rows.Remove(ItemsDataSets.CurrentSelectedCartItem);
            DeleteItemBtn.IsEnabled = false;
            EditItemBtn.IsEnabled = false;
            if (ItemsDataSets.ShoppingCart.Tables[0].Rows.Count <= 0)
            {
                EmptyCartBtn.IsEnabled = false;
                EditItemBtn.IsEnabled = false;
                CreateOrder.IsEnabled = false;
                DeleteItemBtn.IsEnabled = false;
                AddAllNeededBtn.IsEnabled = true;
            }
            else
            {
                CreateOrder.IsEnabled = true;
            }
        }

        private void ShoppingCartGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                DeleteItemBtn.IsEnabled = true;
                EditItemBtn.IsEnabled = true;
                ItemsDataSets.CurrentSelectedCartItem = row_selected.Row;
            }
        }

        private void EditItemBtn_Click(object sender, RoutedEventArgs e)
        {
            ShoppingCartModel.ItemQuantity = ItemsDataSets.CurrentSelectedCartItem["order_quantity"].ToString();
            EditItemCartQuantWindow openEdit = new EditItemCartQuantWindow();
            Nullable<bool> dialogResult = openEdit.ShowDialog();
            ItemsDataSets.CurrentSelectedCartItem["order_quantity"] = ShoppingCartModel.ItemQuantity.ToString();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (allItems.IsChecked == true)
            {
                neededItems.IsEnabled = false;
                AlmostItems.IsEnabled = false;
                OnOrderItems.IsEnabled = false;
                if (searchBox.Text != "")
                {
                    DataSet output = ItemsDataSets.AllItems.Copy();
                    output.Tables[0].Rows.Clear();

                    foreach (DataRow row in ItemsDataSets.AllItems.Tables[0].Rows)
                    {
                        if (row["item_ident"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description_2"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_diameter"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                        {
                            output.Tables[0].ImportRow(row);
                        }
                    }
                    GridItems.DataContext = output;
                    GridItems.ItemsSource = new DataView(output.Tables[0]);
                }
                else
                {
                    GridItems.DataContext = ItemsDataSets.AllItems;

                    GridItems.ItemsSource = new DataView(ItemsDataSets.AllItems.Tables[0]);
                    neededItems.IsEnabled = true;
                    AlmostItems.IsEnabled = true;
                    OnOrderItems.IsEnabled = true;
                }
            }
            else if (neededItems.IsChecked == true)
            {
                allItems.IsEnabled = false;
                AlmostItems.IsEnabled = false;
                OnOrderItems.IsEnabled = false;
                if (searchBox.Text != "")
                {
                    DataSet output = ItemsDataSets.NeededItems.Copy();
                    output.Tables[0].Rows.Clear();

                    foreach (DataRow row in ItemsDataSets.NeededItems.Tables[0].Rows)
                    {
                        if (row["item_ident"].ToString().Contains(searchBox.Text) | row["item_description"].ToString().Contains(searchBox.Text) | row["vendor"].ToString().Contains(searchBox.Text))
                        {
                            output.Tables[0].ImportRow(row);
                        }
                    }
                    GridItems.DataContext = output;
                    GridItems.ItemsSource = new DataView(output.Tables[0]);
                }
                else
                {
                    GridItems.DataContext = ItemsDataSets.NeededItems;
                    GridItems.ItemsSource = new DataView(ItemsDataSets.NeededItems.Tables[0]);
                    allItems.IsEnabled = true;
                    AlmostItems.IsEnabled = true;
                    OnOrderItems.IsEnabled = true;
                }
            }
            else if (AlmostItems.IsChecked == true)
            {
                allItems.IsEnabled = false;
                neededItems.IsEnabled = false;
                OnOrderItems.IsEnabled = false;
                if (searchBox.Text != "")
                {
                    DataSet output = ItemsDataSets.MinItems.Copy();
                    output.Tables[0].Rows.Clear();

                    foreach (DataRow row in ItemsDataSets.MinItems.Tables[0].Rows)
                    {
                        if (row["item_ident"].ToString().Contains(searchBox.Text) | row["item_description"].ToString().Contains(searchBox.Text) | row["vendor"].ToString().Contains(searchBox.Text))
                        {
                            output.Tables[0].ImportRow(row);
                        }
                    }
                    GridItems.DataContext = output;
                    GridItems.ItemsSource = new DataView(output.Tables[0]);
                }
                else
                {
                    GridItems.DataContext = ItemsDataSets.MinItems;
                    GridItems.ItemsSource = new DataView(ItemsDataSets.MinItems.Tables[0]);
                    allItems.IsEnabled = true;
                    neededItems.IsEnabled = true;
                    OnOrderItems.IsEnabled = true;
                }
            }
            else if (OnOrderItems.IsChecked == true)
            {
                allItems.IsEnabled = false;
                neededItems.IsEnabled = false;
                AlmostItems.IsEnabled = false;
                if (searchBox.Text != "")
                {
                    DataSet output = ItemsDataSets.MinItems.Copy();
                    output.Tables[0].Rows.Clear();

                    foreach (DataRow row in ItemsDataSets.OnOrderItems.Tables[0].Rows)
                    {
                        if (row["item_ident"].ToString().Contains(searchBox.Text) | row["item_description"].ToString().Contains(searchBox.Text) | row["vendor"].ToString().Contains(searchBox.Text))
                        {
                            output.Tables[0].ImportRow(row);
                        }
                    }
                    GridItems.DataContext = output;
                    GridItems.ItemsSource = new DataView(output.Tables[0]);
                }
                else
                {
                    GridItems.DataContext = ItemsDataSets.OnOrderItems;
                    GridItems.ItemsSource = new DataView(ItemsDataSets.OnOrderItems.Tables[0]);
                    allItems.IsEnabled = true;
                    AlmostItems.IsEnabled = true;
                    neededItems.IsEnabled = true;
                }
            }
        }

        private void AddAllNeeded(object sender, RoutedEventArgs e)
        {

            bool check = false;
            for (var i = 0; i < ItemsDataSets.ShoppingCart.Tables[0].Rows.Count; i++)
            {
                for (var j = 0; j < ItemsDataSets.NeededItems.Tables[0].Rows.Count; j++)
                {
                    if (ItemsDataSets.ShoppingCart.Tables[0].Rows[i][0].ToString() == ItemsDataSets.NeededItems.Tables[0].Rows[j]["item_id"].ToString())
                    {
                        check = true; break;
                    }
                }

            }
            if (check == false)
            {


                for (var j = 0; j < ItemsDataSets.NeededItems.Tables[0].Rows.Count; j++)
                {
                    ItemsDataSets.ShoppingCart.Tables[0].ImportRow(ItemsDataSets.NeededItems.Tables[0].Rows[j]);
                    ItemsDataSets.ShoppingCart.Tables[0].Rows[ItemsDataSets.ShoppingCart.Tables[0].Rows.Count - 1]["order_quantity"] = ItemsDataSets.NeededItems.Tables[0].Rows[j]["item_quantity_min"];
                }


                CreateOrder.IsEnabled = true;
                EmptyCartBtn.IsEnabled = true;
                AddAllNeededBtn.IsEnabled = false;
            }

            else if (ShoppingCartModel.check == false)

            {
                MessageBox.Show((string)FindResource("errorText50"));
            }
        }

        private void CopyItemIdent(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ItemsDataSets.CurrentSelectedRow["item_ident"].ToString());
        }

        private void CopyDescription(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ItemsDataSets.CurrentSelectedRow["item_description"].ToString());
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ItemsDataSets.CurrentSelectedRow["item_ident"].ToString() + "; " + ItemsDataSets.CurrentSelectedRow["item_description"].ToString());
        }
    }
}
