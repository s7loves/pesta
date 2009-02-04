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
    /// Summary description for UrlImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class UrlImpl : ListFieldImpl, Url
    {
        /**
    * An enumeration of the field names used in Url objects.
    */

        public new class Field : EnumBaseType<Field>
        {
            public Field(int key, string value)
                : base(key, value)
            {

            }
            /** the name of the value field. */
            public static readonly Field VALUE = new Field("value");
            /** the name of the linkText field. */
            public static readonly Field LINK_TEXT = new Field("linkText");
            /** the name of the type field. */
            public static readonly Field TYPE = new Field("type");

            /**
             * The name of this field
             */
            private readonly String jsonString;

            /**
             * Construct a new field based on a name.
             *
             * @param jsonString the name of the field
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

        private String linkText;

        public UrlImpl() { }

        public UrlImpl(String value, String linkText, String type)
            : base(type, value)
        {
            this.linkText = linkText;
        }

        public String getLinkText()
        {
            return linkText;
        }

        public void setLinkText(String linkText)
        {
            this.linkText = linkText;
        }
    }
}