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
using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Security;
using System.Text;
using org.apache.shindig.gadgets;
using org.apache.shindig.gadgets.servlet;
using Jayrock.Json;

namespace Pesta
{
    /// <summary>
    /// Summary description for GadgetRenderingTask
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetRenderingTask
    {
        protected static string CAJA_PARAM = "caja";
        protected static string STRICT_MODE_DOCTYPE
            = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">";
        protected static int DEFAULT_CACHE_TTL = 300; //secs
        protected static string LIBS_PARAM_NAME = "libs";
        private java.util.List filters;
        private GadgetContext _context;
        private GadgetServer _server;
        private HttpRequest request;
        private HttpResponse response;
        private string container = null;
        private ContainerConfig containerConfig;
        private UrlGenerator urlGenerator;
        private LockedDomainService domainLocker;
        private GadgetFeatureRegistry registry;
        private MessageBundleFactory messageBundleFactory;
        public GadgetRenderingTask()
        {
            _server = GadgetServer.Instance;
            domainLocker = HashLockedDomainService.Instance;
            containerConfig = ContainerConfig.Instance;
            messageBundleFactory = BasicMessageBundleFactory.Instance;
            filters = new java.util.LinkedList();
            urlGenerator = DefaultUrlGenerator.Instance;
            registry = GadgetFeatureRegistry.Instance;
        }
        public void process(HttpContext context)
        {
            this.request = context.Request;
            this.response = context.Response;

            _context = new HttpGadgetContext(context);
            java.net.URI url = _context.getUrl();
            if (url == null)
            {
                response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                response.StatusDescription = "Missing or malformed url parameter";
                return;
            }

            if (!"http".Equals(url.getScheme().ToLower()) && !"https".Equals(url.getScheme().ToLower()))
            {
                response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                response.StatusDescription = "Unsupported scheme (must be http or https).";
                return;
            }

            if (!validateParent())
            {
                response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                response.StatusDescription = "Unsupported parent parameter. Check your container code.";
                return;
            }

            bool addedCaja = false;
            if (getUseCaja())
            {
                filters.add(new CajaContentFilter(url));
                addedCaja = true;
            }

            Gadget gadget = _server.ProcessGadget(_context);
            if (!addedCaja &&
                gadget.Spec.getModulePrefs().getFeatures().ContainsKey("caja"))
            {
                filters.add(new CajaContentFilter(url));
            }
            outputGadget(gadget);

        }

        private void outputErrors(Exception error)
        {
            // Log the errors here for now. We might want different severity levels
            // for different error codes.
            String message = error.Message;
            if (message == null || message.Length == 0)
            {
                message = "Failed to render gadget: NULL message";
            }

            // cache this for 1 minute
            // TODO: make this a container specific configuration option
            if (!"1".Equals(request.Params["nocache"]))
            {
                response.Cache.SetExpires(DateTime.Now.AddSeconds(60));
            }

            response.Output.Write(message);
        }

        protected bool getUseCaja()
        {
            string cajaParam = request.Params[CAJA_PARAM];
            return "1".Equals(cajaParam);
        }

        private bool validateParent()
        {
            string container = getContainerForRequest();

            string parent = request.Params["parent"];

            if (parent == null)
            {
                // If there is no parent parameter, we are still safe because no
                // dependent code ever has to trust it anyway.
                return true;
            }

            Jayrock.Json.JsonArray parents = containerConfig.getJsonArray(container, "gadgets.parent");
            if (parents == null)
            {
                return true;
            }
            else
            {
                // We need to check each possible parent parameter against this regex.
                for (int i = 0, j = parents.Length; i < j; ++i)
                {
                    // TODO: Should patterns be cached? Recompiling every request
                    // seems wasteful.
                    if (java.util.regex.Pattern.matches(parents[i].ToString(), parent))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string getContainerForRequest()
        {
            if (container != null)
            {
                return container;
            }
            container = request.Params["container"];
            if (container == null)
            {
                // The parameter used to be called 'synd' FIXME: schedule removal
                container = request.Params["synd"];
                if (container == null)
                {
                    container = ContainerConfig.DEFAULT_CONTAINER;
                }
            }
            return container;
        }

        private void outputGadget(Gadget gadget)
        {
            View view = gadget.CurrentView;
            if (view == null)
            {
                throw new GadgetException(GadgetException.Code.UNKNOWN_VIEW_SPECIFIED,
                    "No appropriate view could be found for gadget: " + gadget.Spec.getUrl());
            }

            switch (view.getType().ToString())
            {
                case "HTML":
                    outputHtmlGadget(gadget, view);
                    break;
                case "URL":
                    outputUrlGadget(gadget, view);
                    break;
            }
        }

        private bool mustRedirectToLockedDomain(Gadget gadget)
        {
            string host = request.Headers["Host"];
            string container = _context.getContainer();
            if (domainLocker.gadgetCanRender(host, gadget, container))
            {
                return false;
            }

            // Gadget tried to render on wrong domain.
            String gadgetUrl = request.Url.ToString();
            String required = domainLocker.getLockedDomainForGadget(gadgetUrl, container);
            String redir = request.Url.Scheme + "://" +
                            required +
                            request.Url.PathAndQuery;
            response.Redirect(redir);
            return true;
        }

        private void outputHtmlGadget(Gadget gadget, View view)
        {
            if (mustRedirectToLockedDomain(gadget))
            {
                return;
            }

            response.ContentType = "text/html; charset=UTF-8";
            StringBuilder markup = new StringBuilder();

            if (!view.getQuirks())
            {
                markup.Append(STRICT_MODE_DOCTYPE);
            }

            // TODO: Substitute gadgets.skins values in here.
            String boilerPlate
            = "<html><head><style type=\"text/css\">" +
            "body,td,div,span,p{font-family:arial,sans-serif;}" +
            "a {color:#0000cc;}a:visited {color:#551a8b;}" +
            "a:active {color:#ff0000;}" +
            "body{margin: 0px;padding: 0px;background-color:white;}" +
            "</style></head>";
            markup.Append(boilerPlate);
            LocaleSpec localeSpec = gadget.Spec.getModulePrefs().getLocale(gadget.Context.getLocale());
            if (localeSpec == null)
            {
                markup.Append("<body>");
            }
            else
            {
                markup.Append("<body dir=\"")
                    .Append(localeSpec.getLanguageDirection())
                    .Append("\">");
            }

            StringBuilder inlineJs = new StringBuilder();
            string externFmt = "<script src=\"{0}\"></script>";
            string forcedLibs = request.Params["libs"];
            HashKey<string> libs = new HashKey<string>();
            if (forcedLibs != null)
            {
                if (forcedLibs.Trim().Length == 0)
                {
                    libs.Add("core");
                }
                else
                {
                    foreach (String item in forcedLibs.Split(':'))
                    {
                        libs.Add(item);
                    }
                }
            }

            // Forced libs are always done first.
            if (libs.Count != 0)
            {
                String jsUrl = urlGenerator.getBundledJsUrl(libs, _context);
                markup.Append(String.Format(externFmt, jsUrl));

                // Transitive dependencies must be added. This will always include core
                // so is therefore always "safe".
                ICollection<GadgetFeature> features = registry.getFeatures(libs);
                foreach (GadgetFeature dep in features)
                {
                    libs.Add(dep.getName());
                }
            }
            // Inline any libs that weren't forced
            foreach (JsLibrary library in gadget.JsLibraries)
            {
                if (library._Type.Equals(JsLibrary.Type.URL))
                {
                    if (inlineJs.Length > 0)
                    {
                        markup.Append("<script><!--\n").Append(inlineJs)
                            .Append("\n-->\n</script>");
                        inlineJs.Length = 0;
                    }
                    markup.Append(String.Format(externFmt, library.Content));
                }
                else
                {
                    if (!libs.Contains(library.Feature))
                    {
                        // already pulled this file in from the shared contents.
                        if (_context.getDebug())
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
            foreach (JsLibrary library in gadget.JsLibraries)
            {
                libs.Add(library.Feature);
            }

            appendJsConfig(gadget, libs, inlineJs);

            // message bundles for prefs object.
            GadgetContext context = gadget.Context;
            MessageBundle bundle = messageBundleFactory.getBundle(
                gadget.Spec, context.getLocale(), context.getIgnoreCache());

            String msgs = new JsonObject(bundle.getMessages()).ToString();
            inlineJs.Append("gadgets.Prefs.setMessages_(").Append(msgs).Append(");");

            appendPreloads(gadget, inlineJs);

            if (inlineJs.Length > 0)
            {
                markup.Append("<script><!--\n").Append(inlineJs)
                        .Append("\n-->\n</script>");
            }

            // Content to output now comes from the Gadget, which is processed,
            // rather than the Gadget's View, a sub-component of immutable GadgetSpec
            String content = gadget.getContent();
            for (java.util.Iterator iter = filters.iterator(); iter.hasNext(); )
            {
                GadgetContentFilter filter = iter.next() as GadgetContentFilter;
                content = filter.filter(content);
            }

            markup.Append(content)
                    .Append("<script>gadgets.util.runOnLoadHandlers();</script>")
                    .Append("</body></html>");
            if (context.getIgnoreCache())
            {
                response.Cache.SetNoStore();
                response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
            else if (request.Params["v"] != null)
            {
                // Versioned files get cached indefinitely
                response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            }
            else
            {
                // Unversioned files get cached for 5 minutes.
                // TODO: This should be configurable
                response.Cache.SetExpires(DateTime.Now.AddSeconds(DEFAULT_CACHE_TTL));
                response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            }
            response.Output.Write(markup.ToString());
        }

        private void appendPreloads(Gadget gadget, StringBuilder inlineJs)
        {
            // Output preloads. We will allow the gadget render to continue
            // even if a preload fails
            JsonObject resp = new JsonObject();
            foreach (var entry in gadget.Preloads)
            {
                Preload preload = entry.Key;
                AsyncResult result = (AsyncResult)entry.Value;
                if (result.AsyncWaitHandle.WaitOne(15000, true))
                {
                    GadgetServer.PreloadTask task = (GadgetServer.PreloadTask)((Delegate)(result.AsyncDelegate)).Target;
                    sResponse response = task.response;
                    // Use raw param as key as URL may have to be decoded
                    JsonObject jsonEntry = new JsonObject();
                    jsonEntry.Put("body", response.responseString)
                                .Put("rc", (int)response.getHttpStatusCode());
                    // Merge in additional response data
                    foreach (var metadata in response.getMetadata())
                    {
                        jsonEntry.Put(metadata.Key, metadata.Value);
                    }
                    resp.Put(preload.getHref().toString(), jsonEntry);
                }
            }
            inlineJs.Append("gadgets.io.preloaded_ = ").Append(resp.ToString()).Append(";\n");
        }

        private void appendJsConfig(Gadget gadget, HashSet<string> reqs, StringBuilder js)
        {
            JsonObject json = HttpUtil.getJsConfig(containerConfig, _context, reqs);
            // Add gadgets.util support. This is calculated dynamically based on
            // request inputs.
            ModulePrefs prefs = gadget.Spec.getModulePrefs();
            JsonObject featureMap = new JsonObject();
            foreach (Feature feature in prefs.getFeatures().Values)
            {
                featureMap.Put(feature.getName(), feature.getParams());
            }
            json.Put("core.util", featureMap);

            // Add authentication token config
            SecurityToken authToken = _context.getToken();
            if (authToken != null)
            {
                JsonObject authConfig = new JsonObject();
                string updatedToken = authToken.getUpdatedToken();
                if (updatedToken != null)
                {
                    authConfig.Put("authToken", updatedToken);
                }
                string trustedJson = authToken.getTrustedJson();
                if (trustedJson != null)
                {
                    authConfig.Put("trustedJson", trustedJson);
                }
                json.Put("shindig.auth", authConfig);
            }
            js.Append("gadgets.config.init(").Append(json.ToString()).Append(");\n");
        }

        private void outputUrlGadget(Gadget gadget, View view)
        {
            // TODO: generalize this as injectedArgs on Gadget object

            // Preserve existing query string parameters.
            java.net.URI href = view.getHref();
            String queryStr = href.getQuery();
            StringBuilder query = new StringBuilder(queryStr == null ? "" : queryStr);
            String fragment = href.getFragment();

            // TODO: figure out a way to make this work with forced libs.
            ICollection<string> libs
                = gadget.Spec.getModulePrefs().getFeatures().Keys;
            appendLibsToQuery(libs, query);

            try
            {
                href = new java.net.URI(href.getScheme(),
                href.getUserInfo(),
                href.getHost(),
                href.getPort(),
                href.getPath(),
                null,
                null);
            }
            catch (Exception e)
            {
                // Not really ever going to happen; input values are already OK.
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                throw e;
            }
            // Necessary to avoid double-URL-encoding of the JavaScript bundle portion of the query.
            StringBuilder redirectHref = new StringBuilder(href.toString());
            if (query.ToString() != null)
            {
                redirectHref.Append('?');
                redirectHref.Append(query.ToString());
            }
            if (fragment != null)
            {
                redirectHref.Append('#');
                redirectHref.Append(fragment);
            }
            response.Redirect(redirectHref.ToString());
        }

        private void appendLibsToQuery(ICollection<string> libs, StringBuilder query)
        {
            query.Append('&')
                .Append(LIBS_PARAM_NAME)
                .Append('=')
                .Append(HttpUtility.UrlEncode(urlGenerator.getBundledJsParam(libs, _context)));
        }
    } 
}
