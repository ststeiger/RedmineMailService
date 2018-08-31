using System;
using System.Collections.Generic;
using System.Text;

//namespace RedmineMailService.Tests
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

}
