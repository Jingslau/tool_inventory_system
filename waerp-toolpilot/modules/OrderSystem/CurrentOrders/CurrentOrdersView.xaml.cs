using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using waerp_toolpilot.application.Global;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.Functions;
using waerp_toolpilot.models;
using waerp_toolpilot.modules.OrderSystem.CurrentOrders;
using waerp_toolpilot.sql;
using waerp_toolpilot.store;

namespace waerp_toolpilot.application.OrderSystem.CurrentOrders
{
    /// <summary>
    /// Interaction logic for CurrentOrdersView.xaml
    /// </summary>
    public partial class CurrentOrdersView : UserControl
    {
        public static class OrderData
        {
            public static DataSet VendorData = new DataSet();
            public static DataSet ProductData = new DataSet();
            public static DataRow CurrentSelectedItem;
            public static DataRow CurrentSelectedVendor;
        }
        public CurrentOrdersView()
        {
            InitializeComponent();
            OrderData.VendorData = CurrentOrdersQueries.GetAllVendors();

            vendorData.DataContext = OrderData.VendorData;
            vendorData.ItemsSource = new DataView(OrderData.VendorData.Tables[0]);
            if (OrderData.VendorData.Tables[0].Rows.Count > 0)
            {
                vendorData.SelectedIndex = 0;
                CreatePDFBtn.IsEnabled = true;
            }
            else
            {
                CreatePDFBtn.IsEnabled = false;
            }
        }

