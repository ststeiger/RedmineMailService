
using System;

namespace RedmineMailService
{


    class Program
    {


        [System.STAThread]
        static void Main(string[] args)
        {
            // https://github.com/dasMulli/dotnet-win32-service
            // ServiceStarter.Start(args);
            
            // MailSender.Test();
            
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
