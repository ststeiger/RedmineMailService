
namespace RedmineMailService
{
    

    // https://stackoverflow.com/questions/637866/sending-mail-without-installing-an-smtp-server
    // http://www.nullskull.com/articles/20030316.asp
    // https://www.redmine.org/boards/2/topics/26198
    // https://www.redmine.org/boards/2/topics/22259
    static class MailSender
    {


        public static void Test()
        {
            // http://vmstzhdb/Reports_HBD_DBH
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient())
            {

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    client.Port = 25;
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    // client.UseDefaultCredentials = true;

                    // Must be after UseDefaultCredentials 
                    // client.Credentials = new System.Net.NetworkCredential("mailboxname", "password", "example.com");
                    // client.Port = 587;
                    // client.EnableSsl = true;

                    client.Host = "COR-EXCHANGE.cor.local";
                    mail.Subject = "this is a test email.";
                    mail.Body = "Test";


                    // mail.From = new System.Net.Mail.MailAddress("somebody@friends.com", "SomeBody");
                    mail.From = new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.info, "COR ServiceDesk");

                    

                    mail.To.Add(new System.Net.Mail.MailAddress(RedmineMailService.Trash.UserData.Email, "A"));
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
