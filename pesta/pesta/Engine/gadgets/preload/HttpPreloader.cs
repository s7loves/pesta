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
using Jayrock.Json;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.oauth;
using Pesta.Engine.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.preload
{
    public class HttpPreloader : Preloader
    {
        private readonly ContentFetcherFactory fetcher;
        

        public HttpPreloader()
        {
            this.fetcher = ContentFetcherFactory.Instance;
        }

        public override Dictionary<String, preloadProcessor> createPreloadTasks(GadgetContext context,
                                                                                GadgetSpec gadget) 
        {
            Dictionary<String, preloadProcessor> preloads = new Dictionary<String, preloadProcessor>();

            foreach(Preload preload in gadget.getModulePrefs().getPreloads()) 
            {
                HashSet<String> preloadViews = preload.getViews();
                if (preloadViews.Count == 0 || preloadViews.Contains(context.getView())) 
                {
                    PreloadTask task = new PreloadTask(context, preload);
                    preloadProcessor process = new preloadProcessor(task.call);
                    preloads.Add(preload.getHref().ToString(), process);
                }
            }

            return preloads;
        }

        private class PreloadTask 
        {
            private readonly GadgetContext context;
            private readonly Preload preload;

            public PreloadTask(GadgetContext context, Preload preload) 
            {
                this.context = context;
                this.preload = preload;
            }

            public PreloadedData call()
            {
                // TODO: This should be extracted into a common helper that takes any
                // org.apache.shindig.gadgets.spec.RequestAuthenticationInfo.
                sRequest request = new sRequest(preload.getHref())
                    .setSecurityToken(context.getToken())
                    .setOAuthArguments(new OAuthArguments(preload))
                    .setAuthType(preload.getAuthType())
                    .setContainer(context.getContainer())
                    .setGadget(Uri.fromJavaUri(context.getUrl()));
                return new HttpPreloadData(ContentFetcherFactory.Instance.fetch(request));
            }
        }

        /**
        * Implements PreloadData by returning a Map that matches the output format used by makeRequest.
        */
        private struct HttpPreloadData : PreloadedData 
        {
            private readonly JsonObject data;

            public HttpPreloadData(sResponse response) 
            {
                JsonObject data = null;
                try 
                {
                    data = FetchResponseUtils.getResponseAsJson(response, response.responseString);
                } 
                catch (JsonException e) 
                {
                    data = new JsonObject();
                }
                this.data = data;
            }

            public Object toJson() 
            {
                return data;
            }
        }
    }
}