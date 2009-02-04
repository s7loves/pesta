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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Jayrock.Json;
using Pesta.Engine.common.xml;

namespace pestaServer.Models.gadgets.spec
{
    /// <summary>
    /// Summary description for MessageBundle
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class MessageBundle
    {
        public static readonly MessageBundle EMPTY = new MessageBundle();

        private readonly Dictionary<String, String> messages;
        private readonly String languageDirection;
        private readonly String jsonString;

        /**
        * Constructs a message bundle from input xml (fetched from an external file).
        *
        * @param locale The LocaleSpec element that this bundle was constructed from.
        * @param xml The content of the remote file.
        * @throws SpecParserException if parsing fails.
        */
        public MessageBundle(LocaleSpec locale, String xml)
        {
            XmlElement doc;
            try
            {
                doc = XmlUtil.Parse(xml);
            }
            catch (XmlException e)
            {
                throw new SpecParserException("Malformed XML in file " + locale.getMessages()
                                              + ": " + e.Message);
            }
            messages = parseMessages(doc);
            jsonString = new JsonObject(messages).ToString();
            languageDirection = locale.getLanguageDirection();
        }

        /**
        * Constructs a message bundle from a /ModulePrefs/Locale with nested messages.
        */
        public MessageBundle(XmlElement element)
        {
            messages = parseMessages(element);
            languageDirection = XmlUtil.getAttribute(element, "language_direction", "ltr");
            jsonString = new JsonObject(messages).ToString();
        }

        /**
        * Create a MessageBundle by merging child messages into the parent.
        *
        * @param parent The base bundle.
        * @param child The bundle containing overriding messages.
        */
        public MessageBundle(MessageBundle parent, MessageBundle child)
        {
            Dictionary<String, String> merged = new Dictionary<string,string>();
            String dir = null;
            if (parent != null)
            {
                foreach (var item in parent.messages)
                {
                    merged[item.Key] = item.Value;
                }
                dir = parent.languageDirection;
            }
            if (child != null)
            {
                foreach (var item in child.messages)
                {
                    merged[item.Key] = item.Value;
                }
                dir = child.languageDirection;
            }
            messages = merged;
            jsonString = new JsonObject(messages).ToString();
            languageDirection = dir;
        }

        private MessageBundle()
        {
            messages = new Dictionary<string, string>();
            jsonString = "{}";
            languageDirection = "ltr";
        }

        /**
        * @return The language direction associated with this message bundle, derived from the LocaleSpec
        * element that the bundle was constructed from.
        */
        public String getLanguageDirection()
        {
            return languageDirection;
        }

        /**
        * @return A read-only view of the message bundle.
        */
        public Dictionary<String, String> getMessages()
        {
            return messages;
        }

        /**
        * Extracts messages from an element.
        */
        private Dictionary<String, String> parseMessages(XmlElement element)
        {
            XmlNodeList nodes = element.GetElementsByTagName("msg");
            Dictionary<String, String> _messages = new Dictionary<String, String>(nodes.Count);

            for (int i = 0, j = nodes.Count; i < j; ++i)
            {
                XmlElement msg = (XmlElement)nodes[i];
                String name = XmlUtil.getAttribute(msg, "name");
                if (name == null)
                {
                    throw new SpecParserException(
                        "All message bundle entries must have a name attribute.");
                }
                _messages[name] = msg.InnerText.Trim();
            }
            return _messages;
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<messagebundle>\n");
            foreach (var entry in messages)
            {
                buf.Append("<msg name=\"").Append(entry.Key).Append("\">")
                    .Append(entry.Value)
                    .Append("</msg>\n");
            }
            buf.Append("</messagebundle>");
            return buf.ToString();
        }

        public String ToJSONString()
        {
            return jsonString;
        }
    }
}