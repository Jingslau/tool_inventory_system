using System;
using System.Windows;
using System.Windows.Media.Imaging;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.rebookItem
{
    /// <summary>
    /// Interaction logic for RebookSuccessWindow.xaml
    /// </summary>
    public partial class RebookSuccessWindow : Window
    {
        public RebookSuccessWindow()
        {
            InitializeComponent();
            OldLocationName.Text = CurrentRebookModel.OldLocationName;
            ItemIdent.Text = (string)FindResource("errorText54") + " " + CurrentRebookModel.ItemIdentStr + " " + (string)FindResource("errorText55");
            NewLocationName.Text = CurrentRebookModel.NewLocationName;

            if (CurrentRebookModel.ItemImagePath != "")
            {
                try
                {
                    Uri imageUri = new Uri(CurrentRebookModel.ItemImagePath, UriKind.RelativeOrAbsolute);

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
