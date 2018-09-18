
namespace RedmineMailService
{


    public class MailService
        : IMailService
    {

        public MailSettings m_mailSettings;


        // When multiple handlers are associated with a single event in C# 
        // and the handler signature has a return type, 
        // then the value returned by the last handler executed 
        // will be the one returned to the event raiser.
        public delegate void SaveEventHandler_t(MailSettings ms, BaseMailTemplate mail, System.DateTime tm, System.Exception exception);
        public event SaveEventHandler_t OnStart;
        public event SaveEventHandler_t OnSuccess;
        public event SaveEventHandler_t OnError;
        public event SaveEventHandler_t OnDone;
        


        public MailService(MailSettings ms)
        {
            this.m_mailSettings = ms;
        } // End Constructor 


        // https://stackoverflow.com/questions/18358534/send-inline-image-in-email
        private static System.Net.Mail.AlternateView GetAlternativeView(string htmlBody, System.Collections.Generic.List<Resource> embeddedImages)
        {
            foreach (Resource thisResource in embeddedImages)
            {
                htmlBody = htmlBody.Replace(thisResource.FileName, "cid:" + thisResource.UID);
            } // Next thisResource 

            System.Net.Mail.AlternateView alternateView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(
                  htmlBody
                , null
                , System.Net.Mime.MediaTypeNames.Text.Html
            );

            foreach (Resource thisResource in embeddedImages)
            {
                System.Net.Mail.LinkedResource res = new System.Net.Mail.LinkedResource(
                      thisResource.Stream 
                    , thisResource.ContentType 
                );

                res.ContentId = thisResource.UID;
                alternateView.LinkedResources.Add(res);
            } // Next thisResource 

            return alternateView;
        } // End Function GetAlternativeView 


        public void SendMail(BaseMailTemplate mt, System.Data.DataRow dr)
        {
            if(OnStart != null)
                OnStart(this.m_mailSettings, mt, System.DateTime.Now, null);

            try
            {

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    mail.HeadersEncoding = System.Text.Encoding.UTF8;

                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Subject = mt.Subject;
                    
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.BodyTransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    mail.IsBodyHtml = true;
                    mail.Body = mt.TemplateString;
                    
                    if(mt.EmbeddedImages.Count > 0)
                        mail.AlternateViews.Add(GetAlternativeView(mail.Body, mt.EmbeddedImages));

                    foreach (Resource thisAttachment in mt.AttachmentFiles)
                    {
                        new System.Net.Mail.Attachment(
                              thisAttachment.Stream
                            , thisAttachment.FileName
                            , thisAttachment.ContentType
                        );

                    } // Next i 

                    mail.From = new System.Net.Mail.MailAddress(mt.From, mt.FromName);
                    mail.To.Add(new System.Net.Mail.MailAddress(mt.To, mt.ToName));

                    // mail.To.Add(new System.Net.Mail.MailAddress("user1@friends.com", "Friend 1"));
                    // mail.To.Add(new System.Net.Mail.MailAddress("user2@friends.com", "Friend 2"));

                    mail.ReplyToList.Add(new System.Net.Mail.MailAddress(mt.ReplyTo, mt.ReplyToName));

                    Send(mail);

                    if (OnSuccess != null)
                        OnSuccess(this.m_mailSettings, mt, System.DateTime.Now, null);
                } // End Using mail 

            } // End Try 
            catch (System.Exception ex)
            {
                if (OnError != null)
                    OnError(this.m_mailSettings, mt, System.DateTime.Now, ex);

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

            if (OnDone != null)
                OnDone(this.m_mailSettings, mt, System.DateTime.Now, null);
        } // End Sub SendMail 


        void IMailService.SendMail(BaseMailTemplate template, System.Data.DataRow dr)
        {
            this.SendMail(template, dr);
        } // End Sub IMailService.SendMail 


        private void Send(System.Net.Mail.MailMessage message)
        {
            // http://vmstzhdb/Reports_HBD_DBH
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient())
            {
                client.Host = this.m_mailSettings.Host;
                client.Port = this.m_mailSettings.Port;
                client.EnableSsl = this.m_mailSettings.Ssl;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                client.UseDefaultCredentials = this.m_mailSettings.DefaultCredentials;

                // Must be after UseDefaultCredentials 
                if (client.UseDefaultCredentials)
                    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                else
                    client.Credentials = new System.Net.NetworkCredential(
                          this.m_mailSettings.Username
                        , this.m_mailSettings.Password
                    );

                client.Send(message);
            } // End Using client 

        } // End Sub Send 


    } // End Class MailService 


} // End Namespace RedmineMailService 
