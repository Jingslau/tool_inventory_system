using System;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.RebookSystem.RebookGroup
{
    /// <summary>
    /// Interaction logic for RebookGroupConfirmWindow.xaml
    /// </summary>
    public partial class RebookGroupConfirmWindow : Window
    {
        public RebookGroupConfirmWindow()
        {
            InitializeComponent();
            TextGroup1.Text = RebookGroupModel.RebookGroupText1;
            TextGroup2.Text = RebookGroupModel.RebookGroupText2;




        }

        private void ConfirmRebook_Click(object sender, RoutedEventArgs e)
        {
            RebookGroupQueries.RebookGroup();
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
