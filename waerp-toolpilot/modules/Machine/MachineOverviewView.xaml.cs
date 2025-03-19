using System.Data;
using System.Windows.Controls;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Machine
{
    /// <summary>
    /// Interaction logic for MachineOverviewView.xaml
    /// </summary>
    public partial class MachineOverviewView : UserControl
    {
        public MachineOverviewView()
        {
            InitializeComponent();
            LoadMachineData();

        }

        private void LoadMachineData()
        {
            DataSet machines = new DataSet();

            machines = AdministrationQueries.GetAllInfo("machines");

            if (machines.Tables[0].Rows.Count > 0)
            {
                machines.Tables[0].Columns.Add("quantity");

                for (int i = 0; i < machines.Tables[0].Rows.Count; i++)
                {
                    DataSet tmp = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE machine_id = {machines.Tables[0].Rows[i]["machine_id"]}");
                    if (tmp.Tables[0].Rows.Count > 0)
                    {
                        int totalQuant = 0;

                        for (int j = 0; j < tmp.Tables[0].Rows.Count; j++)
                        {
                            totalQuant += int.Parse(tmp.Tables[0].Rows[j]["rent_quantity"].ToString());
                        }

                        machines.Tables[0].Rows[i]["quantity"] = totalQuant.ToString();
                    }
                    else
                    {
                        machines.Tables[0].Rows[i]["quantity"] = "0";
                    }
                }

                MachineDataItems.DataContext = machines;
                MachineDataItems.ItemsSource = new DataView(machines.Tables[0]);
                MachineDataItems.SelectedIndex = 0;
            }
        }

        private void MachineDataItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemsInMachineData.DataContext = null;
            ItemsInMachineData.ItemsSource = null;
            DataSet itemsInMachine = new DataSet();

            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                itemsInMachine = AdministrationQueries.RunSql($"SELECT * FROM item_rents WHERE machine_id = {row_selected["machine_id"]}");
            }

            if (itemsInMachine.Tables[0].Rows.Count > 0)
            {
                itemsInMachine.Tables[0].Columns.Add("user");
                itemsInMachine.Tables[0].Columns.Add("item_description");
                itemsInMachine.Tables[0].Columns.Add("item_description_2");

                itemsInMachine.Tables[0].Columns.Add("item_ident");

                for (int i = 0; i < itemsInMachine.Tables[0].Rows.Count; i++)
                {
                    itemsInMachine.Tables[0].Rows[i]["item_description"] = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {itemsInMachine.Tables[0].Rows[i]["item_id"]}").Tables[0].Rows[0]["item_description"].ToString();
                    itemsInMachine.Tables[0].Rows[i]["item_description_2"] = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {itemsInMachine.Tables[0].Rows[i]["item_id"]}").Tables[0].Rows[0]["item_description_2"].ToString();

                    itemsInMachine.Tables[0].Rows[i]["item_ident"] = AdministrationQueries.RunSql($"SELECT * FROM item_objects WHERE item_id = {itemsInMachine.Tables[0].Rows[i]["item_id"]}").Tables[0].Rows[0]["item_ident"].ToString();
                    itemsInMachine.Tables[0].Rows[i]["user"] = AdministrationQueries.RunSql($"SELECT * FROM users WHERE user_id = {itemsInMachine.Tables[0].Rows[i]["user_id"]}").Tables[0].Rows[0]["username"].ToString();
                }

                ItemsInMachineData.DataContext = itemsInMachine;
                ItemsInMachineData.ItemsSource = new DataView(itemsInMachine.Tables[0]);
            }
        }
    }
}
