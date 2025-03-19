using System;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.MachineAdministration
{
    /// <summary>
    /// Interaction logic for AddMachineWindow.xaml
    /// </summary>
    public partial class AddMachineWindow : Window
    {
        public AddMachineWindow()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse button is pressed
            DragMove();
        }

        private void CreateItem_Click(object sender, RoutedEventArgs e)
        {
            if (MachineIndex.Text != "" || MachineIndex.Text != "")
            {
                if (AdministrationQueries.RunSql($"SELECT * FROM machines WHERE name = '{MachineName.Text}'").Tables[0].Rows.Count == 0)
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
                            AdministrationQueries.RunSqlExec($"INSERT INTO machines (name, machine_group_index, machine_magazine_size, machine_max_items) VALUES ('{MachineName.Text}', '{MachineIndex.Text}', '{MagazineSize.Text}', '{MaxItems.Text}')");


                            ErrorHandlerModel.ErrorText = "Die Maschine wurde erfolgreich angelegt!";
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

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

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
