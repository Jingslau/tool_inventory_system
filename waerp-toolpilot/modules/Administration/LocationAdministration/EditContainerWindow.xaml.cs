using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.modules.Administration.LocationAdministration
{
    /// <summary>
    /// Interaction logic for EditContainerWindow.xaml
    /// </summary>
    public partial class EditContainerWindow : Window
    {
        DataSet containerInfo = AdministrationQueries.RunSql($"SELECT * FROM container_objects WHERE container_id = {TempLocationsModel.ContainerID}");
        public EditContainerWindow()
        {
            InitializeComponent();

            ContainerNameLong.Text = containerInfo.Tables[0].Rows[0]["container_name_long"].ToString();
            ContainerNameShort.Text = containerInfo.Tables[0].Rows[0]["container_name_short"].ToString();
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
        private void EditContainer_Click(object sender, RoutedEventArgs e)
        {
            Boolean check = false;
            if (ContainerNameShort.Text.Length > 0 && ContainerNameLong.Text.Length > 0)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM container_objects WHERE container_name_long = '{ContainerNameLong.Text}'");
                DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM container_objects WHERE container_name_short = '{ContainerNameShort.Text}'");




                if (ContainerNameLong.Text == containerInfo.Tables[0].Rows[0]["container_name_long"].ToString() && ContainerNameShort.Text == containerInfo.Tables[0].Rows[0]["container_name_short"].ToString())
                {
                    DialogResult = false;
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0 && ContainerNameLong.Text != containerInfo.Tables[0].Rows[0]["container_name_long"].ToString())
                    {
                        check = true;
                    }
                    else if (ds2.Tables[0].Rows.Count > 0 && ContainerNameShort.Text != containerInfo.Tables[0].Rows[0]["container_name_short"].ToString())
                    {
                        check = true;
                    }

                    if (!check)
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE container_objects SET container_name_long = '{ContainerNameLong.Text}', container_name_short = '{ContainerNameShort.Text}' WHERE container_id = {TempLocationsModel.ContainerID}");
                        ErrorHandlerModel.ErrorText = (string)FindResource("editLocationText1a");
                        ErrorHandlerModel.ErrorType = "SUCCESS";
                        ErrorWindow showSuccess = new ErrorWindow();
                        showSuccess.ShowDialog();
                        DialogResult = false;
                    }
                    else
                    {
                        ErrorHandlerModel.ErrorText = (string)FindResource("editLocationText1");
                        ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        ErrorWindow showError = new ErrorWindow();
                        showError.ShowDialog();
                    }
                }
            }
        }
    }
}
