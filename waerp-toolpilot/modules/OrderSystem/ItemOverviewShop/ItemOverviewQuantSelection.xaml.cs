using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.OrderSystem.ItemOverviewShop
{
    /// <summary>
    /// Interaction logic for ItemOverviewQuantSelection.xaml
    /// </summary>
    public partial class ItemOverviewQuantSelection : Window
    {
        public ItemOverviewQuantSelection()
        {
            InitializeComponent();
            QuantityInput.Text = ItemsDataSets.CurrentSelectedRow["item_orderquant_min"].ToString();
        }
        private void CloseCurrentDialog(object sender, RoutedEventArgs e)
        {
            ShoppingCartModel.check = true;
            DialogResult = false;

        }

        private void PlusQuantity_Click(object sender, RoutedEventArgs e)
        {
            int quant = int.Parse(QuantityInput.Text);
            quant++;
            QuantityInput.Text = quant.ToString();
        }

        private void MinusQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) > int.Parse(ItemsDataSets.CurrentSelectedRow["item_orderquant_min"].ToString()))
            {
                int quant = int.Parse(QuantityInput.Text);
                quant--;
                QuantityInput.Text = quant.ToString();

            }

        }

        private void QuantityNumInput_Click(object sender, RoutedEventArgs e)
        {
            ShoppingCartModel.ItemQuantity = QuantityInput.Text.ToString();
            OrderNumberInputView numberInput = new OrderNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            if (int.Parse(ShoppingCartModel.ItemQuantity.ToString()) >= int.Parse(ItemsDataSets.CurrentSelectedRow["item_orderquant_min"].ToString()))
            {
                QuantityInput.Text = ShoppingCartModel.ItemQuantity.ToString();

            }
            else
            {
                QuantityInput.Text = ItemsDataSets.CurrentSelectedRow["item_orderquant_min"].ToString();
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!int.Parse(QuantityInput.Text).Equals(0))
            {
                ShoppingCartModel.ItemQuantity = QuantityInput.Text;
                DialogResult = false;
            }


        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
