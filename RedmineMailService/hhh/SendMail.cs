
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

"
                    
                    
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
                    string sql = @"
INSERT INTO T_EML_Delivery ( EML_UID, EML_Module, EML_SendStart) VALUES 
(
     '" + mail.MailId + @"'
    ,N'Schlüsselbestandeskontrolle SwissLife'
    ,'"+ System.DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + @"'
);
";
                    
                    SQL.ExecuteNonQuery(sql);

                    if (string.IsNullOrEmpty(mail.CC))
                        mail.CC = "";
                    
                    if (string.IsNullOrEmpty(mail.Bcc))
                        mail.Bcc = "";

                    if (string.IsNullOrEmpty(mail.ReplyTo))
                        mail.ReplyTo = "";
                    
                    sql = @"
UPDATE T_EML_Delivery  
    SET  EML_From = N'"+ mail.From.Replace("'","''") + @"' 
        ,EML_ReplyTo = N'"+ mail.ReplyTo.Replace("'","''") + @"' 
        ,EML_To = N'"+ mail.To.Replace("'","''") + @"' 
        ,EML_CC = N'"+ mail.CC.Replace("'","''") + @"' 
        ,EML_BCC = N'"+ mail.Bcc.Replace("'","''") + @"' 
        ,EML_Body = N'"+ mail.TemplateString.Replace("'","''") + @"' 
WHERE EML_UID = '" + mail.MailId + @"' 
;";
                    
                    System.Console.WriteLine("Start ! ");
                    SQL.ExecuteNonQuery(sql);
                };
                
                ms.OnSuccess += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    string sql = @"
UPDATE T_EML_Delivery  
    SET  EML_SendSuccess ='"+ System.DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + @"' 
WHERE EML_UID = '" + mail.MailId + @"' 
;";
                    
                    System.Console.WriteLine("Success ! ");
                    SQL.ExecuteNonQuery(sql);
                };
                
                ms.OnError += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    string exString = exception.ToString();
                    if(string.IsNullOrEmpty(exString))
                        exString = "";
                    
                    string sql = @"
UPDATE T_EML_Delivery  
    SET  EML_SendError ='"+ System.DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + @"' 
        ,EML_Exception = N'"+ exString.Replace("'","''") +@"' 
WHERE EML_UID = '" + mail.MailId + @"' 
;";
                    
                    SQL.ExecuteNonQuery(sql);
                    System.Console.WriteLine("Error ! ");
                };
                
                ms.OnDone += delegate (MailSettings mset, BaseMailTemplate mail, System.DateTime tm, System.Exception exception)
                {
                    string sql = @"
UPDATE T_EML_Delivery  
    SET  EML_SendEnd = '"+ System.DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + @"'
WHERE EML_UID = '" + mail.MailId + @"' 
;";                    
                    
                    SQL.ExecuteNonQuery(sql);
                    System.Console.WriteLine("Done ! ");
                };
                
                ms.SendMail(mt, dt.Rows[0]);
            } // End Sub Test 
            
            
        } // End Sub Test 
        
        
    } // End Class SendMail 
    
    
} // End Namespace RedmineMailService 
