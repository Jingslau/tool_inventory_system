using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules
{
    public partial class development_test : UserControl
    {
        // Lists for each filter ComboBox source
        // ObservableCollection to hold the items for the DataGrid
        public ObservableCollection<Item> Items { get; set; }

        // List of filter options for ComboBoxes in each Filter column
        public ObservableCollection<FilterOption> FilterOptions1 { get; set; }
        public ObservableCollection<FilterOption> FilterOptions2 { get; set; }
        public ObservableCollection<FilterOption> FilterOptions3 { get; set; }
        public ObservableCollection<FilterOption> FilterOptions4 { get; set; }
        public ObservableCollection<FilterOption> FilterOptions5 { get; set; }

        public ObservableCollection<Item> OriginalSet { get; set; }



        // List to hold main data


        public development_test()
        {
            InitializeComponent();
            OriginalSet = new ObservableCollection<Item>();
            Items = new ObservableCollection<Item>();

            LoadFilters();



            // Bind the DataContext to this instance to allow ComboBoxes to bind to Filter1Items, etc.
            DataContext = this;

            // Set the DataGrid's ItemsSource to MyDataItems to populate data rows

            LoadData();
        }

        private void LoadFilters()
        {


            //ExtractNamesFromDataSet(AdministrationQueries.RunSql("SELECT * FROM filter1_names ORDER by name ASC"), FilterOptions1);
            //ExtractNamesFromDataSet(AdministrationQueries.RunSql("SELECT * FROM filter2_names ORDER by name ASC"), FilterOptions2);
            //ExtractNamesFromDataSet(AdministrationQueries.RunSql("SELECT * FROM filter3_names ORDER by name ASC"), FilterOptions3);
            //ExtractNamesFromDataSet(AdministrationQueries.RunSql("SELECT * FROM filter4_names ORDER by name ASC"), FilterOptions4);
            //ExtractNamesFromDataSet(AdministrationQueries.RunSql("SELECT * FROM filter5_names ORDER by name ASC"), FilterOptions5);



            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM filter1_names ORDER by name ASC");
            FilterOptions1 = new ObservableCollection<FilterOption> { new FilterOption { FilterId = null, FilterName = "-" } };

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["name"] != DBNull.Value)
                    {
                        FilterOptions1.Add(new FilterOption
                        {
                            FilterId = row["filter_id"].ToString(),
                            FilterName = row["name"].ToString()
                        });

                    }
                }
            }

            ds = AdministrationQueries.RunSql("SELECT * FROM filter2_names ORDER by name ASC");
            FilterOptions2 = new ObservableCollection<FilterOption> { new FilterOption { FilterId = null, FilterName = "-" } };

            if (ds != null && ds.Tables.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["name"] != DBNull.Value)
                    {
                        FilterOptions2.Add(new FilterOption
                        {
                            FilterId = row["filter_id"].ToString(),
                            FilterName = row["name"].ToString()
                        });
                    }
                }
            }
            ds = AdministrationQueries.RunSql("SELECT * FROM filter3_names ORDER by name ASC");
            FilterOptions3 = new ObservableCollection<FilterOption> { new FilterOption { FilterId = null, FilterName = "-" } };

            if (ds != null && ds.Tables.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["name"] != DBNull.Value)
                    {
                        FilterOptions3.Add(new FilterOption
                        {
                            FilterId = row["filter_id"].ToString(),
                            FilterName = row["name"].ToString()
                        });
                    }
                }
            }
            ds = AdministrationQueries.RunSql("SELECT * FROM filter4_names ORDER by name ASC");
            FilterOptions4 = new ObservableCollection<FilterOption> { new FilterOption { FilterId = null, FilterName = "-" } };

            if (ds != null && ds.Tables.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["name"] != DBNull.Value)
                    {
                        FilterOptions4.Add(new FilterOption
                        {
                            FilterId = row["filter_id"].ToString(),
                            FilterName = row["name"].ToString()
                        });
                    }
                }
            }
            ds = AdministrationQueries.RunSql("SELECT * FROM filter5_names ORDER by name ASC");
            FilterOptions5 = new ObservableCollection<FilterOption> { new FilterOption { FilterId = null, FilterName = "-" } };

            if (ds != null && ds.Tables.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["name"] != DBNull.Value)
                    {
                        FilterOptions5.Add(new FilterOption
                        {
                            FilterId = row["filter_id"].ToString(),
                            FilterName = row["name"].ToString()
                        });
                    }
                }
            }

        }

        private void LoadData()
        {
            // Assuming we have a DataSet 'dataSet' with a DataTable containing our items

            Items.Clear();
            OriginalSet.Clear();


            DataSet dataSet = GetDataSetFromDatabase();

            // Fill the ObservableCollection from the DataSet
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Items.Add(new Item
                {
                    item_id = row["item_id"].ToString(),
                    filter1_id = row["filter1_id"].ToString(),
                    filter2_id = row["filter2_id"].ToString(),
                    filter3_id = row["filter3_id"].ToString(),
                    filter4_id = row["filter4_id"].ToString(),
                    filter5_id = row["filter5_id"].ToString(),
                    item_name = row["item_name"].ToString(),
                    item_description = row["item_description"].ToString(),
                    item_description_2 = row["item_description_2"].ToString(),
                    item_diameter = row["item_diameter"].ToString(),
                    item_orderquant_min = row["item_orderquant_min"].ToString(),
                    item_quantity_min = row["item_quantity_min"].ToString(),
                    filter1_name = row["filter1_name"].ToString(),
                    filter2_name = row["filter2_name"].ToString(),
                    filter3_name = row["filter3_name"].ToString(),
                    filter4_name = row["filter4_name"].ToString(),
                    filter5_name = row["filter5_name"].ToString()
                });
                OriginalSet.Add(new Item
                {
                    item_id = row["item_id"].ToString(),
                    filter1_id = row["filter1_id"].ToString(),
                    filter2_id = row["filter2_id"].ToString(),
                    filter3_id = row["filter3_id"].ToString(),
                    filter4_id = row["filter4_id"].ToString(),
                    filter5_id = row["filter5_id"].ToString(),
                    item_name = row["item_name"].ToString(),
                    item_description = row["item_description"].ToString(),
                    item_description_2 = row["item_description_2"].ToString(),
                    item_diameter = row["item_diameter"].ToString(),
                    item_orderquant_min = row["item_orderquant_min"].ToString(),
                    item_quantity_min = row["item_quantity_min"].ToString(),
                    filter1_name = row["filter1_name"].ToString(),
                    filter2_name = row["filter2_name"].ToString(),
                    filter3_name = row["filter3_name"].ToString(),
                    filter4_name = row["filter4_name"].ToString(),
                    filter5_name = row["filter5_name"].ToString()
                });

            }

            // Bind the Items collection to the DataGrid
            itemGrid.ItemsSource = Items;
        }

        private void ExtractNamesFromDataSet(DataSet myDataSet, ObservableCollection<string> filterList)
        {
            if (myDataSet != null && myDataSet.Tables.Count > 0)
            {
                filterList = new ObservableCollection<string>();

                foreach (DataRow row in myDataSet.Tables[0].Rows)
                {
                    if (row["name"] != DBNull.Value)
                    {
                        filterList.Add(row["name"].ToString());
                    }
                }
            }
        }

        private DataSet GetDataSetFromDatabase()
        {
            // Replace this method with actual database code
            DataSet itemDataSet = new DataSet();

            string sqlQuery = @"
           SELECT 
    io.item_ident AS item_name,
    io.item_diameter,
    io.item_orderquant_min, 
    io.item_quantity_min,
    io.item_description, 
    io.item_description_2,
    f1.name AS filter1_name,
    f2.name AS filter2_name,
    f3.name AS filter3_name,
    f4.name AS filter4_name,
    f5.name AS filter5_name,
    ifr.item_id,
    ifr.filter1_id,
    ifr.filter2_id,
    ifr.filter3_id,
    ifr.filter4_id,
    ifr.filter5_id
FROM 
    item_filter_relations ifr
JOIN 
    item_objects io ON ifr.item_id = io.item_id
LEFT JOIN 
    filter1_names f1 ON ifr.filter1_id = f1.filter_id
LEFT JOIN 
    filter2_names f2 ON ifr.filter2_id = f2.filter_id
LEFT JOIN 
    filter3_names f3 ON ifr.filter3_id = f3.filter_id
LEFT JOIN 
    filter4_names f4 ON ifr.filter4_id = f4.filter_id
LEFT JOIN 
    filter5_names f5 ON ifr.filter5_id = f5.filter_id;
";

            itemDataSet = AdministrationQueries.RunSql(sqlQuery);




            return itemDataSet;
        }

        private void SaveChanges_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int changes = 0;

            foreach (Item row in itemGrid.Items)
            {
                changes++;
                string updateFilterSql = $@"
    UPDATE item_filter_relations
    SET filter1_id = {(string.IsNullOrEmpty(row.filter1_id) ? "NULL" : $"'{row.filter1_id}'")},
        filter2_id = {(string.IsNullOrEmpty(row.filter2_id) ? "NULL" : $"'{row.filter2_id}'")},
        filter3_id = {(string.IsNullOrEmpty(row.filter3_id) ? "NULL" : $"'{row.filter3_id}'")},
        filter4_id = {(string.IsNullOrEmpty(row.filter4_id) ? "NULL" : $"'{row.filter4_id}'")},
        filter5_id = {(string.IsNullOrEmpty(row.filter5_id) ? "NULL" : $"'{row.filter5_id}'")}
    WHERE item_id = '{row.item_id}'";
                AdministrationQueries.RunSqlExec(updateFilterSql);


                string updateItemSql = $@"
            UPDATE item_objects SET item_ident = '{row.item_name}', item_description = '{row.item_description}', item_description_2 = '{row.item_description_2}', item_diameter = '{row.item_diameter}', item_orderquant_min ='{row.item_orderquant_min}', item_quantity_min ='{row.item_quantity_min}' WHERE item_id = {row.item_id}";
                AdministrationQueries.RunSqlExec(updateItemSql);

            }

            ErrorHandlerModel.ErrorText = "Die Verarbeitung der Artikel wurde erfolgreich abgeschlossen!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();

        }

        private void RevertChanges_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Items = new ObservableCollection<Item>();

            LoadFilters();



            // Bind the DataContext to this instance to allow ComboBoxes to bind to Filter1Items, etc.
            DataContext = this;

            // Set the DataGrid's ItemsSource to MyDataItems to populate data rows

            LoadData();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (Item row in itemGrid.Items)
            {

                if (row.item_name.ToString().ToLower().Contains(searchBox.Text.ToLower()))
                {
                    // Set the selected index
                    itemGrid.SelectedIndex = itemGrid.Items.IndexOf(row);

                    // Scroll to the selected item
                    itemGrid.ScrollIntoView(row); break;
                }
            }
        }

        private void Integer_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextInteger(e.Text);
        }
        private void Float_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextFloat(e.Text);
        }
        private bool IsTextInteger(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }

        private bool IsTextFloat(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]*(,[0-9]*)?$");
        }
    }
    public class FilterOption
    {
        public string FilterId { get; set; }
        public string FilterName { get; set; }

        // Override ToString to display the name in the dropdown
        public override string ToString()
        {
            return FilterName;
        }
    }
    public class Item
    {
        public string item_id { get; set; }
        public string filter1_id { get; set; }
        public string filter2_id { get; set; }
        public string filter3_id { get; set; }
        public string filter4_id { get; set; }
        public string filter5_id { get; set; }


        public string item_name { get; set; }
        public string item_description { get; set; }
        public string item_description_2 { get; set; }
        public string item_diameter { get; set; }
        public string item_orderquant_min { get; set; }
        public string item_quantity_min { get; set; }

        public string filter1_name { get; set; }
        public string filter2_name { get; set; }
        public string filter3_name { get; set; }
        public string filter4_name { get; set; }
        public string filter5_name { get; set; }

        public override bool Equals(object obj)
        {
            // Check if obj is null or not of type Item
            if (obj == null || !(obj is Item))
                return false;

            var other = (Item)obj;

            // Compare properties one-by-one
            return filter1_id == other.filter1_id &&
                   filter1_name == other.filter1_name &&
                   filter2_id == other.filter2_id &&
                   filter2_name == other.filter2_name &&
                   filter3_id == other.filter3_id &&
                   filter3_name == other.filter3_name &&
                   filter4_id == other.filter4_id &&
                   filter4_name == other.filter4_name &&
                   filter5_id == other.filter5_id &&
                   filter5_name == other.filter5_name &&
                   item_description == other.item_description &&
                   item_description_2 == other.item_description_2 &&
                   item_diameter == other.item_diameter &&
                   item_id == other.item_id &&
                   item_name == other.item_name &&
                   item_orderquant_min == other.item_orderquant_min &&
                   item_quantity_min == other.item_quantity_min;
        }

        public override int GetHashCode()
        {
            unchecked // Allow overflow to handle large hash codes
            {
                int hash = 17;
                hash = hash * 23 + (filter1_id?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter1_name?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter2_id?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter2_name?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter3_id?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter3_name?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter4_id?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter4_name?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter5_id?.GetHashCode() ?? 0);
                hash = hash * 23 + (filter5_name?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_description?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_description_2?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_diameter?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_id?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_name?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_orderquant_min?.GetHashCode() ?? 0);
                hash = hash * 23 + (item_quantity_min?.GetHashCode() ?? 0);
                return hash;
            }
        }

    }

}
