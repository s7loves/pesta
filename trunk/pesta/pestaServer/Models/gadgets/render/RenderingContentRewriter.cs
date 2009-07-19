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
using System.Linq;
using System.Text;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using org.apache.shindig.gadgets.rewrite;
using org.apache.shindig.common.xml;
using org.w3c.dom;
using Pesta.Engine.auth;
using Pesta.Utilities;
using pestaServer.Models.common;
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.preload;
using pestaServer.Models.gadgets.spec;
using ContentRewriter = pestaServer.Models.gadgets.rewrite.IContentRewriter;
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets.render
{
    public class RenderingContentRewriter : ContentRewriter
    {
        
        private const String DEFAULT_CSS =
                "body,td,div,span,p{font-family:arial,sans-serif;}" +
                "a {color:#0000cc;}a:visited {color:#551a8b;}" +
                "a:active {color:#ff0000;}" +
                "body{margin: 0px;padding: 0px;background-color:white;}";

        private const string INSERT_BASE_ELEMENT_KEY = "gadgets.insertBaseElement";
        private const string FEATURES_KEY = "gadgets.features";

        private readonly MessageBundleFactory messageBundleFactory;
        private readonly ContainerConfig containerConfig;
        private readonly GadgetFeatureRegistry featureRegistry;
        private readonly UrlGenerator urlGenerator;

        /**
       * @param messageBundleFactory Used for injecting message bundles into gadget output.
       */
        public RenderingContentRewriter()
        {
            messageBundleFactory = DefaultMessageBundleFactory.Instance;
            containerConfig = JsonContainerConfig.Instance;
            featureRegistry = GadgetFeatureRegistry.Instance;
            urlGenerator = DefaultUrlGenerator.Instance;
        }

        public RewriterResults rewrite(sRequest req, sResponse resp,  MutableContent content) 
        {
            return null;
        }

        public RewriterResults rewrite(Gadget gadget, MutableContent mutableContent)
        {
            Document document = mutableContent.getDocument();

            Element head = (Element)DomUtil.getFirstNamedChildNode(document.getDocumentElement(), "head");

            // Remove all the elements currently in head and add them back after we inject content
            NodeList children = head.getChildNodes();
            List<Node> existingHeadContent = new List<Node>(children.getLength());
            for (int i = 0; i < children.getLength(); i++) 
            {
                existingHeadContent.Add(children.item(i));
            }

            foreach(Node n in existingHeadContent) 
            {
                head.removeChild(n);
            }

            // Only inject default styles if no doctype was specified.
            if (document.getDoctype() == null) 
            {
                Element defaultStyle = document.createElement("style");
                defaultStyle.setAttribute("type", "text/css");
                head.appendChild(defaultStyle);
                defaultStyle.appendChild(defaultStyle.getOwnerDocument().
                                             createTextNode(DEFAULT_CSS));
            }

            InjectBaseTag(gadget, head);
            InjectFeatureLibraries(gadget, head);

            // This can be one script block.
            Element mainScriptTag = document.createElement("script");
            InjectMessageBundles(gadget, mainScriptTag);
            InjectDefaultPrefs(gadget, mainScriptTag);
            InjectPreloads(gadget, mainScriptTag);

            // We need to inject our script before any developer scripts.
            head.appendChild(mainScriptTag);

            Element body = (Element)DomUtil.getFirstNamedChildNode(document.getDocumentElement(), "body");

            LocaleSpec localeSpec = gadget.getLocale();
            if (localeSpec != null) {
                body.setAttribute("dir", localeSpec.getLanguageDirection());
            }

            // re append head content
            foreach(Node node in existingHeadContent)
            {
                head.appendChild(node);
            }

            InjectOnLoadHandlers(body);

            mutableContent.documentChanged();
            return RewriterResults.notCacheable();
        }

        private void InjectBaseTag(Gadget gadget, Node headTag)
        {
            GadgetContext context = gadget.getContext();
            if ("true".Equals(containerConfig.Get(context.getContainer(), INSERT_BASE_ELEMENT_KEY))) 
            {
                Uri baseUrl = gadget.getSpec().getUrl();
                View view = gadget.getCurrentView();
                if (view != null && view.getHref() != null) 
                {
                    baseUrl = view.getHref();
                }
                Element baseTag = headTag.getOwnerDocument().createElement("base");
                baseTag.setAttribute("href", baseUrl.ToString());
                headTag.insertBefore(baseTag, headTag.getFirstChild());
            }
        }

        private static void InjectOnLoadHandlers(Node bodyTag) 
        {
            Element onloadScript = bodyTag.getOwnerDocument().createElement("script");
            bodyTag.appendChild(onloadScript);
            onloadScript.appendChild(bodyTag.getOwnerDocument().createTextNode(
                "gadgets.util.runOnLoadHandlers();"));
        }

        /// <summary>
        /// Injects javascript libraries needed to satisfy feature dependencies.
        /// </summary>
        /// <param name="gadget"></param>
        /// <param name="headTag"></param>
        private void InjectFeatureLibraries(Gadget gadget, Node headTag)
        {
            // TODO: If there isn't any js in the document, we can skip this. Unfortunately, that means
            // both script tags (easy to detect) and event handlers (much more complex).
            GadgetContext context = gadget.getContext();
            GadgetSpec spec = gadget.getSpec();
            String forcedLibs = context.getParameter("libs");
            HashKey<String> forced;
            if (string.IsNullOrEmpty(forcedLibs)) 
            {
                forced = new HashKey<string>();
            } 
            else 
            {
                forced = new HashKey<string>();
                foreach (var item in forcedLibs.Split(':'))
                {
                    forced.Add(item);
                }
            }


            // Forced libs are always done first.
            if (forced.Count != 0) 
            {
                String jsUrl = urlGenerator.getBundledJsUrl(forced, context);
                Element libsTag = headTag.getOwnerDocument().createElement("script");
                libsTag.setAttribute("src", jsUrl);
                headTag.appendChild(libsTag);

                // Forced transitive deps need to be added as well so that they don't get pulled in twice.
                // TODO: Figure out a clean way to avoid having to call getFeatures twice.
                foreach(GadgetFeature dep in featureRegistry.GetFeatures(forced)) 
                {
                    forced.Add(dep.getName());
                }
            }

            // Inline any libs that weren't forced. The ugly context switch between inline and external
            // Js is needed to allow both inline and external scripts declared in feature.xml.
            String container = context.getContainer();
            ICollection<GadgetFeature> features = GetFeatures(spec, forced);

            // Precalculate the maximum length in order to avoid excessive garbage generation.
            int size = 0;
            foreach(GadgetFeature feature in features) 
            {
                foreach(JsLibrary library in feature.getJsLibraries(RenderingContext.GADGET, container))
                {
                    if (library._Type == JsLibrary.Type.URL)
                    {
                        size += library.Content.Length;
                    }
                }
            }

            // Really inexact.
            StringBuilder inlineJs = new StringBuilder(size);

            foreach (GadgetFeature feature in features)
            {
                foreach (JsLibrary library in feature.getJsLibraries(RenderingContext.GADGET, container))
                {
                    if (library._Type == JsLibrary.Type.URL)
                    {
                        if (inlineJs.Length > 0)
                        {
                            Element inlineTag = headTag.getOwnerDocument().createElement("script");
                            headTag.appendChild(inlineTag);
                            inlineTag.appendChild(headTag.getOwnerDocument().createTextNode(inlineJs.ToString()));
                            inlineJs.Length = 0;
                        }
                        Element referenceTag = headTag.getOwnerDocument().createElement("script");
                        referenceTag.setAttribute("src", library.Content);
                        headTag.appendChild(referenceTag);
                    }
                    else
                    {
                        if (!forced.Contains(feature.getName()))
                        {
                            // already pulled this file in from the shared contents.
                            if (context.getDebug())
                            {
                                inlineJs.Append(library.DebugContent);
                            }
                            else
                            {
                                inlineJs.Append(library.Content);
                            }
                            inlineJs.Append(";\n");
                        }
                    }
                }
            }

            inlineJs.Append(GetLibraryConfig(gadget, features));

            if (inlineJs.Length > 0) 
            {
                Element inlineTag = headTag.getOwnerDocument().createElement("script");
                headTag.appendChild(inlineTag);
                inlineTag.appendChild(headTag.getOwnerDocument().createTextNode(inlineJs.ToString()));
            }
        }

        /// <summary>
        /// Get all features needed to satisfy this rendering request.
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="forced">Forced libraries; added in addition to those found in the spec. Defaults to "core"</param>
        /// <returns></returns>
        private ICollection<GadgetFeature> GetFeatures(GadgetSpec spec, ICollection<String> forced)
        {
            Dictionary<String, Feature> features = spec.getModulePrefs().getFeatures();
            HashKey<String> libs = new HashKey<string>();
            foreach (var item in features.Keys)
            {
                libs.Add(item);
            }
            if (forced.Count != 0) 
            {
                foreach (var item in forced)
                {
                    libs.Add(item);
                }
            }

            HashSet<String> unsupported = new HashSet<String>();
            ICollection<GadgetFeature> feats = featureRegistry.GetFeatures(libs, unsupported);
            foreach (var item in forced)
            {
                unsupported.Remove(item);
            }

            if (unsupported.Count != 0)
            {
                // Remove non-required libs
                unsupported.RemoveWhere(x => !features[x].getRequired());

                // Throw error with full list of unsupported libraries
                if (unsupported.Count != 0) 
                {
                    throw new UnsupportedFeatureException(String.Join(",", unsupported.ToArray()));
                }
            }
            return feats;
        }

        /**
        * Creates a set of all configuration needed to satisfy the requested feature set.
        *
        * Appends special configuration for gadgets.util.hasFeature and gadgets.util.getFeatureParams to
        * the output js.
        *
        * This can't be handled via the normal configuration mechanism because it is something that
        * varies per request.
        *
        * @param reqs The features needed to satisfy the request.
        * @throws GadgetException If there is a problem with the gadget auth token
        */
        private String GetLibraryConfig(Gadget gadget, ICollection<GadgetFeature> reqs)

        {
            GadgetContext context = gadget.getContext();

            JsonObject features = containerConfig.GetJsonObject(context.getContainer(), FEATURES_KEY);

            Dictionary<String, Object> config = new Dictionary<string, object>(features == null ? 2 : features.Names.Count + 2);

            if (features != null) 
            {
                // Discard what we don't care about.
                foreach (GadgetFeature feature in reqs) 
                {
                    String name = feature.getName();
                    Object conf = features.Opt(name);
                    if (conf != null) 
                    {
                      config.Add(name, conf);
                    }
                }
            }

                // Add gadgets.util support. This is calculated dynamically based on request inputs.
                ModulePrefs prefs = gadget.getSpec().getModulePrefs();
                var values = prefs.getFeatures().Values;
                Dictionary<String, Dictionary<String, String>> featureMap = 
                    new Dictionary<string, Dictionary<string, string>>(values.Count);

                foreach(Feature feature in values)
                {
                    featureMap.Add(feature.getName(), feature.getParams());
                }
                config.Add("core.util", featureMap);

                // Add authentication token config
                ISecurityToken authToken = context.getToken();
                if (authToken != null) 
                {
                    Dictionary<String,String> authConfig = new Dictionary<String,String>(2);
                    String updatedToken = authToken.getUpdatedToken();
                    if (updatedToken != null) 
                    {
                        authConfig.Add("authToken", updatedToken);
                    }
                    String trustedJson = authToken.getTrustedJson();
                    if (trustedJson != null)
                    {
                        authConfig.Add("trustedJson", trustedJson);
                    }
                    config.Add("shindig.auth", authConfig);
                }
                    return "gadgets.config.init(" + JsonConvert.ExportToString(config) + ");\n";
        }

        /**
        * Injects message bundles into the gadget output.
        * @throws GadgetException If we are unable to retrieve the message bundle.
        */
        private void InjectMessageBundles(Gadget gadget, Node scriptTag) 
        {
            GadgetContext context = gadget.getContext();
            MessageBundle bundle = messageBundleFactory.getBundle(
                gadget.getSpec(), context.getLocale(), context.getIgnoreCache());

            String msgs = bundle.ToJSONString();

            Text text = scriptTag.getOwnerDocument().createTextNode("gadgets.Prefs.setMessages_(");
            text.appendData(msgs);
            text.appendData(");");
            scriptTag.appendChild(text);

        }

        /**
        * Injects default values for user prefs into the gadget output.
        */
        private static void InjectDefaultPrefs(Gadget gadget, Node scriptTag)
        {
                List<UserPref> prefs = gadget.getSpec().getUserPrefs();
                Dictionary<String, String> defaultPrefs = new Dictionary<string, string>(prefs.Count);

                foreach(UserPref up in gadget.getSpec().getUserPrefs())
                {
                    defaultPrefs.Add(up.getName(), up.getDefaultValue());
                }
                Text text = scriptTag.getOwnerDocument().createTextNode("gadgets.Prefs.setDefaultPrefs_(");
                text.appendData(JsonConvert.ExportToString(defaultPrefs));
                text.appendData(");");
                scriptTag.appendChild(text);
        }

        /**
        * Injects preloads into the gadget output.
        *
        * If preloading fails for any reason, we just output an empty object.
        */
        private static void InjectPreloads(Gadget gadget, Node scriptTag) 
        {
            IPreloads preloads = gadget.getPreloads();

            Dictionary<String, Object> preload = new Dictionary<string, object>();

            foreach(PreloadedData preloaded in preloads.getData()) 
            {
                foreach(var entry in preloaded.toJson()) 
                {
                    preload.Add(entry.Key, entry.Value);
                }
            }
            Text text = scriptTag.getOwnerDocument().createTextNode("gadgets.io.preloaded_=");
            text.appendData(JsonConvert.ExportToString(preload));
            text.appendData(";");
            scriptTag.appendChild(text);
        }
    }
}