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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Text;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using org.apache.shindig.gadgets.parse;
using org.apache.shindig.gadgets.rewrite;
using org.apache.shindig.common.xml;
using org.w3c.dom;
using Pesta.Engine.auth;
using Pesta.Engine.common;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.preload;
using Pesta.Engine.gadgets.spec;
using Pesta.Utilities;
using ContentRewriter=Pesta.Engine.gadgets.rewrite.IContentRewriter;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.render
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
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
            try
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

                injectBaseTag(gadget, head);
                injectFeatureLibraries(gadget, head);

                // This can be one script block.
                Element mainScriptTag = document.createElement("script");
                injectMessageBundles(gadget, mainScriptTag);
                injectDefaultPrefs(gadget, mainScriptTag);
                injectPreloads(gadget, mainScriptTag);

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

                injectOnLoadHandlers(body);

                mutableContent.documentChanged();
                return RewriterResults.notCacheable();
            } 
            catch (GadgetException ex) 
            {
                // TODO: Rewriter interface needs to be modified to handle GadgetException or
                // RewriterException or something along those lines.
                throw ex;
            }

        }

        private void injectBaseTag(Gadget gadget, Node headTag)
        {
            GadgetContext context = gadget.getContext();
            if ("true".Equals(containerConfig.get(context.getContainer(), INSERT_BASE_ELEMENT_KEY))) 
            {
                Uri _base = gadget.getSpec().getUrl();
                View view = gadget.getCurrentView();
                if (view != null && view.getHref() != null) 
                {
                    _base = view.getHref();
                }
                Element baseTag = headTag.getOwnerDocument().createElement("base");
                baseTag.setAttribute("href", _base.ToString());
                headTag.insertBefore(baseTag, headTag.getFirstChild());

            }
        }

        private void injectOnLoadHandlers(Node bodyTag) 
        {
            Element onloadScript = bodyTag.getOwnerDocument().createElement("script");
            bodyTag.appendChild(onloadScript);
            onloadScript.appendChild(bodyTag.getOwnerDocument().createTextNode(
                "gadgets.util.runOnLoadHandlers();"));
        }

        /**
        * Injects javascript libraries needed to satisfy feature dependencies.
        */
        private void injectFeatureLibraries(Gadget gadget, Node headTag)
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
                foreach(GadgetFeature dep in featureRegistry.getFeatures(forced)) 
                {
                    forced.Add(dep.getName());
                }
            }

            // Inline any libs that weren't forced. The ugly context switch between inline and external
            // Js is needed to allow both inline and external scripts declared in feature.xml.
            String container = context.getContainer();
            ICollection<GadgetFeature> features = getFeatures(spec, forced);

            // Precalculate the maximum length in order to avoid excessive garbage generation.
            int size = 0;
            foreach(GadgetFeature feature in features) 
            {
                foreach(JsLibrary library in feature.getJsLibraries(RenderingContext.GADGET, container))
                {
                    if (library.GetType().Equals(JsLibrary.Type.URL))
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
                    if (library._Type.Equals(JsLibrary.Type.URL))
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

            inlineJs.Append(getLibraryConfig(gadget, features));

            if (inlineJs.Length > 0) 
            {
                Element inlineTag = headTag.getOwnerDocument().createElement("script");
                headTag.appendChild(inlineTag);
                inlineTag.appendChild(headTag.getOwnerDocument().createTextNode(inlineJs.ToString()));
            }
        }

        /**
   * Get all features needed to satisfy this rendering request.
   *
   * @param forced Forced libraries; added in addition to those found in the spec. Defaults to
   * "core".
   */
        private ICollection<GadgetFeature> getFeatures(GadgetSpec spec, ICollection<String> forced)
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
            ICollection<GadgetFeature> feats = featureRegistry.getFeatures(libs, unsupported);
            foreach (var item in forced)
            {
                unsupported.Remove(item);
            }

            if (unsupported.Count != 0)
            {
                // Remove non-required libs
                
                foreach (String missing in unsupported)
                {
                    if (!features[missing].getRequired())
                    {
                        unsupported.Remove(missing);
                    }
                }
                

                // Throw error with full list of unsupported libraries
                if (unsupported.Count != 0) 
                {
                    throw new UnsupportedFeatureException(unsupported.ToString());
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
        private String getLibraryConfig(Gadget gadget, ICollection<GadgetFeature> reqs)

        {
            GadgetContext context = gadget.getContext();

            JsonObject features = containerConfig.getJsonObject(context.getContainer(), FEATURES_KEY);

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
        private void injectMessageBundles(Gadget gadget, Node scriptTag) 
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
        private void injectDefaultPrefs(Gadget gadget, Node scriptTag)
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
        private void injectPreloads(Gadget gadget, Node scriptTag) 
        {
            IPreloads preloads = gadget.getPreloads();

            Dictionary<String, Object> preload = new Dictionary<string, object>();

            foreach(PreloadedData preloaded in preloads.getData()) 
            {
                try 
                {
                    foreach(var entry in preloaded.toJson()) 
                    {
                        preload.Add(entry.Key, entry.Value);
                    }
                } 
                catch (PreloadException pe) 
                {
                    // This will be thrown in the event of some unexpected exception. We can move on.
                }
            }
            Text text = scriptTag.getOwnerDocument().createTextNode("gadgets.io.preloaded_=");
            text.appendData(JsonConvert.ExportToString(preload));
            text.appendData(";");
            scriptTag.appendChild(text);
        }
    }
}