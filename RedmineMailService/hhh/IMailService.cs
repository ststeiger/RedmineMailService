using System;
using System.Collections.Generic;
using System.Text;

namespace RedmineMailService.hhh
{

    public class MailSettings
    {
        public string Server;
        public uint Port;

        public string SenderAddress;
        public string ReplyToAddress;
        public string Username;
        public string Password;

    }

    public class MailService
    {
        public MailService(MailSettings ms)
        { }

        public void SendMail(MailTemplate mt, string[] to)
        {

        }
    }


    class IMailService
    {
    }
}
