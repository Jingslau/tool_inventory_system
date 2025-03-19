using System;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.RebookSystem.RebookFloorGroup
{
    /// <summary>
    /// Interaction logic for ConfirmRebookFloorGroupWindow.xaml
    /// </summary>
    public partial class ConfirmRebookFloorGroupWindow : Window
    {
        public ConfirmRebookFloorGroupWindow()
        {
            InitializeComponent();
            OldLocationName.Text = RebookGroupModel.CurrentGroupName;
            NewLocationName.Text = RebookGroupModel.NewLocationName;
        }

        private void ConfirmRebook_Click(object sender, RoutedEventArgs e)
        {
            RebookGroupQueries.RebookFloorGroup();
            ErrorHandlerModel.ErrorText = "Die Palette wurde erfolgreich im System umgebucht. Bitte stelle sicher, dass die Palette/n im richtigen Lagerort sind!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            Nullable<bool> dialogResult = openSuccess.ShowDialog();
            DialogResult = false;
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
