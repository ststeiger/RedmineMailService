
namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>http://florianreischl.blogspot.ro/search/label/c%23</remarks>
    public class XmlStreamingSerializer<T>
    {
        static System.Xml.Serialization.XmlSerializerNamespaces ns;
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        System.Xml.XmlWriter writer;
        bool finished;

        static XmlStreamingSerializer()
        {
            ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
        }

        private XmlStreamingSerializer()
        {
            serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        }

        public XmlStreamingSerializer(System.IO.TextWriter w)
                : this(System.Xml.XmlWriter.Create(w))
        {
        }

        public XmlStreamingSerializer(System.Xml.XmlWriter writer)
                : this()
        {
            this.writer = writer;
            writer.WriteStartDocument();
            writer.WriteStartElement("ArrayOf" + typeof(T).Name);
        }

        public void Finish()
        {
            writer.WriteEndDocument();
            writer.Flush();
            finished = true;
        }

        public void Close()
        {
            if (!finished)
                Finish();
            writer.Close();
        }

        public void Serialize(T item)
        {
            serializer.Serialize(writer, item, ns);
        }
    }
}