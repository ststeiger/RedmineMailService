
namespace Microsoft.Exchange.WebServices.Autodiscover
{
    using System.Collections.Generic;
    using Microsoft.Exchange.WebServices.Data; // for ExchangeServiceBase
    //using System.DirectoryServices;
    //using System.DirectoryServices.ActiveDirectory;


    /// <summary>
    /// Represents a set of helper methods for using Active Directory services.
    /// </summary>
    internal class DirectoryHelper
    {

        private ExchangeServiceBase service;

        public DirectoryHelper(ExchangeServiceBase service)
        {
            this.service = service;
        }


        public List<string> GetAutodiscoverScpUrlsForDomain(string domainName)
        {
#if (NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD1_8 || NETSTANDARD1_9 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETSTANDARD2_2)
            //todo: implement ldap autodiscover
            return new System.Collections.Generic.List<string>();
#else
            DirectoryHelper helper = new DirectoryHelper(this);
            return helper.GetAutodiscoverScpUrlsForDomain(domainName);
#endif
        }

    }

}
