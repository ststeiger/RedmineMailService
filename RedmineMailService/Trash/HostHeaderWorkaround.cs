
namespace RedmineClient
{


    class HostHeaderWorkaround
    {


        public class ModifiedWebClient : System.Net.WebClient
        {
            protected override System.Net.WebRequest GetWebRequest(System.Uri address)
            {
                System.Net.WebRequest request = (System.Net.WebRequest)base.GetWebRequest(address);
                //string host = "redmine.cor-management.ch";

                // not necessary...
                //request.Headers.GetType().InvokeMember("ChangeInternal",
                //    System.Reflection.BindingFlags.NonPublic |
                //    System.Reflection.BindingFlags.Instance |
                //    System.Reflection.BindingFlags.InvokeMethod, null,
                //    request.Headers, new object[] { "Host", host }
                //);

                request.Proxy = new System.Net.WebProxy("http://88.84.21.77:80");//server IP and port


                // .NET 4.0 only
                System.Net.HttpWebRequest foo = (System.Net.HttpWebRequest)request;
                //foo.Host = host;


                // https://yoursunny.com/t/2009/HttpWebRequest-IP/
                System.Reflection.FieldInfo horribleProxyServicePoint = (typeof(System.Net.ServicePoint))
                    .GetField("m_ProxyServicePoint", System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                horribleProxyServicePoint.SetValue(foo.ServicePoint, false);
                return foo;

                return request;
            }


        }


        public static void TestWorkaround()
        {
            ModifiedWebClient wc = new ModifiedWebClient();

            // wc.Headers.Add("Host", "redmine.cor.local");
            // wc.Headers.Set("Host", "redmine.cor.local");
            // wc.Headers["Host"] = "redmine.cor.local";

            string str = wc.DownloadString("http://redmine.cor-management.ch");
            System.Console.WriteLine(str);
        }


    }


}
