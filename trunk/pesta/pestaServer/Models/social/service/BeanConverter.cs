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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Xml;
using Jayrock.Json;
using Pesta.Engine.social;
using Pesta.Engine.social.core.model;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using Pesta.Utilities;

namespace pestaServer.Models.social.service
{
    public abstract class BeanConverter
    {
        protected static readonly Object[] EMPTY_OBJECT = { };
        protected static readonly HashSet<String> EXCLUDED_FIELDS = new HashSet<String> { "class", "declaringclass" };
        protected const String GETTER_PREFIX = "get";
        protected const String SETTER_PREFIX = "set";

        // Only compute the filtered getters/setters once per-class
        protected static readonly Dictionary<Type, List<MethodPair>> GETTER_METHODS = new Dictionary<Type, List<MethodPair>>();
        protected static readonly Dictionary<Type, List<MethodPair>> SETTER_METHODS = new Dictionary<Type, List<MethodPair>>();


        protected const string XMLVERSION = "1.0";
        protected const string CHARSET = "UTF-8";
        protected const string osNameSpace = "http://ns.opensocial.org/2008/opensocial";

        protected XmlDocument xmlDoc;

        public abstract Object ConvertToObject(String str, Type className);
        public abstract String ConvertToString(Object pojo);
        public abstract String ConvertToString(Object pojo, RequestItem request);
        public abstract String GetContentType();

        protected XmlNode AddNode(XmlNode node, string name, string value)
        {
            return AddNode(xmlDoc, node, name, value, null, "");
        }
        protected XmlNode AddNode(XmlNode node, string name, string value, object attributes, string nameSpace)
        {
            return AddNode(xmlDoc, node, name, value, attributes, nameSpace);
        }

        protected static XmlNode AddNode(XmlDocument doc, XmlNode node, string name, string value, object attributes, string nameSpace)
        {
            XmlNode childNode;
            if (!string.IsNullOrEmpty(nameSpace))
            {
                childNode = node.AppendChild(doc.CreateElement("", name, nameSpace));
            }
            else
            {
                childNode = node.AppendChild(doc.CreateElement(name));
            }

            if (!string.IsNullOrEmpty(value))
            {
                if (name == "id")
                {
                    value = HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + ":" + value;
                }
                childNode.AppendChild(doc.CreateTextNode(value));
            }

            if (attributes != null && attributes is IDictionary)
            {
                foreach (DictionaryEntry attr in (IDictionary)attributes)
                {
                    XmlAttribute childNodeAttr = childNode.Attributes.Append(doc.CreateAttribute(attr.Key.ToString()));
                    if (!String.IsNullOrEmpty(attr.Value.ToString()))
                    {
                        childNodeAttr.Value = attr.Value.ToString();
                    }
                }
            }
            return childNode;
        }

        protected XmlNode AddData(XmlNode element, string name, object data)
        {
            return AddData(xmlDoc, element, name, data, "");
        }
        protected XmlNode AddData(XmlNode element, string name, object data, string nameSpace)
        {
            return AddData(xmlDoc, element, name, data, nameSpace);
        }
        private XmlNode AddData(XmlDocument doc, XmlNode element, string name, object data, string nameSpace)
        {
            XmlNode newElement = element;
            int dummy;
           
            if (data is String)
            {
                newElement = !string.IsNullOrEmpty(nameSpace) ? element.AppendChild(doc.CreateElement("", name, nameSpace)) : element.AppendChild(doc.CreateElement(name));
                newElement.AppendChild(doc.CreateTextNode(data.ToString()));
            }
            else if (typeof(Enums<>).IsAssignableFrom(data.GetType()))
            {
                newElement = !string.IsNullOrEmpty(nameSpace) ? element.AppendChild(doc.CreateElement("", name, nameSpace)) : element.AppendChild(doc.CreateElement(name));
                // enums are output as : <NAME key="entry.Key">$displayValue</NAME> 
                var keyEntry = newElement.AppendChild(doc.CreateAttribute("key"));
                keyEntry.AppendChild(doc.CreateTextNode(((Enums<EnumKey>)data).getValue().ToString()));
                newElement.AppendChild(doc.CreateTextNode(((Enums<EnumKey>)data).getDisplayValue()));
            }
            else if (data is IDictionary)
            {
                newElement = !string.IsNullOrEmpty(nameSpace) ? element.AppendChild(doc.CreateElement("", name, nameSpace)) : element.AppendChild(doc.CreateElement(name));
                foreach (DictionaryEntry entry in (IDictionary)data)
                {
                    object key = entry.Key;
                    if (entry.Value != null || entry.Value is Object[])
                    {
                        // prevent invalid names.. try to guess a good one :)
                        if (int.TryParse(key.ToString(), out dummy))
                        {
                            key = entry.Value != null ? entry.Value.GetType().Name : name;
                        }
                        AddData(newElement, key.ToString(), entry.Value);
                    }
                    else
                    {
                        if (int.TryParse(key.ToString(), out dummy))
                        {
                            key = entry.Value != null ? entry.Value.GetType().Name : name;
                        }
                        if (entry.Value != null)
                        {
                            AddNode(newElement, key.ToString(), entry.Value.ToString());
                        }
                    }
                }
            }
            else if (data is JsonArray)
            {
                foreach (var dataEntry in (JsonArray)data)
                {
                    AddData(element, name, dataEntry);
                }
            }
            else
            {
                newElement = !string.IsNullOrEmpty(nameSpace) ? element.AppendChild(doc.CreateElement("", name, nameSpace)) : element.AppendChild(doc.CreateElement(name));
                JsonObject values = convertMethodsToJson(data);
                foreach (var value in values.Names)
                {
                    string key = value.ToString();
                    var val = values[key];
                    if (val is JsonObject ||
                        val is JsonArray)
                    {
                        AddData(newElement, key, val);
                    }
                    else
                    {
                        AddNode(newElement, key, val.ToString());
                    }
                }
            }
            return newElement;
        }
        protected static string GetRequestType(RequestItem requestItem, Dictionary<string, string> entryTypes)
        {
            // map the Request URL to the content type to use  
            string service = requestItem.getService();
            if (string.IsNullOrEmpty(service))
            {
                throw new Exception("Unsupported request type");
            }
            string type = entryTypes[service];
            if (type == null)
            {
                throw new Exception("Unsupported request type");
            }
            return type;
        }

