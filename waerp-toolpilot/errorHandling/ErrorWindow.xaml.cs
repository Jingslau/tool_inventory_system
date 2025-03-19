using System;
using System.Windows;
using System.Windows.Media;
using waerp_toolpilot.store.Administration;
using waerp_toolpilot.ViewModels;

namespace waerp_toolpilot.errorHandling
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
            if (ErrorHandlerModel.ErrorType == "ERROR")
            {
                if (!CurrentUserAdministrationModel.ShowSendReportBtn)
                {
                    SendErrorReport.Visibility = Visibility.Collapsed;
                }
                else
                {
                    SendErrorReport.Visibility = Visibility.Visible;
                }
                if (BufferLoaderModel.BufferType == "REPORT")
                {
                    MessageTitle.Text = "ERFOLG";
                    MessageTitle.Foreground = GetColorFromHexa("#4265A9");
                    ErrorButtonClose.Content = "Schließen";
                    ErrorWindowBorder.Background = GetColorFromHexa("#4265A9");
                    ErrorWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.MessageReplyTextOutline;
                    ErrorWindowIcon.Foreground = GetColorFromHexa("#4265A9");
                    ErrorWindowText.Text = "Der Fehlerbericht wurde gesendet. Es wird sich in kürze jemand bei der hinterlegten Rufnummer melden.";

                    SendErrorReport.HorizontalAlignment = HorizontalAlignment.Right;
                    SendErrorReport.Visibility = Visibility.Hidden;
                    ErrorButtonClose.HorizontalAlignment = HorizontalAlignment.Left;
                    BufferLoaderModel.BufferType = "";
                }
                else
                {
                    MessageTitle.Text = "FEHLER";
                    MessageTitle.Foreground = GetColorFromHexa("#FF5252");
                    ErrorButtonClose.Content = "Schließen";
                    ErrorWindowBorder.Background = GetColorFromHexa("#FF5252");
                    ErrorWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.AlertOutline;
                    ErrorWindowIcon.Foreground = GetColorFromHexa("#FF5252");
                    ErrorWindowText.Text = ErrorHandlerModel.ErrorText;
                    SendErrorReport.Background = GetColorFromHexa("#FF5252");
                }



            }
            else if (ErrorHandlerModel.ErrorType == "NOTALLOWED")
            {
                MessageTitle.Text = "FEHLER";
                MessageTitle.Foreground = GetColorFromHexa("#FF5252");
                ErrorButtonClose.Content = "OK";
                ErrorWindowBorder.Background = GetColorFromHexa("#FF5252");
                ErrorWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                ErrorWindowIcon.Foreground = GetColorFromHexa("#FF5252");
                ErrorWindowText.Text = ErrorHandlerModel.ErrorText;
                SendErrorReport.HorizontalAlignment = HorizontalAlignment.Right;
                SendErrorReport.Visibility = Visibility.Hidden;
                ErrorButtonClose.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (ErrorHandlerModel.ErrorType == "WARNING")
            {
                MessageTitle.Text = "WARNUNG";
                MessageTitle.Foreground = GetColorFromHexa("#FF5252");
                ErrorButtonClose.Content = "OK";
                ErrorWindowBorder.Background = GetColorFromHexa("#FF5252");
                ErrorWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                ErrorWindowIcon.Foreground = GetColorFromHexa("#FF5252");
                ErrorWindowText.Text = ErrorHandlerModel.ErrorText;
                SendErrorReport.HorizontalAlignment = HorizontalAlignment.Right;
                SendErrorReport.Visibility = Visibility.Hidden;
                ErrorButtonClose.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (ErrorHandlerModel.ErrorType == "SUCCESS")
            {
                MessageTitle.Text = "ERFOLG";
                MessageTitle.Foreground = GetColorFromHexa("#42A95F");
                ErrorButtonClose.Content = "Schließen";
                ErrorWindowBorder.Background = GetColorFromHexa("#42A95F");
                ErrorWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedCircleOutline;
                ErrorWindowIcon.Foreground = GetColorFromHexa("#42A95F");
                ErrorWindowText.Text = ErrorHandlerModel.ErrorText;

                SendErrorReport.HorizontalAlignment = HorizontalAlignment.Right;
                SendErrorReport.Visibility = Visibility.Hidden;
                ErrorButtonClose.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (ErrorHandlerModel.ErrorType == "INFO")
            {
                MessageTitle.Text = "INFO";
                MessageTitle.Foreground = GetColorFromHexa("#4265A9");
                ErrorButtonClose.Content = "Schließen";
                ErrorWindowBorder.Background = GetColorFromHexa("#4265A9");
                ErrorWindowIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.MessageReplyTextOutline;
                ErrorWindowIcon.Foreground = GetColorFromHexa("#4265A9");
                ErrorWindowText.Text = ErrorHandlerModel.ErrorText;
                SendErrorReport.HorizontalAlignment = HorizontalAlignment.Right;
                SendErrorReport.Visibility = Visibility.Hidden;
                ErrorButtonClose.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            byte r = Convert.ToByte(hexaColor.Substring(1, 2), 16);
            byte g = Convert.ToByte(hexaColor.Substring(3, 2), 16);
            byte b = Convert.ToByte(hexaColor.Substring(5, 2), 16);
            SolidColorBrush soliColorBrush = new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
            return soliColorBrush;
        }

        private void ErrorButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SendErrorReport_Click(object sender, RoutedEventArgs e)
        {
            BufferLoaderModel.BufferTitle = "Fehlerbericht wird gesendet";
            BufferLoaderModel.BufferType = "REPORT";


            LoadingHandler w = new LoadingHandler();
            w.Start();


            ErrorReporter.SendErrorReport("SQL ERROR", ErrorHandlerModel.ErrorTime, ErrorHandlerModel.ErrorMessage);
            w.Stop();
            ErrorWindow successReport = new ErrorWindow();
            Nullable<bool> dialogResult = successReport.ShowDialog();
            DialogResult = false;


        }

    }
}
