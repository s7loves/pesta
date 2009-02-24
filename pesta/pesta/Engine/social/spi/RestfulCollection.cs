#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations under the License.
 */
#endregion

using System.Collections;
using System.Collections.Generic;

namespace Pesta.Engine.social.spi
{
    /// <summary>
    /// Summary description for RestfulCollection
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class RestfulCollection<T> : IRestfulCollection
    {
        private List<T> entry;
        private int startIndex;
        private int totalResults;

        private bool filtered = true;
        private bool sorted = true;
        private bool updatedSince = true;

        public RestfulCollection(List<T> entry)
            : this(entry, 0, entry.Count)
        {
        }

        public RestfulCollection(T entry)
            : this(new List<T> { entry }, 0, 1)
        {
        }

        public RestfulCollection(List<T> entry, int startIndex, int totalResults)
        {
            this.entry = entry;
            this.startIndex = startIndex;
            this.totalResults = totalResults;
        }

        public IList getEntry()
        {
            return entry;
        }

        public void setEntry(List<T> _entry)
        {
            entry = _entry;
        }

        public int getStartIndex()
        {
            return startIndex;
        }

        public void setStartIndex(int _startIndex)
        {
            startIndex = _startIndex;
        }

        public int getTotalResults()
        {
            return totalResults;
        }

        public void setTotalResults(int _totalResults)
        {
            totalResults = _totalResults;
        }

        public bool isFiltered()
        {
            return filtered;
        }

        public void setFiltered(bool _filtered)
        {
            filtered = _filtered;
        }

        public bool isSorted()
        {
            return sorted;
        }

        public void setSorted(bool _sorted)
        {
            sorted = _sorted;
        }

        public bool isUpdatedSince()
        {
            return updatedSince;
        }

        public void setUpdatedSince(bool _updatedSince)
        {
            updatedSince = _updatedSince;
        }
    }
}