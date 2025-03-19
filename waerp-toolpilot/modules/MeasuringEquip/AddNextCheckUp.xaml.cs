using System;
using System.Windows;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.MeasuringEquip
{
    /// <summary>
    /// Interaction logic for AddNextCheckUp.xaml
    /// </summary>
    public partial class AddNextCheckUp : Window
    {
        public AddNextCheckUp()
        {
            InitializeComponent();
            MeasureEquipName.Text = MeasuringEquipModel.MeasuringEquipID;
            MeasureEquipVendor.Text = MeasuringEquipModel.MeasuringEquipName;
            MeasureEquipQuant.Text = MeasuringEquipModel.MeasuringEquipVendor;
        }
        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }
        private void CreateCheckUp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDate.Text != null && selectedDate.Text != "")
            {
                DateTime selectedDateStr = DateTime.Parse(selectedDate.Text);
                string formattedStringDate = selectedDateStr.ToString("yyyy-MM-dd HH:mm:ss");
                AdministrationQueries.RunSqlExec($"INSERT INTO measuring_equip_history (measuring_equip_history_id, measuring_equip_id, measuring_equip_planned_date, measuring_equip_checkedOn, measuring_equip_history_isChecked) VALUES ({AdministrationQueries.GetMaxId(AdministrationQueries.RunSql("SELECT * FROM measuring_equip_history"), "measuring_equip_history_id")}, {MeasuringEquipModel.MeasuringEquipID}, '{formattedStringDate}', null, 0)");
                ErrorHandlerModel.ErrorType = "SUCCESS";
                ErrorHandlerModel.ErrorText = (string)FindResource("measureEquipHistory5");
                ErrorWindow openSuccess = new ErrorWindow();
                openSuccess.ShowDialog();

                DialogResult = false;

            }
            else
            {
                ErrorHandlerModel.ErrorText = (string)FindResource("measureEquipHistory3");
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow openError = new ErrorWindow();
                openError.ShowDialog();
            }

        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
