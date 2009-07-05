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
    public class BodyType
    {
        [DataMember(EmitDefaultValue = false)] public String build { get; set; }
        [DataMember(EmitDefaultValue = false)] public String eyeColor { get; set; }
        [DataMember(EmitDefaultValue = false)] public String hairColor { get; set; }
        [DataMember(EmitDefaultValue = false)] public float? height { get; set; }
        [DataMember(EmitDefaultValue = false)] public float? weight { get; set; }

        /**
        * The fields that represent the person object in serialized form.
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
            /** the field name for build. */
            public static readonly Field BUILD = new Field(1, "build");
            /** the field name for build. */
            public static readonly Field EYE_COLOR = new Field(2, "eyeColor");
            /** the field name for hairColor. */
            public static readonly Field HAIR_COLOR = new Field(3, "hairColor");
            /** the field name for height. */
            public static readonly Field HEIGHT = new Field(4, "height");
            /** the field name for weight. */
            public static readonly Field WEIGHT = new Field(5, "weight");

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
