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
    public class Url : ListField
    {
        public new class Field : EnumBaseType<Field>
        {
            public Field(int key, string value)
                : base(key, value)
            {
            }
            /** the name of the value field. */
            public static readonly Field VALUE = new Field(1, "value");
            /** the name of the linkText field. */
            public static readonly Field LINK_TEXT = new Field(2, "linkText");
            /** the name of the type field. */
            public static readonly Field TYPE = new Field(3, "type");

            public static Field GetByValue(string value)
            {
                return GetBaseByValue(value);
            }

            public override String ToString()
            {
                return Value;
            }
        }

        [DataMember]
        private String linkText;

        public Url()
        {
            
        }
        public Url(String value, String linkText, String type)
            : base(type, value)
        {
            this.linkText = linkText;
        }
    } 
}
