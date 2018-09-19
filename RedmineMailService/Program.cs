
namespace RedmineMailService
{


    public class Program
    {


        public static void Doku()
        {
            // alert(&quot;This is a test...&quot;);
            // string res = System.Web.HttpUtility.HtmlAttributeEncode("alert(\"This is a test...\");");
            // System.Console.WriteLine(res);


            // Allow text-only email ? 
            // HtmlAgilityPack

            // <b>Test</b> ==> *Test*
            // <i>Test</i> ==> _Test_
            // <u>Test</u> ==> +Test+
            // <del>Test</del> ==> -Test-
            // <code>Test</code> ==> @test@
            // <pre>Test</pre> ==> <pre>test</pre>

            // <h1>Test</h1> ==> h1. test
            // ul => * hello
            // num => # hello
            // links einrücken > 
            // link
            // image



            /*
            Tracker: Anfrage
            Status: neu
            Kundenname:
            Verrechenbar: nein
            gemeldet von: email


            From: foo@domain.com ==> projekte/ZH
            To: issue.tracker@cor
            Date:

            Sub: <issue title>


            Content: <html>
            Anlage:


            DB:
            projects (sync)
            kundenname (sync)
            kunde 
            --domains (manual)
            zo_kunde_multiAtt(kunde_id, domain/_id, email) UIX where domain is not null 
            zo_kunde_email(kunde, email)
            */

            // Read unread emails
            // Add redmine issue
            // Move email to non-inbox folder
            // don't explode on any step
            // How to ensure text-only email ? 
        } // End Sub Doku 




        [System.STAThread]
        static void Main(string[] args)
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
                mt.To = "stefan.steiger@rsnweb.ch";
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
            }


            // RedmineMailService.Tests.StructureInfo.Test();
            MailSender.Test();
            
            
            
            System.Configuration.ConfigurationManager.AppSettings.Get();
            RedmineMailService.Exchange.SendMailWithAttachment();
            return;
            // https://github.com/dasMulli/dotnet-win32-service
            // ServiceStarter.Start(args);

            // MailSender.Test();

            CertificateCallback.Initialize();
            Titanium.Web.Proxy.Examples.Basic.ProxyTestController controller = Titanium.Web.Proxy.Examples.Basic.ProxyServerProgram.Start();

            // TestMailReader.Test();
            

            controller.Stop();
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace RedmineMailService 
