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

namespace pesta.Data
{
    [DataContract(Namespace = Constants.osNameSpace)]
    public class Address
    {
        public Address()
        {
            
        }
        public Address(String formatted)
        {
            this.formatted = formatted;
        }

        public Address(String country, double? latitude, double? longitude, String locality, String postalCode,
                       String region, String streetAddress, String type)
        {
            this.country = country;
            this.latitude = latitude;
            this.longitude = longitude;
            this.locality = locality;
            this.postalCode = postalCode;
            this.region = region;
            this.streetAddress = streetAddress;
            this.type = type;
        }

        [DataMember(EmitDefaultValue = false)] public String country { get; set; }
        [DataMember(EmitDefaultValue = false)] public double? latitude { get; set; }
        [DataMember(EmitDefaultValue = false)] public double? longitude { get; set; }
        [DataMember(EmitDefaultValue = false)] public String locality { get; set; }
        [DataMember(EmitDefaultValue = false)] public String postalCode { get; set; }
        [DataMember(EmitDefaultValue = false)] public String region { get; set; }
        [DataMember(EmitDefaultValue = false)] public String streetAddress { get; set; }
        [DataMember(EmitDefaultValue = false)] public String type { get; set; }
        [DataMember(EmitDefaultValue = false)] public String formatted { get; set; }
        [DataMember(EmitDefaultValue = false)] public bool primary { get; set; }

    }
}