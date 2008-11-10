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
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Web;
using org.apache.shindig.gadgets.parse;
using org.apache.shindig.gadgets.parse.caja;
using System.Runtime.Remoting.Messaging;

namespace Pesta
{
    /// <summary>
    /// Summary description for GadgetServer
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetServer
    {
        GadgetHtmlParser htmlParser;
        BasicGadgetBlacklist blacklist;
        BasicGadgetSpecFactory specFactory;
        BasicMessageBundleFactory bundleFactory;
        ContainerConfig containerConfig;
        GadgetFeatureRegistry registry;
        DefaultContentRewriterRegistry rewriterRegistry;
        static ContentFetcherFactory preloadFetcherFactory = ContentFetcherFactory.Instance;

        public static readonly GadgetServer Instance = new GadgetServer();
        protected GadgetServer()
        {
            registry = GadgetFeatureRegistry.Instance;
            blacklist = new BasicGadgetBlacklist("");
            specFactory = BasicGadgetSpecFactory.Instance;
            bundleFactory = BasicMessageBundleFactory.Instance;
            containerConfig = ContainerConfig.Instance;
            htmlParser = new CajaHtmlParser();
            rewriterRegistry = DefaultContentRewriterRegistry.Instance;
        }

        public Gadget ProcessGadget(GadgetContext context)
        {
            if (blacklist.isBlacklisted(context.getUrl()))
            {
                throw new Exception(GadgetException.Code.BLACKLISTED_GADGET.ToString());
            }
            // Retrieve the GadgetSpec for the given context.
            GadgetSpec spec = specFactory.getGadgetSpec(context);

            // Create substituted GadgetSpec object, including message bundle
            // substitutions.
            MessageBundle bundle = bundleFactory.getBundle(spec, context.getLocale(), context.getIgnoreCache());
            String dir = bundle.getLanguageDirection();
            Substitutions substituter = new Substitutions();
            substituter.addSubstitutions(
                Substitutions.Type.MESSAGE, bundle.getMessages());
            BidiSubstituter.addSubstitutions(substituter, dir);
            substituter.addSubstitution(Substitutions.Type.MODULE, "ID", context.getModuleId().ToString());
            UserPrefSubstituter.addSubstitutions(
                substituter, spec, context.getUserPrefs());
            spec = spec.substitute(substituter);

            ICollection<JsLibrary> jsLibraries = getLibraries(spec, context);
            Gadget gadget = new Gadget(context, spec, jsLibraries, containerConfig, htmlParser);

            // Perform rewriting operations on the Gadget.
            if (rewriterRegistry != null)
            {
                rewriterRegistry.rewriteGadget(gadget);
            }

            startPreloads(gadget);
            return gadget;
        }

        private ICollection<JsLibrary> getLibraries(GadgetSpec spec, GadgetContext context)
        {
            // Check all required features for the gadget.
            Dictionary<string, Feature> features = spec.getModulePrefs().getFeatures();
            HashSet<string> unsupported = new HashSet<string>();
            HashKey<string> needed = new HashKey<string>();
            foreach (var item in features.Keys)
            {
                needed.Add(item);
            }
            HashSet<GadgetFeature> feats = registry.getFeatures(needed, unsupported);

            if (unsupported.Count != 0)
            {
                // Remove non-required libs
                foreach (var missing in unsupported)
                {
                    if (!(features[missing]).getRequired())
                    {
                        unsupported.Remove(missing);
                    }
                }
                // Throw error with full list of unsupported libraries
                if (unsupported.Count != 0)
                {
                    throw new Exception(unsupported.ToString());
                }
            }
            HashSet<JsLibrary> libraries = new HashSet<JsLibrary>();
            foreach (GadgetFeature feature in feats)
            {
                foreach (JsLibrary library in feature.getJsLibraries(context.getRenderingContext(), context.getContainer()))
                {
                    libraries.Add(library);
                }
            }
            return libraries;
        }

        private void startPreloads(Gadget gadget)
        {
            RenderingContext renderContext = gadget.Context.getRenderingContext();

            if (RenderingContext.GADGET == renderContext)
            {
                foreach (Preload preload in gadget.Spec.getModulePrefs().getPreloads())
                {
                    // Cant execute signed/oauth preloads without the token
                    if ((preload.getAuthType() == AuthType.NONE ||
                        gadget.Context.getToken() != null) &&
                        (preload.getViews().Count == 0 ||
                        preload.getViews().Contains(gadget.Context.getView())))
                    {
                        PreloadTask task = new PreloadTask(gadget.Context, preload);
                        preloadProcessor processor = new preloadProcessor(task.Execute);
                        IAsyncResult future = processor.BeginInvoke(null, task);
                        gadget.Preloads.Add(preload, future);
                    }
                }
            }
        }

        // preloadmap should contain results according to preload
        private delegate void preloadProcessor();

        public struct PreloadTask
        {
            private Preload preload;
            private GadgetContext context;
            public sResponse response;
            public void Execute()
            {
                sRequest request = new sRequest(preload.getHref())
                    .SetSecurityToken(context.getToken())
                    .SetOAuthArguments(new OAuthArguments(preload))
                    .SetAuthType(preload.getAuthType())
                    .SetContainer(context.getContainer())
                    .SetGadget(context.getUrl());

                this.response = preloadFetcherFactory.fetch(request);
            }

            public PreloadTask(GadgetContext context, Preload preload)
            {
                this.preload = preload;
                this.context = context;
                this.response = null;
            }
        }
    } 
}
