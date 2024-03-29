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


namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [System.Xml.Serialization.XmlRoot(RedmineKeys.VALUE)]
    public class CustomFieldValue : System.IEquatable<CustomFieldValue>, System.ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlText]
        public string Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CustomFieldValue other)
        {
            return Info.Equals(other.Info);
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
            return Equals(obj as CustomFieldValue);
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
                hashCode = HashCodeHelper.GetHashCode(Info, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[CustomFieldValue: Info={0}]", Info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            CustomFieldValue customFieldValue = new CustomFieldValue { Info = Info };
            return customFieldValue;
        }
    }
}