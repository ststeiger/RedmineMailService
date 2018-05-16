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
    [System.Xml.Serialization.XmlRoot(RedmineKeys.ISSUE)]
    public class IssueChild 
        : Identifiable<IssueChild>
        , System.Xml.Serialization.IXmlSerializable
        , System.IEquatable<IssueChild>
        , System.ICloneable
    {
        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.TRACKER)]
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.SUBJECT)]
        public string Subject { get; set; }

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
            Id = System.Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID));
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
                    case RedmineKeys.TRACKER: Tracker = new IdentifiableName(reader); break;

                    case RedmineKeys.SUBJECT: Subject = reader.ReadElementContentAsString(); break;

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
        /// <returns></returns>
        public object Clone()
        {
            IssueChild issueChild = new IssueChild { Subject = Subject, Tracker = Tracker };
            return issueChild;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IssueChild other)
        {
            if (other == null) return false;
            return (Id == other.Id && Tracker == other.Tracker && Subject == other.Subject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 13;
                hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Tracker, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Subject, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[IssueChild: {0}, Tracker={1}, Subject={2}]", base.ToString(), Tracker, Subject);
        }
    }
}
