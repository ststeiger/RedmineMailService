
using WebContext = Microsoft.AspNetCore.Http.HttpContext ;



namespace RedmineMailService
{
    
    public delegate void SaveEventHandler_t(MailSettings ms, BaseMailTemplate mail, System.DateTime tm, System.Exception exception);

    public interface IMailHandler
    {    
        void ProcessRequest(WebContext context);
    }
    
    
    public interface IMailService
    {
        void SendMail(BaseMailTemplate template, System.Data.DataRow dr);
        
        event SaveEventHandler_t OnStart;
        event SaveEventHandler_t OnSuccess;
        event SaveEventHandler_t OnError;
        event SaveEventHandler_t OnDone;
        
    } // End IMailService 
    
    
} // End Namespace RedmineMailService 
