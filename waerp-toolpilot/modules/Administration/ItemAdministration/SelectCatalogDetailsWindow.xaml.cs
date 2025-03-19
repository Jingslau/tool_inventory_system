using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.Functions;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.modules.Administration.ItemAdministration
{
    /// <summary>
    /// Interaction logic for SelectCatalogDetailsWindow.xaml
    /// </summary>
    public partial class SelectCatalogDetailsWindow : Window
    {
        public SelectCatalogDetailsWindow()
        {
            InitializeComponent();

            string query = "SELECT vendor_name FROM vendor_objects ORDER BY vendor_name ASC";
            DataSet vendors = AdministrationQueries.RunSql(query);
            VendorSelector.Items.Add("kein Lieferant");
            foreach (DataRow row in vendors.Tables[0].Rows)
            {
                VendorSelector.Items.Add(row["vendor_name"]);
            }

            VendorSelector.SelectedIndex = 0;


            CatalogSizeSelector.Items.Add("63,5 x 38,1 mm (3 x 7 Bogen)");
            CatalogSizeSelector.SelectedIndex = 0;
        }

        private void GenerateCatalog_Click(object sender, RoutedEventArgs e)
        {
            String query = "";

            if (VendorSelector.SelectedItem.Equals("kein Lieferant"))
            {
                query = $@"SELECT 
    io.item_ident,
    io.item_description,
    io.item_description_2,
    CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', io.item_image_path) AS item_image_path   
FROM 
    item_objects io
LEFT JOIN 
    item_vendor_relations ivr ON io.item_id = ivr.item_id
WHERE 
    ivr.vendor_id IS NULL;";
            }
            else
            {
                query = $@"SELECT     
    io.item_ident, 
    io.item_description,    
    io.item_description_2, 
    CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', io.item_image_path) AS item_image_path,    
    vo.vendor_name 
    FROM    
         item_objects io 
    JOIN item_vendor_relations ivr ON io.item_id = ivr.item_id 
    JOIN vendor_objects vo ON ivr.vendor_id = vo.vendor_id 
    WHERE 
        vo.vendor_name = '{VendorSelector.SelectedItem.ToString()}'";
            }

            DataSet items = AdministrationQueries.RunSql(query);

            PDFItemCatalog gene = new PDFItemCatalog();
            Document doc = gene.CreateItemsPdfFromDataTable(items.Tables[0], VendorSelector.SelectedItem.ToString());

            doc.UseCmykColor = true;
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

            // ========================================================================================

            // Create a renderer for the MigraDoc document.
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = doc;

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            // Save the document...

            DateTime now = DateTime.Now;

            string vendor = "Test";
            string vendorEdit = string.Join("", vendor.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            string filename = "";
            if (VendorSelector.SelectedItem.Equals("kein Lieferant"))
            {
                filename = "Artikelkatalog_Toolpilot" + "_" + now.ToString("dd") + "_" + now.ToString("MM") + "_" + now.ToString("yyyy") + "_" + now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString() + ".pdf";
            }
            else
            {
                filename = "Artikelkatalog_Toolpilot" + "_" + VendorSelector.SelectedItem + "_" + now.ToString("dd") + "_" + now.ToString("MM") + "_" + now.ToString("yyyy") + "_" + now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString() + ".pdf";

            }


            string savePath = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_history_path'").Tables[0].Rows[0][2].ToString() + "\\" + filename;

            pdfRenderer.PdfDocument.Save(savePath);

            Process.Start(savePath);

            ErrorHandlerModel.ErrorText = $"Der Katalog wurde erfolgreich in {savePath} abgespeichert!";
            ErrorHandlerModel.ErrorType = "SUCCESS";
            ErrorWindow showSuccess = new ErrorWindow();
            showSuccess.ShowDialog();

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

        private void VendorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CatalogSizeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
