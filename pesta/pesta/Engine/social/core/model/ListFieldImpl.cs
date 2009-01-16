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
using Pesta.Engine.social.model;
using Pesta.Utilities;

namespace Pesta.Engine.social.core.model
{
    /// <summary>
    /// Summary description for ListFieldImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class ListFieldImpl : ListField
    {
        String type;
        String value;
        bool primary;

        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field()
            {
            }
            public Field(int key, string value)
                : base(key, value)
            {

            }
            /** the field name for value. */
            public static readonly Field VALUE = new Field(1, "value");
            /** the field name for type. */
            public static readonly Field TYPE = new Field(2, "type");
            /** the field name for primary. */
            public static readonly Field PRIMARY = new Field(3, "primary");

            /**
             * The field name that the instance represents.
             */
            private readonly String jsonString;

            /**
             * create a field baseD on the name of an element.
             *
             * @param jsonString the name of the element
             */
            private Field(String jsonString)
            {
                this.jsonString = jsonString;
            }

            public override String ToString()
            {
                return this.jsonString;
            }
        }

        public ListFieldImpl() { }

        public ListFieldImpl(String type, String value)
        {
            this.type = type;
            this.value = value;
        }

        public String getType()
        {
            return type;
        }

        public void setType(String type)
        {
            this.type = type;
        }

        public String getValue()
        {
            return value;
        }

        public void setValue(String value)
        {
            this.value = value;
        }

        public bool getPrimary()
        {
            return primary;
        }

        public void setPrimary(bool primary)
        {
            this.primary = primary;
        }
    }
}