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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Pesta.Engine.social.spi;

namespace Pesta.Engine.protocol.conversion
{
    /// <summary>
    /// Summary description for BeanXmlConverter
    /// </summary>
    public class BeanXmlConverter : BeanConverter
    {
        public override String GetContentType()
        {
            return ContentTypes.OUTPUT_XML_CONTENT_TYPE;
        }
        public override String ConvertToString(Object pojo)
        {
            return ConvertToString(pojo, null);
        }
        public override String ConvertToString(Object pojo, RequestItem request)
        {
            return convertToXml(pojo, request);
        }
        
        protected String convertToXml(Object obj, RequestItem request)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true  });
            using (var writer = XmlDictionaryWriter.CreateDictionaryWriter(xw))
            {
                if (obj is IRestfulCollection)
                {
                    IRestfulCollection collection = (IRestfulCollection)obj;
                    var responses = collection.getEntry();
                    writer.WriteStartElement("response", osNameSpace);
                    writer.WriteElementString("startIndex", collection.startIndex.ToString());
                    if (request.getCount().HasValue)
                    {
                        writer.WriteElementString("itemsPerPage", request.getCount().ToString());
                    }
                    writer.WriteElementString("totalResults", collection.totalResults.ToString());
                    writer.WriteElementString("isFiltered", collection.isFiltered.ToString().ToLower());
                    writer.WriteElementString("isSorted", collection.isSorted.ToString().ToLower());
                    writer.WriteElementString("isUpdatedSince", collection.isUpdatedSince.ToString().ToLower());
                    
                    foreach (var item in (IList)responses)
                    {
                        writer.WriteStartElement("entry");
                        //ser.WriteObject(writer, item);
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", osNameSpace);
                        XmlSerializer ser = new XmlSerializer(item.GetType());
                        ser.Serialize(writer, item, ns);
                        writer.WriteEndElement();
                    }
                    
                    writer.WriteEndElement();
                }
                else if (obj is DataCollection)
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", osNameSpace);
                    XmlSerializer ser = new XmlSerializer(obj.GetType());
                    ser.Serialize(writer, obj, ns);
                }
                else  
                {
                    throw new Exception("XMLSerialization: shouldn't have got here");
                }
                writer.Flush();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        
        public override T ConvertToObject<T>(String xml) 
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
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
            return default(T);
        }

        
    }
}