        private void searchBoxVendor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBoxVendor.Text != "")
            {
                DataSet output = OrderData.VendorData.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in OrderData.VendorData.Tables[0].Rows)
                {
                    if (row["vendor_name"].ToString().Contains(searchBoxVendor.Text))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                vendorData.DataContext = output;
                vendorData.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                vendorData.DataContext = OrderData.VendorData;

                vendorData.ItemsSource = new DataView(OrderData.VendorData.Tables[0]);
            }
        }



        private void vendorData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchBox.Text = "";
            searchBoxVendor.Text = "";
            CreatePDFBtn.IsEnabled = true;
            ContactCard.IsEnabled = true;
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                dataGridItems.SelectedIndex = 0;
                OrderData.CurrentSelectedVendor = row_selected.Row;
                OrderStore.OrderIdent = row_selected["vendor_name"].ToString();
                if (row_selected["vendor_website"].ToString() != "")
                {
                    OpenWebsite.IsEnabled = true;
                }
                else
                {
                    OpenWebsite.IsEnabled = false;
                }
                if (row_selected["vendor_contact"].ToString() != "")
                {
                    OpenEmail.IsEnabled = true;
                }
                else
                {
                    OpenEmail.IsEnabled = false;
                }

                OrderData.ProductData = CurrentOrdersQueries.GetOrderedItems(row_selected["vendor_id"].ToString());
                if (OrderData.ProductData != null)
                {
                    dataGridItems.DataContext = OrderData.ProductData;
                    dataGridItems.ItemsSource = new DataView(OrderData.ProductData.Tables[0]);
                }


            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBox.Text != "")
            {
                DataSet output = OrderData.ProductData.Copy();
                output.Tables[0].Rows.Clear();

                foreach (DataRow row in OrderData.ProductData.Tables[0].Rows)
                {
                    if (row["item_ident"].ToString().ToLower().Contains(searchBox.Text.ToLower()) | row["item_description"].ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        output.Tables[0].ImportRow(row);
                    }
                }
                dataGridItems.DataContext = output;
                dataGridItems.ItemsSource = new DataView(output.Tables[0]);
            }
            else
            {
                dataGridItems.DataContext = OrderData.ProductData;

                dataGridItems.ItemsSource = new DataView(OrderData.ProductData.Tables[0]);
            }
        }

        private void OpenBookOrderBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveOrderModel.ItemQuantity = OrderData.CurrentSelectedItem["order_quantity"].ToString();
            ActiveOrderModel.CurrentItemId = OrderData.CurrentSelectedItem["item_id"].ToString();
            ActiveOrderModel.ItemIdentStr = OrderData.CurrentSelectedItem["item_ident"].ToString();
            ActiveOrderModel.Order_Ident = OrderData.CurrentSelectedItem["order_ident"].ToString();
            ActiveOrderModel.ItemDescription = OrderData.CurrentSelectedItem["item_description"].ToString();
            BookSelectedOrderWindow openWindow = new BookSelectedOrderWindow();
            Nullable<bool> dialogResult = openWindow.ShowDialog();
            int oldSelectedIndex = vendorData.SelectedIndex;
            OrderData.VendorData = CurrentOrdersQueries.GetAllVendors();
            if (OrderData.VendorData.Tables[0].Rows.Count > 0)
            {
                vendorData.DataContext = OrderData.VendorData;
                vendorData.ItemsSource = new DataView(OrderData.VendorData.Tables[0]);
                if (OrderData.VendorData.Tables[0].Rows.Count > oldSelectedIndex)
                {
                    vendorData.SelectedIndex = oldSelectedIndex;
                }
                else
                {
                    vendorData.SelectedIndex = -1;
                }
            }
            else
            {
                vendorData.DataContext = null;
                vendorData.ItemsSource = null;
                dataGridItems.DataContext = null;
                dataGridItems.ItemsSource = null;
                DeleteOrderBtn.IsEnabled = false;
                EditOrderBtn.IsEnabled = false;
                OpenBookOrderBtn.IsEnabled = false;
            }



        }

        private void dataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;
            if (row_selected != null)
            {
                OrderData.CurrentSelectedItem = row_selected.Row;
                DeleteOrderBtn.IsEnabled = true;
                EditOrderBtn.IsEnabled = true;
                OpenBookOrderBtn.IsEnabled = true;
                OrderStore.OrderIdent = row_selected["order_ident"].ToString();
                OrderStore.item_id = row_selected["item_id"].ToString();
            }
        }

        private void DeleteOrderBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ConfirmDeleteOrderWindow openConfirm = new ConfirmDeleteOrderWindow();
            openConfirm.ShowDialog();
            int oldSelectedIndex = vendorData.SelectedIndex;
            OrderData.VendorData = CurrentOrdersQueries.GetAllVendors();
            if (OrderData.VendorData.Tables[0].Rows.Count > 0)
            {
                vendorData.DataContext = OrderData.VendorData;
                vendorData.ItemsSource = new DataView(OrderData.VendorData.Tables[0]);
                if (OrderData.VendorData.Tables[0].Rows.Count > oldSelectedIndex)
                {
                    vendorData.SelectedIndex = oldSelectedIndex;
                }
                else
                {
                    vendorData.SelectedIndex = -1;
                }
            }
            else
            {
                vendorData.DataContext = null;
                vendorData.ItemsSource = null;
                dataGridItems.DataContext = null;
                dataGridItems.ItemsSource = null;
                DeleteOrderBtn.IsEnabled = false;
                EditOrderBtn.IsEnabled = false;
                OpenBookOrderBtn.IsEnabled = false;
            }

        }

        private void EditOrderBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ActiveOrderModel.ItemQuantity = OrderData.CurrentSelectedItem["order_quantity"].ToString();
            ActiveOrderModel.CurrentItemId = OrderData.CurrentSelectedItem["item_id"].ToString();
            EditOrderQuantWindow editWindow = new EditOrderQuantWindow();
            Nullable<bool> dialogResult = editWindow.ShowDialog();
            OrderData.CurrentSelectedItem["order_quantity"] = ActiveOrderModel.ItemQuantity;
        }

        private void OpenWebsite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start($"{OrderData.CurrentSelectedVendor["vendor_website"]}");
        }

        private void OpenEmail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start($"mailto:{OrderData.CurrentSelectedVendor["vendor_contact"]}?subject=Bestellung Firma XY: ");
        }

        private void ContactCard_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ContactCardModel.CompanyCity = "";
            ContactCardModel.CompanyName = "";
            ContactCardModel.CompanyAdress = "";
            ContactCardModel.CompanyPostcode = "";
            ContactCardModel.CompanyCountry = "";
            ContactCardModel.CompanyWebsite = "";
            ContactCardModel.CompanyMail = "";
            ContactCardModel.CompanyPhone = "";

            ContactCardModel.CompanyCity = OrderData.CurrentSelectedVendor["vendor_city"].ToString();
            ContactCardModel.CompanyName = OrderData.CurrentSelectedVendor["vendor_name"].ToString();
            ContactCardModel.CompanyAdress = OrderData.CurrentSelectedVendor["vendor_adress"].ToString();
            ContactCardModel.CompanyPostcode = OrderData.CurrentSelectedVendor["vendor_postcode"].ToString();
            ContactCardModel.CompanyCountry = OrderData.CurrentSelectedVendor["vendor_country"].ToString();
            ContactCardModel.CompanyWebsite = OrderData.CurrentSelectedVendor["vendor_website"].ToString();
            ContactCardModel.CompanyMail = OrderData.CurrentSelectedVendor["vendor_contact"].ToString();
            ContactCardModel.CompanyPhone = OrderData.CurrentSelectedVendor["vendor_phone"].ToString();
            ContactCardWindow openContact = new ContactCardWindow();
            Nullable<bool> dialogResult = openContact.ShowDialog();
        }

        //Image draw method for PDF Creator
        void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)
        {
            XImage image = XImage.FromFile(jpegSamplePath);
            gfx.DrawImage(image, x, y, width, height);
        }

        //PDF Creator
        private void CreatePDFBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //PDF Site load
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Roboto", 20);

            //Orderdate creation
            DateTime orderDateTime = DateTime.Now;

            //Variable Settingsw (Margin is loaded from settings file). Page width and height is according to DIN A4 format. 
            page.Height = 842;//842
            page.Width = 590;

            double marginTop = pdfDocument.Default.printSpaceTop;
            double marginBottom = pdfDocument.Default.printSpaceBottom;
            double marginLeft = pdfDocument.Default.printSpaceLeft;
            double marginRight = pdfDocument.Default.printSpaceRight;

            int rect_height = 20;
            int rect_heigt2 = rect_height * 2;
            DrawImage(gfx, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "toolpilot\\ApplicationFiles\\Druckdatei_wærp_Briefpapier.pdf"), 0, 0, 590, 842);

            XSolidBrush rect_style1 = new XSolidBrush(XColors.DarkGray);

            XSolidBrush rect_style2 = new XSolidBrush(XColors.White);

            XSolidBrush rect_style3 = new XSolidBrush(XColors.LightGray);

            //stationery letter


            //Sender Data
            gfx.DrawString("Waerp.Software GmbH", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(page.Width - XUnit.FromCentimeter(7), XUnit.FromCentimeter(6)));
            gfx.DrawString("Musterstrasse 9", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(page.Width - XUnit.FromCentimeter(7), XUnit.FromCentimeter(6.5)));
            gfx.DrawString("Musterstadt", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(page.Width - XUnit.FromCentimeter(7), XUnit.FromCentimeter(7)));
            gfx.DrawString("447788", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(page.Width - XUnit.FromCentimeter(7), XUnit.FromCentimeter(7.5)));

            //Receiver Data
            gfx.DrawString($"{OrderData.CurrentSelectedVendor["vendor_name"].ToString()}", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(XUnit.FromCentimeter(marginLeft), XUnit.FromCentimeter(6)));
            gfx.DrawString($"{OrderData.CurrentSelectedVendor["vendor_adress"].ToString()}", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(XUnit.FromCentimeter(marginLeft), XUnit.FromCentimeter(6.5)));
            gfx.DrawString($"{OrderData.CurrentSelectedVendor["vendor_city"].ToString()}", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(XUnit.FromCentimeter(marginLeft), XUnit.FromCentimeter(7)));
            gfx.DrawString($"{OrderData.CurrentSelectedVendor["vendor_postcode"].ToString()}", new XFont("Roboto", 11, XFontStyle.Regular), XBrushes.Black, new XPoint(XUnit.FromCentimeter(marginLeft), XUnit.FromCentimeter(7.5)));

            //Letter title
            gfx.DrawString($"Bestellübersicht für {OrderData.CurrentSelectedVendor["vendor_name"]}", new XFont("Roboto", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(XUnit.FromCentimeter(marginLeft), XUnit.FromCentimeter(9)));
            gfx.DrawString($"Dieses Dokument wurde automatisch am {orderDateTime.ToString("d")} erstellt", new XFont("Roboto", 8, XFontStyle.Regular), XBrushes.Black, new XPoint(XUnit.FromCentimeter(marginLeft), XUnit.FromCentimeter(9.5)));



            //Order Table
            gfx.DrawRectangle(rect_style1, marginLeft, 280, page.Width - 2 * marginLeft, rect_height);
            gfx.DrawString("Artikelnummer", new XFont("Roboto", 12, XFontStyle.Bold), XBrushes.Black, new XPoint(marginLeft + 20, 280 + 14));
            gfx.DrawString("Beschreibung", new XFont("Roboto", 12, XFontStyle.Bold), XBrushes.Black, new XPoint(marginLeft + 190, 280 + 14));
            gfx.DrawString("Menge", new XFont("Roboto", 12, XFontStyle.Bold), XBrushes.Black, new XPoint(marginLeft + 450, 280 + 14));

            int currentYpostition_line = 280 + rect_height;

            for (int i = 0; i < OrderData.ProductData.Tables[0].Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    gfx.DrawRectangle(rect_style2, marginLeft, currentYpostition_line, page.Width - 2 * marginLeft, rect_heigt2);

                }
                else
                {
                    gfx.DrawRectangle(rect_style3, marginLeft, currentYpostition_line, page.Width - 2 * marginLeft, rect_heigt2);
                }
                gfx.DrawString($"{OrderData.ProductData.Tables[0].Rows[i]["item_ident"]}", new XFont("Roboto", 12, XFontStyle.Regular), XBrushes.Black, new XPoint(marginLeft + 20, currentYpostition_line + 20));
                gfx.DrawString($"{OrderData.ProductData.Tables[0].Rows[i]["item_description"]}", new XFont("Roboto", 12, XFontStyle.Regular), XBrushes.Black, new XPoint(marginLeft + 190, currentYpostition_line + 20));
                gfx.DrawString($"{OrderData.ProductData.Tables[0].Rows[i]["order_quantity"]}", new XFont("Roboto", 12, XFontStyle.Regular), XBrushes.Black, new XPoint(marginLeft + 450, currentYpostition_line + 20));
                currentYpostition_line += rect_heigt2;
            }


            //Save Path 
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            string pathS = key.GetValue("OrderOverviewPath").ToString();

            //Document Save
            document.Save($"{pathS}\\Bestellung_{OrderData.CurrentSelectedVendor["vendor_name"]}_{orderDateTime.ToString("d")}_{orderDateTime.ToString("ss")}.pdf");
            //Open Document in standard PDF Viewer
            System.Diagnostics.Process.Start($"{pathS}\\Bestellung_{OrderData.CurrentSelectedVendor["vendor_name"]}_{orderDateTime.ToString("d")}_{orderDateTime.ToString("ss")}.pdf");

        }
        private void CreatePdfDocument(object sender, System.Windows.RoutedEventArgs e)
        {
            PDFCreaterFunc gene = new PDFCreaterFunc("test");
            Document doc = gene.CreateDocument();

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

            string vendor = OrderData.CurrentSelectedVendor["vendor_name"].ToString();
            string vendorEdit = string.Join("", vendor.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            string filename = vendorEdit + "_" + OrderStore.OrderIdent + "_" + "BESTELLUNG" + "_" + now.ToString("dd") + "_" + now.ToString("MM") + "_" + now.ToString("yyyy") + "_" + now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString() + ".pdf";

            string savePath = AdministrationQueries.RunSql("SELECT * FROM company_settings WHERE settings_name = 'global_orderdoc_path'").Tables[0].Rows[0][2].ToString() + "\\" + filename;

            pdfRenderer.PdfDocument.Save(savePath);

            Process.Start(savePath);

        }

        private void CopyItemIdent(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OrderData.CurrentSelectedItem["item_ident"].ToString());
        }

        private void CopyDescription(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OrderData.CurrentSelectedItem["item_description"].ToString());
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OrderData.CurrentSelectedItem["item_ident"].ToString() + "; " + OrderData.CurrentSelectedItem["item_description"].ToString());
        }
    }
}
