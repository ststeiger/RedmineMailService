
using RedmineMailService;

//namespace RedmineMailService.Tests
namespace ABCDE
{


    public class MassMail 
    {
        public System.Data.DataTable Data;
        protected MailSettings m_mailSettings;

        public MassMail()
        { }

        public MassMail(MailSettings mailSettings)
        {
            this.m_mailSettings = mailSettings;
        }

        // When multiple handlers are associated with a single event in C# 
        // and the handler signature has a return type, 
        // then the value returned by the last handler executed 
        // will be the one returned to the event raiser.
        public delegate bool SaveEventHandler_t(object sender, System.EventArgs e);
        public event SaveEventHandler_t OnStart;
        public event SaveEventHandler_t OnSuccess;
        public event SaveEventHandler_t OnFailure;
        public event SaveEventHandler_t OnAlways;
        public event SaveEventHandler_t OnDone;


        public void Send()
        {
            OnStart(this, null);

            foreach (System.Data.DataRow dr in Data.Rows)
            {
                if( object.Equals("a", "b") )
                    OnSuccess(this, null);
                else
                    OnFailure(this, null);

                OnAlways(this, null);
            } // Next dr 

            OnDone(this, null); 
        } // End Sub Send 


    } // End Class MassMail 



    public class TemplateMail
    {
        public string Template;
        
        public System.Collections.Generic.List<Resource> Images;
        public System.Collections.Generic.List<Resource> Attachments;
        


        public TemplateMail()
        { }

        public void foo()
        {
            var x = new MassMail();
            x.OnFailure += delegate (object sender, System.EventArgs e)
            {
                return false;
            };
        }


    }




    class MyMail
    {


        public static void SendAttachment()
        {
            MailSettings ms = new MailSettings()
            {
                Host = RedmineMailService.Trash.UserData.GMailSMTP,
                Username = RedmineMailService.Trash.UserData.GMail,
                Password = RedmineMailService.Trash.UserData.GMailPassword,
                FromAddress = RedmineMailService.Trash.UserData.info,
                Port = 587, // 25,
                Ssl = true
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ms, Newtonsoft.Json.Formatting.Indented);
            System.Console.WriteLine(json);

            SendAttachment(
                  ms
                , new Resource(@"D:\username\Desktop\Intro to Docker.pdf")
                , new Resource(@"D:\username\Desktop\NET_Core-2.0-Getting_Started_Guide-en-US.pdf")
            );
        }



        public static void SendAttachment(MailSettings ms, params Resource[] att)
        {
            SendAttachment(ms, att, true);
        }


        public static void SendAttachment(MailSettings ms, System.Collections.Generic.IEnumerable<Resource> attachments)
        {
            SendAttachment(ms, attachments, true);
        }


        public static void SendAttachment(MailSettings ms, System.Collections.Generic.IEnumerable<Resource> attachments, bool dispose)
        {
            System.Collections.Generic.List<System.Net.Mail.Attachment> lsAttachments =
                new System.Collections.Generic.List<System.Net.Mail.Attachment>();

            foreach (Resource thisAttachment in attachments)
            {
                lsAttachments.Add(
                    new System.Net.Mail.Attachment(
                          thisAttachment.Stream
                        , thisAttachment.FileName
                        , thisAttachment.ContentType
                    )
                );
            } // Next i 

            SendAttachment(ms, lsAttachments);

            if (dispose)
            {
                for (int i = 0; i < lsAttachments.Count; ++i)
                {
                    if (lsAttachments[i] != null)
                        lsAttachments[i].Dispose();
                } // Next i 

                foreach (Resource thisAttachment in attachments)
                {
                    if (thisAttachment.Stream != null)
                        thisAttachment.Stream.Dispose();
                } // Next thisAttachment 

            } // End if (dispose) 

        } // End Sub SendAttachment 





        // https://stackoverflow.com/questions/18358534/send-inline-image-in-email
        private static System.Net.Mail.AlternateView GetAlternativeView(string htmlBody, string filePath)
        {
            System.Net.Mail.LinkedResource res = new System.Net.Mail.LinkedResource(filePath, "image/png");
            res.ContentId = System.Guid.NewGuid().ToString();
            htmlBody = htmlBody.Replace("{@COR_LOGO}", "cid:" + res.ContentId);

            System.Net.Mail.AlternateView alternateView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(htmlBody, null, System.Net.Mime.MediaTypeNames.Text.Html);
            
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }


        private static void SendAttachment(MailSettings ms, System.Collections.Generic.IEnumerable<System.Net.Mail.Attachment> attachments)
        {

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                mail.HeadersEncoding = System.Text.Encoding.UTF8;

                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Subject = "Test-Mail mit Anlage 你好，世界";

                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.BodyTransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                mail.IsBodyHtml = true;
                mail.Body = System.IO.File.ReadAllText("mail_template.htm");
                mail.AlternateViews.Add(GetAlternativeView(mail.Body, "logo.png"));
                
                foreach (System.Net.Mail.Attachment thisAttachment in attachments)
                {
                    mail.Attachments.Add(thisAttachment);
                } // Next i 

                mail.From = new System.Net.Mail.MailAddress(ms.FromAddress, ms.FromName);

                mail.To.Add(new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.info, "A"));
                // mail.To.Add(new System.Net.Mail.MailAddress("user1@friends.com", "B"));
                // mail.To.Add(new System.Net.Mail.MailAddress("user2@friends.com", "B"));

                mail.ReplyToList.Add(new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.ReplyTo, "Catch22"));

                Send(ms, mail);
            } // End Using mail 

        } // End Sub SendAttachment 


        public static void Send(MailSettings ms, System.Net.Mail.MailMessage message)
        {
            // http://vmstzhdb/Reports_HBD_DBH
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient())
            {
                client.Host = ms.Host;
                client.Port = ms.Port;
                client.EnableSsl = ms.Ssl;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                client.UseDefaultCredentials = ms.DefaultCredentials;
                // Must be after UseDefaultCredentials 
                if (client.UseDefaultCredentials)
                    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                else
                    client.Credentials = new System.Net.NetworkCredential(ms.Username, ms.Password);

                try
                {
                    client.Send(message);
                }
                catch (System.Exception ex)
                {
                    do
                    {
                        System.Console.Write("Fehler: ");
                        System.Console.WriteLine(ex.Message);
                        System.Console.WriteLine("Stacktrace: ");
                        System.Console.WriteLine(ex.StackTrace);
                        System.Console.WriteLine(System.Environment.NewLine);
                        ex = ex.InnerException;
                    } while (ex != null);

                } // End Catch 

            } // End Using client 

        } // End Sub Send 


    }


}
