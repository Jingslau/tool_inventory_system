using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.OrderSystem.CurrentOrders
{
    /// <summary>
    /// Interaction logic for EditOrderQuantWindow.xaml
    /// </summary>
    public partial class EditOrderQuantWindow : Window
    {
        public EditOrderQuantWindow()
        {
            InitializeComponent();

            QuantityInput.Text = ActiveOrderModel.ItemQuantity.ToString();
        }
        private void CloseCurrentDialog(object sender, RoutedEventArgs e)
        {
            ActiveOrderModel.check = true;
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
            if (int.Parse(QuantityInput.Text) > 1)
            {
                int quant = int.Parse(QuantityInput.Text);
                quant--;
                QuantityInput.Text = quant.ToString();

            }

        }

        private void QuantityNumInput_Click(object sender, RoutedEventArgs e)
        {
            ActiveOrderModel.ItemQuantity = QuantityInput.Text.ToString();
            ActiveOrderQuantNumInput numberInput = new ActiveOrderQuantNumInput();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = ActiveOrderModel.ItemQuantity.ToString();
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
                ActiveOrderModel.ItemQuantity = QuantityInput.Text;
                CurrentOrdersQueries.EditOrder(QuantityInput.Text, ActiveOrderModel.CurrentItemId);
                DialogResult = false;
            }


        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
