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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using pesta.Data.Model.Helpers;

namespace pesta.Data
{
    [XmlRoot(ElementName = "messageCollection", Namespace = Constants.osNameSpace)]
    [DataContract(Name = "messageCollection", Namespace = Constants.osNameSpace)]
    public class MessageCollection
    {
        [DataMember(EmitDefaultValue = false)]
        public String id { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String title { get; set; }
        [DataMember]
        public int total { get; set; }
        [DataMember]
        public int unread { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public long? updated { get; set; }
        [XmlIgnore]
        public bool updatedSpecified { get { return updated.HasValue; } }
        [DataMember(EmitDefaultValue = false)]
        public List<Url> urls { get; set; }
        
        [XmlIgnore]
        public String OUTBOX = "@outbox";
        [XmlIgnore]
        public String ALL = "@all";

        public enum Field 
        {
            [Description("id")]
            ID,
            [Description("title")]
            TITLE,
            [Description("total")]
            TOTAL,
            [Description("unread")]
            UNREAD,
            [Description("updated")]
            UPDATED
        }

        public static readonly List<string> ALL_FIELDS = (from Enum e in Enum.GetValues(typeof (Field))
                                                          select e.ToDescriptionString()).ToList();
    }
}