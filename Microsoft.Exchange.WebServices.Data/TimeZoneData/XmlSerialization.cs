
namespace Microsoft.Exchange.WebServices.Data.TimeZoneData
{


    // http://www.switchonthecode.com/tutorials/csharp-tutorial-xml-serialization
    // http://www.codeproject.com/KB/XML/xml_serializationasp.aspx
    internal class Serialization
	{


        public static T DeserializeXmlFromStream<T>(System.IO.Stream strm)
        {
            System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            T ThisType = default(T);

            using (System.IO.StreamReader srEncodingReader = new System.IO.StreamReader(strm, System.Text.Encoding.UTF8))
            {
                ThisType = (T)deserializer.Deserialize(srEncodingReader);
                srEncodingReader.Close();
            } // End Using srEncodingReader

            deserializer = null;

            return ThisType;
        } // End Function DeserializeXmlFromStream


        public static T DeserializeXmlFromFile<T>(string fileName)
		{
			T tReturnValue = default(T);

            using (System.IO.FileStream fstrm = new System.IO.FileStream(fileName, System.IO.FileMode.Open
                , System.IO.FileAccess.Read, System.IO.FileShare.Read)) 
            {
				tReturnValue = DeserializeXmlFromStream<T>(fstrm);
				fstrm.Close();
            } // End Using fstrm

			return tReturnValue;
		} // End Function DeserializeXmlFromFile
        

		public static T DeserializeXmlFromEmbeddedRessourceByExactName<T>(System.Reflection.Assembly asm, string resourceName)
		{
            T tReturnValue = default(T);

			using (System.IO.Stream fstrm = asm.GetManifestResourceStream(resourceName)) 
            {
				tReturnValue = DeserializeXmlFromStream<T>(fstrm);
				fstrm.Close();
            } // End Using fstrm

			return tReturnValue;
		} // End Function DeserializeXmlFromEmbeddedRessource


        private static string FindName(System.Reflection.Assembly asm, string resourceName)
        {
            foreach (string thisResourceName in asm.GetManifestResourceNames())
            {
                if (thisResourceName.EndsWith(resourceName))
                    return thisResourceName;
            }

            return null;
        }


        public static T DeserializeXmlFromEmbeddedRessource<T>(System.Reflection.Assembly asm, string resourceName)
        {
            string exactResourceName = FindName(asm, resourceName);
            if (string.IsNullOrWhiteSpace(exactResourceName))
                throw new System.IO.FileNotFoundException("No embedded resource called \"" + resourceName + "\" found.");

            return DeserializeXmlFromEmbeddedRessourceByExactName<T>(asm, exactResourceName);
        }


        public static T DeserializeXmlFromEmbeddedRessource<T>(string resourceName)
        {
            System.Reflection.Assembly asm = typeof(Serialization).Assembly;
            return DeserializeXmlFromEmbeddedRessource<T>(asm, resourceName);
        }
        

    } // End Class Serialization


} // End Namespace COR.Tools.XML
