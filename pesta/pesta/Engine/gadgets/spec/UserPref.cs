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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Pesta.Engine.common.xml;
using Pesta.Engine.gadgets.variables;
using Pesta.Interop;
using Pesta.Utilities;

namespace Pesta.Engine.gadgets.spec
{
    /// <summary>
    /// Summary description for UserPref
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class UserPref
    {
        /**
        * UserPref@name
        * Message bundles
        */
        private String name;
        public String getName()
        {
            return name;
        }

        /**
        * UserPref@display_name
        * Message bundles
        */
        private String displayName;
        public String getDisplayName()
        {
            return displayName;
        }

        /**
        * UserPref@default_value
        * Message bundles
        */
        private String defaultValue;
        public String getDefaultValue()
        {
            return defaultValue;
        }

        /**
        * UserPref@required
        */
        private readonly bool required;
        public bool getRequired()
        {
            return required;
        }

        /**
        * UserPref@datatype
        */
        private readonly DataType dataType;
        public DataType getDataType()
        {
            return dataType;
        }

        /**
        * UserPref.EnumValue
        * Collapsed so that EnumValue@value is the key and EnumValue@display_value
        * is the value. If display_value is not present, value will be used.
        * Message bundles are substituted into display_value, but not value.
        */
        private Dictionary<String, String> enumValues;
        public Dictionary<String, String> getEnumValues()
        {
            return enumValues;
        }


        /**
         * UserPref.EnumValue (ordered)
         * Useful for rendering ordered lists of user prefs with enum type.
         */
        private List<EnumValuePair> orderedEnumValues;
        public List<EnumValuePair> getOrderedEnumValues()
        {
            return orderedEnumValues;
        }


        /**
        * Performs substitutions on the pref. See field comments for details on what
        * is substituted.
        *
        * @param substituter
        * @return The substituted pref.
        */
        public UserPref substitute(Substitutions substituter)
        {
            UserPref pref = new UserPref(this);
            Substitutions.Type type = Substitutions.Type.MESSAGE;
            pref.displayName = substituter.substituteString(type, displayName);
            pref.defaultValue = substituter.substituteString(type, defaultValue);
            if (enumValues.Count == 0)
            {
                pref.enumValues = new Dictionary<string, string>();
            }
            else
            {
                Dictionary<String, String> values
                    = new Dictionary<String, String>(enumValues.Count);
                foreach (var entry in enumValues)
                {
                    values.Add(entry.Key,
                               substituter.substituteString(type, entry.Value));
                }
                pref.enumValues = values;
            }
            if (orderedEnumValues.Count == 0)
            {
                pref.orderedEnumValues = new List<EnumValuePair>();
            }
            else 
            {
                List<EnumValuePair> orderedValues
                    = new List<EnumValuePair>();
                foreach(EnumValuePair evp in orderedEnumValues) 
                {
                    orderedValues.Add(new EnumValuePair(evp.getValue(),
                                                        substituter.substituteString(type, evp.getDisplayValue())));
                }
                pref.orderedEnumValues = orderedValues;
            }
            return pref;
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<UserPref name=\"")
                .Append(name)
                .Append("\" display_name=\"")
                .Append(displayName)
                .Append("\" default_value=\"")
                .Append(defaultValue)
                .Append("\" required=\"")
                .Append(required)
                .Append("\" datatype=\"")
                .Append(dataType.ToString().ToLower())
                .Append('\"');
            if (enumValues.Count == 0)
            {
                buf.Append("/>");
            }
            else
            {
                buf.Append('\n');
                foreach (var entry in enumValues)
                {
                    buf.Append("<EnumValue value=\"")
                        .Append(entry.Key)
                        .Append("\" value=\"")
                        .Append("\" display_value=\"")
                        .Append(entry.Value)
                        .Append("\"/>\n");
                }
                buf.Append("</UserPref>");
            }
            return buf.ToString();
        }

        /**
        * @param element
        * @throws SpecParserException
        */
        public UserPref(XmlElement element)
        {
            String name = XmlUtil.getAttribute(element, "name");
            if (name == null)
            {
                throw new SpecParserException("UserPref@name is required.");
            }
            this.name = name;

            displayName = XmlUtil.getAttribute(element, "display_name", name);
            defaultValue = XmlUtil.getAttribute(element, "default_value", "");
            required = XmlUtil.getBoolAttribute(element, "required");

            String dataType = XmlUtil.getAttribute(element, "datatype", "string");
            this.dataType = DataType.Parse(dataType);

            XmlNodeList children = element.GetElementsByTagName("EnumValue");
            if (children.Count > 0)
            {
                Dictionary<String, String> enumValues = new Dictionary<String, String>();
                List<EnumValuePair> orderedEnumValues = new List<EnumValuePair>();

                for (int i = 0, j = children.Count; i < j; ++i)
                {
                    XmlElement child = (XmlElement)children.Item(i);
                    String value = XmlUtil.getAttribute(child, "value");
                    if (value == null)
                    {
                        throw new SpecParserException("EnumValue@value is required.");
                    }
                    String displayValue
                        = XmlUtil.getAttribute(child, "display_value", value);
                    enumValues.Add(value, displayValue);
                    orderedEnumValues.Add(new EnumValuePair(value, displayValue));
                }
                this.enumValues = enumValues;
                this.orderedEnumValues = orderedEnumValues;

            }
            else
            {
                this.enumValues = new Dictionary<string, string>();
                this.orderedEnumValues = new List<EnumValuePair>();
            }
        }

        /**
        * Produces a UserPref suitable for substitute()
        * @param userPref
        */
        private UserPref(UserPref userPref)
        {
            name = userPref.name;
            dataType = userPref.dataType;
            required = userPref.required;
        }

        /**
        * Possible values for UserPref@datatype
        */
        public class DataType : EnumBaseType<DataType>
        {
            /// <summary>
            /// Initializes a new instance of the DataType class.
            /// </summary>
            public DataType(int key, string value)
                : base(key, value)
            {
            }
            public static readonly DataType STRING = new DataType(1, "STRING");
            public static readonly DataType HIDDEN = new DataType(2, "HIDDEN");
            public static readonly DataType BOOL = new DataType(3, "BOOL");
            public static readonly DataType ENUM = new DataType(4, "ENUM");
            public static readonly DataType LIST = new DataType(5, "LIST");
            public static readonly DataType NUMBER = new DataType(6, "NUMBER");

            /**
            * Parses a data type from the input string.
            *
            * @param value
            * @return The data type of the given value.
            */
            public static DataType Parse(String value)
            {
                foreach (DataType type in DataType.GetBaseValues())
                {
                    if (type.ToString().ToLower().Equals(value))
                    {
                        return type;
                    }
                }
                return STRING;
            }
        }

        /**
       * Simple data structure representing a value/displayValue pair
       * for UserPref enums. Value is EnumValue@value, and DisplayValue is EnumValue@displayValue.
       */
        public class EnumValuePair 
        {
            private readonly String value;
            private readonly String displayValue;

            public EnumValuePair(String value, String displayValue)
            {
                this.value = value;
                this.displayValue = displayValue;
            }

            public String getValue() 
            {
                return value;
            }

            public String getDisplayValue()
            {
                return displayValue;
            }
        }
    }
}