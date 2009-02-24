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
        public override String getContentType()
        {
            return "application/json";
        }

        /**
         * Convert the passed in object to a string.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public override String convertToString(Object pojo)
        {
            return convertToJson(pojo).ToString();
        }

        public override String convertToString(Object pojo, RequestItem request)
        {
            return convertToJson(pojo).ToString();
        }
        /**
         * Convert the passed in object to a json object.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public Object convertToJson(Object pojo)
        {
            try
            {
                return translateObjectToJson(pojo);
            }
            catch (JsonException e)
            {
                throw new Exception("Could not translate " + pojo + " to json", e);
            }
        }

        public override Object convertToObject(String json, Type className)
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
                object[] attrs = className.GetCustomAttributes(typeof(ImplementedByAttribute), false);
                if (attrs == null)
                {
                    throw new Exception(errorMessage);
                }
                className = ((ImplementedByAttribute)attrs[0]).Implementer;
            }

            try
            {
                Object pojo = Activator.CreateInstance(className);
                return convertToObj(json, pojo);
            }
            catch (JsonException e)
            {
                throw new Exception(errorMessage, e);
            }
            catch (Exception e)
            {
                throw new Exception(errorMessage, e);
            }
        }

        public Object convertToObj(String json, Object pojo)
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

                JsonObject JsonObject = (JsonObject)JsonConvert.Import(json);
                foreach (String key in JsonObject.Names)
                {
                    Object value = convertToObject(JsonObject[key].ToString(), mapValueClass);
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
                JsonObject JsonObject = JsonConvert.Import(json) as JsonObject;
                List<MethodPair> methods;
                if (!SETTER_METHODS.TryGetValue(pojo.GetType(), out methods))
                {
                    methods = getMatchingMethods(pojo.GetType(), SETTER_PREFIX);
                    if (!SETTER_METHODS.ContainsKey(pojo.GetType()))
                    {
                        SETTER_METHODS.Add(pojo.GetType(), methods);
                    }
                }

                foreach (MethodPair setter in methods)
                {
                    if (JsonObject.Contains(setter.fieldName))
                    {
                        callSetterWithValue(pojo, setter.method, JsonObject, setter.fieldName);
                    }
                }
            }
            return pojo;
        }

        private void callSetterWithValue(object pojo, MethodInfo method, JsonObject JsonObject, String fieldName)
        {
            Object value = null;
            Type expectedType = method.GetParameters()[0].ParameterType;
            if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                expectedType = expectedType.GetGenericArguments()[0];
            }

            if (!JsonObject.Contains(fieldName))
            {
                // Skip
            }
            else if (expectedType.GetInterface("IList") != null)
            {
                Type type = expectedType.GetGenericArguments()[0];
                Type listElementClass = type;
                Type rawType = typeof(List<>).MakeGenericType(new[] { type });

                Object list = Activator.CreateInstance(rawType);
                JsonArray JsonArray = (JsonArray)JsonObject[fieldName];
                for (int i = 0; i < JsonArray.Length; i++)
                {
                    if (typeof(Enums<>).IsAssignableFrom(rawType))
                    {
                        ((IList)list).Add(convertEnum(listElementClass, JsonArray.GetObject(i)));
                    }
                    else
                    {
                        ((IList)list).Add(convertToObject(JsonArray.GetString(i), listElementClass));
                    }
                }
                value = list;

            }
            else if (expectedType.GetInterface("IDictionary") != null)
            {
                Type[] types = expectedType.GetGenericArguments();
                Type valueClass = types[1];

                Dictionary<String, String> map = new Dictionary<String, String>();
                JsonObject jsonMap = JsonObject.getJSONObject(fieldName);
                foreach (String keyName in jsonMap.Names)
                {
                    map.Add(keyName, convertToObject(jsonMap[keyName].ToString(), valueClass) as String);
                }

                value = map;

            }
            else if (typeof(Enums<>).IsAssignableFrom(expectedType))
            {
                value = convertEnum(method.GetGenericArguments()[0], JsonObject[fieldName] as JsonObject);
            }
            else if (expectedType.IsEnum)
            {
                if (JsonObject[fieldName] != null)
                {
                    foreach (FieldInfo v in expectedType.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        if (v.Name.Equals(JsonObject[fieldName]))
                        {
                            value = Enum.Parse(expectedType, v.GetRawConstantValue().ToString());
                            break;
                        }
                    }
                    if (value == null)
                    {
                        throw new ArgumentException("No enum value  '" + JsonObject[fieldName]
                                                    + "' in " + expectedType.Name);
                    }
                }
            }
            else if (expectedType.Equals(typeof(String)))
            {
                value = JsonObject[fieldName].ToString();
            }
            else if (expectedType.Equals(typeof(DateTime)))
            {
                // Use JODA ISO parsing for the conversion
                value = DateTime.Parse(JsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(long)))
            {
                value = long.Parse(JsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(int)))
            {
                value = int.Parse(JsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(bool)))
            {
                value = bool.Parse(JsonObject[fieldName].ToString());
            }
            else if (expectedType.Equals(typeof(float)))
            {
                value = float.Parse(JsonObject[fieldName].ToString());
            }
            else
            {
                // Assume its an injected type
                value = convertToObject(JsonObject[fieldName].ToString(), expectedType);
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