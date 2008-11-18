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
using Locale = java.util.Locale;

using System.Text;

namespace Pesta
{
    /// <summary>
    /// Summary description for ModulePrefs
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ModulePrefs
    {
        private static readonly String ATTR_TITLE = "title";
        private static readonly String ATTR_TITLE_URL = "title_url";
        private static readonly String ATTR_DESCRIPTION = "description";
        private static readonly String ATTR_AUTHOR = "author";
        private static readonly String ATTR_AUTHOR_EMAIL = "author_email";
        private static readonly String ATTR_SCREENSHOT = "screenshot";
        private static readonly String ATTR_THUMBNAIL = "thumbnail";
        private static readonly String ATTR_DIRECTORY_TITLE = "directory_title";
        private static readonly String ATTR_AUTHOR_AFFILIATION = "author_affiliation";
        private static readonly String ATTR_AUTHOR_LOCATION = "author_location";
        private static readonly String ATTR_AUTHOR_PHOTO = "author_photo";
        private static readonly String ATTR_AUTHOR_ABOUTME = "author_aboutme";
        private static readonly String ATTR_AUTHOR_QUOTE = "author_quote";
        private static readonly String ATTR_AUTHOR_LINK = "author_link";
        private static readonly String ATTR_SHOW_STATS = "show_stats";
        private static readonly String ATTR_SHOW_IN_DIRECTORY = "show_in_directory";
        private static readonly String ATTR_SINGLETON = "singleton";
        private static readonly String ATTR_SCALING = "scaling";
        private static readonly String ATTR_SCROLLING = "scrolling";
        private static readonly String ATTR_WIDTH = "width";
        private static readonly String ATTR_HEIGHT = "height";
        private static readonly String ATTR_CATEGORY = "category";
        private static readonly String ATTR_CATEGORY2 = "category2";

        private static readonly Uri EMPTY_URI = Uri.parse("");

        private readonly Dictionary<String, String> attributes;

        private readonly Uri _base;

        // Canonical spec items first.

        /**
         * ModulePrefs@title
         *
         * User Pref + Message Bundle + Bidi
         */
        public String getTitle()
        {
            return getAttribute(ATTR_TITLE);
        }

        /**
         * ModulePrefs@title_url
         *
         * User Pref + Message Bundle + Bidi
         */
        public Uri getTitleUrl()
        {
            return getUriAttribute(ATTR_TITLE_URL);
        }

        /**
         * ModulePrefs@description
         *
         * Message Bundles
         */
        public String getDescription()
        {
            return getAttribute(ATTR_DESCRIPTION);
        }

        /**
         * ModulePrefs@author
         *
         * Message Bundles
         */
        public String getAuthor()
        {
            return getAttribute(ATTR_AUTHOR);
        }

        /**
         * ModulePrefs@author_email
         *
         * Message Bundles
         */
        public String getAuthorEmail()
        {
            return getAttribute(ATTR_AUTHOR_EMAIL);
        }

        /**
         * ModulePrefs@screenshot
         *
         * Message Bundles
         */
        public Uri getScreenshot()
        {
            return getUriAttribute(ATTR_SCREENSHOT);
        }

        /**
         * ModulePrefs@thumbnail
         *
         * Message Bundles
         */
        public Uri getThumbnail()
        {
            return getUriAttribute(ATTR_THUMBNAIL);
        }

        // Extended data (typically used by directories)

        /**
         * ModulePrefs@directory_title
         *
         * Message Bundles
         */
        public String getDirectoryTitle()
        {
            return getAttribute(ATTR_DIRECTORY_TITLE);
        }

        /**
         * ModulePrefs@author_affiliation
         *
         * Message Bundles
         */
        public String getAuthorAffiliation()
        {
            return getAttribute(ATTR_AUTHOR_AFFILIATION);
        }

        /**
         * ModulePrefs@author_location
         *
         * Message Bundles
         */
        public String getAuthorLocation()
        {
            return getAttribute(ATTR_AUTHOR_LOCATION);
        }

        /**
         * ModulePrefs@author_photo
         *
         * Message Bundles
         */
        public Uri getAuthorPhoto()
        {
            return getUriAttribute(ATTR_AUTHOR_PHOTO);
        }

        /**
         * ModulePrefs@author_aboutme
         *
         * Message Bundles
         */
        public String getAuthorAboutme()
        {
            return getAttribute(ATTR_AUTHOR_ABOUTME);
        }

        /**
         * ModulePrefs@author_quote
         *
         * Message Bundles
         */
        public String getAuthorQuote()
        {
            return getAttribute(ATTR_AUTHOR_QUOTE);
        }

        /**
         * ModulePrefs@author_link
         *
         * Message Bundles
         */
        public Uri getAuthorLink()
        {
            return getUriAttribute(ATTR_AUTHOR_LINK);
        }

        /**
         * ModulePrefs@show_stats
         */
        public bool getShowStats()
        {
            return getBoolAttribute(ATTR_SHOW_STATS);
        }

        /**
         * ModulePrefs@show_in_directory
         */
        public bool getShowInDirectory()
        {
            return getBoolAttribute(ATTR_SHOW_IN_DIRECTORY);
        }

        /**
         * ModulePrefs@singleton
         */
        public bool getSingleton()
        {
            return getBoolAttribute(ATTR_SINGLETON);
        }

        /**
         * ModulePrefs@scaling
         */
        public bool getScaling()
        {
            return getBoolAttribute(ATTR_SCALING);
        }

        /**
         * ModulePrefs@scrolling
         */
        public bool getScrolling()
        {
            return getBoolAttribute(ATTR_SCROLLING);
        }

        /**
         * ModuleSpec@width
         */
        public int getWidth()
        {
            return getIntAttribute(ATTR_WIDTH);
        }

        /**
         * ModuleSpec@height
         */
        public int getHeight()
        {
            return getIntAttribute(ATTR_HEIGHT);
        }

        /**
         * @return the value of an ModulePrefs attribute by name, or null if the
         *     attribute doesn't exist
         */
        public String getAttribute(String name)
        {
            string retval = "";
            attributes.TryGetValue(name, out retval);
            return retval;
        }

        /**
         * @return the value of an ModulePrefs attribute by name, or the default
         *     value if the attribute doesn't exist
         */
        public String getAttribute(String name, String defaultValue)
        {
            String value = getAttribute(name);
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value;
            }
        }

