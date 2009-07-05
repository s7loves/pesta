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
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using Pesta.Utilities;
using Activity=Pesta.Engine.social.model.Activity;
using MediaItem=Pesta.Engine.social.model.MediaItem;
using Message=Pesta.Engine.social.model.Message;

namespace Pesta.Engine.protocol.conversion
{
    public abstract class BeanConverter
    {
        public const string osNameSpace = "http://ns.opensocial.org/2008/opensocial";

        public abstract T ConvertToObject<T>(String str);
        public abstract String ConvertToString(Object pojo);
        public abstract String ConvertToString(Object pojo, RequestItem request);
        public abstract String GetContentType();

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

        protected static List<Activity> ConvertActivities(XmlDocument xml)
        {
            List<Activity> actlist = new List<Activity>();
            XmlNodeList actnodelist = xml.GetElementsByTagName("activity");
            foreach (XmlNode actnode in actnodelist)
            {
                Activity act = new Activity();
                for (int i = 0; i < actnode.ChildNodes.Count; i++)
                {
                    XmlNode child = actnode.ChildNodes[i];
                    switch (child.Name)
                    {
                        case "id":
                            act.id = child.Value;
                            break;
                        case "title":
                            act.title = child.Value;
                            break;
                        case "body":
                            act.body = child.Value;
                            break;
                        case "streamTitle":
                            act.streamTitle = child.Value;
                            break;
                        case "streamUrl":
                            act.streamUrl = child.Value;
                            break;
                        case "updated":
                            act.updated = DateTime.ParseExact(child.Value, "{0:s}Z", CultureInfo.InvariantCulture);
                            break;
                        case "mediaItems":
                            XmlNodeList mediaList = child.ChildNodes;
                            var mediaitems = new List<MediaItem>();
                            foreach (XmlNode media in mediaList)
                            {
                                MediaItem mediaItem = new MediaItem();
                                XmlNodeList mediaFields = media.ChildNodes;
                                for (int j = 0; j < mediaFields.Count; j++)
                                {
                                    XmlNode mf = mediaFields[j];
                                    switch (mf.Name)
                                    {
                                        case "type":
                                            mediaItem.type = (MediaItem.Type)Enum.Parse(typeof(MediaItem.Type), mf.Value, true);
                                            break;
                                        case "mimeType":
                                            mediaItem.mimeType = mf.Value;
                                            break;
                                        case "url":
                                            mediaItem.url = mf.Value;
                                            break;
                                    }
                                }
                                if (string.IsNullOrEmpty(mediaItem.mimeType) ||
                                    string.IsNullOrEmpty(mediaItem.url))
                                {
                                    throw new Exception("Invalid media item in activity xml");
                                }
                                mediaitems.Add(mediaItem);
                            }
                            act.mediaItems = mediaitems;
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

            List<string> recipients = new List<string>();
            if (xml["title"] == null || xml["content"] == null)
            {
                throw new Exception("Invalid message structure");
            }
            XmlNodeList fields = xml.ChildNodes;
            Message msg = new Message();
            for (int i = 0; i < fields.Count; i++)
            {
                XmlNode field = fields[i];
                switch (field.Name)
                {
                    case "id":
                        msg.id = field.Value;
                        break;
                    case "title":
                        msg.title = msg.sanitizeHTML(field.Value);
                        break;
                    case "content":
                        msg.body = msg.sanitizeHTML(field.Value);
                        break;
                    case "recipient":
                        recipients.Add(field.Value);
                        break;
                }
            }
            if (recipients.Count == 0)
            {
                throw new Exception("Invalid message structure");
            }
            msg.recipients = recipients;

            messages.Add(msg);

            return messages;
        }

        protected static Dictionary<string, Person> ConvertPeople(XmlDocument xml)
        {
            throw new ProtocolException(ResponseError.NOT_IMPLEMENTED, "Operation not supported");
        }
    }
}