        protected void CreateXmlDoc(string xmlVersion, string charSet)
        {
            xmlDoc = new XmlDocument();
            XmlDeclaration declare = xmlDoc.CreateXmlDeclaration(xmlVersion, charSet, "");
            xmlDoc.AppendChild(declare);
        }

        protected static List<Activity> ConvertActivities(XmlDocument xml)
        {
            List<Activity> actlist = new List<Activity>();
            XmlNodeList actnodelist = xml.GetElementsByTagName("activity");
            foreach (XmlNode actnode in actnodelist)
            {
                ActivityImpl act = new ActivityImpl();
                for (int i = 0; i < actnode.ChildNodes.Count; i++)
                {
                    XmlNode child = actnode.ChildNodes[i];
                    switch (child.Name)
                    {
                        case "id":
                            act.setId(child.Value);
                            break;
                        case "title":
                            act.setTitle(child.Value);
                            break;
                        case "body":
                            act.setBody(child.Value);
                            break;
                        case "streamTitle":
                            act.setStreamTitle(child.Value);
                            break;
                        case "streamUrl":
                            act.setStreamUrl(child.Value);
                            break;
                        case "updated":
                            act.setUpdated(DateTime.ParseExact(child.Value, "{0:s}Z", CultureInfo.InvariantCulture));
                            break;
                        case "mediaItems":
                            XmlNodeList mediaList = child.ChildNodes;
                            List<MediaItem> mediaitems = new List<MediaItem>();
                            foreach (XmlNode media in mediaList)
                            {
                                MediaItemImpl mediaItem = new MediaItemImpl();
                                XmlNodeList mediaFields = media.ChildNodes;
                                for (int j = 0; j < mediaFields.Count; j++)
                                {
                                    XmlNode mf = mediaFields[j];
                                    switch (mf.Name)
                                    {
                                        case "type":
                                            mediaItem.setType(EnumBaseType<MediaItem.Type>.GetBaseByValue(mf.Value));
                                            break;
                                        case "mimeType":
                                            mediaItem.setMimeType(mf.Value);
                                            break;
                                        case "url":
                                            mediaItem.setUrl(mf.Value);
                                            break;
                                    }
                                }
                                if (mediaItem.getType() == null ||
                                    string.IsNullOrEmpty(mediaItem.getMimeType()) ||
                                    string.IsNullOrEmpty(mediaItem.getUrl()))
                                {
                                    throw new Exception("Invalid media item in activity xml");
                                }
                                mediaitems.Add(mediaItem);
                            }
                            act.setMediaItems(mediaitems);
                            break;
                    }
                }
                actlist.Add(act);
            }
            return actlist;
        }

