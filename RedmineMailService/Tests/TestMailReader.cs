
// https://components.xamarin.com/gettingstarted/mailkit
// https://stackoverflow.com/questions/31984490/how-can-i-mark-a-message-as-read-in-mailkit
// D:\username\Downloads\Exchange 2013 101 Code Samples


// https://github.com/dotnet/wcf
// https://docs.microsoft.com/en-us/dotnet/framework/wcf/servicemodel-metadata-utility-tool-svcutil-exe

// https://webmail.domain.com/ews/Services.wsdl
namespace RedmineMailService
{


    class TestMailReader
    {

        public static void Test()
        {
            // RedmineMailService.Trash.Exchange.TestSend();
            // RedmineMailService.Trash.Exchange.Test();

            TestPop3(Trash.UserData.RSN, Trash.UserData.RSNA);
            TestImap(Trash.UserData.RSN, Trash.UserData.RSNA);
            TestSMTP(Trash.UserData.RSN, Trash.UserData.RSNA);

            // ExchangeShared();
            // RedmineMailService.Trash.Exchange.FindUnreadEmail();
            // RedmineMailService.Trash.Exchange.DelaySendEmail();
            // RedmineMailService.Trash.Exchange.PlayEmailOnPhone();
            
        }


        // Typically, SMTP uses port 25. However, an alternative SMTP "submission" port has been reserved on port 587. 
        // For Exchange 2007 and 2010, installation will create a "Default" module listening on port 25 as well as a 
        // "Client" module listening on port 587.
        // https://stackoverflow.com/questions/38747822/accessing-exchange-shared-folders-using-mailkit
        public static void ExchangeShared()
        {
            string userName = "main@user.com"; // The email address that has permissions to the shared mailbox
            string sharedMailboxAlias = "aliasName"; // This is the alias name as setup in Exchange
            string password = "";
            using (MailKit.MailStore client = new MailKit.Net.Imap.ImapClient())
            {
                client.Connect("outlook.office365.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(userName + @"\" + sharedMailboxAlias, password);
                MailKit.IMailFolder inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadOnly);
                System.Console.WriteLine("Total messages: {0}", inbox.Count);
                System.Console.WriteLine("Recent messages: {0}", inbox.Recent);
                client.Disconnect(true);
            }
        }


        public static void TestPop3(string username, string password)
        {
            using (MailKit.MailSpool client = new MailKit.Net.Pop3.Pop3Client())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Non-Encrypted: 110
                // SSL/TLS:       995
                //client.Connect(RedmineMailService.Trash.UserData.POP, 110, false);
                client.Connect(RedmineMailService.Trash.UserData.POP, 995, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(username, password);

                for (int i = 0; i < client.Count; i++)
                {
                    MimeKit.MimeMessage message = client.GetMessage(i);
                    System.Console.WriteLine("Subject: {0}", message.Subject);
                } // Next i 

                client.Disconnect(true);
            } // End Using client 

        } // End Sub TestPop3 


        public static void TestImap(string username, string password)
        {
            using (MailKit.MailStore client = new MailKit.Net.Imap.ImapClient())
            {
                // For demo-purposes, accept all SSL certificates
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Non-Encrypted: 143
                // SSL/TLS:       993
                client.Connect(RedmineMailService.Trash.UserData.IMAP, 993, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(username, password);

                // The Inbox folder is always available on all IMAP servers...
                MailKit.IMailFolder inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadOnly);

                System.Console.WriteLine("Total messages: {0}", inbox.Count);
                System.Console.WriteLine("Recent messages: {0}", inbox.Recent);

                for (int i = 0; i < inbox.Count; i++)
                {
                    MimeKit.MimeMessage message = inbox.GetMessage(i);
                    System.Console.WriteLine("Subject: {0}", message.Subject);
                } // Next i 

                client.Disconnect(true);
            } // End Using client 

        } // End Sub TestImap 


        public static void TestSMTP(string username, string password)
        {
            // RedmineClient.CertificateCallback.Initialize();
            
            MimeKit.MimeMessage message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Joey Tribbiani", "joey@friends.com"));
            
            message.To.Add(new MimeKit.MailboxAddress("Mrs. Chanandler Bong", RedmineMailService.Trash.UserData.Email));
            message.Subject = "How you doin'?";

            message.Body = new MimeKit.TextPart("plain")
            {
                Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"
            };


            //using (MailKit.MailTransport client = new MailKit.Net.Smtp.SmtpClient())
            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;


                // Non-Encrypted: 25
                // StartTLS:     587
                // SSL:          465

                // The handshake failed due to an unexpected packet format
                // I had this same problem, for me when using port 587 with SSL it failed, 
                // but when using port 465 with SSL it worked.
                // Which is weird because that's the opposite behaviour of System.Net.Mail.SmtpClient 
                // (which works for 587 with SSL but fails with 465 with SSL).
                // Port 587 is a clear-text port for SMTP which is why you got the error when using an SSL-wrapped connection.
                // The way to get SSL support on port 587 is to use STARTTLS 
                // (which MailKit will do automatically for you once it connects unless you explicitly disable the feature).
                // Ah right, yes using 587 with SSL=false does work. Thanks!
                // A value of true should only ever be used for port 465 unless you know the port is ssl-wrapped
                client.Connect(RedmineMailService.Trash.UserData.SMTP, 587, false);
                
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(username, password);

                client.Send(message);
                client.Disconnect(true);
            } // End Using client 

        } // End Sub TestSMTP 


    } // End Class 


} // End Namespace 
