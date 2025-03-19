using System.Windows;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.SearchItem
{
    /// <summary>
    /// Interaction logic for ItemIdentInputWindow.xaml
    /// </summary>
    public partial class ItemIdentInputWindow : Window
    {
        public ItemIdentInputWindow()
        {
            InitializeComponent();
            NumberInput.Text = CurrentReturnModel.SearchIdent;
        }
        private void NumberNine_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "9";
        }
        private void NumberEight_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "8";
        }
        private void NumberSeven_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "7";
        }
        private void NumberSix_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "6";
        }
        private void NumberFive_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "5";
        }
        private void NumberFour_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "4";
        }
        private void NumberThree_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "3";
        }
        private void NumberTwo_Click(object sender, RoutedEventArgs e)
        {
            NumberInput.Text += "2";
        }
        private void NumberOne_Click(object sender, RoutedEventArgs e)
        {


            NumberInput.Text += "1";

        }
        private void NumberZero_Click(object sender, RoutedEventArgs e)
        {

            NumberInput.Text += "0";

        }

        private void DeleteLastDigit_Click(object sender, RoutedEventArgs e)
        {

            if (NumberInput.Text.Length > 1)
            {
                NumberInput.Text = NumberInput.Text.Remove(NumberInput.Text.Length - 1);
            }
        }
        private void SaveNumberInput_Click(object sender, RoutedEventArgs e)
        {
            CurrentReturnModel.SearchIdent = NumberInput.Text;
            DialogResult = false;
        }
    }
}
