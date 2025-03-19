using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using waerp_toolpilot.models;
using waerp_toolpilot.sql;
using static waerp_toolpilot.application.OrderSystem.CurrentOrders.CurrentOrdersView;

namespace waerp_toolpilot.Functions
{
    /// <summary>
    /// Creates the invoice form.
    /// </summary>
    public class PDFCreaterFunc
    {
        /// <summary>
        /// The MigraDoc document that represents the invoice.
        /// </summary>
        Document document;

        /// <summary>
        /// An XML invoice based on a sample created with Microsoft InfoPath.
        /// </summary>
        readonly XmlDocument invoice;

        /// <summary>
        /// The root navigator for the XML document.
        /// </summary>
        readonly XPathNavigator navigator;

        /// <summary>
        /// The text frame of the MigraDoc document that contains the address.
        /// </summary>
        TextFrame addressFrame;

        /// <summary>
        /// The table of the MigraDoc document that contains the invoice items.
        /// </summary>
        Table table;

        /// <summary>
        /// Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        /// 

        public PDFCreaterFunc(string filename)
        {
            invoice = new XmlDocument();
            //this.invoice.Load(filename);
            //  this.navigator = this.invoice.CreateNavigator();
        }

        /// <summary>
        /// Creates the invoice document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            document = new Document();
            document.Info.Title = "Bestellschein";
            document.Info.Subject = "Bestellschein";
            document.Info.Author = "Werkzeugausgabesystem";
            DefineStyles();

            CreatePage();

            FillContent();

            return document;
        }

