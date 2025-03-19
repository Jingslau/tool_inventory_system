using System;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.ReportLocation
{
    /// <summary>
    /// Interaction logic for ReportLocationView.xaml
    /// </summary>
    public partial class ReportLocationView : Window
    {
        public ReportLocationView()
        {
            InitializeComponent();
            ItemIdent.Text = ReportWrongLocationModel.ItemIdent;
            LocationIdent.Text = ReportWrongLocationModel.LocationIdent;

        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ReportWrongLocationModel.Description = Description.Text;
            //if (ErrorReporter.SendErrorReportLocation())
            //{

            AdministrationQueries.RunSql($"INSERT INTO reports_objects (report_id, report_location, report_item, report_user_id, report_note, report_date, report_status) VALUES ({AdministrationQueries.GetMaxId(AdministrationQueries.GetAllInfo("reports_objects"), "report_id")} ,'{ReportWrongLocationModel.ItemIdent}', '{ReportWrongLocationModel.LocationIdent}', '{Description.Text}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 1)");
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorHandlerModel.ErrorText = (string)FindResource("errorText64");
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();
            DialogResult = false;
            //}
            //else
            //{
            //    ErrorHandlerModel.ErrorType = "NOTALLOWED";
            //    ErrorHandlerModel.ErrorText = (string)FindResource("errorText65");
            //    ErrorWindow openFailure = new ErrorWindow();
            //    openFailure.ShowDialog();
            //    DialogResult = false;
            //}

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