        /**
         * @return the attribute by name converted to an URI, or the empty URI if the
         *    attribute couldn't be converted
         */
        public Uri getUriAttribute(String name)
        {
            String uriAttribute = getAttribute(name);
            if (uriAttribute != null)
            {
                try
                {
                    Uri uri = Uri.parse(uriAttribute);
                    return _base.resolve(uri);
                }
                catch 
                {
                    return EMPTY_URI;
                }
            }
            return EMPTY_URI;
        }

        /**
         * @return the attribute by name converted to a bool (false if the
         *     attribute doesn't exist)
         */
        public bool getBoolAttribute(String name)
        {
            String value = getAttribute(name);
            return !(value == null || "false".Equals(value));
        }

        /**
         * @return the attribute by name converted to an interger, or 0 if the
         *     attribute doesn't exist
         */
        public int getIntAttribute(String name)
        {
            String value = getAttribute(name);
            int valInt = 0;
            if (value != null)
            {
               int.TryParse(value, out valInt);
            }
            return valInt;
        }

        /**
         * ModuleSpec@category
         * ModuleSpec@category2
         * These fields are flattened into a single list.
         */
        private readonly List<String> categories;
        public List<String> getCategories()
        {
            return categories;
        }

        /**
         * ModuleSpec/Require
         * ModuleSpec/Optional
         */
        private readonly Dictionary<String, Feature> features;
        public Dictionary<String, Feature> getFeatures()
        {
            return features;
        }

        /**
         * ModuleSpec/Preload
         */
        private readonly List<Preload> preloads;
        public List<Preload> getPreloads()
        {
            return preloads;
        }

        /**
         * ModuleSpec/Icon
         */
        private readonly List<Icon> icons;
        public List<Icon> getIcons()
        {
            return icons;
        }

        /**
         * ModuleSpec/Locale
         */
        private readonly Dictionary<Locale, LocaleSpec> locales;
        public Dictionary<Locale, LocaleSpec> getLocales()
        {
            return locales;
        }

        /**
         * ModuleSpec/Link
         */
        private readonly Dictionary<String, LinkSpec> links;
        public Dictionary<String, LinkSpec> getLinks()
        {
            return links;
        }

        /**
         * ModuleSpec/OAuthSpec
         */
        private readonly OAuthSpec oauth;
        public OAuthSpec getOAuthSpec()
        {
            return oauth;
        }

