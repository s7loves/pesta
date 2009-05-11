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
using System.Web;
using System.Collections.Generic;
using Jayrock.Json;
using pestaServer.Models.common;

namespace pestaServer.Models.gadgets.servlet
{
    /// <summary>
    ///  
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public static class HttpUtil
    {
        private const int DefaultTtl = 31536000; // 1 year = 365 * 24 * 60 * 60

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        */
        public static void SetCachingHeaders(HttpResponse response)
        {
            SetCachingHeaders(response, DefaultTtl, false);
        }

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        * @param noProxy True if you don't want the response to be cacheable by proxies.
        */
        public static void SetCachingHeaders(HttpResponse response, bool noProxy)
        {
            SetCachingHeaders(response, DefaultTtl, noProxy);
        }

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        * @param ttl The time to cache for, in seconds. If 0, then insure that
        *            this object is not cached.
        */
        public static void SetCachingHeaders(HttpResponse response, int ttl)
        {
            SetCachingHeaders(response, ttl, false);
        }

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        * @param ttl The time to cache for, in seconds. If 0, then insure that
        *            this object is not cached.
        * @param noProxy True if you don't want the response to be cacheable by proxies.
        */
        public static void SetCachingHeaders(HttpResponse response, int ttl, bool noProxy)
        {
            
            if (ttl == 0)
            {
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetNoStore();
            }
            else
            {
                if (noProxy)
                {
                    response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                }
                else
                {
                    response.Cache.SetCacheability(HttpCacheability.Public);
                }
                response.Cache.SetMaxAge(TimeSpan.FromSeconds(ttl));
                response.Cache.SetExpires(DateTime.Now.AddSeconds(ttl));
                response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            }
        }

        /**
        * Fetches js configuration for the given feature set & container.
        *
        * @param config The configuration to extract js config from.
        * @param context The request context.
        * @param features A set of all features needed.
        */
        public static JsonObject GetJsConfig(ContainerConfig config, GadgetContext context, HashSet<string> features)
        {
            JsonObject containerFeatures = config.GetJsonObject(context.getContainer(),
                                                                "gadgets.features");
            JsonObject retv = new JsonObject();
            if (containerFeatures != null)
            {
                foreach (string feat in features)
                {
                    if (containerFeatures.Contains(feat))
                    {
                        retv.Put(feat, containerFeatures[feat]);
                    }
                }
            }
            return retv;
        }
    }
}