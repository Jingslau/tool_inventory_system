using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for EditLocationWindow.xaml
    /// </summary>
    public partial class EditLocationWindow : Window
    {
        public EditLocationWindow()
        {
            InitializeComponent();
            string[] selectedLocation = new string[4];
            selectedLocation = CurrentLocationAdministrationModel.SelectedLocationName.Split(';');
            LocationValA.Text = selectedLocation[0];
            LocationValB.Text = selectedLocation[1];
            LocationValC.Text = selectedLocation[2];
            LocationValD.Text = selectedLocation[3];
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void EditLocation_Click(object sender, RoutedEventArgs e)
        {
            if (LocationValA.Text != "" | LocationValB.Text != "" | LocationValC.Text != "" | LocationValD.Text != "")
            {
                CurrentLocationAdministrationModel.LocationName = LocationValA.Text.Replace(" ", "") + ";" + LocationValB.Text.Replace(" ", "") + ";" + LocationValC.Text.Replace(" ", "") + ";" + LocationValD.Text.Replace(" ", "");
                if (AdministrationQueries.EditLocation())
                {
                    DialogResult = false;
                }


            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText21");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openNotallowed = new ErrorWindow();
                openNotallowed.ShowDialog();
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
