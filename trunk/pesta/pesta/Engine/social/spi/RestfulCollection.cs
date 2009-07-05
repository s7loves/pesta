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
using System.Runtime.Serialization;
using Pesta.Engine.protocol.conversion;

namespace Pesta.Engine.social.spi
{
    [DataContract(Name = "response", Namespace = BeanConverter.osNameSpace)]
    public class RestfulCollection<T> : IRestfulCollection
    {
        [DataMember]
        public List<T> entry { get; set; }

        public RestfulCollection()
            : this(new List<T>(), 0, 0)
        {
        }
        public RestfulCollection(ICollection<T> entry)
            : this(entry, 0, entry.Count)
        {
        }

        public RestfulCollection(T entry)
            : this(new List<T> { entry }, 0, 1)
        {
        }

        public RestfulCollection(IEnumerable<T> entry, int startIndex, int totalResults)
        {
            this.entry = new List<T>(entry);
            this.startIndex = startIndex;
            this.totalResults = totalResults;
            isFiltered = true;
            isSorted = true;
            isUpdatedSince = true;
        }

        public override object getEntry()
        {
            return entry;
        }
    }
}