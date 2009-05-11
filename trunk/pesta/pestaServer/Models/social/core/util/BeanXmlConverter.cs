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
        private static readonly Dictionary<string, string> ENTRY_TYPES = new Dictionary<string, string>
                                                                   {
                                                                       {"people", "entry"},
                                                                       {"appdata", "entry"},
                                                                       {"activities", "entry"},
                                                                       {"messages", "entry"},
                                                                   };

        public override String GetContentType()
        {
            return "application/xml";
        }
        public override String ConvertToString(Object pojo)
        {
            return convertToXml(pojo, null);
        }
        public override String ConvertToString(Object pojo, RequestItem request) 
        {
            return convertToXml(pojo, request);
        }
        protected String convertToXml(Object obj, RequestItem request)
        {
            CreateXmlDoc(XMLVERSION, CHARSET);
            var requestType = GetRequestType(request, ENTRY_TYPES);

            if (obj is IRestfulCollection)
            {
                IRestfulCollection collection = (IRestfulCollection)obj;
                int totalResults = collection.getTotalResults();
                int itemsPerPage = request.GetCount();
                int startIndex = collection.getStartIndex();

                XmlNode entry = AddNode(xmlDoc, "response", "");

                // Required Xml fields
                AddNode(entry, "startIndex", startIndex.ToString());
                AddNode(entry, "itemsPerPage", itemsPerPage.ToString());
                AddNode(entry, "totalResults", totalResults.ToString());
                var responses = collection.getEntry();
                foreach (var response in responses)
                {
                    // recursively add responseItem data to the xml structure
                    AddData(entry, requestType, response);
                }
            }
            else if (obj is DataCollection)
            {
                var appdata = AddNode(xmlDoc, "appdata", null, null, osNameSpace);
                var entries = ((DataCollection)obj).getEntry();
                foreach (var ent in entries)
                {
                    XmlNode entry = AddNode(appdata, "entry", "");
                    AddNode(entry, "key", ent.Key);
                    XmlNode valueNode = AddNode(entry, "value", "");
                    XmlNode entry2 = AddNode(valueNode, "entry", "");
                    foreach (var val in ent.Value)
                    {
                        AddNode(entry2, "key", val.Key);
                        AddNode(entry2, "value", val.Value);
                    }
                }
            }
            else
            {
                var entry = AddNode(xmlDoc, "response", "");
                AddData(entry, "entry", obj);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                xmlDoc.Save(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        
        public override Object ConvertToObject(String xml, Type className) 
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
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