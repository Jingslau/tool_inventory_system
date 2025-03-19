using System.Windows;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;

namespace waerp_toolpilot.Installation
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteDatabaseWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteDatabaseWindow : Window
    {
        public ConfirmDeleteDatabaseWindow()
        {
            InitializeComponent();
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (dbSetup.InitNewDBInstallation())
            {
                DialogResult = false;
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Es ist ein Fehler bei der Erstellung der Datenbank aufgetreten. Bitte wenden Sie sich an den Support!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ErrorButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
