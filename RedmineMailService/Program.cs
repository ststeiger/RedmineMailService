
using RedmineMailService.CertSSL;

namespace RedmineMailService
{


    public class Program
    {


        public static void Doku()
        {
            // alert(&quot;This is a test...&quot;);
            // string res = System.Web.HttpUtility.HtmlAttributeEncode("alert(\"This is a test...\");");
            // System.Console.WriteLine(res);


            // Allow text-only email ? 
            // HtmlAgilityPack

            // <b>Test</b> ==> *Test*
            // <i>Test</i> ==> _Test_
            // <u>Test</u> ==> +Test+
            // <del>Test</del> ==> -Test-
            // <code>Test</code> ==> @test@
            // <pre>Test</pre> ==> <pre>test</pre>

            // <h1>Test</h1> ==> h1. test
            // ul => * hello
            // num => # hello
            // links einrücken > 
            // link
            // image



            /*
            Tracker: Anfrage
            Status: neu
            Kundenname:
            Verrechenbar: nein
            gemeldet von: email


            From: foo@domain.com ==> projekte/ZH
            To: issue.tracker@cor
            Date:

            Sub: <issue title>


            Content: <html>
            Anlage:


            DB:
            projects (sync)
            kundenname (sync)
            kunde 
            --domains (manual)
            zo_kunde_multiAtt(kunde_id, domain/_id, email) UIX where domain is not null 
            zo_kunde_email(kunde, email)
            */

            // Read unread emails
            // Add redmine issue
            // Move email to non-inbox folder
            // don't explode on any step
            // How to ensure text-only email ? 
        } // End Sub Doku 


        public static void TestRootCertificateGeneration()
        {
            // Watch for static variables ! 
            CertificateCallback.Initialize();
            Titanium.Web.Proxy.Examples.Basic.ProxyTestController controller1 = Titanium.Web.Proxy.Examples.Basic.ProxyServerProgram.Start();

            // TestMailReader.Test();

            controller1.Stop();
        }


        public static void ToCertUtil()
        {
            string inputFile = @"D:\username\Desktop\RSS\Desktop.zip";
            string outputFile = @"D:\username\Desktop\RSS\Desktop_plain.txt";
            ToCertUtil(inputFile, outputFile);
        }
        
        
        public static void ToCertUtil(string inputFile, string outputFile)
        {
            //const int BUFFER_SIZE = 4032; // 4032%3=0 && 4032%64=0
            // const int BUFFER_SIZE = 4080; // 4080%3=0 && 4080%8=0
            const int BUFFER_SIZE = 4095; // 4095%3=0 && 4095~=4096 (pageSize) 
            byte[] buffer = new byte[BUFFER_SIZE];
            
            using (System.IO.FileStream outputStream = System.IO.File.OpenWrite(outputFile))
            {
                
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputStream, System.Text.Encoding.ASCII))
                {
                    sw.Write("-----BEGIN CERTIFICATE-----");
                    sw.Write(System.Environment.NewLine);
                    
                    using (System.IO.FileStream inputStream = System.IO.File.OpenRead(inputFile))
                    {
                        using (System.IO.BinaryReader br = new System.IO.BinaryReader(inputStream))
                        {
                            br.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                            long totalLength = inputStream.Length;
                            
                            
                            long totalRead = 0;
                            int bytesRead;
                            while ((bytesRead = br.Read(buffer, 0, BUFFER_SIZE)) > 0)
                            {
                                totalRead += bytesRead;
                                
                                // string b64 = System.Convert.ToBase64String(buffer, 0, bytesRead, System.Base64FormattingOptions.InsertLineBreaks);
                                // string b64 = System.Convert.ToBase64String(buffer);
                                
                                bool isFinal = (bytesRead < BUFFER_SIZE || totalRead == totalLength);
                                CertSSL.cb64.ConvertToBase64Array(sw, buffer, 0, bytesRead, true, isFinal);
                                
                                //if (bytesRead < BUFFER_SIZE || totalRead == totalLength)
                                //    b64 = b64.Substring(0, b64.Length - 2);
                                
                                //sw.Write(b64);
                                
                                // sw.Write(System.Environment.NewLine);
                                // CertSSL.cb64.ConvertToBase64Array(sw, buffer, 0, bytesRead, true);
                            } // Whend 
                            
                            br.Close();
                        } // End Using br 
                        
                    } // End Using inputStream 
                    
                    sw.Write(System.Environment.NewLine);
                    sw.Write("-----END CERTIFICATE-----");
                    sw.Write(System.Environment.NewLine);
                } // End Using sw 
                
            } // End Using outputStream 
            
        } // End Sub ToCertUtil 
        
        
        // https://twitter.com/gitlost
        // https://badhtml.com/
        [System.STAThread]
        static void Main(string[] args)
        {
            // AnySqlWebAdmin.CerGenerator.CreateSignatureRequest();
            AnySqlWebAdmin.CerGenerator.Test2();
            return;
            // AnySqlWebAdmin.CerGenerator.Test();
            // RedmineMailService.MailGender.GetGenders();

            RedmineMailService.SendMail.Test();
            
            
            // RedmineMailService.Tests.StructureInfo.Test();
            MailSender.Test();
            
            
            System.Configuration.ConfigurationManager.AppSettings.Get();
            RedmineMailService.Exchange.SendMailWithAttachment();
            return;
            // https://github.com/dasMulli/dotnet-win32-service
            // ServiceStarter.Start(args);
            
            // MailSender.Test();
            
            CertificateCallback.Initialize();
            Titanium.Web.Proxy.Examples.Basic.ProxyTestController controller = Titanium.Web.Proxy.Examples.Basic.ProxyServerProgram.Start();
            
            // TestMailReader.Test();
            
            controller.Stop();
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 
        
        
    } // End Class Program 
    
    
} // End Namespace RedmineMailService 
