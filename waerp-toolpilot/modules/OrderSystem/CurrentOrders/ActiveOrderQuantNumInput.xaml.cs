using System.Windows;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.OrderSystem.CurrentOrders
{
    /// <summary>
    /// Interaction logic for ActiveOrderQuantNumInput.xaml
    /// </summary>
    public partial class ActiveOrderQuantNumInput : Window
    {
        public ActiveOrderQuantNumInput()
        {
            InitializeComponent();
            NumberInput.Text = ActiveOrderModel.ItemQuantity.ToString();
        }
        private void NumberNine_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "9";
                }
                else
                {
                    NumberInput.Text += "9";
                }
            }
        }
        private void NumberEight_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "8";
                }
                else
                {
                    NumberInput.Text += "8";
                }
            }
        }
        private void NumberSeven_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "7";
                }
                else
                {
                    NumberInput.Text += "7";
                }
            }
        }
        private void NumberSix_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "6";
                }
                else
                {
                    NumberInput.Text += "6";
                }
            }
        }
        private void NumberFive_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "5";
                }
                else
                {
                    NumberInput.Text += "5";
                }
            }
        }
        private void NumberFour_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "4";
                }
                else
                {
                    NumberInput.Text += "4";
                }
            }
        }
        private void NumberThree_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "3";
                }
                else
                {
                    NumberInput.Text += "3";
                }
            }
        }
        private void NumberTwo_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "2";
                }
                else
                {
                    NumberInput.Text += "2";
                }
            }
        }
        private void NumberOne_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) == 0)
                {
                    NumberInput.Text = "1";
                }
                else
                {
                    NumberInput.Text += "1";
                }
            }
        }
        private void NumberZero_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length < 8)
            {
                if (int.Parse(NumberInput.Text) != 0)
                {
                    NumberInput.Text += "0";
                }
            }
        }

        private void DeleteLastDigit_Click(object sender, RoutedEventArgs e)
        {
            if (NumberInput.Text.Length == 1 && NumberInput.Text != "0")
            {
                NumberInput.Text = "0";
            }
            else if (NumberInput.Text.Length > 1)
            {
                NumberInput.Text = NumberInput.Text.Remove(NumberInput.Text.Length - 1);
            }
        }
        private void SaveNumberInput_Click(object sender, RoutedEventArgs e)
        {

            ActiveOrderModel.ItemQuantity = NumberInput.Text;
            DialogResult = false;
        }
    }
}
