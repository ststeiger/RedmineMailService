
namespace System.Configuration.ConfigurationManager
{

    public class AppSettings
    {
        public static string Get(string key)
        {
            // /configuration/appSettings/add[1][@key="countoffiles"]
            return "";
        }
        
    }
}

/*
<? xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="countoffiles" value="7" />
    <add key="logfilelocation" value="abc.txt" />
    <add key="smtp_port" value="25"/>
    <add key="smtp_user" value=""/>
    <add key="smtp_passwort" value=""/>
    <add key="smtp_authenticate" value=""/>
    <add key="smtp_server" value="smtp.somedomain.com"/>
  </appSettings>
</configuration>
*/


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
