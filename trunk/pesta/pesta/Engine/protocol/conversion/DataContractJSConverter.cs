#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain [DataMember(EmitDefaultValue = false)] copy of the License at
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using pesta.Data;
using Pesta.Engine.social.spi;

namespace Pesta.Engine.protocol.conversion
{
    public class DataContractJSConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return ConvertToObject(dictionary, type);
        }

        public object ConvertToObject(IDictionary<string, object> dictionary, Type type)
        {
            var obj = Activator.CreateInstance(type);
            foreach (var entry in dictionary)
            {
                var fieldType = type.GetProperty(entry.Key).PropertyType;
                var valueType = entry.Value.GetType();
                if (typeof(IDictionary).IsAssignableFrom(valueType))
                {
                    ConvertToObject((IDictionary<string, object>)entry.Value, fieldType);
                }
                else
                {
                    if (typeof(IList).IsAssignableFrom(fieldType))
                    {
                        Type argType = fieldType.GetGenericArguments()[0];
                        var listObj = Activator.CreateInstance(fieldType);
                        foreach (var val in (IList)entry.Value)
                        {
                            ((IList)listObj).Add(ConvertToObject((IDictionary<string, object>)val, argType));
                        }
                    }
                    else
                    {
                        type.GetProperty(entry.Key).SetValue(obj, entry.Value, null);
                    }
                }
            }
            return obj;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();
            var result = new Dictionary<string, object>();
            foreach (var info in properties)
            {
                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                var attribs = (DataMemberAttribute[])info.GetCustomAttributes(typeof(DataMemberAttribute), false);
                if (attribs.Length == 0)
                {
                    continue;
                }
                string elementName = attribs[0].Name ?? info.Name;
                
                var valueType = value.GetType();
                // following needed, otherwise enums are serialised into numbers
                if (valueType.IsEnum)
                {
                    value = value.ToString();
                }
                // handle scenario where only one entry
                else if (typeof(IRestfulCollection).IsAssignableFrom(type) &&
                            typeof(IList).IsAssignableFrom(valueType))
                {
                    var entry = ((IList) value);

                    if (entry.Count == 1)
                    {
                        result.Add(elementName, entry[0]);
                        continue;
                    }
                }
                result.Add(elementName, value);
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new Type[]
                           {
                               typeof(Person), 
                               typeof(Account), 
                               typeof(Activity), 
                               typeof(Address), 
                               typeof(BodyType), 
                               typeof(ListField),
                               typeof(MediaItem),
                               typeof(Message),
                               typeof(MessageCollection),
                               typeof(Name),
                               typeof(Organization),
                               typeof(Url),
                               typeof(RestfulCollection<Person>),
                               typeof(RestfulCollection<Activity>),
                           };
            }
        }
    }
}
