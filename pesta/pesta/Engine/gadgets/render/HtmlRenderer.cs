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
using System.Net;
using System.Text;
using Jayrock.Json;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.oauth;
using Pesta.Engine.gadgets.preload;
using Pesta.Engine.gadgets.rewrite;
using Pesta.Engine.gadgets.spec;
using UriBuilder=Pesta.Engine.common.uri.UriBuilder;

namespace Pesta.Engine.gadgets.render
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class HtmlRenderer
    {
        private readonly IRequestPipeline requestPipeline;
        private readonly PreloaderService preloader;
        private readonly IContentRewriterRegistry rewriter;

        public HtmlRenderer() 
        {
            requestPipeline = DefaultRequestPipeline.Instance;
            preloader = new ConcurrentPreloaderService();
            rewriter = DefaultContentRewriterRegistry.Instance;
        }

        /**
        * Render the gadget into a string by performing the following steps:
        *
        * - Retrieve gadget specification information (GadgetSpec, MessageBundle, etc.)
        *
        * - Fetch any preloaded data needed to handle the request, as handled by Preloader.
        *
        * - Perform rewriting operations on the output content, handled by Rewriter.
        *
        * @param gadget The gadget for the rendering operation.
        * @return The rendered gadget content
        * @throws RenderingException if any issues arise that prevent rendering.
        */
        public String render(Gadget gadget) 
        {
            try 
            {
                View view = gadget.getCurrentView();
                GadgetContext context = gadget.getContext();
                GadgetSpec spec = gadget.getSpec();

                IPreloads preloads = preloader.preload(context, spec,
                                PreloaderService.PreloadPhase.HTML_RENDER);
                gadget.setPreloads(preloads);
                String content;

                if (view.getHref() == null) 
                {
                    content = view.getContent();
                } 
                else
                {
                    // TODO: Add current url to GadgetContext to support transitive proxying.
                    UriBuilder uri = new UriBuilder(view.getHref());
                    uri.addQueryParameter("lang", context.getLocale().getLanguage());
                    uri.addQueryParameter("country", context.getLocale().getCountry());

                    sRequest request = new sRequest(uri.toUri())
                        .setIgnoreCache(context.getIgnoreCache())
                        .setOAuthArguments(new OAuthArguments(view))
                        .setAuthType(view.getAuthType())
                        .setSecurityToken(context.getToken())
                        .setContainer(context.getContainer())
                        .setGadget(spec.getUrl());
                    sResponse response = DefaultHttpCache.Instance.getResponse(request);

                    if (response == null || response.isStale())
                    {
                        sRequest proxyRequest = createPipelinedProxyRequest(gadget, request);
                        response = requestPipeline.execute(proxyRequest);
                        DefaultHttpCache.Instance.addResponse(request, response);
                    }

                    if (response.isError())
                    {
                        throw new RenderingException("Unable to reach remote host. HTTP status " +
                                                     response.getHttpStatusCode());
                    }
                    content = response.responseString;

                }

                return rewriter.rewriteGadget(gadget, content);
            }
            catch (GadgetException e)
            {
                throw new RenderingException(e.Message, e);
            }
        }
        /**
        * Creates a proxy request by fetching pipelined data and adding it to an existing request.
        *
        */
        private sRequest createPipelinedProxyRequest(Gadget gadget, sRequest original) 
        {
            sRequest request = new sRequest(original);
            request.setIgnoreCache(true);
            GadgetSpec spec = gadget.getSpec();
            GadgetContext context = gadget.getContext();
            IPreloads proxyPreloads = preloader.preload(context, spec,
                                PreloaderService.PreloadPhase.PROXY_FETCH);
            // TODO: Add current url to GadgetContext to support transitive proxying.

            // POST any preloaded content
            if ((proxyPreloads != null) && proxyPreloads.getData().Count != 0) 
            {
                JsonArray array = new JsonArray();

                foreach(PreloadedData preload in proxyPreloads.getData()) 
                {
                    try 
                    {
                      Dictionary<String, Object> dataMap = preload.toJson();
                      foreach(var entry in dataMap) 
                      {
                        // TODO: the existing, supported content is JSONObjects that contain the
                        // key already.  Discarding the key is odd.
                        array.Put(entry.Value);
                      }
                    } 
                    catch (PreloadException pe)
                    {
                      // TODO: Determine whether this is a terminal path for the request. The spec is not
                      // clear.
                      // logger.log(Level.WARNING, "Unexpected error when preloading", pe);
                    }
                }

                String postContent = array.ToString();
                // POST the preloaded content, with a method override of GET
                // to enable caching
                request.setMethod("POST")
                  .setPostBody(Encoding.UTF8.GetBytes(postContent))
                  .setHeader("Content-Type", "text/json;charset=utf-8");
            }
            return request;
        }

    }
}