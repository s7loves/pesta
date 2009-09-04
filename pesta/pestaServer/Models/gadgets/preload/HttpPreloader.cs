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
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.oauth;
using pestaServer.Models.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets.preload
{
    
    public class HttpPreloader : Preloader
    {
        private static readonly IRequestPipeline requestPipeline = DefaultRequestPipeline.Instance;
        

        public HttpPreloader()
        {

        }

        public override List<preloadProcessor> createPreloadTasks(GadgetContext context,
                                                                                GadgetSpec gadget, PreloaderService.PreloadPhase phase) 
        {
            List<preloadProcessor> preloads = new List<preloadProcessor>();
            if (phase == PreloaderService.PreloadPhase.HTML_RENDER) 
            {
                foreach(Preload preload in gadget.getModulePrefs().getPreloads()) 
                {
                    HashSet<String> preloadViews = preload.getViews();
                    if (preloadViews.Count == 0 || preloadViews.Contains(context.getView())) 
                    {
                        PreloadTask task = new PreloadTask(context, preload, preload.getHref().ToString());
                        preloads.Add(new preloadProcessor(task.call));
                    }
                }
            }
            return preloads;
        }

        public static sRequest newHttpRequest(GadgetContext context,
                    RequestAuthenticationInfo authenticationInfo)
        {
            sRequest request = new sRequest(authenticationInfo.getHref())
                .setSecurityToken(context.getToken())
                .setOAuthArguments(new OAuthArguments(authenticationInfo))
                .setAuthType(authenticationInfo.getAuthType())
                .setContainer(context.getContainer())
                .setGadget(Uri.fromJavaUri(context.getUrl()));
            return request;
        }


        private class PreloadTask 
        {
            private readonly GadgetContext context;
            private readonly Preload preload;
            private readonly String key;

            public PreloadTask(GadgetContext context, Preload preload, String key) 
            {
                this.context = context;
                this.preload = preload;
                this.key = key;
            }

            public PreloadedData call()
            {
                sRequest request = newHttpRequest(context, preload);
                return new HttpPreloadData(requestPipeline.execute(request), key);
            }
        }

        /**
        * Implements PreloadData by returning a Map that matches the output format used by makeRequest.
        */
        private struct HttpPreloadData : PreloadedData 
        {
            private readonly JsonObject data;
            private readonly String key;

            public HttpPreloadData(sResponse response, String key) 
            {
                JsonObject _data;
                try 
                {
                    _data = FetchResponseUtils.getResponseAsJson(response, response.responseString);
                } 
                catch (JsonException) 
                {
                    _data = new JsonObject();
                }
                this.data = _data;
                this.key = key;
            }

            public Dictionary<string, Object> toJson() 
            {
                return new Dictionary<string, object> { {key, data} };
            }
        }
    }
}