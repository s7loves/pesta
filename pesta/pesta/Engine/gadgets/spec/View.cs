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
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Pesta.Engine.common.xml;
using Pesta.Engine.gadgets.variables;
using Pesta.Utilities;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.spec
{
    /// <summary> Represents a Content section, but normalized into an individual
    /// view value after views are split on commas.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class View : RequestAuthenticationInfo
    {
        private static readonly List<String> KNOWN_ATTRIBUTES = new List<string>
                                                                    {
                                                                                      "type", "view", "href", "preferred_height", "preferred_width", "authz", "quirks",
                                                                                      "sign_owner", "sign_viwer"
                                                                                  };
        private readonly Uri _base;
        /**
        * @param elements List of all views, in order, that make up this view.
        *     An ordered list is required per the spec, since values must
        *     overwrite one another.
        * @throws SpecParserException
        */
        public View(String name, List<XmlElement> elements, Uri _base)
        {
            this.name = name;
            this._base = _base;
            bool quirks = true;
            Uri href = null;
            String contentType = null;
            ContentType type = null;
            int preferredHeight = 0;
            int preferredWidth = 0;
            String auth = null;
            bool signOwner = true;
            bool signViewer = true;
            Dictionary<String, String> attributes = new Dictionary<string, string>();
            StringBuilder content = new StringBuilder();
            foreach (XmlElement element in elements)
            {
                contentType = XmlUtil.getAttribute(element, "type");
                if (contentType != null)
                {
                    ContentType newType = ContentType.Parse(contentType);
                    if (type != null && newType != type)
                    {
                        throw new SpecParserException("You may not mix content types in the same view.");
                    }
                    else
                    {
                        type = newType;
                    }
                }
                href = XmlUtil.getUriAttribute(element, "href", href);
                quirks = XmlUtil.getBoolAttribute(element, "quirks", quirks);
                preferredHeight = XmlUtil.getIntAttribute(element, "preferred_height");
                preferredWidth = XmlUtil.getIntAttribute(element, "preferred_width");
                auth = XmlUtil.getAttribute(element, "authz", auth);
                signOwner = XmlUtil.getBoolAttribute(element, "sign_owner", signOwner);
                signViewer = XmlUtil.getBoolAttribute(element, "sign_viewer", signViewer);
                content.Append(element.InnerText);
                XmlNamedNodeMap attrs = element.Attributes;
                for (int i = 0; i < attrs.Count; ++i)
                {
                    XmlNode attr = attrs.Item(i);
                    if (!KNOWN_ATTRIBUTES.Contains(attr.Name))
                    {
                        attributes.Add(attr.Name, attr.Value);
                    }
                }
            }
            this.content = content.ToString();
            this.needsUserPrefSubstitution = this.content.Contains("__UP_");
            this.quirks = quirks;
            this.href = href;
            this.rawType = contentType ?? "html";
            this.type = type ?? ContentType.HTML;
            this.preferredHeight = preferredHeight;
            this.preferredWidth = preferredWidth;
            this.attributes = attributes;
            this.authType = AuthType.Parse(auth);
            this.signOwner = signOwner;
            this.signViewer = signViewer;
            if (type == ContentType.URL && this.href == null)
            {
                throw new SpecParserException("Content@href must be set when Content@type is \"url\".");
            }
        }

        /**
        * Allows the creation of a view from an existing view so that localization
        * can be performed.
        */
        private View(View view, Substitutions substituter)
        {
            needsUserPrefSubstitution = view.needsUserPrefSubstitution;
            name = view.name;
            rawType = view.rawType;
            type = view.type;
            quirks = view.quirks;
            preferredHeight = view.preferredHeight;
            preferredWidth = view.preferredWidth;
            authType = view.authType;
            signOwner = view.signOwner;
            signViewer = view.signViewer;

            content = substituter.substituteString(null, view.content);
            _base = view._base;
            href = _base.resolve(substituter.substituteUri(null, view.href));
            Dictionary<String, String> _attributes = new Dictionary<string, string>();
            foreach (var entry in view.attributes)
            {
                _attributes.Add(entry.Key, substituter.substituteString(null, entry.Value));
            }
            attributes = _attributes;
        }

        /**
        * Content@view
        */
        private readonly String name;
        public String getName()
        {
            return name;
        }

        /**
        * Content@type
        */
        private readonly ContentType type;
        public ContentType getType()
        {
            return type;
        }

        /**
        * Content@type - the raw, possibly non-standard string
        */
        private readonly String rawType;
        public String getRawType()
        {
            return rawType;
        }

        /**
        * Content@href
        *
        * All substitutions
        */
        private Uri href;
        public Uri getHref()
        {
            return href;
        }

        /**
        * Content@quirks
        */
        private readonly bool quirks;
        public bool getQuirks()
        {
            return quirks;
        }

        /**
        * Content@preferred_height
        */
        private readonly int preferredHeight;
        public int getPreferredHeight()
        {
            return preferredHeight;
        }

        /**
        * Content@preferred_width
        */
        private readonly int preferredWidth;
        public int getPreferredWidth()
        {
            return preferredWidth;
        }

        /**
        * Content#CDATA
        *
        * All substitutions
        */
        private String content;
        public String getContent()
        {
            return content;
        }

        /**
        * Set content for a type=html, href=URL style gadget.
        * This is the last bastion of GadgetSpec mutability,
        * and should only be used for the described case.
        * Call nulls out href in order to indicate content was
        * successfully retrieved.
        * @param content New gadget content retrieved from href.
        */
        public void setHrefContent(String content)
        {
            this.content = content;
            this.href = null;
        }

        /**
        * Whether or not the content section has any __UP_ hangman variables.
        */
        private readonly bool needsUserPrefSubstitution;
        public bool getNeedsUserPrefSubstitution()
        {
            return needsUserPrefSubstitution;
        }

        /**
        * Content/@authz
        */
        private readonly AuthType authType;
        public AuthType getAuthType()
        {
            return authType;
        }

        /**
        * Content/@sign_owner
        */
        private readonly bool signOwner;
        public bool isSignOwner()
        {
            return signOwner;
        }

        /**
        * Content/@sign_viewer
        */
        private readonly bool signViewer;
        public bool isSignViewer()
        {
            return signViewer;
        }

        /**
        * All attributes.
        */
        private readonly Dictionary<String, String> attributes;
        public Dictionary<String, String> getAttributes()
        {
            return attributes;
        }

        /**
        * Creates a new view by performing hangman substitution. See field comments
        * for details on what gets substituted.
        *
        * @param substituter
        * @return The substituted view.
        */
        public View substitute(Substitutions substituter)
        {
            return new View(this, substituter);
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<Content")
                .Append(" type='").Append(rawType).Append('\'')
                .Append(" href='").Append(href).Append('\'')
                .Append(" view='").Append(name).Append('\'')
                .Append(" quirks='").Append(quirks).Append('\'')
                .Append(" preferred_height='").Append(preferredHeight).Append('\'')
                .Append(" preferred_width='").Append(preferredWidth).Append('\'')
                .Append(" authz=").Append(authType.ToString().ToLower()).Append('\'');
            foreach (var entry in attributes)
            {
                buf.Append(entry.Key).Append("='").Append(entry.Value).Append('\'');
            }
            buf.Append("'>")
                .Append(content)
                .Append("</Content>");
            return buf.ToString();
        }

        /**
        * Possible values for Content/@type
        */
        public class ContentType : EnumBaseType<ContentType>
        {
            /// <summary>
            /// Initializes a new instance of the ContentType class.
            /// </summary>
            public ContentType(int key, string value)
                : base(key, value)
            {

            }
            public static readonly ContentType HTML = new ContentType(1, "HTML");
            public static readonly ContentType URL = new ContentType(2, "URL");

            /**
            * @param value
            * @return The parsed value (defaults to html)
            */
            public static ContentType Parse(String value)
            {
                return "url".Equals(value.ToLower()) ? ContentType.URL : ContentType.HTML;
            }
        }
    }
}