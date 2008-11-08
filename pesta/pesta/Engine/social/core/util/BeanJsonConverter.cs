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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Reflection;

/// <summary>
/// Summary description for BeanJsonConverter
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class BeanJsonConverter : BeanConverter
{
    private static readonly Object[] EMPTY_OBJECT = {};
    private static readonly HashSet<String> EXCLUDED_FIELDS = new HashSet<string>()
                    {"class", "declaringclass"};
    private static readonly Regex GETTER = new Regex("^get([a-zA-Z]+)$", RegexOptions.Compiled);
    private static readonly Regex SETTER = new Regex("^set([a-zA-Z]+)$", RegexOptions.Compiled);


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
            foreach(Object asd in (Object[]) val) 
            {
                array.Put(translateObjectToJson(asd));
            }
            return array;
        }
        else if (val != null && val.GetType().GetInterface("IList") != null) 
        {
            JsonArray list = new JsonArray();
            foreach(Object item in (IList)val) 
            {
                list.Add(translateObjectToJson(item));
            }
            return list;
        }
        else if (val != null && val.GetType().GetInterface("IDictionary") != null) 
        {
            JsonObject map = new JsonObject();
            IDictionary originalMap = (IDictionary)val;

            foreach(DictionaryEntry item in originalMap) 
            {
                map.Put(item.Key.ToString(), translateObjectToJson(item.Value));
            }
            return map;

        } 
        else if (val != null && val.GetType().IsEnum) 
        {
            return val.ToString();
        } 
        else if (val is String
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
        List<MethodPair> availableGetters = getMatchingMethods(pojo.GetType(), GETTER);

        JsonObject toReturn = new JsonObject();
        foreach(MethodPair getter in availableGetters) 
        {
            String errorMessage = "Could not encode the " + getter.method + " method on "
            + pojo.GetType().Name;
            try 
            {
                Object val = getter.method.Invoke(pojo, EMPTY_OBJECT);
                if (val != null && val.ToString() != "0" && val.ToString() != "False") 
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
        public MethodInfo method;
        public String fieldName;

        public MethodPair(MethodInfo method, String fieldName) 
        {
            this.method = method;
            this.fieldName = fieldName;
        }
    }

    private List<MethodPair> getMatchingMethods(Type pojo, Regex pattern) 
    {
        List<MethodPair> availableGetters = new List<MethodPair>();

        MethodInfo[] methods = pojo.GetMethods();
        foreach (MethodInfo method in methods) 
        {
            Match matcher = pattern.Match(method.Name);
            if (!matcher.Success) 
            {
                continue;
            }

            String name = matcher.Groups[0].Value;
            String fieldName = name.Substring(3, 1).ToLower() + name.Substring(4);
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
            else
            {
                className = ((ImplementedByAttribute)attrs[0]).Implementer;
            }
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

            JsonObject JsonObject = JsonConvert.Import(json) as JsonObject;
            foreach (String key in JsonObject.Names)
	        {
        		 Object value = convertToObject(JsonObject[key].ToString(), mapValueClass);
                 ((Dictionary<String, String>)pojo).Add(key, value.ToString());
	        }
        }
        else if (pojo.GetType().GetInterface("IList") != null) 
        {
            // TODO: process as a JsonArray
            throw new Exception("We don't support lists as a "
                    + "base json type yet. You can put it inside a pojo for now.");

        } 
        else 
        {
            JsonObject JsonObject = JsonConvert.Import(json) as JsonObject;
            List<MethodPair> methods = getMatchingMethods(pojo.GetType(), SETTER);
            foreach(MethodPair setter in methods) 
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
        Type expectedType = method.GetParameters()[0].ParameterType;
        Object value = null;

        if (!JsonObject.Contains(fieldName)) 
        {
        // Skip
        }
        else if (expectedType.GetInterface("IList") != null) 
        {
            Type type = expectedType.GetGenericArguments()[0];
            Type rawType;
            Type listElementClass;
            
            listElementClass = type;
            rawType = typeof(List<>).MakeGenericType(new Type[] { type });

            Object list = Activator.CreateInstance(rawType);
            JsonArray JsonArray = JsonObject[fieldName] as JsonArray;
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
            Dictionary<String, Object> map = new Dictionary<string,object>();
            JsonObject jsonMap = JsonObject[fieldName] as JsonObject;
            foreach (String keyName in jsonMap.Names)
	        {
                map.Add(keyName, convertToObject(jsonMap[keyName] as String, valueClass));
            }

            value = map;

        } 
        else if (typeof(Enums<>).IsAssignableFrom(expectedType)) 
        {
            // TODO Need to stop using Enum as a class name :(
            value = convertEnum(method.GetGenericArguments()[0], JsonObject[fieldName] as JsonObject);
        } 
        else if (expectedType.IsEnum) 
        {
            if (JsonObject[fieldName] != null) 
            {
                foreach (FieldInfo v in expectedType.GetFields(BindingFlags.Static | BindingFlags.Public)) 
                {
                    if (v.Name.ToString().Equals(JsonObject[fieldName])) 
                    {
                        value = v.GetRawConstantValue();
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
            value = JsonObject[fieldName];
        } 
        else if (expectedType.Equals(typeof(DateTime))) 
        {
            // Use JODA ISO parsing for the conversion
            value = DateTime.Parse(JsonObject[fieldName] as string);
        } 
        else if (expectedType.Equals(typeof(long))) 
        {
            value = JsonObject[fieldName];
        } 
        else if (expectedType.Equals(typeof(int))) 
        {
            value = JsonObject[fieldName];
        } 
        else if (expectedType.Equals(typeof(bool))) 
        {
            value = JsonObject[fieldName];
        } 
        else if (expectedType.Equals(typeof(float))) 
        {
            String stringFloat = JsonObject[fieldName] as String;
            value = float.Parse(stringFloat);
        } 
        else 
        {
            // Assume its an injected type
            value = convertToObject(JsonObject[fieldName].ToString(), expectedType);
        }

        if (value != null) 
        {
            method.Invoke(pojo, new object[]{value});
        }
    }

    private Object convertEnum(Type enumKeyType, JsonObject jsonEnum)
    {
        Object value = null;
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