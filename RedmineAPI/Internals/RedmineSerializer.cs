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


using Redmine.Net.Api.Types;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Exceptions;


namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal static class RedmineSerializer
    {
        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>
        /// The System.String that contains the XML document.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        // ReSharper disable once InconsistentNaming
        private static string ToXML<T>(T obj) where T : class
        {
            System.Xml.XmlWriterSettings xws = new System.Xml.XmlWriterSettings { OmitXmlDeclaration = true };
            using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
            {
                using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stringWriter, xws))
                {
                    System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    sr.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize.</typeparam>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <returns>
        /// The T object being deserialized.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        // ReSharper disable once InconsistentNaming
        private static T FromXML<T>(string xml) where T : class
        {
            using (System.IO.StringReader text = new System.IO.StringReader(xml))
            {
                System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return sr.Deserialize(text) as T;
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <param name="type">The type of objects to deserialize.</param>
        /// <returns>
        /// The System.Object being deserialized.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        // ReSharper disable once InconsistentNaming
        private static object FromXML(string xml, System.Type type)
        {
            using (System.IO.StringReader text = new System.IO.StringReader(xml))
            {
                System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(type);
                return sr.Deserialize(text);
            }
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj, MimeFormat mimeFormat) where T : class, new()
        {
            return ToXML(obj);
        }

        /// <summary>
        /// Deserializes the specified response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">could not deserialize:  + response</exception>
        public static T Deserialize<T>(string response, MimeFormat mimeFormat) 
            where T : class, new()
        {
            if (string.IsNullOrEmpty(response))
                throw new RedmineException("could not deserialize: " + response);

            return FromXML<T>(response);
        }

        /// <summary>
        /// Deserializes the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">web response is null!</exception>
        public static PaginatedObjects<T> DeserializeList<T>(string response, MimeFormat mimeFormat) 
            where T : class, new()
        {
            if (string.IsNullOrEmpty(response))
                throw new RedmineException("web response is null!");

            return XmlDeserializeList<T>(response);
        }

        /// <summary>
        /// XMLs the deserialize list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">could not deserialize:  + response</exception>
        private static PaginatedObjects<T> XmlDeserializeList<T>(string response) 
            where T : class, new()
        {
            if (string.IsNullOrEmpty(response))
                throw new RedmineException("could not deserialize: " + response);

            using (System.IO.StringReader stringReader = new System.IO.StringReader(response))
            {
                using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(stringReader))
                {
                    xmlReader.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
                    xmlReader.Read();
                    xmlReader.Read();

                    int totalItems = xmlReader.ReadAttributeAsInt(RedmineKeys.TOTAL_COUNT);

                    System.Collections.Generic.List<T> result = xmlReader.ReadElementContentAsCollection<T>();
                    return new PaginatedObjects<T>()
                    {
                        TotalCount = totalItems,
                        Objects = result
                    };
                }
            }
        }
    }
}