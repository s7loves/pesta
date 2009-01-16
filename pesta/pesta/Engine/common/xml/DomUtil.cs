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
using System.Xml;

namespace Pesta.Engine.common.xml
{
    public class DomUtil
    {
        /**
        * @return first child node matching the specified name
        */
        public static XmlNode getFirstNamedChildXmlNode(XmlNode root, String nodeName) 
        {
            XmlNode current = root.FirstChild;
            while (current != null) 
            {
                if (current.Name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase)) 
                {
                    return current;
                }
                current = current.NextSibling;
            }
            return null;
        }

        /**
        * @return last child node matching the specified name.
        */
        public static XmlNode getLastNamedChildXmlNode(XmlNode root, String nodeName)
        {
            XmlNode current = root.LastChild;
            while (current != null) 
            {
                if (current.Name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase)) 
                {
                    return current;
                }
                current = current.PreviousSibling;
            }
            return null;
        }

        public static List<XmlElement> getXmlElementsByTagNameCaseInsensitive(XmlDocument doc,
                            HashSet<String> lowerCaseNames)
        {
            List<XmlElement> result = new List<XmlElement>();
            foreach (var name in lowerCaseNames)
            {
                foreach (var element in doc.GetElementsByTagName(name))
                {
                    result.Add((XmlElement)element);
                }
            }
            return result;
        }
    }
}
