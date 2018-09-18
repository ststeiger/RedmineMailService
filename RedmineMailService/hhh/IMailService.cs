
namespace RedmineMailService
{


    interface IMailService
    {
        void SendMail(BaseMailTemplate template, System.Data.DataRow dr);
    } // End IMailService 


} // End Namespace RedmineMailService 
