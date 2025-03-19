using System.Windows;
using waerp_toolpilot.application.BookItem;
using waerp_toolpilot.application.rentItem;
using waerp_toolpilot.modules.SearchItem;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.SearchItem
{
    /// <summary>
    /// Interaction logic for SearchSelectionWindow.xaml
    /// </summary>
    public partial class SearchSelectionWindow : Window
    {
        public SearchSelectionWindow()
        {
            InitializeComponent();
            FoundItem.Text = CurrentRentModel.ItemIdentStr + "\n" + CurrentRentModel.ItemDescription;
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void RentItemBtn_Click(object sender, RoutedEventArgs e)
        {
            RentSelectedItemView openRent = new RentSelectedItemView();
            openRent.ShowDialog();







        }

        private void Book_Item(object sender, RoutedEventArgs e)
        {
            BookItemSelectionView openBook = new BookItemSelectionView();
            openBook.ShowDialog();
        }

        private void BookNewItem_Click(object sender, RoutedEventArgs e)
        {
            NewItemBookWindow openBook = new NewItemBookWindow();
            openBook.ShowDialog();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
