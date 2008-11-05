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
using System.Text.RegularExpressions;

namespace Pesta
{
    /// <summary>
    /// Summary description for ContentRewriterFeature
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ContentRewriterFeature
    {
        private static readonly String INCLUDE_URLS = "include-urls";
        private static readonly String EXCLUDE_URLS = "exclude-urls";
        private static readonly String INCLUDE_TAGS = "include-tags";
        private static readonly String EXPIRES = "expires";

        public static readonly String EXPIRES_DEFAULT = "HTTP";

        // Use tree set to maintain order for fingerprint
        private HashSet<String> includeTags;

        private bool includeAll;
        private bool includeNone;

        private Regex include;
        private Regex exclude;

        // If null then dont enforce a min TTL for proxied content. Use contents headers
        private int? expires;

        private int? fingerprint;

        /**
         * Constructor which takes a gadget spec and the default container settings
         *
         * @param spec
         * @param defaultInclude As a regex
         * @param defaultExclude As a regex
         * @param defaultExpires Either "HTTP" or a ttl in seconds
         * @param defaultTags    Set of default tags that can be rewritten
         */
        public ContentRewriterFeature(GadgetSpec spec, String defaultInclude,
                                    String defaultExclude,
                                    String defaultExpires,
                                    HashSet<String> defaultTags)
        {
            Feature f = null;
            spec.getModulePrefs().getFeatures().TryGetValue("content-rewrite", out f);
            String includeRegex = normalizeParam(defaultInclude, null);
            String excludeRegex = normalizeParam(defaultExclude, null);

            this.includeTags = new HashSet<String>(defaultTags);

            List<String> expiresOptions = new List<string>(3);
            if (f != null)
            {
                if (f.getParams().ContainsKey(INCLUDE_URLS))
                {
                    includeRegex = normalizeParam(f.getParams()[INCLUDE_URLS], includeRegex);
                }

                // Note use of default for exclude as null here to allow clearing value in the
                // presence of a container default.
                if (f.getParams().ContainsKey(EXCLUDE_URLS))
                {
                    excludeRegex = normalizeParam(f.getParams()[EXCLUDE_URLS], null);
                }
                String includeTagList = f.getParams()[INCLUDE_TAGS];
                if (includeTagList != null)
                {
                    HashSet<String> tags = new HashSet<String>();
                    foreach (String tag in includeTagList.Split(','))
                    {
                        if (tag != null)
                        {
                            tags.Add(tag.Trim().ToLower());
                        }
                    }
                    includeTags = tags;
                }

                if (f.getParams().ContainsKey(EXPIRES))
                {
                    expiresOptions.Add(normalizeParam(f.getParams()[EXPIRES], null));
                }
            }

            expiresOptions.Add(defaultExpires);
            expiresOptions.Add(EXPIRES_DEFAULT);

            foreach (String expiryOption in expiresOptions)
            {
                try
                {
                    expires = int.Parse(expiryOption);
                    break;
                }
                catch
                {
                    // Not an integer
                    if (EXPIRES_DEFAULT.ToLower().Equals(expiryOption))
                    {
                        break;
                    }
                }
            }

            if (".*".Equals(includeRegex) && excludeRegex == null)
            {
                includeAll = true;
            }

            if (".*".Equals(excludeRegex) || includeRegex == null)
            {
                includeNone = true;
            }

            if (includeRegex != null)
            {
                include = new Regex(includeRegex);
            }
            if (excludeRegex != null)
            {
                exclude = new Regex(excludeRegex);
            }
        }

        private String normalizeParam(String paramValue, String defaultVal)
        {
            if (paramValue == null)
            {
                return defaultVal;
            }
            paramValue = paramValue.Trim();
            if (paramValue.Length == 0)
            {
                return defaultVal;
            }
            return paramValue;
        }

        public bool isRewriteEnabled()
        {
            return !includeNone;
        }

        public bool shouldRewriteURL(String url)
        {
            if (includeNone)
            {
                return false;
            }
            else if (includeAll)
            {
                return true;
            }
            else if (include.Match(url).Success)
            {
                return !(exclude != null && exclude.Match(url).Success);
            }
            return false;
        }

        public bool shouldRewriteTag(String tag)
        {
            if (tag != null)
            {
                return this.includeTags.Contains(tag.ToLower());
            }
            return false;
        }

        public HashSet<String> getIncludedTags()
        {
            return includeTags;
        }

        /**
        * @return the min TTL to enforce or null if proxy should respect headers
        */
        public int? getExpires()
        {
            return expires;
        }

        /**
        * @return fingerprint of rewriting rule for cache-busting
        */
        public int? getFingerprint()
        {
            if (fingerprint == null)
            {
                int result;
                result = (include != null ? include.GetHashCode() : 0);
                result = 31 * result + (exclude != null ? exclude.GetHashCode() : 0);
                foreach (String s in includeTags)
                {
                    result = 31 * result + s.GetHashCode();
                }
                fingerprint = result;
            }
            return fingerprint;
        }

        public class Factory
        {
            private readonly String defaultIncludeUrls;
            private readonly String defaultExcludeUrls;
            private readonly String defaultExpires;
            private readonly HashSet<String> defaultIncludeTags;

            public Factory(String includeUrls, String excludeUrls, String expires, HashSet<String> includeTags)
            {
                defaultIncludeUrls = includeUrls;
                defaultExcludeUrls = excludeUrls;
                defaultExpires = expires;
                defaultIncludeTags = includeTags;
            }

            public ContentRewriterFeature get(GadgetSpec spec)
            {
                ContentRewriterFeature rewriterFeature =
                            (ContentRewriterFeature)spec.getAttribute("content-rewrite");
                if (rewriterFeature == null)
                {
                    rewriterFeature = new ContentRewriterFeature(spec, defaultIncludeUrls,
                                defaultExcludeUrls, defaultExpires, defaultIncludeTags);
                    spec.setAttribute("content-rewrite", rewriterFeature);
                }
                return rewriterFeature;
            }
        }
    } 
}
