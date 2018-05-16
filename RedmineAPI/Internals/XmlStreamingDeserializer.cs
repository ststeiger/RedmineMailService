
namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>http://florianreischl.blogspot.ro/search/label/c%23</remarks>
    public class XmlStreamingDeserializer<T>
    {
        static System.Xml.Serialization.XmlSerializerNamespaces ns;
        System.Xml.Serialization.XmlSerializer serializer;
        System.Xml.XmlReader reader;

        static XmlStreamingDeserializer()
        {
            ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
        }

        private XmlStreamingDeserializer()
        {
            serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        }

        public XmlStreamingDeserializer(System.IO.TextReader reader)
                : this(System.Xml.XmlReader.Create(reader))
        {
        }

        public XmlStreamingDeserializer(System.Xml.XmlReader reader)
                : this()
        {
            this.reader = reader;
        }

        public void Close()
        {
            reader.Close();
        }

        public T Deserialize()
        {
            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Depth == 1 && reader.Name == typeof(T).Name)
                {
                    System.Xml.XmlReader xmlReader = reader.ReadSubtree();
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
            return default(T);
        }
    }
}