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
using System.Collections.Specialized;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;
using Jayrock.Json;
using Pesta.Engine.social.core.model;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using Pesta.Utilities;
using pestaServer.Models.social.service;

namespace pestaServer.Models.social.core.util
{
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BeanAtomConverter : BeanConverter
    {
        private const string osearchNameSpace = "http://a9.com/-/spec/opensearch/1.1";
        private readonly string guidPrefix = "urn:guid:" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];

        private static readonly Dictionary<string, string> entryTypes = new Dictionary<string, string>
                                                                   {
                                                                       {"people", "person"},
                                                                       {"appdata", "appdata"},
                                                                       {"activities", "activity"},
                                                                       {"messages", "messages"},
                                                                   };

        public override String GetContentType() 
        {
            return "application/atom+xml";
        }

        public override String ConvertToString(Object pojo)
        {
            return convertToAtom(pojo, null);
        }

        public override String ConvertToString(Object pojo, RequestItem request) 
        {
            return convertToAtom(pojo, request);
        }

        public String convertToAtom(Object obj, RequestItem request) 
        {
            CreateXmlDoc(XMLVERSION, CHARSET);
            var userIds = request.getUsers();
            IEnumerator<UserId> users = userIds.GetEnumerator();
            users.MoveNext();
            SyndicationFeed feed = new SyndicationFeed();
            XmlQualifiedName ns2 = new XmlQualifiedName("osearch", "http://www.w3.org/2000/xmlns/");
            feed.AttributeExtensions.Add(ns2, osearchNameSpace);
            SyndicationPerson fperson = new SyndicationPerson();
            fperson.Uri = guidPrefix + ":" + users.Current.getUserId();
            fperson.Name = guidPrefix + ":" + users.Current.getUserId();
            feed.Authors.Add(fperson);
            feed.LastUpdatedTime = DateTime.UtcNow;
            //addNode(entry, "title", requestType + " feed for id " + authorName + " (" + startIndex + " - " + (endPos - 1) + " of " + totalResults + ")");
            //addNode(entry, "id", guid);

            // Check to see if this is a single entry, or a collection, and construct either an atom
            // feed (collection) or an entry (single)
            if (obj is IRestfulCollection)
            {
                IRestfulCollection collection = (IRestfulCollection)obj;
                int totalResults = collection.getTotalResults();
                int itemsPerPage = request.GetCount();
                int startIndex = collection.getStartIndex();

                //int endPos = (startIndex + itemsPerPage) > totalResults ? totalResults : (startIndex + itemsPerPage);
                
                SyndicationLink slink = new SyndicationLink
                                            {
                                                Uri =
                                                    new Uri("http://" + request.getParameter("HTTP_HOST") +
                                                            request.getParameter("URL")),
                                                RelationshipType = "self"
                                            };
                feed.Links.Add(slink);
                
                // Add osearch & next link to the entry
                addPagingFields(request, feed, startIndex, itemsPerPage, totalResults);
                
                // Add response entries to feed
                var responses = collection.getEntry();
                List<SyndicationItem> items = new List<SyndicationItem>();
                foreach (var response in responses)
                {
                    // Special hoisting rules for activities
                    if (response is ActivityImpl) 
                    {
                        items.Add(addActivityData((Activity)response));
                    }
                    else if (response is PersonImpl)
                    {
                        items.Add(addPeopleData((Person)response));
                    }
                    else
                    {
                        throw new Exception("AtomConverter: unsupported object");
                    }
                }
                feed.Items = items;
            } 
            else if (obj is DataCollection)
            {
                // only appdata?
                addAppDataData(feed, (DataCollection)obj);
            }
            else if (obj is JsonArray)
            {
                List<SyndicationItem> items = new List<SyndicationItem>();
                foreach (var s in (JsonArray)obj)
                {
                    SyndicationItem sitem = new SyndicationItem();
                    XmlNode xmldata = addAtomData("supported", s, "");
                    XmlSyndicationContent content = new XmlSyndicationContent("text/xml", new SyndicationElementExtension(xmldata.FirstChild));
                    sitem.Content = content;
                    items.Add(sitem);
                }
                feed.Items = items;
            }
            else 
            {
                throw new Exception("AtomConverter: unsupported object");
            }
            MemoryStream ms = new MemoryStream();
            var xw = XmlWriter.Create(ms);
            if (xw == null)
            {
                return "";
            }
            feed.SaveAsAtom10(xw);
            xw.Flush();
            xw.Close();
            string xml = Encoding.UTF8.GetString(ms.ToArray());
            return xml;
        }
        private SyndicationItem addPeopleData(Person data)
        {
            SyndicationItem sitem = new SyndicationItem();
            SyndicationPerson fperson = new SyndicationPerson();
            fperson.Name = data.getDisplayName();
            sitem.Authors.Add(fperson);
            sitem.Id = guidPrefix + ":" + data.getId();
            if (data.getUpdated().HasValue)
            {
                sitem.LastUpdatedTime = data.getUpdated().Value;
            }
            sitem.Title = new TextSyndicationContent(data.getDisplayName());
            XmlNode xmldata = addAtomData("person", data, osNameSpace);
            XmlSyndicationContent content = new XmlSyndicationContent("application/xml", new SyndicationElementExtension(xmldata.FirstChild));
            sitem.Content = content;
            return sitem;
        }
        private SyndicationItem addActivityData(Activity data)
        {
            SyndicationItem sitem = new SyndicationItem();
            SyndicationCategory category = new SyndicationCategory();
            category.Name = "status";
            sitem.Categories.Add(category);
            sitem.LastUpdatedTime = UnixTime.ConvertFromUnixTimestamp(data.getPostedTime().Value).ToUniversalTime();
            sitem.Id = "urn:guid:" + data.getId();
            // <link rel="self" type="application/atom+xml" href="http://api.example.org/activity/feeds/.../af3778"/>
            sitem.Title = new TextSyndicationContent(data.getTitle());
            sitem.Summary = new TextSyndicationContent(data.getBody());
            SyndicationPerson fperson = new SyndicationPerson();
            fperson.Uri = guidPrefix + ":" + data.getUserId();
            sitem.Authors.Add(fperson);
            XmlNode xmldata = addAtomData("activity", data, osNameSpace);
            XmlSyndicationContent content = new XmlSyndicationContent("application/xml", new SyndicationElementExtension(xmldata.FirstChild));
            sitem.Content = content;
            return sitem;
        }
        private void addAppDataData(SyndicationFeed feedEntry, DataCollection data)
        {
            var entries = data.getEntry();
            List<SyndicationItem> items = new List<SyndicationItem>();
            foreach (var ent in entries)
            {
                SyndicationItem sitem = new SyndicationItem();
                sitem.LastUpdatedTime = DateTime.UtcNow;
                // <link rel="self" type="application/atom+xml" href="http://api.example.org/activity/feeds/.../af3778"/>
                SyndicationPerson fperson = new SyndicationPerson();
                fperson.Uri = guidPrefix + ":" + ent.Key;
                sitem.Authors.Add(fperson);
                sitem.Id = guidPrefix + ":" + ent.Key;
                sitem.Title = new TextSyndicationContent("");
                XmlNode xmldata = addAtomData("appdata", ent.Value, "");
                XmlSyndicationContent content = new XmlSyndicationContent("text/xml", new SyndicationElementExtension(xmldata.FirstChild));
                sitem.Content = content;
                items.Add(sitem);
            }
            feedEntry.Items = items;
        }
        private void addPagingFields(RequestItem request, SyndicationFeed feed, int startIndex, int itemsPerPage, int totalResults) 
        {
            XmlElement tr = xmlDoc.CreateElement("osearch", "totalResults", osearchNameSpace);
            tr.InnerText = totalResults.ToString();
            feed.ElementExtensions.Add(tr);
            XmlElement si = xmlDoc.CreateElement("osearch", "startIndex", osearchNameSpace);
            si.InnerText = startIndex.ToString();
            feed.ElementExtensions.Add(si);
            XmlElement ipp = xmlDoc.CreateElement("osearch", "itemsPerPage", osearchNameSpace);
            ipp.InnerText = itemsPerPage.ToString();
            feed.ElementExtensions.Add(ipp);
            // Create a "next" link based on our current url if this is a pageable collection & there is more to display
            if ((startIndex + itemsPerPage) < totalResults) 
            {
                int nextStartIndex = (startIndex + itemsPerPage) - 1;
                NameValueCollection parameters =  HttpUtility.ParseQueryString(request.getParameter("QUERY_STRING"));
                parameters[RequestItem.START_INDEX] = nextStartIndex.ToString();
                parameters[RequestItem.COUNT] = itemsPerPage.ToString();
                List<string> outParams = new List<string>();
                foreach (var p in parameters) 
                {
                    string key = p.ToString();
                    outParams.Add(key + "=" + parameters[key]);
                }
                string outParamString = "?" + String.Join("&", outParams.ToArray());
                string nextUri = "http://" + request.getParameter("HTTP_HOST") + request.getParameter("URL") + outParamString;

                SyndicationLink slink = new SyndicationLink {Uri = new Uri(nextUri), RelationshipType = "next"};
                feed.Links.Add(slink);
            }
        }
        private XmlNode addAtomData(string name, object data, string namespc)
        {
            XmlNode xmldata = xmlDoc.CreateElement("xmldata");
            AddData(xmldata, name, data, namespc);
            return xmldata;
        }

        public override object ConvertToObject(String xml, Type className)
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

            switch (className.Name)
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
            return null;
        }
    }
}