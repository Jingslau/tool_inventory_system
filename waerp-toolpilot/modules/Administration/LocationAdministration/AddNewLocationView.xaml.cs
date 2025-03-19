using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for AddNewLocationView.xaml
    /// </summary>
    public partial class AddNewLocationView : Window
    {

        public AddNewLocationView()
        {
            InitializeComponent();


        }
        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void CreateLocation_Click(object sender, RoutedEventArgs e)
        {
            if (LocationValA.Text != "" | LocationValB.Text != "" | LocationValC.Text != "" | LocationValD.Text != "")
            {
                CurrentLocationAdministrationModel.LocationName = LocationValA.Text.Replace(" ", "") + ";" + LocationValB.Text.Replace(" ", "") + ";" + LocationValC.Text.Replace(" ", "") + ";" + LocationValD.Text.Replace(" ", "");
                if (AdministrationQueries.CreateLocation())
                {
                    DialogResult = false;
                }

            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText20");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openNotallowed = new ErrorWindow();
                openNotallowed.ShowDialog();
            }


        }

    }
}
