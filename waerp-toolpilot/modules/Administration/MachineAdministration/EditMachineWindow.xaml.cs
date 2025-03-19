using System;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.modules.Administration.MachineAdministration
{
    /// <summary>
    /// Interaction logic for EditMachineWindow.xaml
    /// </summary>
    public partial class EditMachineWindow : Window
    {
        public EditMachineWindow()
        {
            InitializeComponent();
            MachineIndex_Text.Text = "Aktueller Maschinenindex: " + CurrentItemAdministrationModel.SelectedItem["machine_group_index"].ToString();
            MachineName_Text.Text = "Aktueller Maschinenname: " + CurrentItemAdministrationModel.SelectedItem["name"].ToString();
            MachineName.Text = CurrentItemAdministrationModel.SelectedItem["name"].ToString();
            MachineIndex.Text = CurrentItemAdministrationModel.SelectedItem["machine_group_index"].ToString();
            MagazineSize.Text = CurrentItemAdministrationModel.SelectedItem["machine_magazine_size"].ToString();
            MaxItems.Text = CurrentItemAdministrationModel.SelectedItem["machine_max_items"].ToString();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }
        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CreateItem_Click(object sender, RoutedEventArgs e)
        {
            if (MachineIndex.Text != "" && MachineIndex.Text != "")
            {
                if (AdministrationQueries.RunSql($"SELECT * FROM machines WHERE name = '{MachineName.Text}'").Tables[0].Rows.Count <= 1)
                {

                    if (MagazineSize.Text != "" && MaxItems.Text != "")
                    {

                        if (int.Parse(MagazineSize.Text) <= 0 && int.Parse(MaxItems.Text) <= 0)
                        {


                            ErrorHandlerModel.ErrorText = "Bitte geben Sie einen Wert für die maximale Anzahl von Artikeln an, die auf dieser Maschine gelagert werden können!";
                            ErrorHandlerModel.ErrorType = "NOTALLOWED";
                            ErrorWindow showError = new ErrorWindow();
                            showError.ShowDialog();
                        }
                        else
                        {

                            AdministrationQueries.RunSqlExec($"UPDATE machines SET name = '{MachineName.Text}', machine_group_index = '{MachineIndex.Text}', machine_magazine_size = '{MagazineSize.Text}', machine_max_items = '{MaxItems.Text}' WHERE machine_id = {CurrentItemAdministrationModel.SelectedItem["machine_id"]}");


                            ErrorHandlerModel.ErrorText = "Die Maschine wurde erfolgreich bearbeitet!";
                            ErrorHandlerModel.ErrorType = "SUCCESS";
                            ErrorWindow showSuccess = new ErrorWindow();
                            showSuccess.ShowDialog();

                            DialogResult = false;

                        }
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = "Bitte geben Sie einen Wert für die maximale Anzahl von Artikeln an, die auf dieser Maschine gelagert werden können!";
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }

                }
                else
                {
                    ErrorHandlerModel.ErrorText = "Es existiert bereits eine Maschine mit diesem Namen!";
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow showError = new ErrorWindow();
                    showError.ShowDialog();
                }


            }
            else
            {
                ErrorHandlerModel.ErrorText = "Bitte geben Sie einen Maschinenindex und Maschinennamen an!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }

        }

        private void MagazineSize_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Allow only numeric characters, comma, and period
            e.Handled = !IsTextAllowed(e.Text);

        }
        private bool IsTextAllowed(string text)
        {
            // Allow only numeric characters, comma, and period
            return Array.TrueForAll(text.ToCharArray(), c => char.IsDigit(c) || c == ',' || c == '.');
        }
    }
}
