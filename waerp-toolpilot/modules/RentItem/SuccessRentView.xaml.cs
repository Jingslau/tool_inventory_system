using System;
using System.Windows;
using System.Windows.Media.Imaging;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.rentItem
{
    /// <summary>
    /// Interaction logic for SuccessRentView.xaml
    /// </summary>
    public partial class SuccessRentView : Window
    {
        public SuccessRentView()
        {
            InitializeComponent();
            ItemIdent.Text = (string)FindResource("errorText62") + " " + CurrentRentModel.ItemIdentStr + " " + (string)FindResource("errorText63");
            LocationName.Text = CurrentRentModel.RentLocation;
            if (CurrentRentModel.ItemImagePath != "")
            {
                try
                {
                    Uri imageUri = new Uri(CurrentRentModel.ItemImagePath, UriKind.RelativeOrAbsolute);

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
