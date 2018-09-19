
namespace RedmineMailService
{


    class SendMail
    {


        public static void Test()
        {
            string json = SQL.ExecuteScalar("SELECT TOP 1 FC_Value FROM T_FMS_Configuration WHERE FC_Key LIKE 'SMTP'"); ;
            MailSettings ms = MailSettings.FromJson(json);
            MailService mss = new MailService(ms);

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("col", typeof(string));
            dt.Rows.Add("& Ciao");


            using (MailTemplate mt = new MailTemplate())
            {
                mt.TemplateString = "Hello {@col} World<img src='naegeli1.jpg' /><img src='pp.jpg' />";
                mt.Subject = "This is a test";
                mt.From = ms.FromAddress;
                mt.To = "undiclosed@example.com";
                mt.ReplyTo = ms.FromAddress;

                mt.EmbeddedImages.Add(
                    new Resource(@"C:\Users\Administrator\Pictures\naegeli1.jpg")
                );

                mt.EmbeddedImages.Add(
                    new Resource(@"C:\Users\Administrator\Pictures\pp.jpg")
                );

                mt.AttachmentFiles.Add(
                    new Resource(@"C:\Program Files\Microsoft\R Client\R_SERVER\doc\NEWS.pdf")
                );

                mss.OnStart += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    System.Console.WriteLine("Start ! ");
                };

                mss.OnSuccess += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    System.Console.WriteLine("Success ! ");
                };

                mss.OnError += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    System.Console.WriteLine("Error ! ");
                };

                mss.OnDone += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    System.Console.WriteLine("Done ! ");
                };

                mss.SendMail(mt, dt.Rows[0]);
            } // End Sub Test 


        } // End Sub Test 


    } // End Class SendMail 


} // End Namespace RedmineMailService 
