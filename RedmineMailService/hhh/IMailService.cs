
namespace RedmineMailService.hhh
{
    
    
    public class MailSettings
    {
        public string Server;
        public uint Port;
        public string SenderAddress;
        public string Username;
        public string Password;
        public string Token;
    }
    
    
    public class MailService
    {
        
        public MailService(MailSettings ms)
        { }
        
        public void SendMail(BaseMailTemplate mt, string[] to)
        {
            // MailKit.
        }
    }
    
    
    interface IMailService
    {
        bool SendMail(BaseMailTemplate template);
        
        
    }
    
    
}
