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
using Pesta.Engine.protocol.conversion;
using Pesta.Utilities;

namespace Pesta.Engine.social.model
{
    [DataContract(Namespace = BeanConverter.osNameSpace)]
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

        /**
       * The fields that represent the address object in json form.
       */
        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field(int key, string value)
                : base(key, value)
            {

            }
            public static readonly Field COUNTRY = new Field(1, "country");
            /** the field name for latitude. */
            public static readonly Field LATITUDE = new Field(2, "latitude");
            /** the field name for locality. */
            public static readonly Field LOCALITY = new Field(3, "locality");
            /** the field name for longitude. */
            public static readonly Field LONGITUDE = new Field(4, "longitude");
            /** the field name for postalCode. */
            public static readonly Field POSTAL_CODE = new Field(5, "postalCode");
            /** the field name for region. */
            public static readonly Field REGION = new Field(6, "region");
            /** the feild name for streetAddress this field may be multiple lines. */
            public static readonly Field STREET_ADDRESS = new Field(7, "streetAddress");
            /** the field name for type. */
            public static readonly Field TYPE = new Field(8, "type");
            /** the field name for formatted. */
            public static readonly Field FORMATTED = new Field(9, "formatted");
            /** the field name for primary. */
            public static readonly Field PRIMARY = new Field(10, "primary");

            public static Field GetByValue(string value)
            {
                return GetBaseByValue(value);
            }

            public override String ToString()
            {
                return Value;
            }
        }
    } 
}
