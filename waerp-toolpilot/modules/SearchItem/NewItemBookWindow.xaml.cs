using System;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.application.RentItem;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.SearchItem
{
    /// <summary>
    /// Interaction logic for NewItemBookWindow.xaml
    /// </summary>
    public partial class NewItemBookWindow : Window
    {
        public NewItemBookWindow()
        {
            InitializeComponent();
            ItemIdent.Text = CurrentRentModel.ItemIdentStr;
            ItemDescription.Text = CurrentRentModel.ItemDescription;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QuantityInputNum_Click(object sender, RoutedEventArgs e)
        {
            CurrentRentModel.RentQuantity = QuantityInput.Text.ToString();
            RentNumberInputView numberInput = new RentNumberInputView();
            Nullable<bool> dialogResult = numberInput.ShowDialog();
            QuantityInput.Text = CurrentRentModel.RentQuantity.ToString();
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            int quant = int.Parse(QuantityInput.Text);
            quant++;
            QuantityInput.Text = quant.ToString();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(QuantityInput.Text) > 0)
            {
                int quant = int.Parse(QuantityInput.Text);
                quant--;
                QuantityInput.Text = quant.ToString();

            }
        }

        private void QuantityInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true; // Cancels the input
                    break;
                }
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BookItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentRentModel.RentQuantity = QuantityInput.Text;
            if (BookItemQueries.BookNewItemInput())
            {
                DialogResult = false;

            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
