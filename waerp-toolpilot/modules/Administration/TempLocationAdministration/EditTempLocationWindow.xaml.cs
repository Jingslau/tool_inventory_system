using System.Windows;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.TempLocationAdministration
{
    /// <summary>
    /// Interaction logic for EditTempLocationWindow.xaml
    /// </summary>
    public partial class EditTempLocationWindow : Window
    {
        public EditTempLocationWindow()
        {
            InitializeComponent();
            LocationValA.Text = CurrentLocationAdministrationModel.SelectedLocationName;
        }

        private void EditTempLocaiton_Click(object sender, RoutedEventArgs e)
        {
            CurrentLocationAdministrationModel.LocationName = LocationValA.Text;
            if (AdministrationQueries.EditTempLocation())
            {
                DialogResult = false;
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
