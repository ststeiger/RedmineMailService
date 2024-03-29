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
    /// Only the roles can be updated, the project and the user of a membership are read-only.
    /// </summary>
    [System.Xml.Serialization.XmlRoot(RedmineKeys.MEMBERSHIP)]
    public class Membership 
        : Identifiable<Membership>
        , System.IEquatable<Membership>
        , System.Xml.Serialization.IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [System.Xml.Serialization.XmlElement(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [System.Xml.Serialization.XmlArray(RedmineKeys.ROLES)]
        [System.Xml.Serialization.XmlArrayItem(RedmineKeys.ROLE)]
        public System.Collections.Generic.List<MembershipRole> Roles { get; set; }

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

                    case RedmineKeys.ROLES: Roles = reader.ReadElementContentAsCollection<MembershipRole>(); break;

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
        public bool Equals(Membership other)
        {
            if (other == null)
                return false;

            return (Id == other.Id && 
                (Project != null ? Project.Equals(other.Project) : other.Project == null) && 
                    (Roles != null ? Roles.Equals<MembershipRole>(other.Roles) : other.Roles == null));
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
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                //hashCode = Utils.GetHashCode(Roles, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Membership: {2}, Project={0}, Roles={1}]", Project, Roles, base.ToString());
        }
    }
}