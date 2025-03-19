using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Mail;
using System.Windows;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MySqlConnector;
using PdfSharp.Pdf;
using waerp_toolpilot.config.SettingsStore;
using waerp_toolpilot.dbtools;
using waerp_toolpilot.errorHandling;
using waerp_toolpilot.Functions;
using waerp_toolpilot.models;
using waerp_toolpilot.store;
using static waerp_toolpilot.application.OrderSystem.CurrentOrders.CurrentOrdersView;

namespace waerp_toolpilot.sql
{


    internal class OrderItemOverviewQueries
    {

        public static MySqlConnection conn = new MySqlConnection(SqlConn.GetConnectionString());

        public static int GetCurrentStock()
        {
            String que = "SELECT * FROM item_objects";
            DataSet ds = RunSql(que);
            int output = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                output += int.Parse(ds.Tables[0].Rows[i]["item_quantity_total"].ToString());
            }
            return output;
        }
        public static int GetCurrentRent()
        {
            String que = "SELECT * FROM item_rents";
            DataSet ds = RunSql(que);
            int output = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                output += int.Parse(ds.Tables[0].Rows[i]["rent_quantity"].ToString());
            }
            return output;
        }

        public static int GetCurrentNew()
        {
            String que = "SELECT * FROM item_objects";
            DataSet ds = RunSql(que);
            int output = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                output += int.Parse(ds.Tables[0].Rows[i]["item_quantity_total_new"].ToString());
            }
            return output;
        }

        public static bool CreateOrder()
        {
            DateTime orderDateTime = DateTime.Now;
            string sqlFormattedDate = orderDateTime.ToString("yyyy-MM-dd HH:mm:ss");


            string CurrentOderString1 = orderDateTime.ToString("yyyy") + orderDateTime.ToString("MM") + orderDateTime.ToString("dd");
            DataSet ds = RunSql($"SELECT * FROM order_objects WHERE order_ident = {CurrentOderString1}");
            int CurrentOrderNo = ds.Tables[0].Rows.Count + 1;

            string CurrentOrderIdent = CurrentOderString1 + "-" + CurrentOrderNo.ToString();
            string orderIdent = "";

            DataSet tmp = new DataSet();
            var vendors = new List<string>();

            for (int i = 0; i < ShoppingCartModel.ShoppingCartInput.Tables[0].Rows.Count; i++)
            {
                tmp = RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {ShoppingCartModel.ShoppingCartInput.Tables[0].Rows[i]["item_id"]}");
                bool check = false;
                for (int j = 0; j < vendors.Count; j++)
                {
                    if (vendors[j] == tmp.Tables[0].Rows[0]["vendor_id"].ToString())
                    {
                        check = true;
                    }
                }
                if (!check)
                {
                    vendors.Add(tmp.Tables[0].Rows[0]["vendor_id"].ToString());
                }
            }

            for (int i = 0; i < ShoppingCartModel.ShoppingCartInput.Tables[0].Rows.Count; i++)
            {
                orderIdent = "";

                string maxIdStr = GetMaxId(RunSql("SELECT * FROM order_item_relations"), "order_id");
                string vendorID = AdministrationQueries.RunSql($"SELECT * FROM item_vendor_relations WHERE item_id = {ShoppingCartModel.ShoppingCartInput.Tables[0].Rows[i]["item_id"]}").Tables[0].Rows[0]["vendor_id"].ToString();

                RunSqlExec($"UPDATE item_objects SET item_onorder = 1 WHERE item_id = {ShoppingCartModel.ShoppingCartInput.Tables[0].Rows[i]["item_id"]}");

                orderIdent = CurrentOrderIdent + "-" + vendorID;


                RunSqlExec($"INSERT INTO order_item_relations (order_id, order_ident, item_id, order_quantity,order_quantity_org, vendor_id, isOpen, createdAt) VALUES ({maxIdStr}, '{orderIdent}', {ShoppingCartModel.ShoppingCartInput.Tables[0].Rows[i]["item_id"]}, {ShoppingCartModel.ShoppingCartInput.Tables[0].Rows[i]["order_quantity"]}, {ShoppingCartModel.ShoppingCartInput.Tables[0].Rows[i]["order_quantity"]}, {vendorID}, 1,'{sqlFormattedDate}')");
            }

            for (int i = 0; i < vendors.Count; i++)
            {
                OrderData.CurrentSelectedVendor = RunSql($"SELECT * FROM vendor_objects WHERE vendor_id = {vendors[i]}").Tables[0].Rows[0];



                RunSqlExec($"INSERT INTO order_objects (order_id, order_ident, order_status, order_email_sent, order_date) VALUES ({GetMaxId(RunSql("SELECT * FROM order_objects"), "order_id")}, '{CurrentOrderIdent + "-" + vendors[i]}', 1, {OrderData.CurrentSelectedVendor["vendor_auto_order"].ToString()}, '{sqlFormattedDate}')");

                DataSet itemCollection = RunSql($"SELECT * FROM order_item_relations WHERE order_ident = '{CurrentOrderIdent + "-" + vendors[i]}'");

                itemCollection.Tables[0].Columns.Add("item_ident");
                itemCollection.Tables[0].Columns.Add("item_description");
                itemCollection.Tables[0].Columns.Add("item_description_2");
                itemCollection.Tables[0].Columns.Add("item_diameter");

                for (int j = 0; j < itemCollection.Tables[0].Rows.Count; j++)
                {
                    DataSet tmp1 = RunSql($"SELECT * FROM item_objects WHERE item_id = {itemCollection.Tables[0].Rows[j]["item_id"]}");
                    DataSet tmp2 = RunSql($"SELECT * FROM order_item_relations WHERE order_ident = '{CurrentOrderIdent + "-" + vendors[i]}' AND item_id = {itemCollection.Tables[0].Rows[j]["item_id"]}");

                    itemCollection.Tables[0].Rows[j]["item_ident"] = tmp1.Tables[0].Rows[0]["item_ident"];
                    itemCollection.Tables[0].Rows[j]["item_description"] = tmp1.Tables[0].Rows[0]["item_description"];
                    itemCollection.Tables[0].Rows[j]["item_description_2"] = tmp1.Tables[0].Rows[0]["item_description_2"];
                    itemCollection.Tables[0].Rows[j]["item_diameter"] = tmp1.Tables[0].Rows[0]["item_diameter"];

                    // itemCollection.Tables[0].Rows[j]["order_quantity"] = tmp2.Tables[0].Rows[0]["order_quantity"];

                }


                OrderStore.OrderIdent = CurrentOrderIdent + "-" + vendors[i];
                OrderData.ProductData = itemCollection;


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

                //if (OrderData.CurrentSelectedVendor["vendor_auto_order"].ToString() == "1")
                //{
                //    SendOrderMail(savePath);
                //}


            }

            return true;
        }

        public static bool SendOrderMail(string attachmentPath)
        {
            DataSet companyInfoDB = new DataSet();
            companyInfoDB = AdministrationQueries.RunSql("SELECT * FROM company_settings");




            System.Net.Mail.SmtpClient mySmtpClient = new System.Net.Mail.SmtpClient();

            mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = new System.Net.NetworkCredential("reportsender@waerp.software", "Waerp_reportsender_2022");
            mySmtpClient.Port = 587;
            mySmtpClient.Host = "smtp.strato.de";
            mySmtpClient.EnableSsl = true;

            // set smtp-client with basicAuthentication

            // add from,to mailaddresses
            MailAddress from = new MailAddress("reportsender@waerp.software", CompanySettings.Default.CompanyName);
            MailAddress to = new MailAddress("Testbestellung@waerp.software", "Test E-Mail Account");
            System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage(from, to);


            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(attachmentPath);
            myMail.Attachments.Add(attachment);


            // add ReplyTo
            MailAddress replyTo = new MailAddress("Testbestellung@waerp.software");
            myMail.ReplyToList.Add(replyTo);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            // set subject and encoding
            myMail.Subject = $"BESTELLAUFTRAG VON: {companyInfoDB.Tables[0].Rows[0][2]}";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            // set body-message and encoding
            myMail.Body = $"Guten Tag, " +
                $"</br> </br> " +
                $"dies ist ein Bestellauftrag der Firma {companyInfoDB.Tables[0].Rows[0][2]}. " +
                $"\n \n " +
                $"Dieser wurde automatisch vom Bestellsystem erstellt." +
                $"<br> " +
                $"Bitte schicken Sie eine Bestellbestätigung und Rechnung an uns zurück. <br><br>" +
                $"Falls Sie Fragen zur Bestellung haben, dann können Sie uns telefonisch unter {companyInfoDB.Tables[0].Rows[4][2]} oder schreiben Sie uns eine E-Mail an {companyInfoDB.Tables[0].Rows[5][2]}. </br> </br>" +
                $"Bitte nutzen Sie im Kontakt mit uns die Referenznummer <b>{OrderStore.OrderIdent}</b>, damit wir Ihre Anfrage der richten Bestellung zuordnen können." +
                $"</br> </br> " +
                $"Mit freundlichen Grüßen," +
                $"<br>" +
                $"{companyInfoDB.Tables[0].Rows[0][2]}";
            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            // text or html
            myMail.IsBodyHtml = true;

            mySmtpClient.Send(myMail);

            return true;
        }


        public static DataSet GetAllItems()
        {
            string query = $@"SELECT 
	`item_objects`.`item_id`,
	`item_objects`.`item_ident`,
	`item_objects`.`item_quantity_total`,
	`item_objects`.`item_quantity_total_new`,
	`item_objects`.`item_onetime_use`,
	`item_objects`.`item_quantity_min`,
	`item_objects`.`item_orderquant_min`,
	`item_objects`.`item_orderable`,
	`item_objects`.`item_onorder`,
	`item_objects`.`item_description`,
	`item_objects`.`item_description_2`,
	`item_objects`.`item_diameter`,
	`item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_orderable = 1
	ORDER BY 
		item_ident ASC;";
            String que = "Select * from item_objects WHERE item_orderable = 1";
            DataSet ds = RunSql(query);
            ds.Tables[0].Columns.Add("vendor");
            ds.Tables[0].Columns.Add("vendor_id");
            ds.Tables[0].Columns.Add("order_quantity");
            ds.Tables[0].Columns.Add("lastOrderDate");
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                que = $"SELECT * FROM item_vendor_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}";
                DataSet ds2 = RunSql(que);
                if (ds2.Tables[0].Rows.Count != 0)
                {
                    que = $"SELECT * FROM vendor_objects WHERE vendor_id = {ds2.Tables[0].Rows[0]["vendor_id"]}";
                    DataSet ds3 = RunSql(que);
                    if (ds3.Tables[0].Rows.Count != 0)
                    {
                        ds.Tables[0].Rows[i]["vendor"] = ds3.Tables[0].Rows[0]["vendor_name"];
                        ds.Tables[0].Rows[i]["vendor_id"] = ds3.Tables[0].Rows[0]["vendor_id"];

                        DataSet oldOrders = RunSql($"SELECT * FROM order_item_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]} ORDER BY createdAt DESC");

                        if (oldOrders.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].Rows[i]["lastOrderDate"] = oldOrders.Tables[0].Rows[0]["createdAt"].ToString() + " (" + oldOrders.Tables[0].Rows[0]["order_quantity"] + ")";
                        }
                    }
                }


            }
            return ds;
        }
        public static DataSet GetAllItemsNeeded()
        {
            string query = $@"SELECT 
	`item_objects`.`item_id`,
	`item_objects`.`item_ident`,
	`item_objects`.`item_quantity_total`,
	`item_objects`.`item_quantity_total_new`,
	`item_objects`.`item_onetime_use`,
	`item_objects`.`item_quantity_min`,
	`item_objects`.`item_orderquant_min`,
	`item_objects`.`item_orderable`,
	`item_objects`.`item_onorder`,
	`item_objects`.`item_description`,
	`item_objects`.`item_description_2`,
	`item_objects`.`item_diameter`,
	`item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_quantity_total_new < item_quantity_min AND item_onorder = 0 AND item_orderable = 1
	ORDER BY 
		item_ident ASC;";
            String que = "Select * from item_objects WHERE item_quantity_total_new < item_quantity_min AND item_onorder = 0 AND item_orderable = 1";
            DataSet ds = RunSql(query);
            ds.Tables[0].Columns.Add("vendor");
            ds.Tables[0].Columns.Add("vendor_id");
            ds.Tables[0].Columns.Add("lastOrderDate");

            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                que = $"SELECT * FROM item_vendor_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}";
                DataSet ds2 = RunSql(que);
                if (ds2.Tables[0].Rows.Count != 0)
                {
                    que = $"SELECT * FROM vendor_objects WHERE vendor_id = {ds2.Tables[0].Rows[0]["vendor_id"]}";
                    DataSet ds3 = RunSql(que);
                    if (ds3.Tables[0].Rows.Count != 0)
                    {
                        ds.Tables[0].Rows[i]["vendor"] = ds3.Tables[0].Rows[0]["vendor_name"];
                        ds.Tables[0].Rows[i]["vendor_id"] = ds3.Tables[0].Rows[0]["vendor_id"];

                        DataSet oldOrders = RunSql($"SELECT * FROM order_item_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]} ORDER BY createdAt DESC");

                        if (oldOrders.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].Rows[i]["lastOrderDate"] = oldOrders.Tables[0].Rows[0]["createdAt"].ToString() + " (" + oldOrders.Tables[0].Rows[0]["order_quantity"] + ")";

                        }
                    }
                }


            }
            return ds;
        }

        public static DataSet GetAllItemsMin()
        {
            string query = $@"SELECT 
	`item_objects`.`item_id`,
	`item_objects`.`item_ident`,
	`item_objects`.`item_quantity_total`,
	`item_objects`.`item_quantity_total_new`,
	`item_objects`.`item_onetime_use`,
	`item_objects`.`item_quantity_min`,
	`item_objects`.`item_orderquant_min`,
	`item_objects`.`item_orderable`,
	`item_objects`.`item_onorder`,
	`item_objects`.`item_description`,
	`item_objects`.`item_description_2`,
	`item_objects`.`item_diameter`,
	`item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_quantity_total_new - item_quantity_min <= 2 AND item_onorder = 0 AND item_orderable = 1
	ORDER BY 
		item_ident ASC;";
            String que = "Select * from item_objects WHERE item_quantity_total_new - item_quantity_min <= 2 AND item_onorder = 0 AND item_orderable = 1";
            DataSet ds = RunSql(query);
            ds.Tables[0].Columns.Add("vendor");
            ds.Tables[0].Columns.Add("vendor_id");
            ds.Tables[0].Columns.Add("lastOrderDate");

            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                que = $"SELECT * FROM item_vendor_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}";
                DataSet ds2 = RunSql(que);
                if (ds2.Tables[0].Rows.Count != 0)
                {
                    que = $"SELECT * FROM vendor_objects WHERE vendor_id = {ds2.Tables[0].Rows[0]["vendor_id"]}";
                    DataSet ds3 = RunSql(que);
                    if (ds3.Tables[0].Rows.Count != 0)
                    {
                        ds.Tables[0].Rows[i]["vendor"] = ds3.Tables[0].Rows[0]["vendor_name"];
                        ds.Tables[0].Rows[i]["vendor_id"] = ds3.Tables[0].Rows[0]["vendor_id"];
                        DataSet oldOrders = RunSql($"SELECT * FROM order_item_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]} ORDER BY createdAt DESC");

                        if (oldOrders.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].Rows[i]["lastOrderDate"] = oldOrders.Tables[0].Rows[0]["createdAt"].ToString() + " (" + oldOrders.Tables[0].Rows[0]["order_quantity"] + ")";


                        }
                    }
                }


            }
            return ds;
        }
        public static DataSet GetAllItemsOrdered()
        {
            string query = $@"SELECT 
	`item_objects`.`item_id`,
	`item_objects`.`item_ident`,
	`item_objects`.`item_quantity_total`,
	`item_objects`.`item_quantity_total_new`,
	`item_objects`.`item_onetime_use`,
	`item_objects`.`item_quantity_min`,
	`item_objects`.`item_orderquant_min`,
	`item_objects`.`item_orderable`,
	`item_objects`.`item_onorder`,
	`item_objects`.`item_description`,
	`item_objects`.`item_description_2`,
	`item_objects`.`item_diameter`,
	`item_objects`.`item_documentation_path`,
	`item_objects`.`item_objectscol`,
	CONCAT('{RunningParameters.imagePath.Replace("\\", "\\\\")}', '\\', `item_image_path`) AS item_image_path
	FROM 
		item_objects WHERE item_onorder = 1 AND item_orderable = 1
	ORDER BY 
		item_ident ASC;";
            String que = "Select * from item_objects WHERE item_onorder = 1 AND item_orderable = 1";
            DataSet ds = RunSql(query);
            ds.Tables[0].Columns.Add("vendor");
            ds.Tables[0].Columns.Add("vendor_id");
            ds.Tables[0].Columns.Add("lastOrderDate");

            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                que = $"SELECT * FROM item_vendor_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]}";
                DataSet ds2 = RunSql(que);
                if (ds2.Tables[0].Rows.Count != 0)
                {
                    que = $"SELECT * FROM vendor_objects WHERE vendor_id = {ds2.Tables[0].Rows[0]["vendor_id"]}";
                    DataSet ds3 = RunSql(que);
                    if (ds3.Tables[0].Rows.Count != 0)
                    {
                        ds.Tables[0].Rows[i]["vendor"] = ds3.Tables[0].Rows[0]["vendor_name"];
                        ds.Tables[0].Rows[i]["vendor_id"] = ds3.Tables[0].Rows[0]["vendor_id"];

                        DataSet oldOrders = RunSql($"SELECT * FROM order_item_relations WHERE item_id = {ds.Tables[0].Rows[i]["item_id"]} ORDER BY createdAt DESC");

                        if (oldOrders.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].Rows[i]["lastOrderDate"] = oldOrders.Tables[0].Rows[0]["createdAt"].ToString() + " (" + oldOrders.Tables[0].Rows[0]["order_quantity"] + ")";

                        }
                    }
                }


            }
            return ds;
        }
        public static string GetMaxId(DataSet ds, string Prompt)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                return "0";
            }
            else
            {
                int maxID = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (int.Parse(row[Prompt].ToString()) > maxID)
                    {
                        maxID = int.Parse(row[Prompt].ToString());
                    }
                }
                maxID++;
                return maxID.ToString();
            }

        }
        private static DataSet RunSql(string query)
        {
            try
            {
                conn.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter();
                DataSet ReturnDataSet = new DataSet();
                adp = new MySqlDataAdapter(new MySqlCommand(query, conn));
                adp.Fill(ReturnDataSet);
                conn.Close();
                return ReturnDataSet;
            }
            catch (MySqlException ex)
            {
                ErrorLogger.LogSqlError(ex);
                return null;

            }
            finally { conn.Close(); }
        }
        private static void RunSqlExec(string query)
        {
            try
            {
                conn.Open();
                new MySqlCommand(query, conn).ExecuteNonQuery();
                conn.Close();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("ASSDJSJ " + ex);
                //  ErrorLogger.LogSqlError(ex);

            }
            finally { conn.Close(); }
        }
    }
}
