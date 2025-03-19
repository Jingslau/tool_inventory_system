using System;
using System.Windows;
using Microsoft.Win32;
using waerp_toolpilot.errorHandling;

namespace waerp_toolpilot.mainGUI
{
    /// <summary>
    /// Interaction logic for OrderSystemAlertWindow.xaml
    /// </summary>
    public partial class OrderSystemAlertWindow : Window
    {
        public bool StopReminder = false;
        public OrderSystemAlertWindow()
        {
            InitializeComponent();
            ErrorWindowText.Text = ErrorHandlerModel.ErrorText;
        }

        private void ErrorButtonClose_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            DateTime now = DateTime.Now;
            if (StopReminder)
            {
                key.SetValue("RemindMeLaterDate", now.ToString("g"));
            }
            else
            {
                key.SetValue("RemindMeLaterDate", "");
            }
            DialogResult = false;
        }

        private void StopRemindMe_Click(object sender, RoutedEventArgs e)
        {
            if (StopRemindMe.IsChecked == true)
            {
                StopReminder = true;
            }
            else
            {
                StopReminder = false;
            }
        }
    }
}
