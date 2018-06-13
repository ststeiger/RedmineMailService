﻿
namespace System.Configuration.ConfigurationManager
{
    
    
    public class AppSettings
    {

        private static System.Xml.XmlDocument m_document;

        private static System.Xml.XmlNamespaceManager m_nsmgr;
        
        static AppSettings()
        {
            m_document = new System.Xml.XmlDocument();
            // m_document.Load("filename");
            m_document.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <appSettings>
        <add key=""count&quot;offiles"" value=""7&quot;8"" />
        <add key=""countoffiles"" value=""7&quot;8"" />
        <add key=""logfilelocation"" value=""abc.txt"" />
        <add key=""smtp_port"" value=""25""/>
        <add key=""smtp_user"" value=""""/>
        <add key=""smtp_passwort"" value=""""/>
        <add key=""smtp_authenticate"" value=""""/>
        <add key=""smtp_server"" value=""smtp.somedomain.com""/>
    </appSettings>
</configuration>
");
            
            m_nsmgr = new System.Xml.XmlNamespaceManager(m_document.NameTable);
            m_nsmgr.AddNamespace("dft", "");
            //m_nsmgr.AddNamespace("", "");
        }
        
        
        public static string Get()
        {
            // string val = GetSanitizedTranslate("field", "value", false);

            // true
            // translate(field,"ABCDEFGHIJKLMNOPQRSTUVWXYZʼ           “”", concat("abcdefghijklmnopqrstuvwxyz'           ",'""'))="value"
            // translate(field,"ABCDEFGHIJKLMNOPQRSTUVWXYZʼ           “”", concat("abcdefghijklmnopqrstuvwxyz'           ",'""')),"value"

            //return Get("count\\\"offiles");
            return Get("countoffiles");
        }


        // https://www.coveros.com/escaping-and-translating-in-xpath/
        static string APOSTROPHES = "\u02BC";
        static string SPACES = "\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200A";
        static string QUOTES = "\u201C\u201D";
        
        public static string GetSanitizedTranslate(string field, string value, bool equals) 
        {
            string sanitizedApostrophes = "";
            string sanitizedSpaces = "";
            string sanitizedQuotes = "";
            // Builds sanitized versions based on length of unsanitary entries.
            for (int i = 0; i < APOSTROPHES.Length; i++) {
                sanitizedApostrophes += "'";
            }
            for (int i = 0; i < SPACES.Length; i++) {
                sanitizedSpaces += " ";
            }
            for (int i = 0; i < QUOTES.Length; i++) {
                sanitizedQuotes += "\"";
            }
            
            string equality = equals ? "=" : ",";
            
            // Touch this if you dare
            return "translate(" + field + ",\"ABCDEFGHIJKLMNOPQRSTUVWXYZ" + APOSTROPHES + SPACES + QUOTES +
                   "\", concat(\"abcdefghijklmnopqrstuvwxyz" + sanitizedApostrophes + sanitizedSpaces + "\",'" +
                   sanitizedQuotes + "'))" + equality + "\"" + value.ToLowerInvariant() + "\"";
        }
        
        
        public static string Get(string key)
        {
            string value = null;
            
            string xpath = "/configuration/appSettings/add[@key=\"" + key + "\"]";
            System.Xml.XmlNode node = m_document.SelectSingleNode(xpath, m_nsmgr);
            
            if (node == null || node.Attributes == null || node.Attributes["value"] == null)
                return value;
            
            value = node.Attributes["value"].Value;
            return value;
        }
        
    }
}


namespace RedmineMailService
{


    public class MailSettings
    {
        public string Host;
        public int Port;
        public bool Ssl;
        public string Username;
        public string Password;


        protected string m_FromAddress;
        public string FromAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(m_FromAddress))
                    return this.m_FromAddress;

                return "servicedesk@cor-management.ch";
            }
            set
            {
                this.m_FromAddress = value;
            }
        }


        protected string m_FromName;
        public string FromName
        {
            get
            {
                if (!string.IsNullOrEmpty(m_FromName))
                    return this.m_FromName;

                if (string.Equals(this.m_FromAddress, "servicedesk@cor-management.ch", System.StringComparison.InvariantCultureIgnoreCase))
                    return "COR ServiceDesk";

                return this.FromAddress;
            }
            set
            {
                this.m_FromName = value;
            }
        }


        protected bool? m_DefaultCredentials;

        public bool DefaultCredentials
        {
            get
            {
                if (m_DefaultCredentials.HasValue)
                    return m_DefaultCredentials.Value;

                return string.IsNullOrEmpty(this.Username);
            }
            set
            {
                this.m_DefaultCredentials = value;
            }
        }
    }
    
    
    // https://stackoverflow.com/questions/637866/sending-mail-without-installing-an-smtp-server
    // http://www.nullskull.com/articles/20030316.asp
    // https://www.redmine.org/boards/2/topics/26198
    // https://www.redmine.org/boards/2/topics/22259
    static class MailSender2
    {


        public static void TestWebConfig()
        {
            string smtpserver = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_server");
            string smtpport = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_port");
            int port = int.TryParse(smtpport, out port) ? port : 25;

            string smtp_user = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_user");
            string smtp_password = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_passwort");

            string smtp_authenticate = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_authenticate");

            bool authenticate = false;

            if (!bool.TryParse(smtp_authenticate, out authenticate))
            {
                authenticate = !string.IsNullOrEmpty(smtp_user);
            }


            MailSettings ms = new MailSettings()
            {
                Host = "smtp.gmail.com",
                Username = smtp_user,
                Password = smtp_password,
                Port = port, // 587, // 25,
                Ssl = true,
                DefaultCredentials = !authenticate
            };

        }


        public static void Send(System.Net.Mail.MailMessage message)
        {
            string smtpserver = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_server");
            string smtpport = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_port");
            int port = int.TryParse(smtpport, out port) ? port : 25;

            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
            {
                smtp.Host = smtpserver;
                smtp.Port = port;

                string smtpauthenticate = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_authenticate");
                if (!string.IsNullOrEmpty(smtpauthenticate) && smtpauthenticate.ToLower().Equals("true"))
                {
                    string smtpuser = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_user");
                    string smtppasswort = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_passwort");

                    if (smtppasswort != "")
                        smtppasswort = ""; // CryptStrings.DeCrypt(smtppasswort);

                    smtp.Credentials = new System.Net.NetworkCredential(smtpuser, smtppasswort);
                }

                smtp.Send(message);
            } // End using smtp 

        } // End Sub Send 



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



        



        public static void SendAttachment()
        {

            MailSettings ms = new MailSettings()
            {
                Host = Trash.UserData.GMailSMTP,
                Username = Trash.UserData.GMail,
                Password = Trash.UserData.GMailPassword,
                FromAddress= Trash.UserData.info,
                Port = 587, // 25,
                Ssl = true 
            };
            

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ms, Newtonsoft.Json.Formatting.Indented);
            System.Console.WriteLine(json);

            System.Collections.Generic.List<byte[]> attachmentBytes = new System.Collections.Generic.List<byte[]>();
            System.Collections.Generic.List<System.IO.MemoryStream> streams = new System.Collections.Generic.List<System.IO.MemoryStream>();
            System.Collections.Generic.List<System.Net.Mail.Attachment> attachments = new System.Collections.Generic.List<System.Net.Mail.Attachment>();




            
            string path = @"D:\Stefan.Steiger\Desktop\Intro to Docker.pdf";
            string fileName = System.IO.Path.GetFileName(path);
            attachmentBytes.Add(System.IO.File.ReadAllBytes(path));

            /*
            for (int i = 0; i < attachmentBytes.Count; ++i)
            {
                streams.Add(new System.IO.MemoryStream(attachmentBytes[i]));
            }

            for (int i = 0; i < streams.Count; ++i)
            {
                attachments.Add(new System.Net.Mail.Attachment(streams[i], fileName, "application/pdf"));
            }

            */

            attachments.Add(new System.Net.Mail.Attachment(path, "application/pdf"));
            attachments.Add(new System.Net.Mail.Attachment(@"D:\Stefan.Steiger\Desktop\NET_Core-2.0-Getting_Started_Guide-en-US.pdf", "application/pdf"));


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


                for (int i = 0; i < attachments.Count; ++i)
                {
                    mail.Attachments.Add(attachments[i]);
                }
                
                mail.From = new System.Net.Mail.MailAddress(ms.FromAddress, ms.FromName);

                mail.To.Add(new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.info, "A"));
                // mail.To.Add(new System.Net.Mail.MailAddress("user1@friends.com", "B"));
                // mail.To.Add(new System.Net.Mail.MailAddress("user2@friends.com", "B"));

                Send(ms, mail);
            }

            for (int i = 0; i < attachments.Count; ++i)
            {
                attachments[i].Dispose();
            }

            for (int i = 0; i < streams.Count; ++i)
            {
                streams[i].Dispose();
            }

        }



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

        } // End Sub Test 



        // .NET 4.5+
        // https://github.com/jstedfast/MailKit
        // https://github.com/jstedfast/MimeKit

        // https://stackoverflow.com/questions/4677258/send-email-using-system-net-mail-through-gmail
        public static void Test(MailSettings ms)
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
                {
                    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }
                else
                    client.Credentials = new System.Net.NetworkCredential(ms.Username, ms.Password);

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    mail.Subject = "this is a test email.";
                    mail.IsBodyHtml = true;
                    mail.Body = "Test";


                    // mail.From = new System.Net.Mail.MailAddress("somebody@friends.com", "SomeBody");
                    string from = Trash.UserData.info;
                    from = Trash.UserData.GMail;
                    mail.From = new System.Net.Mail.MailAddress(from, "COR ServiceDesk");

                    

                    mail.To.Add(new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.info, "A"));
                    // mail.To.Add(new System.Net.Mail.MailAddress("user1@friends.com", "B"));
                    // mail.To.Add(new System.Net.Mail.MailAddress("user2@friends.com", "B"));
                    // mail.To.Add(new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.info, "ServiceDesk"));


                    try
                    {
                        System.Console.WriteLine("Host: " + client.Host);
                        System.Console.WriteLine("Credentials: " + System.Convert.ToString(client.Credentials));
                        client.Send(mail);
                        System.Console.WriteLine("Mail versendet");
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

                } // End Using mail 

            } // End Using client 

        } // End Sub Test 


    } // End Class MailSender 


} // End Namespace RedmineMailService.Trash 
