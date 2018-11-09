
namespace RedmineMailService.Tests
{


    // NuGet: HtmlAgilityPack 
    class HanaLogin
    {


        public class WebClientEx
            : System.Net.WebClient
        {

            private System.Net.CookieContainer container;

            public WebClientEx(System.Net.CookieContainer container)
            {
                this.container = container;
            }

            public WebClientEx()
                : this(new System.Net.CookieContainer())
            { }


            public System.Net.CookieContainer CookieContainer
            {
                get { return container; }
                set { container = value; }
            }



            protected override System.Net.WebRequest GetWebRequest(System.Uri address)
            {
                System.Net.WebRequest r = base.GetWebRequest(address);
                var request = r as System.Net.HttpWebRequest;
                if (request != null)
                {
                    request.CookieContainer = container;
                }
                return r;
            }

            protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request, System.IAsyncResult result)
            {
                System.Net.WebResponse response = base.GetWebResponse(request, result);
                ReadCookies(response);
                return response;
            }

            protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
            {
                System.Net.WebResponse response = base.GetWebResponse(request);
                ReadCookies(response);
                return response;
            }

            private void ReadCookies(System.Net.WebResponse r)
            {
                var response = r as System.Net.HttpWebResponse;
                if (response != null)
                {
                    System.Net.CookieCollection cookies = response.Cookies;
                    container.Add(cookies);
                }
            } // End Sub ReadCookies 
        }



        public class RedirectValues
        {
            public string Action;
            public string Method;

            public System.Collections.Specialized.NameValueCollection PostValues = new System.Collections.Specialized.NameValueCollection();
        } // End Class RedirectValues 


        public static RedirectValues GetFormValues(string html)
        {
            RedirectValues rv = new RedirectValues();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlAgilityPack.HtmlNode form = doc.DocumentNode.SelectSingleNode("//form");
            rv.Method = form.Attributes["method"]?.Value;
            rv.Action = form.Attributes["action"]?.Value;

            foreach (HtmlAgilityPack.HtmlNode node in form.SelectNodes(".//input"))
            {
                HtmlAgilityPack.HtmlAttribute valueAttribute = node.Attributes["value"];
                HtmlAgilityPack.HtmlAttribute nameAttribute = node.Attributes["name"];

                if (nameAttribute != null && valueAttribute != null)
                {
                    rv.PostValues.Add(nameAttribute.Value, valueAttribute.Value);
                } // End if (nameAttribute != null && valueAttribute != null) 
            } // Next node 

            return rv;
        } // End Function GetFormValues 


