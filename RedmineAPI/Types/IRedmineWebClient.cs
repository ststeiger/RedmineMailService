
namespace Redmine.Net.Api.Types
{
    interface IRedmineWebClient
    {
        System.Uri BaseAddress { get; set; }
        System.Collections.Specialized.NameValueCollection QueryString { get; set; }

        bool UseDefaultCredentials { get; set; }
        System.Net.ICredentials Credentials { get; set; }

        bool UseProxy { get; set; }
        System.Net.IWebProxy Proxy { get; set; }

        System.TimeSpan Timeout { get; set; }

        bool UseCookies { get; set; }
        System.Net.CookieContainer CookieContainer { get; set; }

        bool PreAuthenticate { get; set; }

        System.Net.Cache.RequestCachePolicy CachePolicy { get; set; }

        bool KeepAlive { get; set; }
    }
}
