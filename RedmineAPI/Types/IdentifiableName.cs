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


using Redmine.Net.Api.Internals;


namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentifiableName 
        : Identifiable<IdentifiableName>
        , System.Xml.Serialization.IXmlSerializable
        , System.IEquatable<IdentifiableName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableName"/> class.
        /// </summary>
        public IdentifiableName()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableName"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public IdentifiableName(System.Xml.XmlReader reader)
        {
            Initialize(reader);
        }

        private void Initialize(System.Xml.XmlReader reader)
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [System.Xml.Serialization.XmlAttribute(RedmineKeys.NAME)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            Id = System.Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID));
            Name = reader.GetAttribute(RedmineKeys.NAME);
            reader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString(RedmineKeys.ID, Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString(RedmineKeys.NAME, Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[IdentifiableName: Id={0}, Name={1}]", Id, Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IdentifiableName other)
        {
            if (other == null)
                return false;

            return (Id == other.Id && Name == other.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                return hashCode;
            }
        }
    }
}