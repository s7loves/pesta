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
#endregionusing System;
using System.Web;
using System.Web.Caching;

namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class HttpCache
    {
        public static sResponse getResponse(HttpCacheKey key, sRequest request)
        {
            return HttpRuntime.Cache.Get(key.ToString()) as sResponse;
        }
        public static sResponse addResponse(HttpCacheKey key, sRequest request, sResponse response)
        {
            if (key.IsCacheable() && response != null)
            {
                long expire = response.getCacheExpiration();
                HttpRuntime.Cache.Add(key.ToString(), response, null, expire != -1 ? new DateTime(expire) : DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            return response;
        }
        public static sResponse removeResponse(HttpCacheKey key)
        {
            return HttpRuntime.Cache.Remove(key.ToString()) as sResponse;
        }
    }
}