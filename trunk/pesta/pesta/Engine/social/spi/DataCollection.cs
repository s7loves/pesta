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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Pesta.Engine.protocol.conversion;

namespace Pesta.Engine.social.spi
{
    [XmlRoot("appData", Namespace = BeanConverter.osNameSpace)]
    public class DataCollection : IRestfulCollection, IXmlSerializable
    {
        public Dictionary<String, Dictionary<String, String>> entry { get; set; }

        public DataCollection(Dictionary<String, Dictionary<String, String>> entry)
            :this()
        {
            if (entry != null)
            {
                this.entry = entry;
            }
        }

        public DataCollection()
        {
            entry = new Dictionary<string, Dictionary<string, string>>();
        }

        #region Implementation of IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var e in entry)
            {
                writer.WriteStartElement("entry");
                writer.WriteElementString("key", e.Key);
                writer.WriteStartElement("value");
                foreach (var item in e.Value)
                {
                    writer.WriteStartElement("entry");
                    writer.WriteElementString("key", item.Key);
                    writer.WriteElementString("value", item.Value);
                    writer.WriteEndElement(); // entry
                }
                writer.WriteEndElement(); // value
                writer.WriteEndElement(); // entry
            }
        }

        #endregion

        public override object getEntry()
        {
            throw new NotImplementedException();
        }
    }
}