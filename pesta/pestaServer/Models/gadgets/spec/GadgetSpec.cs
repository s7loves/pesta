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
using System.Text;
using System.Xml;
using Pesta.Engine.common.util;
using Pesta.Engine.common.xml;
using Pesta.Utilities;
using pestaServer.Models.gadgets.variables;
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets.spec
{
    /// <summary>
    /// Summary description for GadgetSpec
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetSpec
    {
        public static readonly String DEFAULT_VIEW = "default";
        public static readonly Locale DEFAULT_LOCALE = new Locale("all", "ALL");

        /**
        * The url for this gadget spec.
        */
        private readonly Uri url;
        public Uri getUrl()
        {
            return url;
        }

        /**
        * A checksum of the gadget's content.
        */
        private readonly String checksum;
        public String getChecksum()
        {
            return checksum;
        }

        /**
        * ModulePrefs
        */
        private ModulePrefs modulePrefs;
        public ModulePrefs getModulePrefs()
        {
            return modulePrefs;
        }

        /**
        * UserPref
        */
        private List<UserPref> userPrefs;
        public List<UserPref> getUserPrefs()
        {
            return userPrefs;
        }

        /**
        * Content
        * Mapping is view -> Content section.
        */
        private Dictionary<String, View> views;
        public Dictionary<String, View> getViews()
        {
            return views;
        }

        /**
        * Retrieves a single view by name.
        *
        * @param name The name of the view you want to see
        * @return The view object, if it exists, or null.
        */
        public View getView(String name)
        {
            if (!views.ContainsKey(name))
            {
                return null;
            }
            return views[name];
        }

        /**
        * A map of attributes associated with the instance of the spec
        * Used by handler classes to use specs to carry context.
        * Not defined by the specification
        */
        private readonly Dictionary<String, Object> attributes = new Dictionary<String, Object>();
        public Object getAttribute(String key)
        {
            if (!attributes.ContainsKey(key))
            {
                return null;
            }
            return attributes[key];
        }

        public void setAttribute(String key, Object o)
        {
            attributes.Add(key, o);
        }

        /**
        * Performs substitutions on the spec. See individual elements for
        * details on what gets substituted.
        *
        * @param substituter
        * @return The substituted spec.
        */
        public GadgetSpec substitute(Substitutions substituter)
        {
            GadgetSpec spec = new GadgetSpec(this);
            spec.modulePrefs = modulePrefs.substitute(substituter);
            if (userPrefs.Count == 0)
            {
                spec.userPrefs = new List<UserPref>();
            }
            else
            {
                List<UserPref> prefs = new List<UserPref>();
                foreach (UserPref pref in userPrefs)
                {
                    prefs.Add(pref.substitute(substituter));
                }
                spec.userPrefs = prefs;
            }
            Dictionary<String, View> viewMap = new Dictionary<String, View>(views.Count);
            foreach (View view in views.Values)
            {
                viewMap.Add(view.getName(), view.substitute(substituter));
            }
            spec.views = viewMap;

            return spec;
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<Module>\n")
                .Append(modulePrefs).Append('\n');
            foreach (UserPref pref in userPrefs)
            {
                buf.Append(pref).Append('\n');
            }
            foreach (var view in views)
            {
                buf.Append(view.Value).Append('\n');
            }
            buf.Append("</Module>");
            return buf.ToString();
        }

        /**
        * Creates a new Module from the given xml input.
        *
        * @param url
        * @param xml
        * @throws SpecParserException
        */
        public GadgetSpec(Uri url, String xml)
        {
            XmlElement doc;
            try
            {
                doc = XmlUtil.Parse(xml);
            }
            catch (XmlException e)
            {
                throw new SpecParserException("Malformed XML in file " + url.ToString(), e);
            }
            this.url = url;

            // This might not be good enough; should we take message bundle changes
            // into account?
            this.checksum = HashUtil.checksum(xml);

            XmlNodeList children = doc.ChildNodes;

            ModulePrefs modulePrefs = null;
            List<UserPref> userPrefs = new List<UserPref>();
            Dictionary<String, List<XmlElement>> views = new Dictionary<String, List<XmlElement>>();
            for (int i = 0, j = children.Count; i < j; ++i)
            {
                XmlNode child = children[i];
                if (!(child is XmlElement))
                {
                    continue;
                }
                XmlElement element = (XmlElement)child;
                String name = element.Name;
                if ("ModulePrefs".Equals(name))
                {
                    if (modulePrefs == null)
                    {
                        modulePrefs = new ModulePrefs(element, url);
                    }
                    else
                    {
                        throw new SpecParserException("Only 1 ModulePrefs is allowed.");
                    }
                }
                if ("UserPref".Equals(name))
                {
                    UserPref pref = new UserPref(element);
                    userPrefs.Add(pref);
                }
                if ("Content".Equals(name))
                {
                    String viewNames = XmlUtil.getAttribute(element, "view", "default");
                    foreach (String _view in viewNames.Split(','))
                    {
                        String view = _view.Trim();
                        List<XmlElement> viewElements = null;
                        if (!views.TryGetValue(view, out viewElements))
                        {
                            viewElements = new List<XmlElement>();
                            views.Add(view, viewElements);
                        }
                        viewElements.Add(element);
                    }
                }
            }

            if (modulePrefs == null)
            {
                throw new SpecParserException("At least 1 ModulePrefs is required.");
            }
            
            this.modulePrefs = modulePrefs;

            if (views.Count == 0)
            {
                throw new SpecParserException("At least 1 Content is required.");
            }
            
            Dictionary<String, View> tmpViews = new Dictionary<String, View>();
            foreach (var view in views)
            {
                View v = new View(view.Key, view.Value, url);
                tmpViews.Add(v.getName(), v);
            }
            this.views = tmpViews;
            

            if (userPrefs.Count != 0)
            {
                this.userPrefs = userPrefs;
            }
            else
            {
                this.userPrefs = new List<UserPref>();
            }
        }

        /**
        * Constructs a GadgetSpec for substitute calls.
        * @param spec
        */
        private GadgetSpec(GadgetSpec spec)
        {
            url = spec.url;
            checksum = spec.checksum;
        }
    }
}