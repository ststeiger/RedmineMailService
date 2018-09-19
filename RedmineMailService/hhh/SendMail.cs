
namespace RedmineMailService
{
    
    
    class SendMail
    {
        
        
        public static void Test()
        {
            string json = SQL.ExecuteScalar("SELECT TOP 1 FC_Value FROM T_FMS_Configuration WHERE FC_Key LIKE 'SMTP'"); ;
            MailSettings settings = MailSettings.FromJson(json);
            SmtpMailService ms = new SmtpMailService(settings);   
        }
        
        
        public static void Test(MailSettings settings, IMailService ms)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("col", typeof(string));
            dt.Rows.Add("& Ciao");
            
            
            using (MailTemplate mt = new MailTemplate())
            {
                mt.TemplateString = "Hello {@col} World<img src='naegeli1.jpg' /><img src='pp.jpg' />";
                mt.TemplateString = @"
Sehr geehrte/r Herr/Frau {@KT_Name}

Ich bitte Sie, die Schlüsselkontrolle im Anhang auszufüllen, 
und bis zum erwähnten Datum zurückzusenden. 

Dieses E-Mail wurde automatisch generiert.

This email has been automatically generated. Please do not reply to this email address as all responses are directed to an unattended mailbox, and will not receive a response.


I am sure there is some kind of official translation out there. I just can't remember where I've seen it :(


Cet e-mail a été généré automatiquement. 
Svp n'y répondez pas. 

Nos salutations distinguées, 
Votre équipe de support MAGIX ...
lists.gnu.org/archive/html/ bug-ghostscript/2004-03/msg01553.html - 9k

";
                    
                    
                mt.Subject = "This is a test";
                mt.From = settings.FromAddress;
                mt.To = "undiclosed@example.com";
                mt.ReplyTo = settings.FromAddress;
                
                
                
                
                mt.EmbeddedImages.Add(
                    new Resource(@"D:\username\Pictures\naegeli1.jpg")
                );
                
                mt.EmbeddedImages.Add(
                    new Resource(@"D:\username\Pictures\pp.jpg")
                );
                
                mt.AttachmentFiles.Add(
                    new Resource(@"D:\Program Files\Microsoft\R Client\R_SERVER\doc\NEWS.pdf")
                );
                
                ms.OnStart += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    {
                        using (System.Data.IDbCommand cmd = SQL.CreateCommand(@"
INSERT INTO T_EML_Delivery ( EML_UID, EML_Module, EML_SendStart ) 
VALUES ( @in_mail_id, @in_modul, @in_send_start ); 
"))
                        {
                            SQL.AddParameter(cmd, "in_mail_id", mail.MailId);
                            SQL.AddParameter(cmd, "in_modul", "Schlüsselbestandeskontrolle SwissLife");
                            SQL.AddParameter(cmd, "in_send_start", System.DateTime.Now);
                            SQL.ExecuteNonQuery(cmd);
                        }
                    }



                    if (string.IsNullOrEmpty(mail.CC))
                        mail.CC = "";
                    
                    if (string.IsNullOrEmpty(mail.Bcc))
                        mail.Bcc = "";

                    if (string.IsNullOrEmpty(mail.ReplyTo))
                        mail.ReplyTo = "";


                    using (System.Data.IDbCommand cmd = SQL.CreateCommand(@"
UPDATE T_EML_Delivery  
    SET  EML_From = @in_from 
        ,EML_ReplyTo = @in_reply_to 
        ,EML_To = @in_to 
        ,EML_CC = @in_cc 
        ,EML_BCC = @in_bcc 
        ,EML_Body = @in_body 
WHERE EML_UID = @in_mail_id
"))
                    {
                        SQL.AddParameter(cmd, "in_from", mail.From);
                        SQL.AddParameter(cmd, "in_reply_to", mail.ReplyTo);
                        SQL.AddParameter(cmd, "in_to", mail.To);
                        SQL.AddParameter(cmd, "in_cc", mail.CC);
                        SQL.AddParameter(cmd, "in_bcc", mail.Bcc);
                        SQL.AddParameter(cmd, "in_body", mail.TemplateString);
                        SQL.AddParameter(cmd, "in_mail_id", mail.MailId);
                        SQL.ExecuteNonQuery(cmd);
                    }

                    System.Console.WriteLine("Start ! ");
                    
                };
                
                ms.OnSuccess += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {

                    using (System.Data.IDbCommand cmd = SQL.CreateCommand(@"
UPDATE T_EML_Delivery  
    SET  EML_SendSuccess = @in_success_time 
WHERE EML_UID = @in_mail_id
"))
                    {
                        SQL.AddParameter(cmd, "in_success_time", System.DateTime.Now);
                        SQL.AddParameter(cmd, "in_mail_id", mail.MailId);
                        SQL.ExecuteNonQuery(cmd);
                    }

                    System.Console.WriteLine("Success ! ");
                };
                
                ms.OnError += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    string exString = exception.ToString();
                    if(string.IsNullOrEmpty(exString))
                        exString = "";

                    using (System.Data.IDbCommand cmd = SQL.CreateCommand(@"
UPDATE T_EML_Delivery  
    SET  EML_SendError = @in_error_time 
        ,EML_Exception = @in_error_message
        ,EML_ExceptionDetails = @in_exception
WHERE EML_UID = @in_mail_id
"))
                    {
                        SQL.AddParameter(cmd, "in_error_time", System.DateTime.Now);
                        SQL.AddParameter(cmd, "in_error_message", exception.Message);
                        SQL.AddParameter(cmd, "in_exception", exception.ToString());
                        SQL.AddParameter(cmd, "in_mail_id", mail.MailId);
                        SQL.ExecuteNonQuery(cmd);
                    }

                    System.Console.WriteLine("Error ! ");
                };
                
                ms.OnDone += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {

                    using (System.Data.IDbCommand cmd = SQL.CreateCommand(@"
UPDATE T_EML_Delivery  
    SET  EML_SendEnd = @in_done_time 
WHERE EML_UID = @in_mail_id
"))
                    {
                        SQL.AddParameter(cmd, "in_done_time", System.DateTime.Now);
                        SQL.AddParameter(cmd, "in_mail_id", mail.MailId);
                        SQL.ExecuteNonQuery(cmd);
                    }

                    System.Console.WriteLine("Done ! ");
                };
                
                ms.SendMail(mt, dt.Rows[0]);
            } // End Sub Test 
            
            
        } // End Sub Test 
        
        
    } // End Class SendMail 
    
    
} // End Namespace RedmineMailService 
