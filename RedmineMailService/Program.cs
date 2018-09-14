
using System;
using System.ComponentModel.Design;
using System.IO;
using Newtonsoft.Json;

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
        }
        
        
        public static void ListDirectoryStructure()
        {
            string path = @"/root/github";
            string[] solutions = System.IO.Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories);
            Array.Sort(solutions, System.StringComparer.OrdinalIgnoreCase);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(solutions, Formatting.Indented);
            
            
            // new System.IO.DirectoryInfo("").GetFiles()
            // new System.IO.DirectoryInfo("").GetDirectories()
            // new System.IO.FileInfo("")
            
            //System.IO.DirectoryInfo[] dirs = new System.IO.DirectoryInfo(path)
            //        .GetDirectories("*", SearchOption.AllDirectories);

            System.IO.DirectoryInfo[] dirs = 
                System.Linq.Enumerable.ToArray(GetDirectories(path));
            
            
            Array.Sort(dirs, delegate(DirectoryInfo info, DirectoryInfo directoryInfo)
                {
                    return info.FullName.CompareTo(directoryInfo.FullName);
                });
            
            
            json = JsonConvert.SerializeObject(dirs, Formatting.Indented, new DirectoryInfoJsonConverter(path));
            
            System.Console.WriteLine(json);
            System.Console.WriteLine(dirs);
        }
        
        
        void CreateDirectoryRecursively(string path)
        {
            if (System.IO.Path.DirectorySeparatorChar != '/')
                path = path.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            
            string[] pathParts = path.Split('/');

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (i > 0)
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                
                if (!Directory.Exists(pathParts[i]))
                    Directory.CreateDirectory(pathParts[i]);
            }
        }
        
        
        public static System.Collections.Generic.IEnumerable<string> GetFiles(string path
            , System.Collections.Generic.IEnumerable<string> excludedFiles
            , System.Collections.Generic.IEnumerable<string> excludedExtensions
            , SearchOption searchOption = SearchOption.AllDirectories)
        {
            var files = Directory.EnumerateFiles(path, "*.*", searchOption);

            foreach(var file in files)
            {
                /*
                if(excludedFiles.Contains(file))
                {
                    continue;
                }

                if(excludedExtensions.Contains(Path.GetExtension(file)))
                {
                    continue;
                }
                */
                yield return file;
            }
        }




        public static System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo>
            GetDirectories(string di)
        {
            return GetDirectories(new System.IO.DirectoryInfo(di));
        }


        public static System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo> GetDirectories(
             System.IO.DirectoryInfo di)
        {

            System.IO.DirectoryInfo[] dis = di.GetDirectories();
            
            for (int i = 0; i < dis.Length; ++i)
            {
                if (dis[i].Name.StartsWith("."))
                    continue;

                System.IO.FileInfo[] fis = dis[i].GetFiles("*.sln", SearchOption.TopDirectoryOnly);
                if(fis.Length > 0)
                    yield return dis[i];
                
                foreach (System.IO.DirectoryInfo thisInfo in GetDirectories(dis[i]))
                {
                    yield return thisInfo;
                }
                
            }
            
        }

        
        
        public class DirectoryInfoJsonConverter : JsonConverter
        {

            protected int m_baseDirLength;
            
            public DirectoryInfoJsonConverter()
            { }
            
            public DirectoryInfoJsonConverter(string baseDir)
            {
                this.m_baseDirLength = baseDir.Length + 1;
            }
            
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DirectoryInfo);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value is string s)
                {
                    return new DirectoryInfo(s);
                }

                throw new ArgumentOutOfRangeException(nameof(reader));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (!(value is DirectoryInfo directoryInfo))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                string dirName = directoryInfo.FullName.Substring(this.m_baseDirLength);
                
                if(System.IO.Path.DirectorySeparatorChar != '/')
                    dirName = dirName.Replace(System.IO.Path.DirectorySeparatorChar, '/');
                
                writer.WriteValue(dirName);
            }
        }
        
        
        [System.STAThread]
        static void Main(string[] args)
        {
            ListDirectoryStructure();
            return;
            
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
