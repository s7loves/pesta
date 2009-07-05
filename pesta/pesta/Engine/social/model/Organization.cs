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
    public class Organization
    {
        public Organization()
        {
            
        }
        public Organization(Address address, String description, DateTime? endDate, String field, String name,
            String salary, DateTime? startDate, String subField, String title, String webpage, Organization.Type type)
        {
            this.address = address;
            this.description = description;
            this.endDate = endDate;
            this.field = field;
            this.name = name;
            this.salary = salary;
            this.startDate = startDate;
            this.subField = subField;
            this.title = title;
            this.webpage = webpage;
            this.type = type.ToString();
        }
        [DataMember(EmitDefaultValue = false)] public Address address { get; set; }
        [DataMember(EmitDefaultValue = false)] public String description { get; set; }

        public DateTime? endDate { get; set; }
        [DataMember(Name = "endDate")]
        private string _endDate
        {
            get
            {
                if (endDate.HasValue)
                {
                    return DateUtil.ToString(endDate.Value.ToUniversalTime());
                }
                return null;
            }
            set
            {
                endDate = DateUtil.Parse(value);
            }
        }
        
        [DataMember(EmitDefaultValue = false)] public String field { get; set; }
        [DataMember(EmitDefaultValue = false)] public String name { get; set; }
        [DataMember(EmitDefaultValue = false)] public String salary { get; set; }

        public DateTime? startDate { get; set; }
        [DataMember(Name = "startDate")]
        private string _startDate
        {
            get
            {
                if (startDate.HasValue)
                {
                    return DateUtil.ToString(startDate.Value.ToUniversalTime());
                }
                return null;
            } 
            set
            {
                startDate = DateUtil.Parse(value);
            }
        }
       
        [DataMember(EmitDefaultValue = false)] public String subField { get; set; }
        [DataMember(EmitDefaultValue = false)] public String title { get; set; }
        [DataMember(EmitDefaultValue = false)] public String webpage { get; set; }
        [DataMember(EmitDefaultValue = false)] public String type { get; set; }
        [DataMember(EmitDefaultValue = false)] public bool? primary { get; set; }

        /**
        * Describes a current or past organizational affiliation of this contact. Service Providers that
        * support only a single Company Name and Job Title field should represent them with a single
        * organization element with name and title properties, respectively.
        *
        * see http://code.google.com/apis/opensocial/docs/0.7/reference/opensocial.Organization.Field.html
        *
        */

        /**
        * An Enumberation of field names for Organization.
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

            /** the name of the address field. */
            public static readonly Field ADDRESS = new Field(1, "address");
            /** the name of the description field. */
            public static readonly Field DESCRIPTION = new Field(2, "description");
            /** the name of the endDate field. */
            public static readonly Field END_DATE = new Field(3, "endDate");
            /** the name of the field field. */
            public static readonly Field FIELD = new Field(4, "field");
            /** the name of the name field. */
            public static readonly Field NAME = new Field(5, "name");
            /** the name of the salary field. */
            public static readonly Field SALARY = new Field(6, "salary");
            /** the name of the startDate field. */
            public static readonly Field START_DATE = new Field(7, "startDate");
            /** the name of the subField field. */
            public static readonly Field SUB_FIELD = new Field(8, "subField");
            /** the name of the title field. */
            public static readonly Field TITLE = new Field(9, "title");
            /** the name of the webpage field. */
            public static readonly Field WEBPAGE = new Field(10, "webpage");
            /**
            * the name of the type field, Should have the value of "job" or "school" to be put in the right
            * js fields.
            */
            public static readonly Field TYPE = new Field(11, "type");
            /** the name of the primary field. */
            public static readonly Field PRIMARY = new Field(12, "primary");

            public static Field GetByValue(string value)
            {
                return GetBaseByValue(value);
            }

            public override String ToString()
            {
                return Value;
            }
        }

        public enum Type
        {
            Job,
            School
        }
    } 
}
