using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.Administration.UserAdministration
{
    /// <summary>
    /// Interaction logic for AddNewUserWindow.xaml
    /// </summary>
    public partial class AddNewUserWindow : Window
    {
        public static bool isValidMail = true;
        public static DataSet AllUsers = new DataSet();
        public AddNewUserWindow()
        {
            InitializeComponent();
            addPrivileges.IsEnabled = false;
            removePrivilege.IsEnabled = false;
            DataSet users = AdministrationQueries.GetAllInfo("users");


            AllUsers = users;
            DataSet roles = AdministrationQueries.GetAllInfo("user_roles");
            for (int i = 0; i < roles.Tables[0].Rows.Count; i++)
            {
                user_role.Items.Add(roles.Tables[0].Rows[i]["name"]);
            }
            user_role.SelectedIndex = -1;
            DataSet privileges = AdministrationQueries.GetAllInfo("user_privileges");
            string[] privilegesArr = new string[privileges.Tables[0].Rows.Count];

            for (int i = 0; i < privileges.Tables[0].Rows.Count; i++)
            {
                unselectedPrivileges.Items.Add(privileges.Tables[0].Rows[i]["privileges_name_DE"].ToString());
            }

        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text == "" | user_ident.Text == "" | user_password.Password == "" | name.Text == "" | surname.Text == "")
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("errorText23");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }

            else
            {
                bool check = false;
                bool check2 = false;
                for (int i = 0; i < AllUsers.Tables[0].Rows.Count; i++)
                {
                    if (AllUsers.Tables[0].Rows[i]["username"].ToString() == username.Text)
                    {
                        check = true;
                    }
                    if (AllUsers.Tables[0].Rows[i]["user_ident"].ToString() == user_ident.Text)
                    {
                        check2 = true;
                    }
                }
                if (check && check2)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText24");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow openError = new ErrorWindow();
                    openError.ShowDialog();
                }
                else if (check)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText25");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow openError = new ErrorWindow();
                    openError.ShowDialog();
                }
                else if (check2)
                {
                    ErrorHandlerModel.ErrorText = (string)FindResource("errorText26");
                    ErrorHandlerModel.ErrorType = "NOTALLOWED";
                    ErrorWindow openError = new ErrorWindow();
                    openError.ShowDialog();
                }
                else
                {
                    string[] itemArray = new string[selectedPrivileges.Items.Count];
                    for (int i = 0; i < selectedPrivileges.Items.Count; i++)
                    {
                        itemArray[i] = selectedPrivileges.Items[i].ToString();
                    }

                    if (AdministrationQueries.AddNewUser(user_ident.Text, username.Text, user_password.Password, name.Text, surname.Text, email.Text, user_role.Text, itemArray))
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("errorText27");
                        ErrorHandlerModel.ErrorType = "SUCCESS";
                        ErrorWindow openSuccess = new ErrorWindow();
                        openSuccess.ShowDialog();
                        DialogResult = false;
                    }

                }
            }


        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true; // Cancels the input
                    break;
                }
            }
        }
        private void LetterTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetter(c))
                {
                    e.Handled = true; // Cancels the input
                    break;
                }
            }
        }
        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            string mail = email.Text;

            if (!IsValidEmail(mail))
            {

                isValidMail = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void NoWhitespaceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Cancels the input
            }
        }

        private void selectedPrivileges_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {


            unselectedPrivileges.SelectedItem = null;
            addPrivileges.IsEnabled = false;
            removePrivilege.IsEnabled = true;


        }

        private void unselectedPrivileges_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedPrivileges.SelectedItem = null;
            addPrivileges.IsEnabled = true;
            removePrivilege.IsEnabled = false;
        }

        private void removePrivilege_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPrivileges.SelectedItems != null)
            {
                var selectedItem = selectedPrivileges.SelectedItem;
                selectedPrivileges.Items.Remove(selectedItem);
                unselectedPrivileges.Items.Add(selectedItem);
                addPrivileges.IsEnabled = false;
                removePrivilege.IsEnabled = false;
                if (selectedPrivileges.Items.Count != 0)
                {
                    selectedPrivileges.SelectedIndex = 0;
                }
                else
                {
                    unselectedPrivileges.SelectedIndex = 0;
                }
            }
        }

        private void addPrivileges_Click(object sender, RoutedEventArgs e)
        {
            if (unselectedPrivileges.SelectedItems != null)
            {
                var selectedItem = unselectedPrivileges.SelectedItem;
                unselectedPrivileges.Items.Remove(selectedItem);
                selectedPrivileges.Items.Add(selectedItem);
                addPrivileges.IsEnabled = false;
                removePrivilege.IsEnabled = false;
                if (unselectedPrivileges.Items.Count != 0)
                {
                    unselectedPrivileges.SelectedIndex = 0;
                }
                else
                {
                    selectedPrivileges.SelectedIndex = 0;
                }
            }
        }

        private void unselectedPrivileges_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (unselectedPrivileges.SelectedItems != null)
            {
                var selectedItem = unselectedPrivileges.SelectedItem;
                unselectedPrivileges.Items.Remove(selectedItem);
                selectedPrivileges.Items.Add(selectedItem);
                addPrivileges.IsEnabled = false;
                removePrivilege.IsEnabled = false;
                if (unselectedPrivileges.Items.Count != 0)
                {
                    unselectedPrivileges.SelectedIndex = 0;
                }
                else
                {
                    selectedPrivileges.SelectedIndex = 0;
                }
            }

        }

        private void selectedPrivileges_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (selectedPrivileges.SelectedItems != null)
            {
                var selectedItem = selectedPrivileges.SelectedItem;
                selectedPrivileges.Items.Remove(selectedItem);
                unselectedPrivileges.Items.Add(selectedItem);
                addPrivileges.IsEnabled = false;
                removePrivilege.IsEnabled = false;
                if (selectedPrivileges.Items.Count != 0)
                {
                    selectedPrivileges.SelectedIndex = 0;
                }
                else
                {
                    unselectedPrivileges.SelectedIndex = 0;
                }
            }

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
