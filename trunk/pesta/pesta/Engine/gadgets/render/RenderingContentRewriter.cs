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
using System.Text.RegularExpressions;
using System.Text;
using Jayrock.Json;
using org.apache.shindig.gadgets.rewrite;
using Pesta.Engine.auth;
using Pesta.Engine.common;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.preload;
using Pesta.Engine.gadgets.spec;
using Pesta.Utilities;
using ContentRewriter=Pesta.Engine.gadgets.rewrite.ContentRewriter;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.render
{
    public class RenderingContentRewriter : ContentRewriter
    {
        static readonly Regex DOCUMENT_SPLIT_PATTERN = new Regex(
            "(.*)<head>(.*?)<\\/head>(?:.*)<body(.*?)>(.*?)<\\/body>(?:.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase );
        static readonly int BEFORE_HEAD_GROUP = 1;
        static readonly int HEAD_GROUP = 2;
        static readonly int BODY_ATTRIBUTES_GROUP = 3;
        static readonly int BODY_GROUP = 4;
        static readonly String DEFAULT_HEAD_CONTENT =
            "<style type=\"text/css\">" +
            "body,td,div,span,p{font-family:arial,sans-serif;}" +
            "a {color:#0000cc;}a:visited {color:#551a8b;}" +
            "a:active {color:#ff0000;}" +
            "body{margin: 0px;padding: 0px;background-color:white;}" +
            "</style>";
        static readonly String INSERT_BASE_ELEMENT_KEY = "gadgets.insertBaseElement";
        static readonly String FEATURES_KEY = "gadgets.features";

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
            return RewriterResults.cacheableIndefinitely();
        }

        public RewriterResults rewrite(Gadget gadget, MutableContent mutableContent)
        {
            try 
            {
                GadgetContent content = createGadgetContent(gadget, mutableContent);

                injectBaseTag(gadget, content);
                injectFeatureLibraries(gadget, content);
                // This can be one script block.
                content.appendHead("<script>");
                injectMessageBundles(gadget, content);
                injectDefaultPrefs(gadget, content);
                injectPreloads(gadget, content);
                content.appendHead("</script>");
                injectOnLoadHandlers(content);
                // TODO: Use preloads when RenderedGadget gets promoted to Gadget.
                mutableContent.setContent(readonlyizeDocument(gadget, content));
                return RewriterResults.notCacheable();
            } 
            catch (GadgetException e) 
            {
                // TODO: Rewriter interface needs to be modified to handle GadgetException or
                // RewriterException or something along those lines.
                throw e;
            }
        }

        private void injectBaseTag(Gadget gadget, GadgetContent content)
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
                content.appendHead("<base href='" + _base + "'/>");
            }
        }

        private void injectOnLoadHandlers(GadgetContent content) 
        {
            content.appendBody("<script>gadgets.util.runOnLoadHandlers();</script>");
        }

        /**
        * Injects javascript libraries needed to satisfy feature dependencies.
        */
        private void injectFeatureLibraries(Gadget gadget, GadgetContent content)
        {
            // TODO: If there isn't any js in the document, we can skip this. Unfortunately, that means
            // both script tags (easy to detect) and event handlers (much more complex).
            GadgetContext context = gadget.getContext();
            GadgetSpec spec = gadget.getSpec();
            String forcedLibs = context.getParameter("libs");
            HashKey<String> forced;
            if (forcedLibs == null || forcedLibs.Length == 0) 
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

            String externFmt = "<script src=\"{0}\"></script>";

            // Forced libs are always done first.
            if (forced.Count != 0) 
            {
                String jsUrl = urlGenerator.getBundledJsUrl(forced, context);
                content.appendHead(String.Format(externFmt, jsUrl));
                // Forced transitive deps need to be added as well so that they don't get pulled in twice.
                // TODO: Figure out a clean way to avoid having to call getFeatures twice.
                foreach(GadgetFeature dep in featureRegistry.getFeatures(forced)) 
                {
                    forced.Add(dep.getName());
                }
            }

            StringBuilder inlineJs = new StringBuilder();

            // Inline any libs that weren't forced. The ugly context switch between inline and external
            // Js is needed to allow both inline and external scripts declared in feature.xml.
            String container = context.getContainer();
            ICollection<GadgetFeature> features = getFeatures(spec, forced);

            foreach(GadgetFeature feature in features) 
            {
                foreach(JsLibrary library in feature.getJsLibraries(RenderingContext.GADGET, container))
                {
                    if (library.GetType().Equals(JsLibrary.Type.URL))
                    {
                        if (inlineJs.Length > 0) 
                        {
                            content.appendHead("<script>")
                                .appendHead(inlineJs.ToString())
                                .appendHead("</script>");
                            inlineJs.Length = 0;
                        }
                        content.appendHead(String.Format(externFmt, library.Content));
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
                content.appendHead("<script>")
                    .appendHead(inlineJs.ToString())
                    .appendHead("</script>");
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

            try 
            {
                // Discard what we don't care about.
                JsonObject config;
                if (features == null) 
                {
                    config = new JsonObject();
                } 
                else 
                {
                    String[] properties = new String[reqs.Count];
                    int i = 0;
                    foreach(GadgetFeature feature in reqs)
                    {
                        properties[i++] = feature.getName();
                    }
                    config = new JsonObject(features, properties);
                }

                // Add gadgets.util support. This is calculated dynamically based on request inputs.
                ModulePrefs prefs = gadget.getSpec().getModulePrefs();
                JsonObject featureMap = new JsonObject();

                foreach(Feature feature in prefs.getFeatures().Values)
                {
                    featureMap.Put(feature.getName(), feature.getParams());
                }
                config.Put("core.util", featureMap);

                // Add authentication token config
                SecurityToken authToken = context.getToken();
                if (authToken != null) 
                {
                    JsonObject authConfig = new JsonObject();
                    String updatedToken = authToken.getUpdatedToken();
                    if (updatedToken != null) 
                    {
                        authConfig.Put("authToken", updatedToken);
                    }
                    String trustedJson = authToken.getTrustedJson();
                    if (trustedJson != null)
                    {
                        authConfig.Put("trustedJson", trustedJson);
                    }
                    config.Put("shindig.auth", authConfig);
                }
                return "gadgets.config.init(" + config + ");\n";
            }
            catch (JsonException e) 
            {
                // Shouldn't be possible.
                throw e;
            }
        }

        /**
        * Injects message bundles into the gadget output.
        * @throws GadgetException If we are unable to retrieve the message bundle.
        */
        private void injectMessageBundles(Gadget gadget, GadgetContent content) 
        {
            GadgetContext context = gadget.getContext();
            MessageBundle bundle = messageBundleFactory.getBundle(
                gadget.getSpec(), context.getLocale(), context.getIgnoreCache());

            String msgs = new JsonObject(bundle.getMessages()).ToString();
            content.appendHead("gadgets.Prefs.setMessages_(")
                .appendHead(msgs)
                .appendHead(");");
        }

        /**
        * Injects default values for user prefs into the gadget output.
        */
        private void injectDefaultPrefs(Gadget gadget, GadgetContent content)
        {
            JsonObject defaultPrefs = new JsonObject();
            try 
            {
                foreach(UserPref up in gadget.getSpec().getUserPrefs())
                {
                    defaultPrefs.Put(up.getName(), up.getDefaultValue());
                }
            } 
            catch (JsonException) 
            {
                // Never happens. Name is required (cannot be null). Default value is a String.
            }
            content.appendHead("gadgets.Prefs.setDefaultPrefs_(")
                .appendHead(defaultPrefs.ToString())
                .appendHead(");");
        }

        /**
        * Injects preloads into the gadget output.
        *
        * If preloading fails for any reason, we just output an empty object.
        */
        private void injectPreloads(Gadget gadget, GadgetContent content) 
        {
            JsonObject preload = new JsonObject();
            Preloads preloads = gadget.getPreloads();

            foreach(var preloaded in preloads.getData()) 
            {
                try 
                {
                    foreach (var entry in preloaded.toJson()) 
                    {
                        preload.Put(entry.Key, entry.Value);
                    }
                } 
                catch (PreloadException pe)
                {
                    // This will be thrown in the event of some unexpected exception. We can move on.
                }

            }

            content.appendHead("gadgets.io.preloaded_=")
                .appendHead(preload.ToString())
                .appendHead(";");
        }

        /**
        * Produces GadgetContent by parsing the document into 3 pieces (head, body, and tail). If the
        */
        private GadgetContent createGadgetContent(Gadget gadget, MutableContent mutableContent)
        {
            String doc = mutableContent.getContent();
            // Quick check for full document tags
            String head = doc.Substring(0, Math.Min(150, doc.Length));
            if (head.Contains("<HTML") || head.Contains("<html")) 
            {
                Match matcher = DOCUMENT_SPLIT_PATTERN.Match(doc);
                if (matcher.Success) 
                {
                    GadgetContent content = new GadgetContent();
                    content.appendHead(matcher.Groups[BEFORE_HEAD_GROUP].Value)
                        .appendHead("<head>");

                    content.appendBody(matcher.Groups[HEAD_GROUP].Value)
                        .appendBody("</head>")
                        .appendBody(createBodyTag(gadget, matcher.Groups[BODY_ATTRIBUTES_GROUP].Value))
                        .appendBody(matcher.Groups[BODY_GROUP].Value);

                    content.appendTail("</body></html>");
                    return content;
                }
                return makeDefaultContent(gadget, mutableContent);
            }
            return makeDefaultContent(gadget, mutableContent);
        }

        /**
        * Inserts basic content for a gadget. Used when the content does not contain a valid html doc.
        */
        private GadgetContent makeDefaultContent(Gadget gadget, MutableContent mutableContent)
        {
            GadgetContent content = new GadgetContent();
            content.appendHead("<html><head>");
            content.appendHead(DEFAULT_HEAD_CONTENT);
            content.appendBody("</head>");
            content.appendBody(createBodyTag(gadget, ""));
            content.appendBody(mutableContent.getContent());
            content.appendTail("</body></html>");
            return content;
        }

        /**
        * Produces the default body tag, inserting language direction as needed.
        */
        private String createBodyTag(Gadget gadget, String extra) 
        {
            LocaleSpec localeSpec = gadget.getLocale();
            if (localeSpec == null) 
            {
                return "<body" + extra + ">";
            }
            return "<body" + extra + " dir='" + localeSpec.getLanguageDirection() + "'>";
        }

        /**
        * Produces a readonly document for the gadget's content.
        */
        private String readonlyizeDocument(Gadget gadget, GadgetContent content)
        {
            return content.assemble();
        }

        private class GadgetContent 
        {
            private readonly StringBuilder head = new StringBuilder();
            private readonly StringBuilder body = new StringBuilder();
            private readonly StringBuilder tail = new StringBuilder();

            public GadgetContent appendHead(String content) 
            {
                head.Append(content);
                return this;
            }

            public GadgetContent appendBody(String content)
            {
                body.Append(content);
                return this;
            }

            public GadgetContent appendTail(String content) 
            {
                tail.Append(content);
                return this;
            }

            /**
            * @return The readonly content for the gadget.
            */
            public String assemble() 
            {
                return new StringBuilder(head.Length + body.Length + tail.Length)
                    .Append(head.ToString())
                    .Append(body.ToString())
                    .Append(tail.ToString())
                    .ToString();
            }

            public override String ToString()
            {
                return assemble();
            }
        }
    }
}