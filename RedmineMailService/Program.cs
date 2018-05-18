
using System;

namespace RedmineMailService
{


    class Program
    {



        public static void foo()
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
            // https://github.com/dasMulli/dotnet-win32-service
            // ServiceStarter.Start(args);

            // MailSender.Test();

            foo();

            CertificateCallback.Initialize();
            
            /*
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                string text = wc.DownloadString("https://www.whatismybrowser.com/detect/what-is-my-user-agent");
                System.Console.WriteLine(text);
            }
            */

            TestMailReader.Test();
            
            
            
            
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace RedmineMailService 
