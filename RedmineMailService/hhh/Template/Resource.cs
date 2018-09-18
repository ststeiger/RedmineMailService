
namespace RedmineMailService
{


    public class Resource
    {
        public System.IO.Stream Stream;
        public string FileName;
        public string ContentType;

        protected string m_uid;

        public string UID
        {
            get
            {
                if (m_uid != null)
                    return m_uid;

                this.m_uid = System.Guid.NewGuid().ToString();
                return this.m_uid;
            }
        }
        


        public Resource()
        { } // Constructor 


        private static string ContentTypeFromFileName(string fileName)
        {
            System.Collections.Generic.Dictionary<string, string> dictMimes =
                RedmineMailService.MimeMapper.SetupFileExtensionDictionary();

            string extension = System.IO.Path.GetExtension(fileName);

            if (dictMimes.ContainsKey(extension))
                return dictMimes[extension];

            return "application/pdf";
        } // End Function ContentTypeFromFileName 


        public Resource(System.IO.Stream stream, string fileName, string contentType)
        {
            this.Stream = stream;
            this.FileName = fileName;
            this.ContentType = contentType;
        } // Constructor 


        public Resource(System.IO.Stream stream, string fileName)
            : this(stream, fileName, ContentTypeFromFileName(fileName))
        { } // Constructor 


        public Resource(byte[] fileContent, string fileName, string contentType)
            : this(new System.IO.MemoryStream(fileContent), fileName, contentType)
        { } // Constructor 


        public Resource(byte[] fileContent, string fileName)
            : this(new System.IO.MemoryStream(fileContent), fileName, ContentTypeFromFileName(fileName))
        { } // Constructor 


        public Resource(string fileNameAndPath, string fileName, string contentType)
            : this(
                new System.IO.FileStream(
                      fileNameAndPath
                    , System.IO.FileMode.Open
                    , System.IO.FileAccess.Read
                    , System.IO.FileShare.Read
                )
                , fileName
                , contentType
            )
        { } // Constructor 


        public Resource(string fileNameAndPath, string fileName)
            : this(fileNameAndPath, fileName, ContentTypeFromFileName(fileName))
        { } // Constructor 


        public Resource(string fileNameAndPath)
            : this(fileNameAndPath, System.IO.Path.GetFileName(fileNameAndPath), ContentTypeFromFileName(fileNameAndPath))
        { } // Constructor 


    } // End Class Resource 


} // End Namespace RedmineMailService 
