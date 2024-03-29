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


namespace Redmine.Net.Api.Types
{


    /// <summary>
    /// the module name: boards, calendar, documents, files, gantt, issue_tracking, news, repository, time_tracking, wiki.
    /// </summary>
    [System.Xml.Serialization.XmlRoot(RedmineKeys.ENABLED_MODULE)]
    public class ProjectEnabledModule : IdentifiableName, IValue
    {

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get { return Name; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ProjectEnabledModule: {0}]", base.ToString());
        }


    }


}