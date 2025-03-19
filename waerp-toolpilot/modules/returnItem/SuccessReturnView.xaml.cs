using System;
using System.Windows;
using System.Windows.Media.Imaging;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.returnItem
{
    /// <summary>
    /// Interaction logic for SuccessReturnView.xaml
    /// </summary>
    public partial class SuccessReturnView : Window
    {
        public SuccessReturnView()
        {
            InitializeComponent();
            ItemIdent.Text = (string)FindResource("errorText54") + " " + CurrentReturnModel.ItemIdentStr + " " + (string)FindResource("errorText56");
            LocationName.Text = CurrentReturnModel.ReturnLocation;
            if (CurrentReturnModel.ItemImagePath != "")
            {
                try
                {
                    Uri imageUri = new Uri(CurrentReturnModel.ItemImagePath, UriKind.RelativeOrAbsolute);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = imageUri;
                    bitmapImage.EndInit();

                    ItemImage.Source = bitmapImage;
                }
                catch (Exception exp)
                {
                    ErrorLogger.LogSysError(exp);
                    Uri imageUri = new Uri("pack://application:,,,/assets/images/default/default.jpg", UriKind.RelativeOrAbsolute);

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = imageUri;
                    bitmapImage.EndInit();

                    ItemImage.Source = bitmapImage;
                }
            }
            else
            {
                Uri imageUri = new Uri("pack://application:,,,/assets/images/default/default.jpg", UriKind.RelativeOrAbsolute);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = imageUri;
                bitmapImage.EndInit();

                ItemImage.Source = bitmapImage;

            }
        }
        private void CloseCurrentDialog(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