        /**
         * Attempts to retrieve a valid LocaleSpec for the given Locale.
         * First tries to find an exact language / country match.
         * Then tries to find a match for language / all.
         * Then tries to find a match for all / all.
         * Finally gives up.
         * @param locale
         * @return The locale spec, if there is a matching one, or null.
         */
        public LocaleSpec getLocale(Locale locale)
        {
            if (locales.Count == 0)
            {
                return null;
            }
            LocaleSpec localeSpec = null;
            if (!locales.TryGetValue(locale, out localeSpec))
            {
                locale = new Locale(locale.getLanguage(), "ALL");
                if (!locales.TryGetValue(locale, out localeSpec))
                {
                    locales.TryGetValue(GadgetSpec.DEFAULT_LOCALE, out localeSpec);
                }
            }

            return localeSpec;
        }

        /**
         * Produces a new ModulePrefs by substituting hangman variables from
         * substituter. See comments on individual fields to see what actually
         * has substitutions performed.
         *
         * @param substituter
         */
        public ModulePrefs substitute(Substitutions substituter)
        {
            return new ModulePrefs(this, substituter);
        }


        /**
         * Walks child nodes of the given node.
         * @param element
         * @param visitors Map of tag names to visitors for that tag.
         */
        private static void walk(XmlElement element, Dictionary<String, ElementVisitor> visitors)
        {
            XmlNodeList children = element.ChildNodes;
            for (int i = 0, j = children.Count; i < j; ++i)
            {
                XmlNode child = children[i];
                ElementVisitor visitor = visitors[child.Name];
                if (visitor != null)
                {
                    visitor.visit((XmlElement)child);
                }
            }
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<ModulePrefs");

            foreach (var attr in attributes)
            {
                buf.Append(' ').Append(attr.Key).Append("=\"")
                    .Append(attr.Value).Append('\"');
            }
            buf.Append(">\n");

            foreach (Preload preload in preloads)
            {
                buf.Append(preload).Append('\n');
            }
            foreach (Feature feature in features.Values)
            {
                buf.Append(feature).Append('\n');
            }
            foreach (Icon icon in icons)
            {
                buf.Append(icon).Append('\n');
            }
            foreach (LocaleSpec locale in locales.Values)
            {
                buf.Append(locale).Append('\n');
            }
            foreach (LinkSpec link in links.Values)
            {
                buf.Append(link).Append('\n');
            }
            if (oauth != null)
            {
                buf.Append(oauth).Append('\n');
            }
            buf.Append("</ModulePrefs>");
            return buf.ToString();
        }

        /**
         * @param element
         * @param specUrl
         */
        public ModulePrefs(XmlElement element, Uri inbase)
        {
            this._base = inbase;
            attributes = new Dictionary<String, String>();
            XmlNamedNodeMap attributeNodes = element.Attributes;
            for (int i = 0; i < attributeNodes.Count; i++)
            {
                XmlNode node = attributeNodes.Item(i);
                attributes.Add(node.Name, node.Value);
            }

            if (getTitle() == null)
            {
                throw new SpecParserException("ModulePrefs@title is required.");
            }

            categories = new List<string>() { getAttribute(ATTR_CATEGORY, ""), getAttribute(ATTR_CATEGORY2, "") };

            // Child elements
            PreloadVisitor preloadVisitor = new PreloadVisitor(_base);
            FeatureVisitor featureVisitor = new FeatureVisitor(_base);
            OAuthVisitor oauthVisitor = new OAuthVisitor(_base);
            IconVisitor iconVisitor = new IconVisitor(_base);
            LocaleVisitor localeVisitor = new LocaleVisitor(_base);
            LinkVisitor linkVisitor = new LinkVisitor(_base);

            Dictionary<String, ElementVisitor> visitors = new Dictionary<String, ElementVisitor>();
            visitors.Add("Preload", preloadVisitor);
            visitors.Add("Optional", featureVisitor);
            visitors.Add("Require", featureVisitor);
            visitors.Add("OAuth", oauthVisitor);
            visitors.Add("Icon", iconVisitor);
            visitors.Add("Locale", localeVisitor);
            visitors.Add("Link", linkVisitor);

            walk(element, visitors);

            preloads = preloadVisitor.preloaded;
            features = featureVisitor.features;
            icons = iconVisitor.icons;
            locales = localeVisitor.localeMap;
            links = linkVisitor.linkMap;
            oauth = oauthVisitor.oauthSpec;
        }

