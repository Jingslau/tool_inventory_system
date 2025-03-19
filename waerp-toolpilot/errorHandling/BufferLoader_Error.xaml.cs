using System;
using System.Windows;
using System.Windows.Media;

namespace waerp_toolpilot.errorHandling
{
    /// <summary>
    /// Interaction logic for BufferLoader_Error.xaml
    /// </summary>
    public partial class BufferLoader_Error : Window
    {
        public BufferLoader_Error()
        {
            InitializeComponent();
            MessageTitle.Text = ErrorHandlerModel.ErrorText;
        }

        public static void SendReport()
        {
            if (ErrorReporter.SendErrorReport("SQL ERROR", ErrorHandlerModel.ErrorTime, ErrorHandlerModel.ErrorMessage))
            {

                ErrorWindow showSuccessReport = new ErrorWindow();
                Nullable<bool> dialogResult = showSuccessReport.ShowDialog();
                dialogResult = false;
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
    }
}
