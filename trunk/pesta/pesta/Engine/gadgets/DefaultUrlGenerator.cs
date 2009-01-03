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
using Pesta.Engine.common;
using Pesta.Engine.common.util;
using Pesta.Engine.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;
using UriBuilder=Pesta.Engine.common.uri.UriBuilder;


namespace Pesta.Engine.gadgets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class DefaultUrlGenerator : UrlGenerator
    {
        protected static readonly Regex ALLOWED_FEATURE_NAME = new Regex("[0-9a-zA-Z\\.\\-]+", RegexOptions.Compiled);
        protected static readonly String IFRAME_URI_PARAM = "gadgets.iframeBaseUri";
        protected static readonly String JS_URI_PARAM = "gadgets.jsUriTemplate";
        private readonly String jsChecksum;
        private readonly Dictionary<String, Uri> iframeBaseUris;
        private readonly Dictionary<String, String> jsUriTemplates;
        private readonly LockedDomainService lockedDomainService;
        public readonly static DefaultUrlGenerator Instance = new DefaultUrlGenerator();
        protected DefaultUrlGenerator()
        {
            ContainerConfig containerConfig = JsonContainerConfig.Instance;
            LockedDomainService lockedDomainService = HashLockedDomainService.Instance;
            GadgetFeatureRegistry registry = GadgetFeatureRegistry.Instance;

            iframeBaseUris = new Dictionary<string,Uri>();
            jsUriTemplates = new Dictionary<string,string>();
            foreach (String container in containerConfig.getContainers())
            {
                iframeBaseUris.Add(container, Uri.parse(containerConfig.get(container, IFRAME_URI_PARAM)));
                jsUriTemplates.Add(container, containerConfig.get(container, JS_URI_PARAM));
            }

            this.lockedDomainService = lockedDomainService;

            StringBuilder jsBuf = new StringBuilder();
            foreach (GadgetFeature feature in registry.getAllFeatures())
            {
                foreach(JsLibrary library in feature.getJsLibraries(null, null)) 
                {
                    jsBuf.Append(library.Content);
                }
            }
            jsChecksum = HashUtil.checksum(jsBuf.ToString());

        }

        public String getBundledJsUrl(ICollection<String> features, GadgetContext context) 
        {
            String jsPrefix;
            if (!jsUriTemplates.TryGetValue(context.getContainer(), out jsPrefix)) 
            {
                return "";
            }

            return jsPrefix.Replace("%host%", context.getHost())
                .Replace("%js%", getBundledJsParam(features, context));
        }

        public String getBundledJsParam(ICollection<String> features, GadgetContext context) 
        {
            StringBuilder buf = new StringBuilder();
            bool first = false;
            foreach (String feature in features) 
            {
                if (ALLOWED_FEATURE_NAME.Match(feature).Success)
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
        * TODO: This is in need of a rewrite most likely. It doesn't even take locked domain into
        * consideration!
        */
        public String getIframeUrl(Gadget gadget) 
        {
            GadgetContext context = gadget.getContext();
            GadgetSpec spec = gadget.getSpec();
            String url = context.getUrl().ToString();
            View view = gadget.getCurrentView();
            View.ContentType type;
            if (view == null) 
            {
                type = View.ContentType.HTML;
            } 
            else
            {
                type = view.getType();
            }

            UriBuilder uri;
            if (type == View.ContentType.URL)
            {
                uri = new UriBuilder(view.getHref());
            }
            else
            {
                // TODO: Locked domain support.
                Uri iframeBaseUri;
                uri = iframeBaseUris.TryGetValue(context.getContainer(), out iframeBaseUri) ? new UriBuilder(iframeBaseUri) : new UriBuilder();
                String host = lockedDomainService.getLockedDomainForGadget(spec, context.getContainer());
                if (host != null) 
                {
                    uri.setAuthority(host);
                }
            }

            uri.addQueryParameter("container", context.getContainer());
            if (context.getModuleId() != 0) 
            {
                uri.addQueryParameter("mid", context.getModuleId().ToString());
            }
            if (context.getIgnoreCache())
            {
                uri.addQueryParameter("nocache", "1");
            } 
            else
            {
                uri.addQueryParameter("v", spec.getChecksum());
            }

            uri.addQueryParameter("lang", context.getLocale().getLanguage());
            uri.addQueryParameter("country", context.getLocale().getCountry());
            uri.addQueryParameter("view", context.getView());

            UserPrefs prefs = context.getUserPrefs();
            foreach(UserPref pref in gadget.getSpec().getUserPrefs()) 
            {
                String name = pref.getName();
                String value = prefs.getPref(name);
                if (value == null) 
                {
                    value = pref.getDefaultValue();
                }
                uri.addQueryParameter("up_" + pref.getName(), value);
            }
            // add url last to work around browser bugs
            if(!type.Equals(View.ContentType.URL))
            {
                uri.addQueryParameter("url", url);
            }

            return uri.ToString();
        }
    }
}