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
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.common.xml
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class XmlUtil
    {

        private XmlUtil()
        {
        }

        /// <summary> Extracts an attribute from a node.
        /// 
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <param name="def">
        /// </param>
        /// <returns> The value of the attribute, or def
        /// </returns>
        public static String getAttribute(XmlNode node, String attr, String def)
        {
            XmlNamedNodeMap attrs = (XmlAttributeCollection)node.Attributes;
            XmlNode val = attrs.GetNamedItem(attr);
            if (val != null)
            {
                return val.Value;
            }
            return def;
        }

        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <returns> The value of the given attribute, or null if not present.
        /// </returns>
        public static String getAttribute(XmlNode node, String attr)
        {
            return getAttribute(node, attr, null);
        }

        /// <summary> Retrieves an attribute as a Uri.</summary>
        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <returns> The parsed uri, or def if the attribute doesn't exist or can not
        /// be parsed as a Uri.
        /// </returns>
        public static Uri getUriAttribute(XmlNode node, String attr, Uri def)
        {
            String uri = getAttribute(node, attr);
            if (uri != null)
            {
                try
                {
                    return Uri.parse(uri);
                }
                catch
                {
                    return def;
                }
            }
            return def;
        }

        /// <summary> Retrieves an attribute as a Uri.</summary>
        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <returns> The parsed uri, or null.
        /// </returns>
        public static Uri getUriAttribute(XmlNode node, String attr)
        {
            return getUriAttribute(node, attr, null);
        }

        /// <summary> Retrieves an attribute as a Uri, and verifies that the Uri is an http
        /// or https Uri.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <param name="def">
        /// </param>
        /// <returns> the parsed uri, or def if the attribute is not a valid http or
        /// https Uri.
        /// </returns>
        public static Uri getHttpUriAttribute(XmlNode node, String attr, Uri def)
        {
            Uri uri = getUriAttribute(node, attr, def);
            if (uri == null)
            {
                return def;
            }
            if (!"http".Equals(uri.getScheme().ToLower()) && !"https".Equals(uri.getScheme().ToLower()))
            {
                return def;
            }
            return uri;
        }

        /// <summary> Retrieves an attribute as a Uri, and verifies that the Uri is an http
        /// or https Uri.
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <returns> the parsed uri, or null if the attribute is not a valid http or
        /// https Uri.
        /// </returns>
        public static Uri getHttpUriAttribute(XmlNode node, String attr)
        {
            return getHttpUriAttribute(node, attr, null);
        }

        /// <summary> Retrieves an attribute as a bool.
        /// 
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <param name="def">
        /// </param>
        /// <returns> True if the attribute exists and is not equal to "false"
        /// false if equal to "false", and def if not present.
        /// </returns>
        public static bool getBoolAttribute(XmlNode node, String attr, bool def)
        {
            String value_Renamed = getAttribute(node, attr);
            if (value_Renamed == null)
            {
                return def;
            }
            return bool.Parse(value_Renamed);
        }

        /// <param name="node">
        /// </param>
        /// <param name="attr">
        /// </param>
        /// <returns> True if the attribute exists and is not equal to "false"
        /// false otherwise.
        /// </returns>
        public static bool getBoolAttribute(XmlNode node, String attr)
        {
            return getBoolAttribute(node, attr, false);
        }

        /// <returns> An attribute coerced to an integer.
        /// </returns>
        public static int getIntAttribute(XmlNode node, String attr, int def)
        {
            String value_Renamed = getAttribute(node, attr);
            if (value_Renamed == null)
            {
                return def;
            }
            try
            {
                return System.Int32.Parse(value_Renamed);
            }
            catch
            {
                return def;
            }
        }

        /// <returns> An attribute coerced to an integer.
        /// </returns>
        public static int getIntAttribute(XmlNode node, String attr)
        {
            return getIntAttribute(node, attr, 0);
        }

        /**
        * Attempts to parse the input xml into a single element.
        * @param xml
        * @return The document object
        * @throws XmlException if a parse error occured.
        */
        public static XmlElement Parse(String xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return doc.DocumentElement;
            }
            catch (Exception e)
            {
                throw new XmlException(e.Message);
            }
        }
    }
}