        protected static DataCollection ConvertAppData(XmlDocument xml)
        {
            XmlNodeList entrylist = xml.GetElementsByTagName("entry");
            if (entrylist.Count == 0)
            {
                throw new Exception("Mallformed AppData xml");
            }
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (XmlNode entry in entrylist)
            {
                XmlNodeList fields = entry.ChildNodes;
                string key = "";
                string value = "";
                for (int i = 0; i < fields.Count; i++)
                {
                    XmlNode field = fields[i];
                    switch (field.Name)
                    {
                        case "key":
                            key = field.Value;
                            break;
                        case "value":
                            value = field.Value;
                            break;
                    }
                    if (string.IsNullOrEmpty(key))
                    {
                        throw new Exception("Mallformed AppData xml");
                    }
                }
                data[key] = value;
            }

            return new DataCollection(new Dictionary<string, Dictionary<string, string>> { { "", data } });
        }

        protected static List<Message> ConvertMessages(XmlDocument xml)
        {
            List<Message> messages = new List<Message>();

            List<Person> recipients = new List<Person>();
            if (xml["title"] == null || xml["content"] == null)
            {
                throw new Exception("Invalid message structure");
            }
            XmlNodeList fields = xml.ChildNodes;
            MessageImpl msg = new MessageImpl();
            for (int i = 0; i < fields.Count; i++)
            {
                XmlNode field = fields[i];
                switch (field.Name)
                {
                    case "id":
                        msg.setId(field.Value);
                        break;
                    case "title":
                        msg.setTitle(msg.sanitizeHTML(field.Value));
                        break;
                    case "content":
                        msg.setBody(msg.sanitizeHTML(field.Value));
                        break;
                    case "recipient":
                        PersonImpl per = new PersonImpl();
                        per.setId(field.Value);
                        recipients.Add(per);
                        break;
                }
            }
            if (recipients.Count == 0)
            {
                throw new Exception("Invalid message structure");
            }
            msg.setRecipients(recipients);

            messages.Add(msg);

            return messages;
        }

        protected static Dictionary<string, Person> ConvertPeople(XmlDocument xml)
        {
            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Operation not supported");
        }

        /**
 * Convert the object to {@link JsonObject} reading Pojo properties
 *
 * @param pojo The object to convert
 * @return A JsonObject representing this pojo
 */

        protected static JsonObject convertMethodsToJson(object pojo)
        {
            List<MethodPair> availableGetters;
            if (!GETTER_METHODS.TryGetValue(pojo.GetType(), out availableGetters))
            {
                availableGetters = GetMatchingMethods(pojo.GetType(), GETTER_PREFIX);
                if (!GETTER_METHODS.ContainsKey(pojo.GetType()))
                {
                    GETTER_METHODS.Add(pojo.GetType(), availableGetters);
                }

            }

            JsonObject toReturn = new JsonObject();
            foreach (MethodPair getter in availableGetters)
            {
                String errorMessage = "Could not encode the " + getter.Method + " method on "
                                      + pojo.GetType().Name;
                try
                {
                    Object val = getter.Method.Invoke(pojo, EMPTY_OBJECT);
                    if (val != null && val.ToString() != "")
                    {
                        toReturn.Put(getter.FieldName, TranslateObjectToJson(val));
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(errorMessage, e);
                }
            }
            return toReturn;
        }


        protected static Object TranslateObjectToJson(Object val)
        {
            if (val is Object[])
            {
                JsonArray array = new JsonArray();
                foreach (Object asd in (Object[])val)
                {
                    array.Put(TranslateObjectToJson(asd));
                }
                return array;
            }

            if (val != null && val.GetType().GetInterface("IList") != null)
            {
                JsonArray list = new JsonArray();
                foreach (Object item in (IList)val)
                {
                    list.Add(TranslateObjectToJson(item));
                }
                return list;
            }

            if (val != null && val.GetType().GetInterface("IDictionary") != null)
            {
                JsonObject map = new JsonObject();
                IDictionary originalMap = (IDictionary)val;

                foreach (DictionaryEntry item in originalMap)
                {
                    map.Put(item.Key.ToString(), TranslateObjectToJson(item.Value));
                }
                return map;

            }

            if (val != null && val.GetType().IsEnum)
            {
                return val.ToString();
            }

            if (val == null 
                || val is String
                || val is Boolean
                || val is int
                || val is DateTime
                || val is long
                || val is float
                || val is JsonObject
                || val is JsonArray)
            {
                return val;
            }


            return convertMethodsToJson(val);
        }


        protected class MethodPair
        {
            public readonly MethodInfo Method;
            public readonly String FieldName;

            public MethodPair(MethodInfo method, String fieldName)
            {
                Method = method;
                FieldName = fieldName;
            }
        }

        protected static List<MethodPair> GetMatchingMethods(Type pojo, String prefix)
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

    }
}