/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


namespace Redmine.Net.Api.Extensions
{


    /// <summary>
    /// 
    /// </summary>
    public static partial class XmlExtensions
    {
        /// <summary>
        /// Reads the attribute as int.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static int ReadAttributeAsInt(this System.Xml.XmlReader reader, string attributeName)
        {
            string attribute = reader.GetAttribute(attributeName);
            int result;

            if (string.IsNullOrEmpty(attribute) 
                || !int.TryParse(attribute, System.Globalization.NumberStyles.Any
                , System.Globalization.NumberFormatInfo.InvariantInfo, out result))
                return default(int);

            return result;
        }

        /// <summary>
        /// Reads the attribute as nullable int.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static int? ReadAttributeAsNullableInt(this System.Xml.XmlReader reader, string attributeName)
        {
            string attribute = reader.GetAttribute(attributeName);
            int result;

            if (string.IsNullOrEmpty(attribute) 
                || !int.TryParse(attribute, System.Globalization.NumberStyles.Any
                , System.Globalization.NumberFormatInfo.InvariantInfo, out result))
                return default(int?);

            return result;
        }

        /// <summary>
        /// Reads the attribute as boolean.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static bool ReadAttributeAsBoolean(this System.Xml.XmlReader reader, string attributeName)
        {
            string attribute = reader.GetAttribute(attributeName);
            bool result;

            if (string.IsNullOrEmpty(attribute) || !bool.TryParse(attribute, out result))
                return false;
            
            return result;
        }

        /// <summary>
        /// Reads the element content as nullable date time.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static System.DateTime? ReadElementContentAsNullableDateTime(this System.Xml.XmlReader reader)
        {
            string str = reader.ReadElementContentAsString();
            System.DateTime result;

            if (string.IsNullOrEmpty(str) || !System.DateTime.TryParse(str, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable float.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static float? ReadElementContentAsNullableFloat(this System.Xml.XmlReader reader)
        {
            string str = reader.ReadElementContentAsString();
            float result;

            if (string.IsNullOrEmpty(str) || !float.TryParse(str
                , System.Globalization.NumberStyles.Any, 
                System.Globalization.NumberFormatInfo.InvariantInfo, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable int.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static int? ReadElementContentAsNullableInt(this System.Xml.XmlReader reader)
        {
            string str = reader.ReadElementContentAsString();
            int result;

            if (string.IsNullOrEmpty(str) 
                || !int.TryParse(str, System.Globalization.NumberStyles.Any
                , System.Globalization.NumberFormatInfo.InvariantInfo, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable decimal.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static decimal? ReadElementContentAsNullableDecimal(this System.Xml.XmlReader reader)
        {
            string str = reader.ReadElementContentAsString();
            decimal result;

            if (string.IsNullOrEmpty(str) 
                || !decimal.TryParse(str, System.Globalization.NumberStyles.Any
                , System.Globalization.NumberFormatInfo.InvariantInfo, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static System.Collections.Generic.List<T> ReadElementContentAsCollection<T>(this System.Xml.XmlReader reader) where T : class
        {
            System.Collections.Generic.List<T> result = new System.Collections.Generic.List<T>();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            string xml = reader.ReadOuterXml();
            using (System.IO.StringReader sr = new System.IO.StringReader(xml))
            {
                using (System.Xml.XmlTextReader xmlTextReader = new System.Xml.XmlTextReader(sr))
                {
                    xmlTextReader.ReadStartElement();
                    while (!xmlTextReader.EOF)
                    {
                        if (xmlTextReader.NodeType == System.Xml.XmlNodeType.EndElement)
                        {
                            xmlTextReader.ReadEndElement();
                            continue;
                        }

                        T obj;

                        if (xmlTextReader.IsEmptyElement && xmlTextReader.HasAttributes)
                        {
                            obj = serializer.Deserialize(xmlTextReader) as T;
                        }
                        else
                        {
                            System.Xml.XmlReader subTree = xmlTextReader.ReadSubtree();
                            obj = serializer.Deserialize(subTree) as T;
                        }
                        if (obj != null)
                            result.Add(obj);

                        if (!xmlTextReader.IsEmptyElement)
                            xmlTextReader.Read();
                    }
                }
            }
            return result;
        }
    }
}