using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MySqlConnector;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.History
{
    /// <summary>
    /// Interaktionslogik für HistoryView.xaml
    /// </summary>
    public partial class HistoryView : UserControl
    {
        public class HistoryParams
        {
            public static DataSet CurrentDB = new DataSet();
            public static DataSet HistoryDB = new DataSet();
            public static string SelectedID = "";

        }
        MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());
        public HistoryView()
        {
            InitializeComponent();


            DataSet history_log = AdministrationQueries.RunSql("SELECT * FROM history_log ORDER BY createdAt DESC");


            history_log.Tables[0].Columns.Add("log_action_name");
            history_log.Tables[0].Columns.Add("date");
            history_log.Tables[0].Columns.Add("name_DE");

            DataSet history_log_ext = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.Add("history_id");

            dt.Columns.Add("Artikelnummer");
            dt.Columns.Add("Menge");
            dt.Columns.Add("Lagerort_Alt");
            dt.Columns.Add("Lagerort_Neu");
            dt.Columns.Add("Mitarbeiter");
            dt.Columns.Add("Aktion");
            dt.Columns.Add("Datum");


            history_log_ext.Tables.Add(dt);







            DataSet action_names = AdministrationQueries.RunSql("Select * FROM log_action_names");


            if (history_log.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in history_log.Tables[0].Rows)
                {

                    DateTime oldFormat = DateTime.Parse(row["createdAt"].ToString());
                    string newFormat = oldFormat.ToString("G");
                    row["date"] = newFormat;


                    foreach (DataRow row2 in action_names.Tables[0].Rows)
                    {
                        if (row["action_id"].ToString() == row2["action_id"].ToString())
                        {
                            row["name_DE"] = row2["name_DE"];
                        }
                    }

                    DataRow newRow = history_log_ext.Tables[0].NewRow();
                    newRow["history_id"] = row[0];

                    newRow["Artikelnummer"] = row[1];
                    newRow["Menge"] = row[2];
                    newRow["Lagerort_Alt"] = row[3];
                    newRow["Lagerort_Neu"] = row[4];
                    newRow["Mitarbeiter"] = row[8];
                    newRow["Aktion"] = row[14];
                    newRow["Datum"] = row[13];


                    history_log_ext.Tables[0].Rows.Add(newRow);
                }



                HistoryParams.HistoryDB = history_log_ext;
                HistoryParams.CurrentDB = HistoryParams.HistoryDB;
                dataGridItems.DataContext = history_log_ext;
                dataGridItems.ItemsSource = new DataView(history_log_ext.Tables[0]);
                dataGridItems.Items.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Descending));
                dataGridItems.Items.Refresh();
                ExportDataBtn.IsEnabled = true;
                searchBox.IsEnabled = true;
            }
            else
            {
                ExportDataBtn.IsEnabled = false;
                searchBox.IsEnabled = false;
            }


        }

        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                HistoryParams.SelectedID = row_selected["history_id"].ToString();
            }
        }



        private void ExportDataBtn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = HistoryParams.CurrentDB.Tables[0].Copy();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString() == "")
                    {
                        dt.Rows[i][j] = dt.Rows[i][j] = "0";
                    }


                    //if (dt.Rows[i][j].ToString().Contains(";") | dt.Rows[i][j].ToString().Contains(",") | dt.Rows[i][j].ToString().Contains(":"))
                    //{
                    //    dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace(";", " ");
                    //    dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace(",", " ");
                    //}


                    if (dt.Rows[i][j].ToString().Contains("Ö"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("Ö", "OE");
                    }
                    if (dt.Rows[i][j].ToString().Contains("Ü"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("Ü", "UE");
                    }
                    if (dt.Rows[i][j].ToString().Contains("Ö"))
                    {
                        dt.Rows[i][j] = dt.Rows[i][j].ToString().Replace("Ä", "AE");
                    }

                }

            }
            string currentTime = DateTime.Now.ToString("dd.mm.yyyy") + "_" + DateTime.Now.ToString("T");
            currentTime = currentTime.Replace(":", ".");
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            string path = key.GetValue("HistoryLogsPath").ToString() + "\\history_overview_" + currentTime + ".csv";

            ToCSV(dt, path);
            ErrorHandlerModel.ErrorText = (string)FindResource("errorText39");
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow openSuccess = new ErrorWindow();
            openSuccess.ShowDialog();
        }
        private void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(", ");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(", ");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet ds = HistoryParams.HistoryDB.Copy();
                DataSet output = HistoryParams.HistoryDB.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["Artikelnummer"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["Mitarbeiter"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["Lagerort_Alt"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["Lagerort_Neu"].ToString().ToLower().Contains(searchBox.Text.ToLower())
                        | row["Aktion"].ToString().ToLower().Contains(searchBox.Text.ToLower()))

                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                HistoryParams.CurrentDB = output;
                dataGridItems.DataContext = output;
                dataGridItems.ItemsSource = new DataView(output.Tables[0]);
                dataGridItems.Items.SortDescriptions.Clear();
                dataGridItems.Items.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Descending));
                dataGridItems.Items.Refresh();
                if (output.Tables[0].Rows.Count == 0)
                {
                    ExportDataBtn.IsEnabled = false;
                }
            }
            else
            {
                dataGridItems.DataContext = HistoryParams.HistoryDB;
                dataGridItems.ItemsSource = new DataView(HistoryParams.HistoryDB.Tables[0]);
                dataGridItems.Items.SortDescriptions.Clear();
                dataGridItems.Items.SortDescriptions.Add(new SortDescription("Datum", ListSortDirection.Descending));
                dataGridItems.Items.Refresh();
                searchBox.IsEnabled = true;
                ExportDataBtn.IsEnabled = true;
            }
        }
    }
}
