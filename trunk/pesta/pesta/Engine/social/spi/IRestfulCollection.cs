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

using System.Runtime.Serialization;
using Pesta.Engine.protocol.conversion;

namespace Pesta.Engine.social.spi
{
    [DataContract(Name = "response", Namespace = BeanConverter.osNameSpace)]
    public abstract class IRestfulCollection
    {
        [DataMember]
        public int startIndex { get; set; }

        [DataMember]
        public int totalResults { get; set; }

        [DataMember]
        public int itemsPerPage { get; set; }

        [DataMember]
        public bool isFiltered { get; set; }

        [DataMember]
        public bool isSorted { get; set; }

        [DataMember]
        public bool isUpdatedSince { get; set; }

        public abstract object getEntry();
    }
}
