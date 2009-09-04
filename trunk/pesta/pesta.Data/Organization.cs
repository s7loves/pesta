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
using pesta.Data.Model.Helpers;

namespace pesta.Data
{
    [DataContract(Namespace = Constants.osNameSpace)]
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
        public string _endDate
        {
            get
            {
                if (endDate.HasValue)
                {
                    return DateHelper.ToString(endDate.Value.ToUniversalTime());
                }
                return null;
            }
            set
            {
                endDate = DateHelper.Parse(value);
            }
        }
        
        [DataMember(EmitDefaultValue = false)] public String field { get; set; }
        [DataMember(EmitDefaultValue = false)] public String name { get; set; }
        [DataMember(EmitDefaultValue = false)] public String salary { get; set; }

        public DateTime? startDate { get; set; }
        [DataMember(Name = "startDate")]
        public string _startDate
        {
            get
            {
                if (startDate.HasValue)
                {
                    return DateHelper.ToString(startDate.Value.ToUniversalTime());
                }
                return null;
            } 
            set
            {
                startDate = DateHelper.Parse(value);
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

        public enum Type
        {
            Job,
            School
        }
    }
}