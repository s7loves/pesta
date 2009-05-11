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
using System.Collections;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Reflection;
using Pesta.Engine.social.core.model;
using Pesta.Engine.social.model;
using Pesta.Utilities;
using pestaServer.Models.social.service;

namespace pestaServer.Models.social.core.util
{
    /// <summary>
    /// Summary description for BeanJsonConverter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BeanJsonConverter : BeanConverter
    {
        public override String GetContentType()
        {
            return "application/json";
        }

        /**
         * Convert the passed in object to a string.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public override String ConvertToString(Object pojo)
        {
            return ConvertToJson(pojo).ToString();
        }

        public override String ConvertToString(Object pojo, RequestItem request)
        {
            return ConvertToJson(pojo).ToString();
        }
        /**
         * Convert the passed in object to a json object.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public static Object ConvertToJson(Object pojo)
        {
            try
            {
                return TranslateObjectToJson(pojo);
            }
            catch (JsonException e)
            {
                throw new Exception("Could not translate " + pojo + " to json", e);
            }
        }

        public override Object ConvertToObject(String json, Type className)
        {
            // ignore strings
            if (className == typeof(String))
            {
                return json;
            }

            String errorMessage = "Could not convert " + json + " to " + className;

            // if abstract class or interface, try go get implementer
            if (className.IsInterface || className.IsAbstract)
            {
                object[] attrs = className.GetCustomAttributes(typeof(ImplementedBy), false);
                if (attrs == null)
                {
                    throw new Exception(errorMessage);
                }
                className = ((ImplementedBy)attrs[0]).Implementer;
            }

            try
            {
                Object pojo = Activator.CreateInstance(className);
                return ConvertToObj(json, pojo);
            }
            catch (Exception e)
            {
                throw new Exception(errorMessage, e);
            }
        }

        private Object ConvertToObj(String json, Object pojo)
        {
            if (pojo.GetType() == typeof(String))
            {
                pojo = json;
            }
            else if (pojo.GetType().GetInterface("IDictionary") != null)
            {
                // TODO: Figure out how to get the actual generic type for the
                // second Map parameter. Right now we are hardcoding to String
                Type mapValueClass = typeof(String);

                JsonObject jsonObject = (JsonObject)JsonConvert.Import(json);
                foreach (String key in jsonObject.Names)
                {
                    Object value = ConvertToObject(jsonObject[key].ToString(), mapValueClass);
                    ((Dictionary<String, String>)pojo).Add(key, value.ToString());
                }
            }
            else if (pojo.GetType().GetInterface("IList") != null)
            {
                JsonArray array = new JsonArray(json);
                for (int i = 0; i < array.Count; i++)
                {
                    ((List<Object>)pojo).Add(array[i]);
                }

            }
            else
            {
                JsonObject jsonObject = JsonConvert.Import(json) as JsonObject;
                List<MethodPair> methods;
                if (!SETTER_METHODS.TryGetValue(pojo.GetType(), out methods))
                {
                    methods = GetMatchingMethods(pojo.GetType(), SETTER_PREFIX);
                    if (!SETTER_METHODS.ContainsKey(pojo.GetType()))
                    {
                        SETTER_METHODS.Add(pojo.GetType(), methods);
                    }
                }

                foreach (MethodPair setter in methods)
                {
                    if (jsonObject != null && jsonObject.Contains(setter.FieldName))
                    {
                        CallSetterWithValue(pojo, setter.Method, jsonObject, setter.FieldName);
                    }
                }
            }
            return pojo;
        }

        private void CallSetterWithValue(object pojo, MethodInfo method, JsonObject jsonObject, String fieldName)
        {
            Object value = null;
            Type expectedType = method.GetParameters()[0].ParameterType;
            if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                expectedType = expectedType.GetGenericArguments()[0];
            }

            if (!jsonObject.Contains(fieldName))
            {
                // Skip
            }
            else if (expectedType.GetInterface("IList") != null)
            {
                Type type = expectedType.GetGenericArguments()[0];
                Type listElementClass = type;
                Type rawType = typeof(List<>).MakeGenericType(new[] { type });

                Object list = Activator.CreateInstance(rawType);
                JsonArray jsonArray = (JsonArray)jsonObject[fieldName];
                for (int i = 0; i < jsonArray.Length; i++)
                {
                    if (typeof(Enums<>).IsAssignableFrom(rawType))
                    {
                        ((IList)list).Add(convertEnum(listElementClass, jsonArray.GetObject(i)));
                    }
                    else
                    {
                        ((IList)list).Add(ConvertToObject(jsonArray.GetString(i), listElementClass));
                    }
                }
                value = list;

            }
            else if (expectedType.GetInterface("IDictionary") != null)
            {
                Type[] types = expectedType.GetGenericArguments();
                Type valueClass = types[1];

                Dictionary<String, String> map = new Dictionary<String, String>();
                JsonObject jsonMap = jsonObject.getJSONObject(fieldName);
                foreach (String keyName in jsonMap.Names)
                {
                    map.Add(keyName, ConvertToObject(jsonMap[keyName].ToString(), valueClass) as String);
                }

                value = map;

            }
            else if (typeof(Enums<>).IsAssignableFrom(expectedType))
            {
                value = convertEnum(method.GetGenericArguments()[0], jsonObject[fieldName] as JsonObject);
            }
            else if (expectedType.IsEnum || 
                expectedType.GetCustomAttributes(typeof(IsJavaEnum), true).Length != 0)
            {
                if (jsonObject[fieldName] != null)
                {
                    foreach (FieldInfo v in expectedType.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        if (string.Compare(v.Name, jsonObject[fieldName].ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            value = expectedType.IsEnum ? Enum.Parse(expectedType, v.GetRawConstantValue().ToString()) : v.GetValue(null);
                            break;
                        }
                    }
                    if (value == null)
                    {
                        throw new ArgumentException("No enum value  '" + jsonObject[fieldName]
                                                    + "' in " + expectedType.Name);
                    }
                }
            }
            else if (expectedType.Equals(typeof(String)))
            {
                value = jsonObject[fieldName].ToString();
            }
            else if (expectedType.Equals(typeof(DateTime)))
            {
                // Use JODA ISO parsing for the conversion
                value = DateTime.Parse(jsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(long)))
            {
                value = long.Parse(jsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(int)))
            {
                value = int.Parse(jsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(bool)))
            {
                value = bool.Parse(jsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(float)))
            {
                value = float.Parse(jsonObject[fieldName].ToString());
            }
            else
            {
                // Assume its an injected type
                value = ConvertToObject(jsonObject[fieldName].ToString(), expectedType);
            }

            if (value != null)
            {
                method.Invoke(pojo, new[] { value });
            }
        }

        private Object convertEnum(Type enumKeyType, JsonObject jsonEnum)
        {
            Object value;
            if (jsonEnum.Contains(Enums<EnumKey>.Field.VALUE.Value))
            {
                EnumKey enumKey = (EnumKey)enumKeyType.GetField(jsonEnum[Enums<EnumKey>.Field.VALUE.Value] as String).GetValue(null);
                value = new EnumImpl<EnumKey>(enumKey, jsonEnum[Enums<EnumKey>.Field.DISPLAY_VALUE.ToString()] as String);
            }
            else
            {
                value = new EnumImpl<EnumKey>(null, jsonEnum[Enums<EnumKey>.Field.DISPLAY_VALUE.ToString()] as String);
            }
            return value;
        }
    }
}