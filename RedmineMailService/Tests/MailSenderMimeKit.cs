
// using MailKit.Net.Smtp;
// using MailKit.Security;
// using MimeKit;


namespace RedmineMailService.Tests
{


    // https://www.c-sharpcorner.com/article/emailservice/
    // https://github.com/jstedfast/MimeKit/blob/master/samples/DkimVerifier/DkimVerifier/Program.cs
    public class MailSenderMimeKit
    {


        public static void foo()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("cellName", typeof(double));
            dt.Rows.Add(123.45);


            string val = ((double)dt.Rows[0]["cellName"]).ToString("N"); // val = "123.45"

            System.Data.DataRow dr = dt.Rows[0];

            double a = (double)dr["col1"];
            string val2 = $"Hello {a}";
            string val3 = $"Hello {dr["col1"].ToString()}";
            string val4 = $"Hello {((double)dr["col1"]).ToString("N")}"; 
        }


        // https://stackoverflow.com/questions/24170852/strip-attachments-from-emails-using-mailkit-mimekit
        static void SaveAttachments()
        {
            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.Load(@"x:\sample.eml");

            System.Collections.Generic.List<MimeKit.MimeEntity> attachments = 
                System.Linq.Enumerable.ToList(mimeMessage.Attachments);

            if(System.Linq.Enumerable.Any(attachments))
            {
                // Only multipart mails can have attachments
                MimeKit.Multipart multipart = mimeMessage.Body as MimeKit.Multipart;
                if (multipart != null)
                {

                    foreach (MimeKit.MimeEntity attachment in mimeMessage.Attachments)
                    {
                        multipart.Remove(attachment);
                    } // Next attachment 

                } // End if (multipart != null) 

                mimeMessage.Body = multipart;
            } // End if(System.Linq.Enumerable.Any(attachments)) 

            mimeMessage.WriteTo(new System.IO.FileStream(@"x:\stripped.eml", System.IO.FileMode.CreateNew));
        } // End Sub SaveAttachments 


        // https://github.com/jstedfast/MailKit/blob/master/Documentation/Examples/SmtpExamples.cs
        public static void SendMessage(MimeKit.MimeMessage message)
        {

            // using (SmtpClient client = new SmtpClient (new ProtocolLogger ("smtp.log"))) 
            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate("username", "password");
                client.Send(message);
                client.Disconnect(true);
            } // End Using client 

        } // End Sub SendMessage 


        class SmtpClientWithStatusNotification : MailKit.Net.Smtp.SmtpClient
        {
            protected override MailKit.DeliveryStatusNotification?
                GetDeliveryStatusNotifications(MimeKit.MimeMessage message, MimeKit.MailboxAddress mailbox)
            {
                //if (/* some criteria for deciding whether to get DSN's... */)
                return MailKit.DeliveryStatusNotification.Delay |
                       MailKit.DeliveryStatusNotification.Failure |
                       MailKit.DeliveryStatusNotification.Success;

                // return null;
            }
        }


        // https://stackoverflow.com/questions/39912942/mailkit-sendmail-doubts
        // https://stackoverflow.com/questions/37853903/can-i-send-files-via-email-using-mailkit
        public static void SendMessageComplex()
        {
            string userName = "";
            string userPass = "";
            string subject = "How you doin?";
            string plainText = @"Hey Alice,

What are you up to this weekend? Monica is throwing one of her parties on
Saturday and I was hoping you could make it.

Will you be my +1?

-- Joey
";
            string path = @"D:\testfile.pdf";

            string host = "smtp.gmail.com";
            int portNum = 443;
            bool useSsl = false;


            MimeKit.MimeMessage message = new MimeKit.MimeMessage ();
            message.From.Add (new MimeKit.MailboxAddress ("Joey", "joey@friends.com"));
            message.To.Add (new MimeKit.MailboxAddress ("Alice", "alice@wonderland.com"));
            message.Headers.Add("Sensitivity", "Company-confidential");

            // https://tools.ietf.org/html/rfc2076#page-16
            // https://tools.ietf.org/html/rfc1911
            // The case-insensitive values are "Personal" and "Private" 

            // If a sensitivity header is present in the message, a conformant
            // system MUST prohibit the recipient from forwarding this message to
            // any other user.  If the receiving system does not support privacy and
            // the sensitivity is one of "Personal" or "Private", the message MUST
            // be returned to the sender with an appropriate error code indicating
            // that privacy could not be assured and that the message was not
            // delivered [X400].


            MailKit.DeliveryStatusNotification delivery =
                  MailKit.DeliveryStatusNotification.Delay |
                  MailKit.DeliveryStatusNotification.Failure |
                  // MailKit.DeliveryStatusNotification.Never |
                  MailKit.DeliveryStatusNotification.Success;


            message.Headers.Add(
                new MimeKit.Header(MimeKit.HeaderId.ReturnReceiptTo, "test@example.com")
            ); // Delivery report


            message.ReplyTo.Add (new MimeKit.MailboxAddress ("Alice", "alice@wonderland.com"));

            message.Cc.Add (new MimeKit.MailboxAddress ("Alice", "alice@wonderland.com"));
            message.Bcc.Add (new MimeKit.MailboxAddress ("Alice", "alice@wonderland.com"));
            // message.Date = new System.DateTimeOffset(System.DateTime.Now);
            message.Date = System.DateTimeOffset.Now;
            //message.Attachments

            message.Importance = MimeKit.MessageImportance.High;
            message.Priority = MimeKit.MessagePriority.Urgent;
            message.XPriority = MimeKit.XMessagePriority.Highest;


            // message.HtmlBody
            // message.TextBody
            // message.Body
            // message.InReplyTo
            // message.MessageId
            // message.Sign();
            // message.Verify()
            // message.SignAndEncrypt();




            // Body (Mensagem)
            MimeKit.BodyBuilder bodyBuilder = new MimeKit.BodyBuilder();
            
            bodyBuilder.Attachments.Add(
                new MimeKit.MimePart("image", "gif"){
                     Content = new MimeKit.MimeContent(
                      System.IO.File.OpenRead(path)
                    , MimeKit.ContentEncoding.Default
                    )
                    ,ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment)
                    ,ContentTransferEncoding = MimeKit.ContentEncoding.Base64
                    ,FileName = System.IO.Path.GetFileName(path)
                 }
            );
            

            // bodyBuilder.LinkedResources.Add("fn", (byte[]) null, new MimeKit.ContentType("text", "html"));
            MimeKit.MimeEntity image = bodyBuilder.LinkedResources.Add("selfie.jpg"
                , (byte[])null
                , new MimeKit.ContentType("image", "jpeg")
            );

            image.ContentId = MimeKit.Utils.MimeUtils.GenerateMessageId();

            bodyBuilder.TextBody = "This is some plain text";
            // bodyBuilder.HtmlBody = "<b>This is some html text</b>";
            bodyBuilder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
