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
using URI = java.net.URI;
using System.Text;

namespace Pesta
{
    /// <summary>
    /// Summary description for Preload
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class Preload : RequestAuthenticationInfo
    {
        private static readonly List<String> KNOWN_ATTRIBUTES
                    = new List<string>() { "views", "href", "authz", "sign_owner", "sign_viewer" };

        /**
        * Creates a new Preload from an xml node.
        *
        * @param preload The Preload to create
        * @throws SpecParserException When the href is not specified
        */
        public Preload(XmlElement preload)
        {
            href = XmlUtil.getUriAttribute(preload, "href");
            if (href == null)
            {
                throw new SpecParserException("Preload/@href is missing or invalid.");
            }

            // Record all the associated views
            String viewNames = XmlUtil.getAttribute(preload, "views", "");
            List<String> views = new List<String>();
            foreach (String _s in viewNames.Split(','))
            {
                String s = _s.Trim();
                if (s.Length > 0)
                {
                    views.Add(s.Trim());
                }
            }
            this.views = views;

            auth = AuthType.Parse(XmlUtil.getAttribute(preload, "authz"));
            signOwner = XmlUtil.getBoolAttribute(preload, "sign_owner", true);
            signViewer = XmlUtil.getBoolAttribute(preload, "sign_viewer", true);
            Dictionary<String, String> attributes = new Dictionary<string, string>();
            XmlNamedNodeMap attrs = preload.Attributes;
            for (int i = 0; i < attrs.Count; ++i)
            {
                XmlNode attr = attrs.Item(i);
                if (!KNOWN_ATTRIBUTES.Contains(attr.Name))
                {
                    attributes.Add(attr.Name, attr.Value);
                }
            }
            this.attributes = attributes;
        }

        private Preload(Preload preload, Substitutions substituter)
        {
            views = preload.views;
            auth = preload.auth;
            signOwner = preload.signOwner;
            signViewer = preload.signViewer;
            href = substituter.substituteUri(null, preload.href);
            Dictionary<String, String> attributes = new Dictionary<string, string>();
            foreach (var entry in preload.attributes)
            {
                attributes.Add(entry.Key, substituter.substituteString(null, entry.Value));
            }
            this.attributes = attributes;
        }

        /**
        * Preload@href
        */
        private readonly URI href;
        public URI getHref()
        {
            return href;
        }

        /**
        * Preload@auth
        */
        private readonly AuthType auth;
        public AuthType getAuthType()
        {
            return auth;
        }

        /**
        * Preload/@sign_owner
        */
        private readonly bool signOwner;
        public bool isSignOwner()
        {
            return signOwner;
        }

        /**
        * Preload/@sign_viewer
        */
        private readonly bool signViewer;
        public bool isSignViewer()
        {
            return signViewer;
        }

        /**
        * All attributes from the preload tag
        */
        private readonly Dictionary<String, String> attributes;
        public Dictionary<String, String> getAttributes()
        {
            return attributes;
        }

        /**
        * Prelaod@views
        */
        private readonly List<String> views;
        public List<String> getViews()
        {
            return views;
        }

        public Preload substitute(Substitutions substituter)
        {
            return new Preload(this, substituter);
        }

        /**
        * Produces an xml representation of the Preload.
        */

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<Preload href='").Append(href).Append('\'')
            .Append(" authz='").Append(auth.ToString().ToLower()).Append('\'')
            .Append(" views='").Append(String.Join(",", views.ToArray())).Append('\'');
            foreach (String attr in attributes.Keys)
            {
                buf.Append(' ').Append(attr).Append("='").Append(attributes[attr])
                            .Append('\'');
            }
            buf.Append("/>");
            return buf.ToString();
        }
    } 
}
