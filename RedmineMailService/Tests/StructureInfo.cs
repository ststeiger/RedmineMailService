
namespace RedmineMailService.Tests 
{


    public class StructureInfo
    {


        public static void CreateDirectoryRecursively(string path)
        {
            if (System.IO.Path.DirectorySeparatorChar != '/')
                path = path.Replace(System.IO.Path.DirectorySeparatorChar, '/');

            string[] pathParts = path.Split('/');

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (i > 0)
                    pathParts[i] = System.IO.Path.Combine(pathParts[i - 1], pathParts[i]);

                if (!System.IO.Directory.Exists(pathParts[i]))
                    System.IO.Directory.CreateDirectory(pathParts[i]);
            } // Next i 

        } // End Sub CreateDirectoryRecursively 





        public static System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo>
            GetDirectories1(string di)
        {
            return GetDirectories(new System.IO.DirectoryInfo(di));
        } // End Function GetDirectories 


        public static System.IO.DirectoryInfo[] GetDirectories(string di)
        {
            System.IO.DirectoryInfo[] dis = System.Linq.Enumerable.ToArray(
                GetDirectories( new System.IO.DirectoryInfo(di) )
            );

            System.Array.Sort(dis,
                delegate (System.IO.DirectoryInfo info, System.IO.DirectoryInfo directoryInfo)
                {
                    return info.FullName.CompareTo(directoryInfo.FullName);
                });

            return dis;
        } // End Function GetDirectories 


        public static System.Collections.Generic.IEnumerable<System.IO.DirectoryInfo> 
            GetDirectories(System.IO.DirectoryInfo di)
        {
            System.IO.DirectoryInfo[] dis = di.GetDirectories();

            for (int i = 0; i < dis.Length; ++i)
            {
                if (dis[i].Name.StartsWith("."))
                    continue;

                System.IO.FileInfo[] fis = dis[i].GetFiles("*.sln", System.IO.SearchOption.TopDirectoryOnly);
                if (fis.Length > 0)
                    yield return dis[i];

                foreach (System.IO.DirectoryInfo thisInfo in GetDirectories(dis[i]))
                {
                    yield return thisInfo;
                } // Next thisInfo 

            } // Next i 

        } // End Function GetDirectories 



        public static void GetDirectoriesAsJSON()
        {
            string path = @"/root/github";
            GetDirectoriesAsJSON(path);
        }


        public static string GetDirectoriesByFileAsJSON(string path)
        {
            string[] solutions = System.IO.Directory.GetFiles(path, "*.sln",
                System.IO.SearchOption.AllDirectories
            );

            // new System.IO.DirectoryInfo("").GetFiles()
            // new System.IO.DirectoryInfo("").GetDirectories()
            // new System.IO.FileInfo("")

            //System.IO.DirectoryInfo[] dirs = new System.IO.DirectoryInfo(path)
            //        .GetDirectories("*", SearchOption.AllDirectories);


            System.Array.Sort(solutions, System.StringComparer.OrdinalIgnoreCase);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(solutions
                , Newtonsoft.Json.Formatting.Indented);

            return json;
        }


        public static string GetDirectoriesAsJSON(string path)
        {
            System.IO.DirectoryInfo[] dirs = GetDirectories(path);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dirs,
                Newtonsoft.Json.Formatting.Indented
                , new DirectoryInfoJsonConverter(path)
            );

            return json;
        } // End Sub GetDirectories 


        public static void Test()
        {
            string json = GetDirectoriesAsJSON(
                   @"C:\Users\Administrator\Documents\Visual Studio 2015\TFS"
           );

            System.Console.WriteLine(json);
        }



        public static System.Collections.Generic.IEnumerable<string> 
            GetFiles(
              string path
            , System.Collections.Generic.IEnumerable<string> excludedFiles
            , System.Collections.Generic.IEnumerable<string> excludedExtensions
            , System.IO.SearchOption searchOption = System.IO.SearchOption.AllDirectories
        )
        {
            System.Collections.Generic.IEnumerable<string> files =
                System.IO.Directory.EnumerateFiles(path, "*.*", searchOption);

            foreach (string file in files)
            {
                if(System.Linq.Enumerable.Contains(excludedFiles, file))
                {
                    continue;
                }

                if (System.Linq.Enumerable.Contains(excludedExtensions, System.IO.Path.GetExtension(file)))
                {
                    continue;
                }

                yield return file;
            } // Next file 

        } // End Function GetFiles 


        public class DirectoryInfoJsonConverter
            : Newtonsoft.Json.JsonConverter
        {

            protected int m_baseDirLength;

            public DirectoryInfoJsonConverter()
            { } // End Constructor 

            public DirectoryInfoJsonConverter(string baseDir)
            {
                this.m_baseDirLength = baseDir.Length + 1;
            } // End Constructor 

            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(System.IO.DirectoryInfo);
            } // End Function CanConvert 

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType
                , object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (reader.Value is string s)
                {
                    return new System.IO.DirectoryInfo(s);
                }

                throw new System.ArgumentOutOfRangeException(nameof(reader));
            } // End Function ReadJson 

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer
                , object value
                , Newtonsoft.Json.JsonSerializer serializer)
            {
                if (!(value is System.IO.DirectoryInfo directoryInfo))
                {
                    throw new System.ArgumentOutOfRangeException(nameof(value));
                } // End if (!(value is DirectoryInfo directoryInfo)) 

                string dirName = directoryInfo.FullName.Substring(this.m_baseDirLength);

                if (System.IO.Path.DirectorySeparatorChar != '/')
                    dirName = dirName.Replace(System.IO.Path.DirectorySeparatorChar, '/');

                writer.WriteValue(dirName);
            } // End Sub WriteJson 

        } // End Class DirectoryInfoJsonConverter 



    } // End Class StructureInfo 


} // End Namespace RedmineMailService.Redmine 