        public static void Test()
        {
            string projName = RedmineMailService.SecretManager.GetSecret<string>("SAP_Project_Name");
            string url = RedmineMailService.SecretManager.GetSecret<string>("URL", projName);
            string username = RedmineMailService.SecretManager.GetSecret<string>("Username", projName);
            string password = RedmineMailService.SecretManager.GetSecret<string>("Password", projName);

            string base64Creds = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
            string basicCredentials = "Basic " + base64Creds;


            using (WebClientEx wc = new WebClientEx())
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
                wc.Headers.Add("Authorization", basicCredentials);


                // Request 1
                string redirect = wc.DownloadString(url);
                RedirectValues rv1 = GetFormValues(redirect);

                byte[] retValue = wc.UploadValues(rv1.Action, rv1.Method, rv1.PostValues);
                string resp = System.Text.Encoding.UTF8.GetString(retValue);
                System.Console.WriteLine(resp);


                RedirectValues rv2 = GetFormValues(resp);
                System.Console.WriteLine(rv2);
                // Request 2
                wc.Headers["referer"] = url;
                rv2.PostValues["j_username"] = username;
                rv2.PostValues["j_password"] = password;
                // string url2 = "https://accounts.sap.com" + rv2.Action;
                System.Uri baseUri = new System.Uri(rv1.Action, System.UriKind.Absolute);
                string url2 = baseUri.Scheme + "://" + baseUri.Authority + rv2.Action;

                byte[] loginRetValue = wc.UploadValues(url2, rv2.Method, rv2.PostValues);
                string strLoginRetValue = System.Text.Encoding.UTF8.GetString(loginRetValue);
                System.Console.WriteLine(strLoginRetValue);
                System.Console.WriteLine(wc.CookieContainer);


                RedirectValues rv3 = GetFormValues(strLoginRetValue);
                System.Console.WriteLine(rv3);
                // Request 3
                wc.Headers["referer"] = url2;
                byte[] retValueRedirect = wc.UploadValues(rv3.Action, rv3.Method, rv3.PostValues);
                string strRedirectResult = System.Text.Encoding.UTF8.GetString(retValueRedirect);
                System.Console.WriteLine(strRedirectResult);
            } // End Using wc 

        } // End Sub Test  


    } // End Class HanaLogin 


    internal sealed class DisfunctionalHanaLogin
    {


        private static System.Net.HttpWebRequest GetHttpWebRequest(string pURL, string pUsername, string pPassword)
        {
            System.Net.HttpWebRequest tRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(pURL);
            tRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";

            string tCredentials = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(pUsername + ":" + pPassword));
            tRequest.Headers[System.Net.HttpRequestHeader.KeepAlive] = "true";
            tRequest.Headers[System.Net.HttpRequestHeader.Authorization] = string.Format("Basic {0}", tCredentials);
            // tRequest.Headers(System.Net.HttpRequestHeader.Cookie) = Me.Cookie

            return tRequest;
        } // End Function getHttpWebRequest 


        // 'REM: https://stackoverflow.com/questions/8601302/c-sharp-webclient-login-to-accounts-google-com
        public static string Test(string pURL, string pUsername, string pPassword)
        {
            // 'REM: Erste-Anfrage, um die Keksdose zu bekommen
            System.Net.HttpWebRequest tRequest = GetHttpWebRequest(pURL, pUsername, pPassword);

            System.Net.CookieContainer tCookieContainer = new System.Net.CookieContainer();
            tRequest.CookieContainer = tCookieContainer;

            string tHTML = string.Empty;

            try
            {
                using (System.Net.WebResponse tResponse = tRequest.GetResponse())
                {
                    using (System.IO.StreamReader tStreamReader = new System.IO.StreamReader(tResponse.GetResponseStream()))
                    {
                        tHTML = tStreamReader.ReadToEnd();
                    } // End Using tStreamReader

                } // End Using tResponse 

            } // End Try 
            catch (System.Net.WebException tWebException)
            {
                System.Console.WriteLine(tWebException.Message);

                using (System.Net.WebResponse tResponse = tWebException.Response)
                {
                    System.Net.HttpWebResponse tHttpWebResponse = (System.Net.HttpWebResponse)tResponse;
                    System.Console.WriteLine("Error code: " + tHttpWebResponse.StatusCode);
                    System.Console.WriteLine(System.Environment.NewLine);

                    using (System.IO.StreamReader tStreamReader = new System.IO.StreamReader(tResponse.GetResponseStream()))
                    {
                        string foo = tStreamReader.ReadToEnd();
                        System.Console.WriteLine(foo);
                    } // End Using tStreamReader 

                } // End Using tResponse 

            } // End Catch System.Net.WebException
            catch (System.Exception tException)
            {
                System.Console.WriteLine(tException.Message);
            } // End Catch System.Exception 

            if (!string.IsNullOrWhiteSpace(tHTML))
            {
                // 'REM: Zweite-Anfrage, um das XML zu bekommen
                System.Net.HttpWebRequest tRequest2 = GetHttpWebRequest(pURL, pUsername, pPassword);
                tRequest2.Method = "POST";
                tRequest2.ContentType = "application/x-www-form-urlencoded";
                tRequest2.CookieContainer = tCookieContainer;

                System.Text.StringBuilder tParameters = new System.Text.StringBuilder();
                tParameters.AppendFormat("{0}={1}&", System.Uri.EscapeDataString("j_username"), System.Uri.EscapeDataString(pUsername));
                tParameters.AppendFormat("{0}={1}&", System.Uri.EscapeDataString("j_password"), System.Uri.EscapeDataString(pPassword));
                // tParameters.AppendFormat("{0}={1}&", HttpUtility.UrlEncode("j_password"), HttpUtility.UrlEncode(pPassword));

                using (System.IO.StreamWriter tWriter = new System.IO.StreamWriter(tRequest2.GetRequestStream()))
                {
                    tWriter.Write(tParameters.ToString());
                } // End Using tWriter 

                tHTML = string.Empty;

                using (System.Net.WebResponse tResponse = tRequest2.GetResponse())
                {
                    using (System.IO.StreamReader tStreamReader = new System.IO.StreamReader(tResponse.GetResponseStream()))
                    {
                        tHTML = tStreamReader.ReadToEnd();
                    } // End Using tStreamReader 

                } // End Using tResponse 

            } // End if (!string.IsNullOrWhiteSpace(tHTML)) 

            return tHTML;
        } // End Function Test 


        public static void Test()
        {

            string projName = RedmineMailService.SecretManager.GetSecret<string>("SAP_Project_Name");
            string url = RedmineMailService.SecretManager.GetSecret<string>("URL", projName);
            string username = RedmineMailService.SecretManager.GetSecret<string>("Username", projName);
            string password = RedmineMailService.SecretManager.GetSecret<string>("Password", projName);

            string base64Creds = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
            string basicCredentials = "Basic " + base64Creds;

            string result = Test(url, username, password);
            System.Console.WriteLine(result);
        } // End Sub Test 


    } // End Class DisfunctionalHanaLogin


} // End Namespace 
