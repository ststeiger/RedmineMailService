
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;

namespace RedmineMailService.Tests
{
    public class SendTest
    {
        
        
        // https://stackoverflow.com/questions/37853903/can-i-send-files-via-email-using-mailkit
        public static void fa()
        {
            var message = new MimeMessage ();
            message.From.Add (new MailboxAddress ("Joey", "joey@friends.com"));
            message.To.Add (new MailboxAddress ("Alice", "alice@wonderland.com"));
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
            
            
            message.ReplyTo.Add (new MailboxAddress ("Alice", "alice@wonderland.com"));

            message.Cc.Add (new MailboxAddress ("Alice", "alice@wonderland.com"));
            message.Bcc.Add (new MailboxAddress ("Alice", "alice@wonderland.com"));
            // message.Date = new System.DateTimeOffset(System.DateTime.Now);
            message.Date = System.DateTimeOffset.Now;
            //message.Attachments

            message.Importance = MessageImportance.High;
            message.Priority = MessagePriority.Urgent;
            message.XPriority = XMessagePriority.Highest;
            
            
            //message.HtmlBody
            //message.TextBody
            // message.Body
            // message.InReplyTo
            // message.MessageId
            // message.Sign();
            // message.Verify()
            // message.SignAndEncrypt();
            message.
            
            
            
            message.Subject = "How you doin?";

            // create our message text, just like before (except don't set it as the message.Body)
            var body = new TextPart ("plain") {
                Text = @"Hey Alice,

What are you up to this weekend? Monica is throwing one of her parties on
Saturday and I was hoping you could make it.

Will you be my +1?

-- Joey
"
            };
            
            string path = "";
            
            // create an image attachment for the file located at path
            var attachment = new MimePart ("image", "gif") 
            {
                ContentObject = new ContentObject (
                    System.IO.File.OpenRead (path)
                    , ContentEncoding.Default
                    ),
                ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = System.IO.Path.GetFileName (path)
            };

            // now create the multipart/mixed container to hold the message text and the
            // image attachment
            var multipart = new Multipart ("mixed");
            multipart.Add (body);
            multipart.Add (attachment);
            
            // now set the multipart/mixed as the message body
            message.Body = multipart;   
        }
        
        
        // https://github.com/jstedfast/MailKit/blob/master/Documentation/Examples/SmtpExamples.cs
        public static void SendMessage (MimeMessage message)
        {
            // using (var client = new SmtpClient (new ProtocolLogger ("smtp.log"))) 
            using (var client = new SmtpClient())
            {
                client.Connect ("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate ("username", "password");
                client.Send (message);
                client.Disconnect (true);
            }
        }
        
        

        public static void foo()
        {
        string emailID = "";
        // Nova Mensagem 
        var message = new MimeMessage();
        try
        {
            // Acede aos parâmetros do email caso este seja do gestobrigweb gmail.com
            if (emailSettings.email == "gestobrigweb@gmail.com") {
                EmailProvider provider = new EmailProviders().GetEmailProvider(Convert.ToInt32(ConfigurationManager.AppSettings["gestObrigWebProviderID"]));

                if (emailSettings.provider == null)
                    emailSettings.provider = provider;
            }

            // UserName
            string userName = emailSettings.email;   
            // Password
            if (userPass == "")
                userPass = cripter.Decrypt(emailSettings.pass);

            // From
            message.From.Add(new MailboxAddress(emailSettings.email, emailSettings.email));

            // TO
            if (recipient.Contains(";"))
                foreach (string recipt in recipient.Split(';'))
                    message.To.Add(new MailboxAddress(recipt, recipt));
            else
                if (recipient.Contains(","))
                    foreach (string recipt in recipient.Split(','))
                        message.To.Add(new MailboxAddress(recipt, recipt));
                else
                  message.To.Add(new MailboxAddress(recipient, recipient));


            // Se Assume CC
            if (assumeCC)
                message.Cc.Add(new MailboxAddress(emailSettings.email,emailSettings.email));

            // Destinatário
            if (cc != "")
            {
                if (cc.Contains(";"))
                    foreach (string emailCC in cc.Split(';'))
                    message.Cc.Add(new MailboxAddress(emailCC,emailCC));
                else
                    message.Cc.Add(new MailboxAddress(cc, cc));
            }

            // Assunto
            message.Subject = subject;

            // Body (Mensagem)
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            // Envio
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(emailSettings.emailServer, emailSettings.serviceType, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(userName, userPass);

                client.Send(message);
                client.Disconnect(true);
                return true;
        }
        }



    }
}