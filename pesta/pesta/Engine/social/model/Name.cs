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
using Pesta.Engine.social.core.model;
using Pesta.Interop;

namespace Pesta.Engine.social.model
{
    /// <summary>
    /// Summary description for Name
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(NameImpl))]
    public abstract class Name
    {
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
            public static readonly Field ADDITIONAL_NAME = new Field(1, "additionalName");
            public static readonly Field FAMILY_NAME = new Field(2, "familyName");
            public static readonly Field GIVEN_NAME = new Field(3, "givenName");
            public static readonly Field HONORIFIC_PREFIX = new Field(4, "honorificPrefix");
            public static readonly Field HONORIFIC_SUFFIX = new Field(5, "honorificSuffix");
            public static readonly Field FORMATTED = new Field(6, "formatted");

            private readonly String jsonString;

            private Field(String jsonString)
            {
                this.jsonString = jsonString;
            }

            public override String ToString()
            {
                return this.jsonString;
            }
        }

        public abstract String getFormatted();

        public abstract void setFormatted(String formatted);

        public abstract String getAdditionalName();

        public abstract void setAdditionalName(String additionalName);

        public abstract String getFamilyName();

        public abstract void setFamilyName(String familyName);

        public abstract String getGivenName();

        public abstract void setGivenName(String givenName);

        public abstract String getHonorificPrefix();

        public abstract void setHonorificPrefix(String honorificPrefix);

        public abstract String getHonorificSuffix();

        public abstract void setHonorificSuffix(String honorificSuffix);
    } 
}
