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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace pesta.Data
{
    [XmlRoot(ElementName = "name", Namespace = Constants.osNameSpace)]
    [DataContract(Name = "name", Namespace = Constants.osNameSpace)]
    public class Name
    {
        [DataMember(EmitDefaultValue = false)] 
        public String additionalName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String familyName { get; set; } // last name
        [DataMember(EmitDefaultValue = false)]
        public String givenName { get; set; }  // first name
        [DataMember(EmitDefaultValue = false)]
        public String honorificPrefix { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String honorificSuffix { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String formatted { get; set; }

        public Name()
        {
            
        }
        public Name(string firstName, string lastName)
        {
            formatted = firstName + " " + lastName;
            familyName = lastName;
            givenName = firstName;
        }
    }
}