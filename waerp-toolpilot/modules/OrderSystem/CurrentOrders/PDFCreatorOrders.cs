using System.Data;
using System.Windows;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using waerp_toolpilot.sql;


namespace waerp_toolpilot.modules.OrderSystem.CurrentOrders
{
    internal class PDFCreatorOrders
    {
        private static Document _document;
        //    private PdfDocument _pdfDocument;
        public static void CreatePDF(string RecipientName, string RecipientAdress, string RecipientPostcode, string RecipientCity, string RecipientCountry)
        {

            DataSet CompanyInfo = AdministrationQueries.GetAllInfo("company_settings");

            // Create a new MigraDoc document
            _document = new Document();
            DefineStyles();

            // Set the page size and orientation
            _document.DefaultPageSetup.PageFormat = PageFormat.A4;  // Set the desired page format
            _document.DefaultPageSetup.Orientation = Orientation.Portrait;  // Set the desired orientation

            // Set the margins (in points)
            _document.DefaultPageSetup.LeftMargin = Unit.FromMillimeter(int.Parse(CompanyInfo.Tables[0].Rows[14][2].ToString()));
            _document.DefaultPageSetup.RightMargin = Unit.FromMillimeter(int.Parse(CompanyInfo.Tables[0].Rows[15][2].ToString()));
            _document.DefaultPageSetup.TopMargin = Unit.FromMillimeter(int.Parse(CompanyInfo.Tables[0].Rows[13][2].ToString()));
            _document.DefaultPageSetup.BottomMargin = Unit.FromMillimeter(int.Parse(CompanyInfo.Tables[0].Rows[12][2].ToString()));

            // Add a new section to the document
            var section = _document.AddSection();
            var backgroundImage = section.AddImage(CompanyInfo.Tables[0].Rows[11][2].ToString()); // Replace with the actual path to your background image file
            backgroundImage.Width = _document.DefaultPageSetup.PageWidth;
            backgroundImage.Height = _document.DefaultPageSetup.PageHeight;
            backgroundImage.RelativeVertical = RelativeVertical.Page;
            backgroundImage.RelativeHorizontal = RelativeHorizontal.Page;
            backgroundImage.LockAspectRatio = true;
            // Add the recipient's address
            var recipientAddress = section.AddTextFrame();
            recipientAddress.AddParagraph(RecipientName);
            recipientAddress.AddParagraph(RecipientAdress);
            recipientAddress.AddParagraph(RecipientPostcode + " " + RecipientCity);
            recipientAddress.AddParagraph(RecipientCountry);
            recipientAddress.Width = "7cm";
            recipientAddress.Left = ShapePosition.Left;
            recipientAddress.RelativeVertical = RelativeVertical.Page;
            recipientAddress.Top = _document.DefaultPageSetup.TopMargin + Unit.FromCentimeter(2);

            // Add the sender's address
            var senderAddress = section.AddTextFrame();
            senderAddress.AddParagraph(CompanyInfo.Tables[0].Rows[0][2].ToString());
            senderAddress.AddParagraph(CompanyInfo.Tables[0].Rows[1][2].ToString());
            senderAddress.AddParagraph(CompanyInfo.Tables[0].Rows[2][2].ToString() + " " + CompanyInfo.Tables[0].Rows[3][2]);
            senderAddress.Width = "7cm";
            senderAddress.Left = ShapePosition.Right;
            senderAddress.RelativeVertical = RelativeVertical.Page;
            senderAddress.Top = _document.DefaultPageSetup.TopMargin;

            // Add the letter content
            var letterContent = section.AddParagraph();
            letterContent.Format.SpaceBefore = Unit.FromCentimeter(5);
            letterContent.AddText((string)Application.Current.FindResource("errorText45a") + " " + RecipientName);
            letterContent.AddLineBreak();
            letterContent.AddLineBreak();
            letterContent.AddText((string)Application.Current.FindResource("errorText45b"));


            MessageBox.Show(CompanyInfo.Tables[0].Rows[11][2].ToString());
            // Set the background image


            // Set the image as background on every page



            // Save the document to a PDF file
            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = _document;
            pdfRenderer.RenderDocument();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            string pathS = key.GetValue("OrderOverviewPath").ToString();
            string pdfPath = pathS + "\\path_to_save_pdf.pdf"; // Replace with the actual path to save the PDF
            pdfRenderer.PdfDocument.Save(pdfPath);
        }

        private static void DefineStyles()
        {
            MigraDoc.DocumentObjectModel.Style style = _document.Styles["Normal"];

            style.Font.Name = "Calibri";

            style = _document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            style = _document.Styles.AddStyle("Table", "Normal");
            style.Font.Size = 16;
            style.Font.Name = "Verdana";

            style = _document.Styles.AddStyle("Headline", "Normal");
            style.ParagraphFormat.Font.Size = 18;
            style.ParagraphFormat.Font.Bold = true;
            style.ParagraphFormat.Font.Name = "Verdana";
            style.ParagraphFormat.Font.Color = Color.Parse("Black");

            style = _document.Styles.AddStyle("Text", "Normal");
            style.ParagraphFormat.Font.Size = 14;
            style.ParagraphFormat.Font.Name = "Verdana";
            style.ParagraphFormat.Font.Color = Color.Parse("Black");

            style = _document.Styles.AddStyle("Bold", "Normal");
            style.ParagraphFormat.Font.Size = 14;
            style.ParagraphFormat.Font.Bold = true;
            style.ParagraphFormat.Font.Name = "Verdana";
            style.ParagraphFormat.Font.Color = Color.Parse("Black");
        }
    }
}