        /// <summary>
        /// Defines the styles used to format the MigraDoc document.
        /// </summary>
        void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];

            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Arial";
            style.Font.Name = "Arial";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        /// <summary>
        /// Creates the static parts of the invoice.
        /// </summary>
        void CreatePage()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            DataSet companyInfoDB = new DataSet();
            companyInfoDB = AdministrationQueries.RunSql("SELECT * FROM company_settings");


            int marginBottom = int.Parse(companyInfoDB.Tables[0].Rows[12]["settings_value"].ToString());
            int marginTop = int.Parse(companyInfoDB.Tables[0].Rows[13]["settings_value"].ToString());
            int marginLeft = int.Parse(companyInfoDB.Tables[0].Rows[14]["settings_value"].ToString());
            int marginRight = int.Parse(companyInfoDB.Tables[0].Rows[15]["settings_value"].ToString());


            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.TopMargin = Unit.FromMillimeter(marginTop);

            if (File.Exists(companyInfoDB.Tables[0].Rows[11][2].ToString()))
            {
                // Put a logo in the header
                Image image = section.Headers.Primary.AddImage(companyInfoDB.Tables[0].Rows[11][2].ToString());
                image.Height = "29.7cm";
                image.Width = "21cm";
                image.RelativeHorizontal = RelativeHorizontal.Page;
                image.RelativeVertical = RelativeVertical.Page;
                image.Top = ShapePosition.Top;
                image.Left = ShapePosition.Left;
            }




            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            //paragraph.AddText($"E-Mail: {companyInfoDB.Tables[0].Rows[5][2]} · Telefon: {companyInfoDB.Tables[0].Rows[4][2]}");
            //paragraph.AddLineBreak();
            //paragraph.AddText($"Ref.-Nr.: {OrderStore.OrderIdent}");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;




            // Create the text frame for the address
            addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = Unit.FromMillimeter(marginTop);
            addressFrame.RelativeVertical = RelativeVertical.Page;



            // Put sender in address frame



            paragraph = addressFrame.AddParagraph($"{companyInfoDB.Tables[0].Rows[0][2]} · {companyInfoDB.Tables[0].Rows[1][2]} · {companyInfoDB.Tables[0].Rows[3][2]} {companyInfoDB.Tables[0].Rows[2][2]}");
            paragraph.Format.Font.Name = "Arial";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;

            paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.AddFormattedText($"Ref.-Nr.: {OrderStore.OrderIdent}");


            if (!File.Exists(companyInfoDB.Tables[0].Rows[11][2].ToString()))
            {
                paragraph.AddLineBreak();
                paragraph.AddFormattedText(companyInfoDB.Tables[0].Rows[0][2].ToString());
                paragraph.AddLineBreak();
                paragraph.AddFormattedText(companyInfoDB.Tables[0].Rows[4][2].ToString());
                paragraph.AddLineBreak();
                paragraph.AddFormattedText(companyInfoDB.Tables[0].Rows[5][2].ToString());

            }

            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 8;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = Unit.FromCentimeter(3);
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("BESTELLUNG", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText($"{companyInfoDB.Tables[0].Rows[2][2]}, ");
            paragraph.AddDateField("dd.MM.yyyy");

            //Write Text
            paragraph = section.AddParagraph();
            paragraph.AddFormattedText($"Dies ist ein Bestellauftrag (KD-Nr.: {OrderData.CurrentSelectedVendor["vendor_customerid"]}). Bestellt werden alle Artikel aus der unten aufgeführten Tabelle. Sollte eine Artikelnummer nicht zu finden sein oder nicht bestellbar sein, dann schreiben Sie uns eine E-Mail oder rufen Sie uns an. Die Kontaktdetails finden Sie im Briefkopf.");
            paragraph.Format.SpaceAfter = "1cm";


            // Create the item table
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = TableBorder;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = TableBlue;
            row.Cells[0].AddParagraph("Pos.");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph("Artikelnummer");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].AddParagraph("Bezeichnung 1");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].AddParagraph("Bezeichnung 2");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].AddParagraph("D (mm)");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[5].AddParagraph("Menge");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
            table.Rows.Alignment = RowAlignment.Center;
            table.SetEdge(0, 0, 5, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

        }

        /// <summary>
        /// Creates the dynamic parts of the invoice.
        /// </summary>
        void FillContent()
        {
            // Fill address in address text frame
            //   XPathNavigator item = SelectItem("/invoice/to");
            Paragraph paragraph = addressFrame.AddParagraph();
            paragraph.AddText(OrderData.CurrentSelectedVendor["vendor_name"].ToString());
            paragraph.AddLineBreak();
            paragraph.AddText(OrderData.CurrentSelectedVendor["vendor_adress"].ToString());
            paragraph.AddLineBreak();
            paragraph.AddText(OrderData.CurrentSelectedVendor["vendor_postcode"].ToString() + " " + OrderData.CurrentSelectedVendor["vendor_city"].ToString());
            paragraph.AddLineBreak();
            paragraph.AddText(OrderData.CurrentSelectedVendor["vendor_country"].ToString());
            // Iterate the invoice items
            // double totalExtendedPrice = 0;

            DataSet data = OrderData.ProductData;

            // XPathNodeIterator iter = this.navigator.Select("/invoice/items/*");
            for (int i = 0; i < data.Tables[0].Rows.Count; i++)
            {
                string quantity = data.Tables[0].Rows[i][0].ToString();
                string price = data.Tables[0].Rows[i][1].ToString();
                string discount = data.Tables[0].Rows[i][2].ToString();

                // Each item fills two rows
                Row row1 = table.AddRow();
                Row row2 = table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].MergeDown = 1;

                row1.Cells[5].Shading.Color = TableGray;

                int currentPosition = i + 1;
                row1.Cells[0].AddParagraph(currentPosition.ToString());

                row1.Cells[1].AddParagraph(data.Tables[0].Rows[i]["item_ident"].ToString());
                row1.Cells[2].AddParagraph(data.Tables[0].Rows[i]["item_description"].ToString());
                row1.Cells[3].AddParagraph(data.Tables[0].Rows[i]["item_description_2"].ToString());
                row1.Cells[4].AddParagraph(data.Tables[0].Rows[i]["item_diameter"].ToString());


                row1.Cells[5].AddParagraph(data.Tables[0].Rows[i]["order_quantity"].ToString());
                row1.Cells[5].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[5].MergeDown = 1;

                row2.Cells[1].AddParagraph("Vermerk:");
                row2.Cells[1].MergeRight = 2;
                row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                table.SetEdge(0, table.Rows.Count - 2, 5, 2, Edge.Box, BorderStyle.Single, 0.75);
            }

            // Add an invisible row as a space line to the table
            Row row = table.AddRow();
            row.Borders.Visible = false;

            //// Add the total price row
            //row = this.table.AddRow();
            //row.Cells[0].Borders.Visible = false;
            //row.Cells[0].AddParagraph("Total Price");
            //row.Cells[0].Format.Font.Bold = true;
            //row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            //row.Cells[0].MergeRight = 4;
            //row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " ");

            //// Add the VAT row
            //row = this.table.AddRow();
            //row.Cells[0].Borders.Visible = false;
            //row.Cells[0].AddParagraph("VAT (19%)");
            //row.Cells[0].Format.Font.Bold = true;
            //row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            //row.Cells[0].MergeRight = 4;
            //row.Cells[5].AddParagraph((0.19 * totalExtendedPrice).ToString("0.00") + " ");

            //// Add the additional fee row
            //row = this.table.AddRow();
            //row.Cells[0].Borders.Visible = false;
            //row.Cells[0].AddParagraph("Shipping and Handling");
            //row.Cells[5].AddParagraph(0.ToString("0.00") + " ");
            //row.Cells[0].Format.Font.Bold = true;
            //row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            //row.Cells[0].MergeRight = 4;

            //// Add the total due row
            //row = this.table.AddRow();
            //row.Cells[0].AddParagraph("Total Due");
            //row.Cells[0].Borders.Visible = false;
            //row.Cells[0].Format.Font.Bold = true;
            //row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            //row.Cells[0].MergeRight = 4;
            //totalExtendedPrice += 0.19 * totalExtendedPrice;
            //row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " ");

            // Set the borders of the specified cell range
            table.SetEdge(4, table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);

            // Add the notes paragraph

            //item = SelectItem("/invoice");
            //paragraph.AddText(GetValue(item, "notes"));
        }

        /// <summary>
        /// Selects a subtree in the XML data.
        /// </summary>
        XPathNavigator SelectItem(string path)
        {
            XPathNodeIterator iter = navigator.Select(path);
            iter.MoveNext();
            return iter.Current;
        }

        /// <summary>
        /// Gets an element value from the XML data.
        /// </summary>
        static string GetValue(XPathNavigator nav, string name)
        {
            //nav = nav.Clone();
            XPathNodeIterator iter = nav.Select(name);
            iter.MoveNext();
            return iter.Current.Value;
        }

        /// <summary>
        /// Gets an element value as double from the XML data.
        /// </summary>
        static double GetValueAsDouble(XPathNavigator nav, string name)
        {
            try
            {
                string value = GetValue(nav, name);
                if (value.Length == 0)
                    return 0;
                return Double.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return 0;
        }

        // Some pre-defined colors
#if true
        // RGB colors
        static readonly Color TableBorder = new Color(74, 74, 74);
        static readonly Color TableBlue = new Color(208, 170, 170);
        static readonly Color TableGray = new Color(242, 242, 242);
#else
    // CMYK colors
    readonly static Color tableBorder = Color.FromCmyk(100, 50, 0, 30);
    readonly static Color tableBlue = Color.FromCmyk(0, 80, 50, 30);
    readonly static Color tableGray = Color.FromCmyk(30, 0, 0, 0, 100);
#endif
    }
}