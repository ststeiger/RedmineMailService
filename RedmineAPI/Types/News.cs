﻿/*
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
    /// Availability 1.1
    /// </summary>
    [System.Xml.Serialization.XmlRoot(RedmineKeys.NEWS)]
    public class News 
        : Identifiable<News>
        , System.IEquatable<News>
        , System.Xml.Serialization.IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.TITLE)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.SUMMARY)]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.DESCRIPTION)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.CREATED_ON, IsNullable = true)]
        public System.DateTime? CreatedOn { get; set; }

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

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.SUMMARY: Summary = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(News other)
        {
            if (other == null)
                return false;

            return (Id == other.Id
                && Project == other.Project
                && Author == other.Author
                && Title == other.Title
                && Summary == other.Summary
                && Description == other.Description
                && CreatedOn == other.CreatedOn);
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
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Summary, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[News: {6}, Project={0}, Author={1}, Title={2}, Summary={3}, Description={4}, CreatedOn={5}]",
                Project, Author, Title, Summary, Description, CreatedOn, base.ToString());
        }
    }
}