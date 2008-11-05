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
using System.Web;
using System.Text.RegularExpressions;
using org.apache.shindig.gadgets;


namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class DefaultUrlGenerator : UrlGenerator
    {
        public readonly static DefaultUrlGenerator Instance = new DefaultUrlGenerator();
        private String jsPrefix;
        private String iframePrefix;
        private String jsChecksum;
        private static Regex ALLOWED_FEATURE_NAME = new Regex("[0-9a-zA-Z\\.\\-]+", RegexOptions.Compiled);

        /**
        * @param features The list of features that js is needed for.
        * @return The url for the bundled javascript that includes all referenced
        *    feature libraries.
        */
        public String getBundledJsUrl(ICollection<string> features, GadgetContext context)
        {
            return jsPrefix + getBundledJsParam(features, context);
        }

        /**
        * @param features
        * @param context
        * @return The bundled js parameter for type=url gadgets.
        */
        public String getBundledJsParam(ICollection<string> features, GadgetContext context)
        {
            StringBuilder buf = new StringBuilder();
            bool first = false;
            foreach (string feature in features)
            {
                if (ALLOWED_FEATURE_NAME.IsMatch(feature))
                {
                    if (!first)
                    {
                        first = true;
                    }
                    else
                    {
                        buf.Append("__");
                    }
                    buf.Append(feature);
                }
            }
            if (!first)
            {
                buf.Append("core");
            }
            buf.Append(".js?v=").Append(jsChecksum)
                .Append("&container=").Append(context.getContainer())
                .Append("&debug=").Append(context.getDebug() ? "1" : "0");
            return buf.ToString();
        }

        /**
        * Generates iframe urls for meta data service.
        * Use this rather than generating your own urls by hand.
        *
        * @param gadget
        * @return The generated iframe url.
        */
        public String getIframeUrl(Gadget gadget)
        {
            StringBuilder buf = new StringBuilder();
            GadgetContext context = gadget.Context;
            GadgetSpec spec = gadget.Spec;
            String url = context.getUrl().toString();
            View view = gadget.CurrentView;
            View.ContentType type;
            if (view == null)
            {
                type = View.ContentType.HTML;
            }
            else
            {
                type = view.getType();
            }
            switch (type.ToString())
            {
                case "URL":
                    // type = url
                    String href = view.getHref().ToString();
                    buf.Append(href);
                    if (href.IndexOf('?') == -1)
                    {
                        buf.Append('?');
                    }
                    else
                    {
                        buf.Append('&');
                    }
                    break;
                case "HTML":
                default:
                    buf.Append(iframePrefix);
                    break;
            }
            buf.Append("container=").Append(context.getContainer());
            if (context.getModuleId() != 0)
            {
                buf.Append("&mid=").Append(context.getModuleId());
            }
            if (context.getIgnoreCache())
            {
                buf.Append("&nocache=1");
            }
            else
            {
                buf.Append("&v=").Append(spec.getChecksum());
            }

            buf.Append("&lang=").Append(context.getLocale().getLanguage());
            buf.Append("&country=").Append(context.getLocale().getCountry());
            buf.Append("&view=").Append(context.getView());

            UserPrefs prefs = context.getUserPrefs();
            foreach (UserPref pref in gadget.Spec.getUserPrefs())
            {
                String name = pref.getName();
                String value = prefs.getPref(name);
                if (value == null)
                {
                    value = pref.getDefaultValue();
                }
                buf.Append("&up_").Append(HttpUtility.UrlEncode(pref.getName()))
                    .Append('=').Append(HttpUtility.UrlEncode(value));
            }
            // add url last to work around browser bugs
            if (!type.Equals(View.ContentType.URL))
            {
                buf.Append("&url=")
                    .Append(HttpUtility.UrlEncode(url));
            }
            return buf.ToString();
        }

        protected DefaultUrlGenerator()
        {
            this.iframePrefix = HttpRuntime.AppDomainAppVirtualPath + "/gadgets/ifr.ashx?";
            this.jsPrefix = HttpRuntime.AppDomainAppVirtualPath + "/gadgets/js/";
            GadgetFeatureRegistry registry = GadgetFeatureRegistry.Instance;

            StringBuilder jsBuf = new StringBuilder();
            foreach (GadgetFeature feature in registry.getAllFeatures())
            {
                foreach (JsLibrary library in feature.getJsLibraries(null, null))
                {
                    jsBuf.Append(library.Content);
                }
            }
            jsChecksum = HashUtil.checksum(UTF8Encoding.UTF8.GetBytes(jsBuf.ToString()));
        }
    } 
}

