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


using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;


namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 2.2
    /// </summary>
    [System.Xml.Serialization.XmlRoot(RedmineKeys.WIKI_PAGE)]
    public class WikiPage 
        : Identifiable<WikiPage>
        , System.Xml.Serialization.IXmlSerializable
        , System.IEquatable<WikiPage>
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.TITLE)]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.TEXT)]
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.COMMENTS)]
        public string Comments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.VERSION)]
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.CREATED_ON)]
        public System.DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.UPDATED_ON)]
        public System.DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        [System.Xml.Serialization.XmlArray(RedmineKeys.ATTACHMENTS)]
        [System.Xml.Serialization.XmlArrayItem(RedmineKeys.ATTACHMENT)]
        public System.Collections.Generic.IList<Attachment> Attachments { get; set; }
        



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.TEXT: Text = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.VERSION: Version = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.TEXT, Text);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);
            writer.WriteValueOrEmpty<int>(Version, RedmineKeys.VERSION);
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WikiPage other)
        {
            if (other == null)
                return false;

            return Id == other.Id
                && Title == other.Title
                && Text == other.Text
                && Comments == other.Comments
                && Version == other.Version
                && Author == other.Author
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn;
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
                hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Text, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Version, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[WikiPage: {8}, Title={0}, Text={1}, Comments={2}, Version={3}, Author={4}, CreatedOn={5}, UpdatedOn={6}, Attachments={7}]",
                Title, Text, Comments, Version, Author, CreatedOn, UpdatedOn, Attachments, base.ToString());
        }
        
    }
}