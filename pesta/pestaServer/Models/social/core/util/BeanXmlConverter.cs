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
using System.IO;
using System.Text;
using System.Xml;
using Pesta.Engine.social.spi;
using pestaServer.Models.social.service;

namespace pestaServer.Models.social.core.util
{
    /// <summary>
    /// Summary description for BeanXmlConverter
    /// </summary>
    public class BeanXmlConverter : BeanConverter
    {
        private static readonly Dictionary<string, string> entryTypes = new Dictionary<string, string>
                                                                   {
                                                                       {"people", "entry"},
                                                                       {"appdata", "entry"},
                                                                       {"activities", "entry"},
                                                                       {"messages", "entry"},
                                                                   };

        public override String getContentType()
        {
            return "application/xml";
        }
        public override String convertToString(Object pojo)
        {
            return convertToXml(pojo, null);
        }
        public override String convertToString(Object pojo, RequestItem request) 
        {
            return convertToXml(pojo, request);
        }
        protected String convertToXml(Object obj, RequestItem request)
        {
            createXmlDoc(xmlVersion, charSet);
            var requestType = getRequestType(request, entryTypes);

            if (obj is IRestfulCollection)
            {
                IRestfulCollection collection = (IRestfulCollection)obj;
                int totalResults = collection.getTotalResults();
                int itemsPerPage = request.getCount();
                int startIndex = collection.getStartIndex();

                XmlNode entry = addNode(xmlDoc, "response", "");

                // Required Xml fields
                addNode(entry, "startIndex", startIndex.ToString());
                addNode(entry, "itemsPerPage", itemsPerPage.ToString());
                addNode(entry, "totalResults", totalResults.ToString());
                var responses = collection.getEntry();
                foreach (var response in responses)
                {
                    // recursively add responseItem data to the xml structure
                    addData(entry, requestType, response);
                }
            }
            else if (obj is DataCollection)
            {
                var appdata = addNode(xmlDoc, "appdata", null, null, osNameSpace);
                var entries = ((DataCollection)obj).getEntry();
                foreach (var ent in entries)
                {
                    XmlNode entry = addNode(appdata, "entry", "");
                    addNode(entry, "key", ent.Key);
                    XmlNode valueNode = addNode(entry, "value", "");
                    XmlNode entry2 = addNode(valueNode, "entry", "");
                    foreach (var val in ent.Value)
                    {
                        addNode(entry2, "key", val.Key);
                        addNode(entry2, "value", val.Value);
                    }
                }
            }
            else
            {
                var entry = addNode(xmlDoc, "response", "");
                addData(entry, "entry", obj);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                xmlDoc.Save(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        
        public override Object convertToObject(String xml, Type className) 
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            switch (className.Name)
            {
                case "Activity":
                    return convertActivities(doc);
                case "DataCollection":
                    return convertAppData(doc);
                case "Message":
                    return convertMessages(doc);
                case "Person":
                    return convertPeople(doc);
            }
            return null;
        }

        
  }
}