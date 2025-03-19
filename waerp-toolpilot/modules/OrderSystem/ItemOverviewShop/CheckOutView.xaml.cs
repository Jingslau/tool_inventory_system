using System;
using System.Data;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;
using waerp_toolpilot.ViewModels;

namespace waerp_toolpilot.application.OrderSystem.ItemOverviewShop
{
    /// <summary>
    /// Interaction logic for CheckOutView.xaml
    /// </summary>
    public partial class CheckOutView : Window
    {
        public CheckOutView()
        {
            InitializeComponent();
            GridItems.DataContext = ShoppingCartModel.ShoppingCartInput;
            GridItems.ItemsSource = new DataView(ShoppingCartModel.ShoppingCartInput.Tables[0]);
        }

        private void GridItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void CancleCeckOut(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void FinishOrder(object sender, RoutedEventArgs e)
        {
            if (ShoppingCartModel.ShoppingCartInput.Tables[0].Rows.Count > 0)
            {


                BufferLoaderModel.BufferTitle = (string)FindResource("errorText46");
                BufferLoaderModel.BufferType = "REPORT";
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText46");


                LoadingHandler w = new LoadingHandler();
                w.Start();


                bool check1 = OrderItemOverviewQueries.CreateOrder();
                w.Stop();


                ShoppingCartModel.check = true;
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText47");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorWindow showResult = new ErrorWindow();
                Nullable<bool> dialogResult = showResult.ShowDialog();
                DialogResult = false;

            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
