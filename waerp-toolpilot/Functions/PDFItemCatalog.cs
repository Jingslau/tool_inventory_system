using System.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

namespace waerp_toolpilot.Functions
{
    public class PDFItemCatalog
    {
        public Document CreateItemsPdfFromDataTable(DataTable dataTable, string vendorName)
        {
            Document document = new Document();
            document.Info.Title = "Tool Drawer Labels";
            document.Info.Subject = "Tool Drawer Label Layout";
            document.Info.Author = "Toolpilot";

            // Define styles
            DefineStyles(document);

            // Label and layout settings
            double labelWidth = 6.35; // cm
            double labelHeight = 3.81; // cm
            double horizontalGap = 0.2; // cm
            int labelsPerRow = 3;
            int rowsPerPage = 7;

            double totalLabelWidth = (labelWidth * labelsPerRow) + (horizontalGap * (labelsPerRow - 1));
            double totalLabelHeight = labelHeight * rowsPerPage;
            double horizontalMargin = ((21 - totalLabelWidth) / 2) + 0.15; // A4 width = 21 cm
            double verticalMargin = ((29.7 - totalLabelHeight) / 2) - 0.15; // A4 height = 29.7 cm

            int labelsPerPage = labelsPerRow * rowsPerPage;
            int totalLabels = dataTable.Rows.Count;

            // Process all labels and create pages dynamically
            for (int pageIndex = 0; pageIndex < (totalLabels + labelsPerPage - 1) / labelsPerPage; pageIndex++)
            {
                // Create a new section for each page
                Section section = document.AddSection();
                section.PageSetup.PageFormat = PageFormat.A4;
                section.PageSetup.Orientation = Orientation.Portrait;
                section.PageSetup.LeftMargin = Unit.FromCentimeter(horizontalMargin);
                section.PageSetup.RightMargin = Unit.FromCentimeter(horizontalMargin);
                section.PageSetup.TopMargin = Unit.FromCentimeter(verticalMargin);
                section.PageSetup.BottomMargin = Unit.FromCentimeter(verticalMargin);

                // Create the table for this page
                Table table = section.AddTable();

                // Add columns for labels and gaps
                for (int i = 0; i < labelsPerRow; i++)
                {
                    table.AddColumn(Unit.FromCentimeter(labelWidth));
                    if (i < labelsPerRow - 1)
                    {
                        table.AddColumn(Unit.FromCentimeter(horizontalGap));
                    }
                }

                // Add rows for this page
                for (int row = 0; row < rowsPerPage; row++)
                {
                    Row tableRow = table.AddRow();
                    tableRow.Height = Unit.FromCentimeter(labelHeight);

                    for (int col = 0; col < labelsPerRow; col++)
                    {
                        int dataIndex = pageIndex * labelsPerPage + row * labelsPerRow + col;
                        if (dataIndex >= totalLabels) break;

                        var cell = tableRow.Cells[col * 2];
                        DataRow data = dataTable.Rows[dataIndex];

                        // Add item information
                        string itemName = data["item_ident"].ToString();
                        string itemDescription = data["item_description"].ToString();
                        string itemDescription2 = data["item_description_2"].ToString();
                        string imagePath = data["item_image_path"].ToString();
                        string vendor = vendorName.Equals("kein Lieferant") ? "" : data["vendor_name"].ToString();

                        Paragraph p = cell.AddParagraph();
                        p.AddFormattedText(itemName, TextFormat.Bold);
                        p.AddLineBreak();
                        p.AddText(itemDescription);
                        p.AddLineBreak();
                        p.AddText(itemDescription2);

                        if (!string.IsNullOrEmpty(imagePath) && !imagePath.Contains("default.jpg"))
                        {
                            if (System.IO.File.Exists(imagePath))
                            {
                                Paragraph imageParagraph = cell.AddParagraph();
                                Image image = imageParagraph.AddImage(imagePath);
                                image.Height = Unit.FromCentimeter(1.5); // Constrain width
                                image.Width = Unit.FromCentimeter(2.5); // Constrain width

                                image.LockAspectRatio = true;
                                imageParagraph.Format.Alignment = ParagraphAlignment.Center;
                            }
                        }

                        // Style the cell
                        cell.Format.Alignment = ParagraphAlignment.Center;
                        cell.VerticalAlignment = VerticalAlignment.Center;
                        cell.Borders.Width = 0.25;
                    }
                }
            }

            return document;
        }

        private void DefineStyles(Document document)
        {
            Style style = document.Styles["Normal"];
            style.Font.Name = "Verdana";
            style.Font.Size = 9;
        }
        private string GenerateBarcode(string content)
        {
            // Implement barcode generation
            return string.Empty; // Placeholder
        }
    }
}
