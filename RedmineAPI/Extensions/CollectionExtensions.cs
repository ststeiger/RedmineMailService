﻿/*
   Copyright 2011 - 2016 Adrian Popescu

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
    public static class CollectionExtensions
    {
        /// <summary>
        /// Clones the specified list to clone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone">The list to clone.</param>
        /// <returns></returns>
        public static System.Collections.Generic.IList<T> Clone<T>(
            this System.Collections.Generic.IList<T> listToClone) where T : System.ICloneable
        {
            if (listToClone == null)
                return null;

            System.Collections.Generic.IList<T> clonedList = new System.Collections.Generic.List<T>();

            foreach (T item in listToClone)
                clonedList.Add((T)item.Clone());

            return clonedList;
        }

        /// <summary>
        /// Equalses the specified list to compare.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="listToCompare">The list to compare.</param>
        /// <returns></returns>
        public static bool Equals<T>(this System.Collections.Generic.IList<T> list
            , System.Collections.Generic.IList<T> listToCompare) where T : class
        {
            if (listToCompare == null)
                return false;

            if (list.Count != listToCompare.Count)
                return false;

            int index = 0;
            while (index < list.Count && (list[index] as T).Equals(listToCompare[index] as T))
                index++;

            return index == list.Count;
        }
    }
}