
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
            string val = GetSanitizedTranslate("field", "value", false);
            
            // true
            // translate(field,"ABCDEFGHIJKLMNOPQRSTUVWXYZʼ           “”", concat("abcdefghijklmnopqrstuvwxyz'           ",'""'))="value"
            // translate(field,"ABCDEFGHIJKLMNOPQRSTUVWXYZʼ           “”", concat("abcdefghijklmnopqrstuvwxyz'           ",'""')),"value"
            
            return val;
            
            return Get("count\\\"offiles");
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
        public bool DefaultCredentials;
    }
    
    
    // https://stackoverflow.com/questions/637866/sending-mail-without-installing-an-smtp-server
    // http://www.nullskull.com/articles/20030316.asp
    // https://www.redmine.org/boards/2/topics/26198
    // https://www.redmine.org/boards/2/topics/22259
    static class MailSender2
    {


        public static void Test()
        {

            string smtpserver = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_server");
            string smtpport = System.Configuration.ConfigurationManager.AppSettings.Get("smtp_port");
            int port = int.TryParse(smtpport, out port) ? port : 25;


            MailSettings ms = new MailSettings()
            {
                Host = "smtp.gmail.com",
                Username = Trash.UserData.GMail,
                Password= Trash.UserData.GMailPassword,
                Port=  587 // 25,
                ,Ssl = true,
                
            };

        }

        public static bool IsInteger(object Expression)
        {
            int n;
            return int.TryParse(System.Convert.ToString(Expression), out n);
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

        // .NET 4.5+
        // https://github.com/jstedfast/MailKit
        // https://github.com/jstedfast/MimeKit

        // https://stackoverflow.com/questions/4677258/send-email-using-system-net-mail-through-gmail
        public static void Test(MailSettings ms)
        {
            //Trash.UserData.RSN, Trash.UserData.RSNA)
            // Trash.UserData.SMTP
            
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
