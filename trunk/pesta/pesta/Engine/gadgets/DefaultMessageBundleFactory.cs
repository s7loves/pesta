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
using System.Net;
using System.Web;
using System.IO;

namespace Pesta
{
    /// <summary>
    /// Summary description for BasicMessageBundleFactory
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class DefaultMessageBundleFactory : AbstractMessageBundleFactory
    {
        public static readonly String CACHE_NAME = "messageBundles";
        private readonly HttpFetcher fetcher;
        //private readonly SoftExpiringCache<Uri, MessageBundle> cache;
        private readonly long refresh;


        public readonly static DefaultMessageBundleFactory Instance = new DefaultMessageBundleFactory();
        protected DefaultMessageBundleFactory()
        {
            this.fetcher = BasicHttpFetcher.Instance;
            //Cache<Uri, MessageBundle> baseCache = cacheProvider.createCache(CACHE_NAME);
            //this.cache = new SoftExpiringCache<Uri, MessageBundle>(baseCache);
            this.refresh = long.Parse(PestaConfiguration.GadgetCacheXmlRefreshInterval);
        }

        protected override MessageBundle fetchBundle(LocaleSpec locale, bool ignoreCache)
        {
            if (ignoreCache)
            {
                return fetchAndCacheBundle(locale, ignoreCache);
            }

            Uri uri = locale.getMessages();

            MessageBundle cached = HttpRuntime.Cache[uri.ToString()] as MessageBundle;

            MessageBundle bundle = null;
            if (cached == null)
            {
                try
                {
                    bundle = fetchAndCacheBundle(locale, ignoreCache);
                }
                catch (GadgetException e)
                {
                    // Enforce negative caching.
                    if (cached != null)
                    {
                        bundle = cached;
                    }
                    else
                    {
                        // We create this dummy spec to avoid the cost of re-parsing when a remote site is out.
                        bundle = MessageBundle.EMPTY;
                    }
                    HttpRuntime.Cache.Insert(uri.ToString(), bundle, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromTicks(refresh));
                }
            }
            else
            {
                bundle = cached;
            }

            return bundle;
        }

        private MessageBundle fetchAndCacheBundle(LocaleSpec locale, bool ignoreCache)
        {
            Uri url = locale.getMessages();
            sRequest request = new sRequest(url).setIgnoreCache(ignoreCache);
            // Since we don't allow any variance in cache time, we should just force the cache time
            // globally. This ensures propagation to shared caches when this is set.
            request.setCacheTtl((int)(refresh / 1000));

            sResponse response = fetcher.fetch(request);
            if (response.getHttpStatusCode() != (int)HttpStatusCode.OK)
            {
                throw new GadgetException(GadgetException.Code.FAILED_TO_RETRIEVE_CONTENT,
                    "Unable to retrieve message bundle xml. HTTP error " +
                    response.getHttpStatusCode());
            }

            MessageBundle bundle = new MessageBundle(locale, response.responseString);
            HttpRuntime.Cache.Insert(url.ToString(), bundle, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromTicks(refresh));
            return bundle;
        }
    } 
}