        /**
         * Produces a new, substituted ModulePrefs
         */
        private ModulePrefs(ModulePrefs prefs, Substitutions substituter)
        {
            _base = prefs._base;
            categories = prefs.getCategories();
            features = prefs.getFeatures();
            locales = prefs.getLocales();
            oauth = prefs.oauth;

            List<Preload> preloads = new List<Preload>();
            if (prefs.preloads != null)
            {
                foreach (Preload preload in prefs.preloads)
                {
                    preloads.Add(preload.substitute(substituter));
                }
            }
            this.preloads = preloads;

            List<Icon> icons = new List<Icon>(prefs.icons.Count);
            foreach (Icon icon in prefs.icons)
            {
                icons.Add(icon.substitute(substituter));
            }
            this.icons = icons;

            Dictionary<String, LinkSpec> links = new Dictionary<String, LinkSpec>(prefs.links.Count);
            foreach (LinkSpec link in prefs.links.Values)
            {
                LinkSpec sub = link.substitute(substituter);
                links.Add(sub.getRel(), sub);
            }
            this.links = links;

            Dictionary<String, String> attributes = new Dictionary<String, String>(prefs.attributes.Count);
            foreach (var attr in prefs.attributes)
            {
                String substituted = substituter.substituteString(null, attr.Value);
                attributes.Add(attr.Key, substituted);
            }
            this.attributes = attributes;
        }
    }

    interface ElementVisitor
    {
        void visit(XmlElement element);
    }

    /**
     * Processes ModulePrefs/Preload into a list.
     */
    class PreloadVisitor : ElementVisitor
    {
        public List<Preload> preloaded;
        Uri _base = null;
        public void visit(XmlElement element)
        {
            Preload preload = new Preload(element, _base);
            if (preloaded == null)
            {
                preloaded = new List<Preload>();
            }
            preloaded.Add(preload);
        }
        /// <summary>
        /// Initializes a new instance of the PreloadVisitor structure.
        /// </summary>
        public PreloadVisitor(Uri url)
        {
            _base = url;
        }
    }

    /**
     * Process ModulePrefs/OAuth
     */
    class OAuthVisitor : ElementVisitor
    {
        Uri _base = null;
        public OAuthSpec oauthSpec;
        public void visit(XmlElement element)
        {
            if (oauthSpec != null)
            {
                throw new SpecParserException("ModulePrefs/OAuth may only occur once.");
            }
            oauthSpec = new OAuthSpec(element, _base);
        }
        /// <summary>
        /// Initializes a new instance of the OAuthVisitor structure.
        /// </summary>
        public OAuthVisitor(Uri url)
        {
            _base = url;
            this.oauthSpec = null;
        }
    }

    /**
     * Processes ModulePrefs/Require and ModulePrefs/Optional
     */
    class FeatureVisitor : ElementVisitor
    {
        Uri _base = null;
        public readonly Dictionary<String, Feature> features = new Dictionary<String, Feature>();
        public void visit(XmlElement element)
        {
            Feature feature = new Feature(element);
            features.Add(feature.getName(), feature);
        }
        /// <summary>
        /// Initializes a new instance of the FeatureVisitor structure.
        /// </summary>
        public FeatureVisitor(Uri url)
        {
            _base = url;
        }
    }

    /**
     * Processes ModulePrefs/Icon
     */
    class IconVisitor : ElementVisitor
    {
        Uri _base = null;
        public readonly List<Icon> icons = new List<Icon>();
        public void visit(XmlElement element)
        {
            icons.Add(new Icon(element));
        }
        /// <summary>
        /// Initializes a new instance of the IconVisitor structure.
        /// </summary>
        public IconVisitor(Uri url)
        {
            _base = url;
        }
    }

    /**
     * Process ModulePrefs/Locale
     */
    class LocaleVisitor : ElementVisitor
    {
        Uri _base = null;
        public readonly Dictionary<Locale, LocaleSpec> localeMap = new Dictionary<Locale, LocaleSpec>();
        public void visit(XmlElement element)
        {
            LocaleSpec locale = new LocaleSpec(element, _base);
            localeMap[new Locale(locale.getLanguage(), locale.getCountry())] = locale;
        }
        /// <summary>
        /// Initializes a new instance of the LocaleVisitor structure.
        /// </summary>
        public LocaleVisitor(Uri url)
        {
            _base = url;
        }
    }

    /**
     * Process ModulePrefs/Link
     */
    class LinkVisitor : ElementVisitor
    {
        Uri _base = null;
        public readonly Dictionary<String, LinkSpec> linkMap = new Dictionary<String, LinkSpec>();

        public void visit(XmlElement element)
        {
            LinkSpec link = new LinkSpec(element, _base);
            linkMap.Add(link.getRel(), link);
        }
        /// <summary>
        /// Initializes a new instance of the LinkVisitor structure.
        /// </summary>
        public LinkVisitor(Uri url)
        {
            _base = url;
        }
    } 
}
