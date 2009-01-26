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
using System.Collections.Generic;
using System;
using Jayrock.Json;

namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class HttpCacheKey
    {
        private IDictionary<String, String> data;
        private bool cacheable;

        /// <summary>
        /// Create a cache key for the specified request.
        /// </summary>
        ///
        /// <param name="request"></param>
        public HttpCacheKey(sRequest request)
        {
            data = new Dictionary<String, String>();
            Cacheable = isCacheable(request);
            
            // In theory we only cache GET, but including the method in the cache
            // key
            // provides some additional insurance that we aren't mixing cache
            // content.
            set("method", request.getMethod());
            set("url", request.getUri().ToString());
            // TODO: We can go ahead and add authentication info here as well.
        }

        /// <summary>
        /// Add additional data to the cache key.
        /// </summary>
        ///
        public void set(String key, String value_ren)
        {
            data.Add(key, value_ren);
        }

        /// <summary>
        /// Remove data from the cache key.
        /// </summary>
        ///
        public void remove(String key)
        {
            data.Remove(key);
        }

        public bool Cacheable
        {
            set
            {
                this.cacheable = value;
            }
        }

        public bool isCacheable()
        {
            return cacheable;
        }

        private static bool isCacheable(sRequest request)
        {
            if (request.IgnoreCache)
            {
                return false;
            }

            if (!"GET".Equals(request.getMethod()) &&
                !"GET".Equals(request.getHeader("X-Method-Override")))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Figure out a string representation of this cache key. The representation
        /// will be:
        /// canonical: identical sets of key/value pairs will always map to the same
        /// string.
        /// unique: different sets of key/value pairs will always map to different
        /// strings.
        /// </summary>
        ///
        public override String ToString()
        {
            List<String> list = new List<String>();
            list.AddRange(data.Keys);
            list.Sort();
            JsonObject json = new JsonObject();
            foreach (String key in list)
            {
                json.Put(key, data[key]);
            }
            return json.ToString();
        }
    }
}