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
using System.Text;
using Jayrock.Json;


namespace Pesta
{
    /// <summary>
    ///  
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class HttpUtil
    {
        // 1 year.
        public static int DEFAULT_TTL = 365;

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        */
        public static void setCachingHeaders(HttpResponse response)
        {
            setCachingHeaders(response, DEFAULT_TTL, false);
        }

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        * @param noProxy True if you don't want the response to be cacheable by proxies.
        */
        public static void setCachingHeaders(HttpResponse response, bool noProxy)
        {
            setCachingHeaders(response, DEFAULT_TTL, noProxy);
        }

        /**
        * Sets HTTP headers that instruct the browser to cache content. Implementations should take care
        * to use cache-busting techniques on the url if caching for a long period of time.
        *
        * @param response The HTTP response
        * @param ttl The time to cache for, in seconds. If 0, then insure that
        *            this object is not cached.
        */
        public static void setCachingHeaders(HttpResponse response, int ttl)
        {
            setCachingHeaders(response, ttl, false);
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
        public static void setCachingHeaders(HttpResponse response, int ttl, bool noProxy)
        {
            response.Cache.SetExpires(DateTime.Now.AddDays(ttl));

            if (ttl == 0)
            {
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetNoStore();
            }
            else
            {
                if (noProxy)
                {
                    response.Cache.SetCacheability(HttpCacheability.Private);
                }
                else
                {
                    response.Cache.SetCacheability(HttpCacheability.Public);
                }
                response.Cache.SetMaxAge(new TimeSpan(ttl, 0, 0, 0));
                // Firefox requires this for certain cases.
                response.Cache.SetLastModified(DateTime.Now);
            }
        }

        /**
        * Fetches js configuration for the given feature set & container.
        *
        * @param config The configuration to extract js config from.
        * @param context The request context.
        * @param features A set of all features needed.
        */
        public static JsonObject getJsConfig(ContainerConfig config, GadgetContext context, HashSet<string> features)
        {
            JsonObject containerFeatures = config.getJsonObject(context.getContainer(),
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
