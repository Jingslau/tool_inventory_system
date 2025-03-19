using System;
using System.Data;
using System.Windows.Controls;
using waerp_toolpilot.sql;
using waerp_toolpilot.store.Administration;

namespace waerp_toolpilot.application.Administration.CustomerAdministration
{
    /// <summary>
    /// Interaction logic for CustomerAdministrationView.xaml
    /// </summary>
    public partial class CustomerAdministrationView : UserControl
    {


        public CustomerAdministrationView()
        {
            InitializeComponent();
            CustomerDataItems.DataContext = AdministrationQueries.GetAllInfo("customer_objects");
            CustomerDataItems.ItemsSource = new DataView(AdministrationQueries.GetAllInfo("customer_objects").Tables[0]);
        }

        private void CustomerDataItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                CurrentCustomerModel.CustomerIDNumber = row_selected["customer_company_id"].ToString();
                CurrentCustomerModel.SelectedCustomerName = row_selected["customer_name"].ToString();
                CurrentCustomerModel.CustomerID = row_selected["customer_id"].ToString();
                CurrentCustomerModel.CustomerName = row_selected["customer_name"].ToString();
                CurrentCustomerModel.CustomerAdress = row_selected["customer_adress"].ToString();
                CurrentCustomerModel.CustomerPostcode = row_selected["customer_postcode"].ToString();
                CurrentCustomerModel.CustomerCity = row_selected["customer_city"].ToString();
                CurrentCustomerModel.CustomerCountry = row_selected["customer_country"].ToString();
                CurrentCustomerModel.CustomerWebsite = row_selected["customer_website"].ToString();
                CurrentCustomerModel.CustomerPhone = row_selected["customer_phone"].ToString();
                CurrentCustomerModel.CustomerMail = row_selected["customer_mail"].ToString();
                CurrentCustomerModel.CustomerContact = row_selected["customer_contact"].ToString();
            }
        }

        private void EditCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditCustomerWindow openEdit = new EditCustomerWindow();
            Nullable<bool> dialogResult = openEdit.ShowDialog();
            CustomerDataItems.DataContext = AdministrationQueries.GetAllInfo("customer_objects");
            CustomerDataItems.ItemsSource = new DataView(AdministrationQueries.GetAllInfo("customer_objects").Tables[0]);

        }

        private void DeleteCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmDeleteWindow openConfirm = new ConfirmDeleteWindow();
            openConfirm.ShowDialog();
            CustomerDataItems.DataContext = AdministrationQueries.GetAllInfo("customer_objects");
            CustomerDataItems.ItemsSource = new DataView(AdministrationQueries.GetAllInfo("customer_objects").Tables[0]);
        }

        private void AddCustomerBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddNewCustomerWindow openAdd = new AddNewCustomerWindow();
            openAdd.ShowDialog();
            CustomerDataItems.DataContext = AdministrationQueries.GetAllInfo("customer_objects");
            CustomerDataItems.ItemsSource = new DataView(AdministrationQueries.GetAllInfo("customer_objects").Tables[0]);

        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet ds = AdministrationQueries.GetAllInfo("customer_objects");
                DataSet output = AdministrationQueries.GetAllInfo("customer_objects");
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["customer_name"].ToString().ToLower().Contains(searchBox.Text.ToLower()) |
                        row["customer_company_id"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_adress"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_city"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_country"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_contact"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_mail"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_website"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["customer_phone"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                         | row["customer_postcode"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                CustomerDataItems.DataContext = output;
                CustomerDataItems.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                CustomerDataItems.DataContext = AdministrationQueries.GetAllInfo("customer_objects");
                CustomerDataItems.ItemsSource = new DataView(AdministrationQueries.GetAllInfo("customer_objects").Tables[0]);
            }
        }
    }
}
