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
using Pesta.Engine.social.service;
using Pesta.Utilities;

namespace Pesta.Engine.social.core.util
{
    /// <summary>
    /// Summary description for BeanJsonConverter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class BeanJsonConverter : BeanConverter
    {
        private static readonly Object[] EMPTY_OBJECT = { };
        private static readonly HashSet<String> EXCLUDED_FIELDS = new HashSet<String>() { "class", "declaringclass" };
        private static readonly String GETTER_PREFIX = "get";
        private static readonly String SETTER_PREFIX = "set";

        // Only compute the filtered getters/setters once per-class
        private static readonly Dictionary<Type, List<MethodPair>> GETTER_METHODS = new Dictionary<Type, List<MethodPair>>();
        private static readonly Dictionary<Type, List<MethodPair>> SETTER_METHODS = new Dictionary<Type, List<MethodPair>>();

        public String getContentType()
        {
            return "application/json";
        }

        /**
         * Convert the passed in object to a string.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public String convertToString(Object pojo)
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

        private Object translateObjectToJson(Object val)
        {
            if (val is Object[])
            {
                JsonArray array = new JsonArray();
                foreach (Object asd in (Object[])val)
                {
                    array.Put(translateObjectToJson(asd));
                }
                return array;
            }

            if (val != null && val.GetType().GetInterface("IList") != null)
            {
                JsonArray list = new JsonArray();
                foreach (Object item in (IList)val)
                {
                    list.Add(translateObjectToJson(item));
                }
                return list;
            }

            if (val != null && val.GetType().GetInterface("IDictionary") != null)
            {
                JsonObject map = new JsonObject();
                IDictionary originalMap = (IDictionary)val;

                foreach (DictionaryEntry item in originalMap)
                {
                    map.Put(item.Key.ToString(), translateObjectToJson(item.Value));
                }
                return map;

            }

            if (val != null && val.GetType().IsEnum)
            {
                return val.ToString();
            }

            if (val is String
                || val is Boolean
                || val is int
                || val is DateTime
                || val is long
                || val is float
                || val is JsonObject
                || val is JsonArray
                || val == null)
            {
                return val;
            }

            return convertMethodsToJson(val);
        }

        /**
         * Convert the object to {@link JsonObject} reading Pojo properties
         *
         * @param pojo The object to convert
         * @return A JsonObject representing this pojo
         */
        private JsonObject convertMethodsToJson(object pojo)
        {
            List<MethodPair> availableGetters;
            if (!GETTER_METHODS.TryGetValue(pojo.GetType(), out availableGetters))
            {
                availableGetters = getMatchingMethods(pojo.GetType(), GETTER_PREFIX);
                if (!GETTER_METHODS.ContainsKey(pojo.GetType()))
                {
                    GETTER_METHODS.Add(pojo.GetType(), availableGetters);
                }

            }

            JsonObject toReturn = new JsonObject();
            foreach (MethodPair getter in availableGetters)
            {
                String errorMessage = "Could not encode the " + getter.method + " method on "
                                      + pojo.GetType().Name;
                try
                {
                    Object val = getter.method.Invoke(pojo, EMPTY_OBJECT);
                    if (val != null && val.ToString() != "")
                    {
                        toReturn.Put(getter.fieldName, translateObjectToJson(val));
                    }
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
            return toReturn;
        }

        private class MethodPair
        {
            public readonly MethodInfo method;
            public readonly String fieldName;

            public MethodPair(MethodInfo method, String fieldName)
            {
                this.method = method;
                this.fieldName = fieldName;
            }
        }

        private List<MethodPair> getMatchingMethods(Type pojo, String prefix)
        {
            List<MethodPair> availableGetters = new List<MethodPair>();

            MethodInfo[] methods = pojo.GetMethods();
            foreach (MethodInfo method in methods)
            {
                String name = method.Name;
                if (!method.Name.StartsWith(prefix))
                {
                    continue;
                }
                int prefixlen = prefix.Length;

                String fieldName = name.Substring(prefixlen, 1).ToLower() +
                                   name.Substring(prefixlen + 1);

                if (EXCLUDED_FIELDS.Contains(fieldName.ToLower()))
                {
                    continue;
                }
                availableGetters.Add(new MethodPair(method, fieldName));
            }
            return availableGetters;
        }

        public Object convertToObject(String json, Type className)
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
                return convertToObject(json, pojo);
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

        public Object convertToObject(String json, Object pojo)
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

                // We only support keys being typed as Strings.
                // Nothing else really makes sense in json.
                Dictionary<String, String> map = new Dictionary<String, String>();
                JsonObject jsonMap = JsonObject.getJSONObject(fieldName);
                foreach (String keyName in jsonMap.Names)
                {
                    map.Add(keyName, convertToObject(jsonMap[keyName] as String, valueClass) as String);
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