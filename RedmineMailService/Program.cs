
using System;

namespace RedmineMailService
{


    class Program
    {

        //Code snippet
        /// <summary>
        /// method for generating a country list, say for populating
        /// a ComboBox, with country options. We return the
        /// values in a Generic List<t>
        /// </t></summary>
        /// <returns></returns>
        public static System.Collections.Generic.List<string> GetCountryList()
        {
            //create a new Generic list to hold the country names returned
            System.Collections.Generic.List<string> cultureList = new System.Collections.Generic.List<string>();

            //create an array of CultureInfo to hold all the cultures found, these include the users local cluture, and all the
            //cultures installed with the .Net Framework
            System.Globalization.CultureInfo[] cultures = System.Globalization.CultureInfo.GetCultures(
                System.Globalization.CultureTypes.AllCultures & ~System.Globalization.CultureTypes.NeutralCultures);

            //loop through all the cultures found
            foreach (System.Globalization.CultureInfo culture in cultures)
            {
                //pass the current culture's Locale ID (http://msdn.microsoft.com/en-us/library/0h88fahh.aspx)
                //to the RegionInfo contructor to gain access to the information for that culture
                System.Globalization.RegionInfo region = new System.Globalization.RegionInfo(culture.LCID);

                //make sure out generic list doesnt already
                //contain this country
                if (!(cultureList.Contains(region.EnglishName)))
                    //not there so add the EnglishName (http://msdn.microsoft.com/en-us/library/system.globalization.regioninfo.englishname.aspx)
                    //value to our generic list
                    cultureList.Add(region.EnglishName);
            }

            return cultureList;
        }

        // https://stackoverflow.com/questions/4967903/linux-windows-timezone-mapping
        // https://stackoverflow.com/questions/19695439/get-the-default-timezone-for-a-country-via-cultureinfo
        public static void ListCOuntries()
        {
            //Example usage
            foreach (string country in GetCountryList())
            {
                //comboBox1.Items.Add(country);
            }

        }



        public static void ListTimeZones()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Collections.Generic.List<TimeZoneInfo> ls = new System.Collections.Generic.List<TimeZoneInfo>();


            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
            {
                ls.Add(z);
            }

            ls.Sort(delegate (TimeZoneInfo x, TimeZoneInfo y)
                {
                    return x.Id.CompareTo(y.Id);
                }
            );

            foreach (TimeZoneInfo z in ls)
            {
                Console.WriteLine(z.Id);
                sb.AppendLine(z.Id);
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ls, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("timezoneinfo.json", json, System.Text.Encoding.UTF8);

            string str = sb.ToString();
            System.IO.File.WriteAllText("timezoneinfo.txt", str, System.Text.Encoding.UTF8);
                
        }



        [System.STAThread]
        static void Main(string[] args)
        {
            //string linuxTimezone = Microsoft.Exchange.WebServices.Data.TimeZoneData.TimeZoneTranslator.WindowsToLinux("W. Europe Standard Time");
            //linuxTimezone = Microsoft.Exchange.WebServices.Data.TimeZoneData.TimeZoneTranslator.WindowsToLinux("Greenwich Standard Time");
            //string windowsTimezone = Microsoft.Exchange.WebServices.Data.TimeZoneData.TimeZoneTranslator.LinuxToWindows("Europe/Zurich");

            //System.Console.WriteLine(linuxTimezone);
            //System.Console.WriteLine(windowsTimezone);

            System.Console.WriteLine(System.Globalization.RegionInfo.CurrentRegion.EnglishName);
            System.Console.WriteLine(System.Globalization.RegionInfo.CurrentRegion.DisplayName);
            System.Console.WriteLine(System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName);


            // https://github.com/dasMulli/dotnet-win32-service
            // ServiceStarter.Start(args);

            // MailSender.Test();

            // ListTimeZones();

            CertificateCallback.Initialize();
            Titanium.Web.Proxy.Examples.Basic.ProxyTestController controller = Titanium.Web.Proxy.Examples.Basic.ProxyServerProgram.Start();


            // System.Console.WriteLine("Hit any key to exit..");
            // System.Console.WriteLine();
            // System.Console.Read();

            // controller.Stop();

            /*
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                // wc.Proxy = new System.Net.WebProxy("127.0.0.1", 8000);

                // string text = wc.DownloadString("http://sps.ch");
                string text = wc.DownloadString("https://www.whatismybrowser.com/detect/what-is-my-user-agent");

                System.Console.WriteLine(text);
            }
            */

            TestMailReader.Test();
            
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
            controller.Stop();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace RedmineMailService 
