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

using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Extensions;


namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [System.Xml.Serialization.XmlRoot(RedmineKeys.CHANGESET)]
    public class ChangeSet : System.Xml.Serialization.IXmlSerializable, System.IEquatable<ChangeSet>
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute(RedmineKeys.REVISION)]
        public int Revision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.USER)]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.COMMENTS)]
        public string Comments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement(RedmineKeys.COMMITTED_ON, IsNullable = true)]
        public System.DateTime? CommittedOn { get; set; }

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

                Revision = reader.ReadAttributeAsInt(RedmineKeys.REVISION);

                switch (reader.Name)
                {
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.COMMITTED_ON: CommittedOn = reader.ReadElementContentAsNullableDateTime(); break;

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
        public bool Equals(ChangeSet other)
        {
            if (other == null) return false;

            return Revision == other.Revision
                && User == other.User
                && Comments == other.Comments
                && CommittedOn == other.CommittedOn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as ChangeSet);
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
                hashCode = HashCodeHelper.GetHashCode(Revision, hashCode);
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CommittedOn, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Revision: {0}, User: '{1}', CommitedOn: {2}, Comments: '{3}'", Revision, User, CommittedOn, Comments);
        }
    }
}