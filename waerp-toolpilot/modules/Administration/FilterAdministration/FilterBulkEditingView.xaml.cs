using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.FilterAdministration
{
    /// <summary>
    /// Interaction logic for FilterBulkEditingView.xaml
    /// </summary>
    public partial class FilterBulkEditingView : UserControl
    {
        public ObservableCollection<FilterList> Filter1List { get; set; }
        public ObservableCollection<FilterList> Filter2List { get; set; }
        public ObservableCollection<FilterList> Filter3List { get; set; }
        public ObservableCollection<FilterList> Filter4List { get; set; }
        public ObservableCollection<FilterList> Filter5List { get; set; }

        public FilterBulkEditingView()
        {
            InitializeComponent();
            Filter1List = new ObservableCollection<FilterList>();
            Filter2List = new ObservableCollection<FilterList>();
            Filter3List = new ObservableCollection<FilterList>();
            Filter4List = new ObservableCollection<FilterList>();
            Filter5List = new ObservableCollection<FilterList>();



            DataContext = this;

            loadFilter1();
            loadFilter2();
            loadFilter3();
            loadFilter4();
            loadFilter5();
        }

        public void loadFilter1()
        {
            Filter1List.Clear(); DataSet ds = AdministrationQueries.RunSql("SELECT * FROM filter1_names ORDER by name ASC");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    Filter1List.Add(new FilterList
                    {
                        filter_id = row["filter_id"].ToString(),
                        name = row["name"].ToString()
                    });
                }
                filterItems_1.ItemsSource = Filter1List;
            }
        }
        public void loadFilter2()
        {
            Filter2List.Clear();

            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM filter2_names ORDER by name ASC");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    Filter2List.Add(new FilterList
                    {
                        filter_id = row["filter_id"].ToString(),
                        name = row["name"].ToString()
                    });
                }
                filterItems_2.ItemsSource = Filter2List;
            }
        }
        public void loadFilter3()
        {
            Filter3List.Clear();

            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM filter3_names ORDER by name ASC");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    Filter3List.Add(new FilterList
                    {
                        filter_id = row["filter_id"].ToString(),
                        name = row["name"].ToString()
                    });
                }
                filterItems_3.ItemsSource = Filter3List;
            }
        }
        public void loadFilter4()
        {
            Filter4List.Clear();

            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM filter4_names ORDER by name ASC");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    Filter4List.Add(new FilterList
                    {
                        filter_id = row["filter_id"].ToString(),
                        name = row["name"].ToString()
                    });
                }
                filterItems_4.ItemsSource = Filter4List;
            }
        }
        public void loadFilter5()
        {
            Filter5List.Clear();
            DataSet ds = AdministrationQueries.RunSql("SELECT * FROM filter5_names ORDER by name ASC");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    Filter5List.Add(new FilterList
                    {
                        filter_id = row["filter_id"].ToString(),
                        name = row["name"].ToString()
                    });
                }
                filterItems_5.ItemsSource = Filter5List;
            }
        }



        private void filterItems_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterItems_1.SelectedItem != null)
            {
                filterItems_2.SelectedIndex = -1;
                filterItems_3.SelectedIndex = -1;
                filterItems_4.SelectedIndex = -1;
                filterItems_5.SelectedIndex = -1;
                FilterModel.filterNumber = 1;
                FilterList selectedFilter = (FilterList)filterItems_1.SelectedItem;
                FilterModel.selectedFilterID = int.Parse(selectedFilter.filter_id.ToString());
                FilterModel.selectedFilterName = selectedFilter.name;

                DeleteFilter1.IsEnabled = true;
            }
            else
            {
                FilterModel.filterNumber = -1;
                FilterModel.selectedFilterID = -1;
                FilterModel.selectedFilterName = "";
                DeleteFilter1.IsEnabled = false;
            }

        }

        private void AddFilter1_Click(object sender, RoutedEventArgs e)
        {
            FilterModel.filterNumber = 1;
            NewFilterWindow newFilter = new NewFilterWindow();
            newFilter.ShowDialog();


            int filter1SelectedIndex = filterItems_1.SelectedIndex;
            loadFilter1();
            filterItems_1.SelectedIndex = filter1SelectedIndex; FilterModel.filterNumber = 1;
        }

        private void SaveFilters1_Click(object sender, RoutedEventArgs e)
        {
            int selectionInt1 = filterItems_1.SelectedIndex;
            int merges = 0;

            int changes = 0;
            foreach (FilterList row in filterItems_1.Items)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter1_names WHERE filter_id = {row.filter_id}");

                if (!ds.Tables[0].Rows[0]["name"].ToString().Equals(row.name))
                {
                    DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM filter1_names WHERE name = '{row.name}'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter1_id = {ds2.Tables[0].Rows[0]["filter_id"]} WHERE filter1_id = {row.filter_id}");
                        AdministrationQueries.RunSqlExec($"DELETE FROM filter1_names WHERE filter_id = {row.filter_id}");

                        //ErrorHandlerModel.ErrorText = $"Es existiert bereits ein Filter in Stufe 1 mit dem Namen {row.name}! Filter mit der ID {row.filter_id} kann nicht verändert werden!";
                        //ErrorHandlerModel.ErrorType = "NOTALLOWED";
                        //ErrorWindow showError = new ErrorWindow();
                        //showError.ShowDialog();
                        changes++;
                        merges++;
                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE filter1_names SET name = '{row.name}' WHERE filter_id = {row.filter_id}");
                        changes++;
                    }
                }


            }
            ErrorHandlerModel.ErrorText = $"Es wurden {changes.ToString()} Filter erfolgreich bearbeitet und {merges.ToString()} zusammengeführt!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();

            loadFilter1();
            filterItems_1.SelectedIndex = selectionInt1;


        }



        private void AddFilter2_Click(object sender, RoutedEventArgs e)
        {
            FilterModel.filterNumber = 2;
            NewFilterWindow newFilter = new NewFilterWindow();
            newFilter.ShowDialog();


            int filter2SelectedIndex = filterItems_2.SelectedIndex;
            loadFilter2();
            filterItems_2.SelectedIndex = filter2SelectedIndex; FilterModel.filterNumber = 2;
        }

        private void SaveFilters2_Click(object sender, RoutedEventArgs e)
        {

            int selectionInt2 = filterItems_2.SelectedIndex;
            int merges = 0;
            int changes = 0;
            foreach (FilterList row in filterItems_2.Items)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter2_names WHERE filter_id = {row.filter_id}");

                if (!ds.Tables[0].Rows[0]["name"].ToString().Equals(row.name))
                {
                    DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM filter2_names WHERE name = '{row.name}'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter2_id = {ds2.Tables[0].Rows[0]["filter_id"]} WHERE filter2_id = {row.filter_id}");
                        AdministrationQueries.RunSqlExec($"DELETE FROM filter2_names WHERE filter_id = {row.filter_id}");
                        changes++;
                        merges++;
                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE filter2_names SET name = '{row.name}' WHERE filter_id = {row.filter_id}");
                        changes++;
                    }
                }


            }
            ErrorHandlerModel.ErrorText = $"Es wurden {changes.ToString()} Filter erfolgreich bearbeitet und {merges.ToString()} zusammengeführt!";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            loadFilter2();

            filterItems_2.SelectedIndex = selectionInt2;

        }



        private void filterItems_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterItems_2.SelectedItem != null)
            {
                filterItems_1.SelectedIndex = -1;
                filterItems_3.SelectedIndex = -1;
                filterItems_4.SelectedIndex = -1;
                filterItems_5.SelectedIndex = -1;

                FilterModel.filterNumber = 2;
                FilterList selectedFilter = (FilterList)filterItems_2.SelectedItem;
                FilterModel.selectedFilterID = int.Parse(selectedFilter.filter_id.ToString());
                FilterModel.selectedFilterName = selectedFilter.name;

                DeleteFilter2.IsEnabled = true;
            }
            else
            {
                FilterModel.filterNumber = -1;
                FilterModel.selectedFilterID = -1;
                FilterModel.selectedFilterName = "";
                DeleteFilter2.IsEnabled = false;
            }
        }

        private void AddFilter3_Click(object sender, RoutedEventArgs e)
        {
            FilterModel.filterNumber = 3;
            NewFilterWindow newFilter = new NewFilterWindow();
            newFilter.ShowDialog();


            int filter3SelectedIndex = filterItems_3.SelectedIndex;
            loadFilter3();
            filterItems_3.SelectedIndex = filter3SelectedIndex; FilterModel.filterNumber = 3;
        }

        private void SaveFilters3_Click(object sender, RoutedEventArgs e)
        {

            int selectionInt3 = filterItems_3.SelectedIndex;
            int merges = 0;
            int changes = 0;
            foreach (FilterList row in filterItems_3.Items)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter3_names WHERE filter_id = {row.filter_id}");

                if (!ds.Tables[0].Rows[0]["name"].ToString().Equals(row.name))
                {
                    DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM filter3_names WHERE name = '{row.name}'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter3_id = {ds2.Tables[0].Rows[0]["filter_id"]} WHERE filter3_id = {row.filter_id}");
                        AdministrationQueries.RunSqlExec($"DELETE FROM filter3_names WHERE filter_id = {row.filter_id}");
                        merges++;
                        changes++;

                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE filter3_names SET name = '{row.name}' WHERE filter_id = {row.filter_id}");
                        changes++;
                    }
                }


            }
            ErrorHandlerModel.ErrorText = $"Es wurden {changes.ToString()} Filter erfolgreich bearbeitet und {merges.ToString()} zusammengeführt!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            loadFilter3();

            filterItems_3.SelectedIndex = selectionInt3;

        }



        private void filterItems_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterItems_3.SelectedItem != null)
            {
                filterItems_1.SelectedIndex = -1;
                filterItems_2.SelectedIndex = -1;
                filterItems_4.SelectedIndex = -1;
                filterItems_5.SelectedIndex = -1;

                FilterModel.filterNumber = 3;
                FilterList selectedFilter = (FilterList)filterItems_3.SelectedItem;
                FilterModel.selectedFilterID = int.Parse(selectedFilter.filter_id.ToString());
                FilterModel.selectedFilterName = selectedFilter.name;

                DeleteFilter3.IsEnabled = true;
            }
            else
            {
                FilterModel.filterNumber = -1;
                FilterModel.selectedFilterID = -1;
                FilterModel.selectedFilterName = "";
                DeleteFilter3.IsEnabled = false;
            }
        }

        private void AddFilter4_Click(object sender, RoutedEventArgs e)
        {
            FilterModel.filterNumber = 4;
            NewFilterWindow newFilter = new NewFilterWindow();
            newFilter.ShowDialog();


            int filter4SelectedIndex = filterItems_4.SelectedIndex;
            loadFilter4();
            filterItems_4.SelectedIndex = filter4SelectedIndex; FilterModel.filterNumber = 4;
        }

        private void SaveFilters4_Click(object sender, RoutedEventArgs e)
        {

            int selectionInt4 = filterItems_4.SelectedIndex;
            int merges = 0;
            int changes = 0;
            foreach (FilterList row in filterItems_4.Items)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter4_names WHERE filter_id = {row.filter_id}");

                if (!ds.Tables[0].Rows[0]["name"].ToString().Equals(row.name))
                {
                    DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM filter4_names WHERE name = '{row.name}'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter4_id = {ds2.Tables[0].Rows[0]["filter_id"]} WHERE filter4_id = {row.filter_id}");
                        AdministrationQueries.RunSqlExec($"DELETE FROM filter4_names WHERE filter_id = {row.filter_id}");
                        merges++;
                        changes++;
                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE filter4_names SET name = '{row.name}' WHERE filter_id = {row.filter_id}");
                        changes++;
                    }
                }


            }
            ErrorHandlerModel.ErrorText = $"Es wurden {changes.ToString()} Filter erfolgreich bearbeitet und {merges.ToString()} zusammengeführt!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            loadFilter4();

            filterItems_4.SelectedIndex = selectionInt4;

        }



        private void filterItems_4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterItems_4.SelectedItem != null)
            {
                filterItems_1.SelectedIndex = -1;
                filterItems_3.SelectedIndex = -1;
                filterItems_2.SelectedIndex = -1;
                filterItems_5.SelectedIndex = -1;

                FilterModel.filterNumber = 4;
                FilterList selectedFilter = (FilterList)filterItems_4.SelectedItem;
                FilterModel.selectedFilterID = int.Parse(selectedFilter.filter_id.ToString());
                FilterModel.selectedFilterName = selectedFilter.name;

                DeleteFilter4.IsEnabled = true;
            }
            else
            {
                FilterModel.filterNumber = -1;
                FilterModel.selectedFilterID = -1;
                FilterModel.selectedFilterName = "";
                DeleteFilter4.IsEnabled = false;
            }
        }

        private void AddFilter5_Click(object sender, RoutedEventArgs e)
        {
            FilterModel.filterNumber = 5;
            NewFilterWindow newFilter = new NewFilterWindow();
            newFilter.ShowDialog();


            int filter5SelectedIndex = filterItems_5.SelectedIndex;
            loadFilter5();
            filterItems_5.SelectedIndex = filter5SelectedIndex;
            FilterModel.filterNumber = 0;

        }

        private void SaveFilters5_Click(object sender, RoutedEventArgs e)
        {

            int selectionInt5 = filterItems_5.SelectedIndex;
            int merges = 0;
            int changes = 0;
            foreach (FilterList row in filterItems_5.Items)
            {
                DataSet ds = AdministrationQueries.RunSql($"SELECT * FROM filter5_names WHERE filter_id = {row.filter_id}");

                if (!ds.Tables[0].Rows[0]["name"].ToString().Equals(row.name))
                {
                    DataSet ds2 = AdministrationQueries.RunSql($"SELECT * FROM filter5_names WHERE name = '{row.name}'");
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE item_filter_relations SET filter5_id = {ds2.Tables[0].Rows[0]["filter_id"]} WHERE filter5_id = {row.filter_id}");
                        AdministrationQueries.RunSqlExec($"DELETE FROM filter5_names WHERE filter_id = {row.filter_id}");
                        changes++;
                        merges++;
                    }
                    else
                    {
                        AdministrationQueries.RunSqlExec($"UPDATE filter5_names SET name = '{row.name}' WHERE filter_id = {row.filter_id}");
                        changes++;
                    }
                }


            }
            ErrorHandlerModel.ErrorText = $"Es wurden {changes.ToString()} Filter erfolgreich bearbeitet und {merges.ToString()} zusammengeführt!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();
            loadFilter5();

            filterItems_5.SelectedIndex = selectionInt5;
        }

        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            DataSet itemsWithSelectedFitler = AdministrationQueries.RunSql($"SELECT * FROM item_filter_relations WHERE filter{FilterModel.filterNumber}_id = {FilterModel.selectedFilterID}");

            if (itemsWithSelectedFitler.Tables[0].Rows.Count > 0)
            {
                ErrorHandlerModel.ErrorText = $"Es haben noch {itemsWithSelectedFitler.Tables[0].Rows.Count} Artikel diesen Filter ({FilterModel.selectedFilterName}) in Filterstufe {FilterModel.filterNumber} hinterlegt! Bitte weisen Sie diesen Artikeln über die Artikelbearbeitung neue Filter hinzu, bevor Sie diesen Filter löschen!";
                ErrorHandlerModel.ErrorType = "NOTALLOWED";
                ErrorWindow showError = new ErrorWindow();
                showError.ShowDialog();
            }
            else
            {
                ConfirmFilterDeletionWindow showConfirm = new ConfirmFilterDeletionWindow();
                showConfirm.ShowDialog();


                int selecetedIndex1 = filterItems_1.SelectedIndex;
                int selecetedIndex2 = filterItems_2.SelectedIndex;
                int selecetedIndex3 = filterItems_3.SelectedIndex;
                int selecetedIndex4 = filterItems_4.SelectedIndex;
                int selecetedIndex5 = filterItems_5.SelectedIndex;


                loadFilter1();
                loadFilter2();
                loadFilter3();
                loadFilter4();
                loadFilter5();

                filterItems_1.SelectedIndex = selecetedIndex1;
                filterItems_2.SelectedIndex = selecetedIndex2;
                filterItems_3.SelectedIndex = selecetedIndex3;
                filterItems_4.SelectedIndex = selecetedIndex4;
                filterItems_5.SelectedIndex = selecetedIndex5;
            }

        }

        private void filterItems_5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterItems_5.SelectedItem != null)
            {
                filterItems_1.SelectedIndex = -1;
                filterItems_3.SelectedIndex = -1;
                filterItems_4.SelectedIndex = -1;
                filterItems_2.SelectedIndex = -1;

                FilterModel.filterNumber = 5;
                FilterList selectedFilter = (FilterList)filterItems_5.SelectedItem;
                FilterModel.selectedFilterID = int.Parse(selectedFilter.filter_id.ToString());
                FilterModel.selectedFilterName = selectedFilter.name;

                DeleteFilter5.IsEnabled = true;
            }
            else
            {
                FilterModel.filterNumber = -1;
                FilterModel.selectedFilterID = -1;
                FilterModel.selectedFilterName = "";
                DeleteFilter5.IsEnabled = false;
            }
        }
    }
    public class FilterList
    {
        public string filter_id { get; set; }
        public string name { get; set; }


    }
}
