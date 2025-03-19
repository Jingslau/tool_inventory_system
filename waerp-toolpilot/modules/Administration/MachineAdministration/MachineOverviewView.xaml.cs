using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.modules.Administration.MachineAdministration
{
    /// <summary>
    /// Interaction logic for MachineOverviewView.xaml
    /// </summary>
    public partial class MachineOverviewView : UserControl
    {
        DataSet allMachines = new DataSet();
        DataRow selectedMachine;
        public MachineOverviewView()
        {
            InitializeComponent();

            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM machines");
            ds.Tables[0].Columns.Add("tool_quantity");

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE machine_id = {ds.Tables[0].Rows[i]["machine_id"]}");

                ds.Tables[0].Rows[i]["tool_quantity"] = tmp.Tables[0].Rows.Count;
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                machineData.SelectedIndex = 0;

                EditMachine.IsEnabled = true;
                DeleteMachine.IsEnabled = true;
            }
            else
            {
                EditMachine.IsEnabled = false;
                DeleteMachine.IsEnabled = false;
            }

            allMachines = ds;

            machineData.DataContext = ds;
            machineData.ItemsSource = new DataView(ds.Tables[0]);
        }

        private void reloadData()
        {
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM machines");
            ds.Tables[0].Columns.Add("tool_quantity");

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE machine_id = {ds.Tables[0].Rows[i]["machine_id"]}");

                ds.Tables[0].Rows[i]["tool_quantity"] = tmp.Tables[0].Rows.Count;
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                machineData.SelectedIndex = 0;

                EditMachine.IsEnabled = true;
                DeleteMachine.IsEnabled = true;
            }
            else
            {
                EditMachine.IsEnabled = false;
                DeleteMachine.IsEnabled = false;
            }

            allMachines = ds;

            if (ds.Tables[0].Rows.Count > 0)
            {

                machineData.DataContext = ds;
                machineData.ItemsSource = new DataView(ds.Tables[0]);
                machineData.SelectedIndex = 0;
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = allMachines.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in allMachines.Tables[0].Rows)
                {
                    if (row["name"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["machine_group_index"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                machineData.DataContext = output;
                machineData.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                machineData.DataContext = allMachines;
                machineData.ItemsSource = new DataView(allMachines.Tables[0]);

                machineData.SelectedIndex = 0;
            }


        }
        private void EditItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                selectedMachine = row_selected.Row;
                CurrentItemAdministrationModel.SelectedItem = row_selected.Row;
            }
        }

        private void DeleteMachine_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMachine != null && int.Parse(selectedMachine["tool_quantity"].ToString()) == 0)
            {

                ConfirmDeleteMachineWindow showDelete = new ConfirmDeleteMachineWindow();
                showDelete.ShowDialog();
                reloadData();
            }
            else
            {
                ErrorHandlerModel.ErrorText = "Es sind noch Werkzeuge in dieser Maschine eingelagert. Bitte lagern Sie diese Werkzeuge um, damit die Maschine gelöscht werden kann!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();

                showError.ShowDialog();
                reloadData();
            }
        }

        private void AddMachine_Click(object sender, RoutedEventArgs e)
        {
            AddMachineWindow openAddMachine = new AddMachineWindow();
            openAddMachine.ShowDialog();
            reloadData();
        }

        private void EditMachine_Click(object sender, RoutedEventArgs e)
        {
            EditMachineWindow showEdit = new EditMachineWindow();
            showEdit.ShowDialog();
            reloadData();
        }
    }
}
