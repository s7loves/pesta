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
using System.Collections.Specialized;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Jayrock;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;

namespace Pesta.Engine.protocol.conversion
{
    public class BeanAtomConverter : BeanConverter
    {
        private const string osearchNameSpace = "http://a9.com/-/spec/opensearch/1.1";
        private readonly string guidPrefix = "urn:guid:" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
        private readonly XmlSerializerNamespaces ns;
        public BeanAtomConverter()
        {
            ns = new XmlSerializerNamespaces();
            ns.Add("", osNameSpace);
        }
        public override String GetContentType() 
        {
            return ContentTypes.OUTPUT_ATOM_CONTENT_TYPE;
        }

        public override String ConvertToString(Object pojo)
        {
            return ConvertToString(pojo, null);
        }

        public override String ConvertToString(Object pojo, RequestItem request) 
        {
            return convertToAtom(pojo, request);
        }

        public String convertToAtom(Object obj, RequestItem request) 
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, OmitXmlDeclaration  = true});
            using (var writer = XmlDictionaryWriter.CreateDictionaryWriter(xw))
            {
                var userIds = request.getUsers();
                IEnumerator<UserId> users = userIds.GetEnumerator();
                users.MoveNext();
                writer.WriteStartElement("feed", "http://www.w3.org/2005/Atom");
                writer.WriteXmlnsAttribute("osearch", osearchNameSpace);
                writer.WriteStartElement("author");
                writer.WriteElementString("uri", guidPrefix + ":" + users.Current.getUserId());
                writer.WriteEndElement(); // author

                if (obj is IRestfulCollection)
                {
                    IRestfulCollection collection = (IRestfulCollection) obj;
                    int totalResults = collection.totalResults;
                    int itemsPerPage = request.getCount() ?? RequestItem.DEFAULT_COUNT;
                    int startIndex = collection.startIndex;

                    //int endPos = (startIndex + itemsPerPage) > totalResults ? totalResults : (startIndex + itemsPerPage);
                    writer.WriteStartElement("link");
                    writer.WriteAttributeString("ref", "self");
                    writer.WriteAttributeString("href", new Uri("http://" + request.getParameter("HTTP_HOST") +
                                                              request.getParameter("URL")).ToString());
                    writer.WriteEndElement(); // link
                    // Add osearch & next link to the entry
                    addPagingFields(writer, request, startIndex, itemsPerPage, totalResults);

                    // Add response entries to feed
                    var responses = collection.getEntry();
                    foreach (var response in (IList)responses)
                    {
                        writer.WriteStartElement("entry");
                        // Special hoisting rules for activities
                        if (response is Activity)
                        {
                            addActivityData(writer, response as Activity);
                        }
                        else if (response is Person)
                        {
                            addPeopleData(writer, response as Person);
                        }
                        else if (response is MessageCollection)
                        {
                            addMesssageCollectionData(writer, response as MessageCollection);
                        }
                        else
                        {
                            throw new Exception("AtomConverter: unsupported object");
                        }
                        writer.WriteEndElement(); // entry
                    }
                }
                else if (obj is DataCollection) // AppData
                {
                    var collection = (DataCollection) obj;
                    // TODO: fixed osearch for appData
                    addPagingFields(writer, request, 0, collection.entry.Count, collection.entry.Count);

                    foreach (var e in collection.entry)
                    {
                        writer.WriteStartElement("entry");
                        writer.WriteElementString("id", e.Key);
                        writer.WriteStartElement("content");
                        writer.WriteAttributeString("type", ContentTypes.OUTPUT_XML_CONTENT_TYPE);
                        foreach (var item in e.Value)
                        {
                            writer.WriteStartElement("entry");
                            writer.WriteElementString("key", item.Key);
                            writer.WriteElementString("value", item.Value);
                            writer.WriteEndElement(); // entry
                        }
                        writer.WriteEndElement(); // content
                        writer.WriteEndElement(); // entry
                    }
                }
                else
                {
                    throw new Exception("AtomConverter: unsupported object");
                }
                writer.WriteEndElement(); // feed
                xw.Flush();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        private void addPeopleData(XmlWriter writer, Person data)
        {
            writer.WriteElementString("id", guidPrefix + ":" + data.id);
            writer.WriteElementString("title", data.displayName);
            writer.WriteElementString("updated", data.updated.HasValue ? data.updated.Value.ToString("o") : DateTime.UtcNow.ToString("o"));
            writer.WriteStartElement("author");
            writer.WriteElementString("name", data.displayName);
            writer.WriteEndElement(); // author
            writer.WriteStartElement("content");
            writer.WriteAttributeString("type", ContentTypes.OUTPUT_XML_CONTENT_TYPE);
            XmlSerializer serializer = new XmlSerializer(data.GetType());
            serializer.Serialize(writer, data, ns);
            writer.WriteEndElement(); // content
        }
        private void addActivityData(XmlWriter writer, Activity data)
        {
            // TODO:<link rel="self" type="application/atom+xml" href="http://api.example.org/activity/feeds/.../af3778"/>
            writer.WriteElementString("id", guidPrefix + ":" + data.id);
            writer.WriteElementString("title", data.title);
            writer.WriteElementString("updated", data.postedTime.HasValue ? UnixTime.ToDateTime(data.postedTime.Value).ToUniversalTime().ToString("o") : DateTime.UtcNow.ToString("o"));
            writer.WriteStartElement("author");
            writer.WriteElementString("uri", guidPrefix + ":" + data.userId);
            //writer.WriteElementString("name", data.displayName???);
            writer.WriteEndElement(); // author
            writer.WriteStartElement("content");
            writer.WriteAttributeString("type", ContentTypes.OUTPUT_XML_CONTENT_TYPE);
            XmlSerializer serializer = new XmlSerializer(data.GetType());
            serializer.Serialize(writer, data, ns);
            writer.WriteEndElement(); // content
        }
        private void addMesssageCollectionData(XmlWriter writer, MessageCollection data)
        {
            // TODO: <link rel="self" type="application/atom+xml" href="http://api.example.org/activity/feeds/.../af3778"/>
            writer.WriteElementString("id", guidPrefix + ":" + data.id);
            writer.WriteElementString("title", data.title);
            writer.WriteElementString("updated", DateTime.UtcNow.ToString("o"));
            writer.WriteStartElement("author");
            //writer.WriteElementString("uri", guidPrefix + ":" + data.userId???);
            //writer.WriteElementString("name", data.displayName???);
            writer.WriteEndElement(); // author
            writer.WriteStartElement("content");
            writer.WriteAttributeString("type", ContentTypes.OUTPUT_XML_CONTENT_TYPE);
            
            // strange that it's different here
            writer.WriteElementString("id", data.id);
            writer.WriteElementString("title", data.title);
            writer.WriteElementString("total", data.total.ToString());
            writer.WriteElementString("unread", data.unread.ToString());
            writer.WriteElementString("updated", data.updated.HasValue ? UnixTime.ToDateTime(data.updated.Value).ToUniversalTime().ToString("o") : DateTime.UtcNow.ToString("o"));
            if (data.urls != null)
            {
                XmlSerializer serializer = new XmlSerializer(data.urls.GetType());
                serializer.Serialize(writer, data.urls, ns);
            }
            writer.WriteEndElement(); // content
        }
        private static void addPagingFields(XmlWriter writer, RequestItem request, int startIndex, int itemsPerPage, int totalResults) 
        {
            writer.WriteElementString("startIndex", osearchNameSpace, startIndex.ToString());
            if (request.getCount() != null)
            {
                writer.WriteElementString("itemsPerPage", osearchNameSpace, itemsPerPage.ToString());
            }
            writer.WriteElementString("totalResults", osearchNameSpace, totalResults.ToString());

            // Create a "next" link based on our current url if this is a pageable collection & there is more to display
            if ((startIndex + itemsPerPage) < totalResults) 
            {
                int nextStartIndex = (startIndex + itemsPerPage) - 1;
                NameValueCollection parameters =  HttpUtility.ParseQueryString(request.getParameter("QUERY_STRING"));
                parameters[RequestItem.START_INDEX] = nextStartIndex.ToString();
                parameters[RequestItem.COUNT] = itemsPerPage.ToString();
                var outParams = new List<string>();
                foreach (var p in parameters) 
                {
                    string key = p.ToString();
                    outParams.Add(key + "=" + parameters[key]);
                }
                string outParamString = "?" + String.Join("&", outParams.ToArray());
                string nextUri = "http://" + request.getParameter("HTTP_HOST") + request.getParameter("URL") + outParamString;

                writer.WriteStartElement("link");
                writer.WriteAttributeString("ref", "next");
                writer.WriteAttributeString("href", new Uri(nextUri).ToString());
            }
        }
        public override T ConvertToObject<T>(String xml)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            Atom10FeedFormatter atom = new Atom10FeedFormatter();
            
            if (!atom.CanRead(reader))
            {
                throw new Exception("Mallformed Atom xml");
            }

            SyndicationFeedFormatter formatter = atom;
            formatter.ReadFrom(reader);
            SyndicationFeed feed = formatter.Feed;

            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlDocument doc = new XmlDocument();
            XmlElement ele = doc.CreateElement("entry");
            ele.WriteTo(writer);
            foreach (var entry in feed.Items)
            {
                SyndicationContent content = entry.Content;
                if (writer != null)
                {
                    content.WriteTo(writer, "entry", "");
                }
            }

            doc.LoadXml(sb.ToString());
            /*
            switch (typeof(T).Name)
            {
                case "Activity":
                    return ConvertActivities(doc);
                case "DataCollection":
                    return ConvertAppData(doc);
                case "Message":
                    return ConvertMessages(doc);
                case "Person":
                    return ConvertPeople(doc);
            }
             * */
            throw new NotImplementedException();
            return default(T);
        }
    }
}