<p>What are you up to this weekend? Monica is throwing one of her parties on
Saturday and I was hoping you could make it.<br>
<p>Will you be my +1?<br>
<p>-- Joey<br>
<center><img src=""cid:{0}""></center>", image.ContentId);

            message.Subject = subject;
            message.Body = bodyBuilder.ToMessageBody();




            // http://www.mimekit.net/docs/html/Creating-Messages.htm
            // create our message text, just like before (except don't set it as the message.Body)
            // MimeKit.MimePart body = new MimeKit.TextPart ("plain") { Text = plainText };

            // MimeKit.MimePart body = new MimeKit.TextPart("html") { Text = "<b>Test Message</b>" };





            // create an image attachment for the file located at path
            //MimeKit.MimePart attachment = new MimeKit.MimePart ("image", "gif") 
            //{
            //    Content = new MimeKit.MimeContent(
            //          System.IO.File.OpenRead(path)
            //        , MimeKit.ContentEncoding.Default
            //    )
            //    ,ContentDisposition = new MimeKit.ContentDisposition (MimeKit.ContentDisposition.Attachment)
            //    ,ContentTransferEncoding = MimeKit.ContentEncoding.Base64
            //    ,FileName = System.IO.Path.GetFileName (path)
            //};

            //// now create the multipart/mixed container to hold the message text and the
            //// image attachment
            //MimeKit.Multipart multipart = new MimeKit.Multipart ("mixed");
            //multipart.Add (body);
            //multipart.Add (attachment);

            // now set the multipart/mixed as the message body
            // message.Body = multipart;

            // https://stackoverflow.com/questions/30507362/c-sharp-mailkit-delivery-status-notification
            // https://github.com/jstedfast/MailKit/blob/master/FAQ.md#SmtpProcessReadReceipt
            // using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            using (MailKit.Net.Smtp.SmtpClient client = new SmtpClientWithStatusNotification())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(host, portNum, useSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(userName, userPass);

                client.Send(message);
                client.Disconnect(true);
            } // End Using client 

        }
        

    }


}
