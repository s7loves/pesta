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
using System.Net;
using System.Web;
using Pesta.Utilities;
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets
{
    public class DefaultMessageBundleFactory : MessageBundleFactory
    {
        private static readonly Locale ALL_ALL = new Locale("all", "ALL");
        public static readonly String CACHE_NAME = "messageBundles";
        private readonly IHttpFetcher fetcher;
        //private readonly SoftExpiringCache<Uri, MessageBundle> cache;
        private readonly long refresh;


        public readonly static DefaultMessageBundleFactory Instance = new DefaultMessageBundleFactory();
        protected DefaultMessageBundleFactory()
        {
            this.fetcher = BasicHttpFetcher.Instance;
            //Cache<Uri, MessageBundle> baseCache = cacheProvider.createCache(CACHE_NAME);
            //this.cache = new SoftExpiringCache<Uri, MessageBundle>(baseCache);
            this.refresh = long.Parse(PestaSettings.GadgetCacheXmlRefreshInterval);
        }

        public MessageBundle getBundle(GadgetSpec spec, Locale locale, bool ignoreCache)
        {
            if (ignoreCache)
            {
                return getNestedBundle(spec, locale, true);
            }

            String key = spec.getUrl().ToString() + '.' + locale.ToString();

            MessageBundle cached = HttpRuntime.Cache[key] as MessageBundle;

            MessageBundle bundle;
            if (cached == null)
            {
                try
                {
                    bundle = getNestedBundle(spec, locale, ignoreCache);
                }
                catch (GadgetException)
                {
                    // Enforce negative caching.
                    bundle = cached ?? MessageBundle.EMPTY;
                }
                HttpRuntime.Cache.Insert(key, bundle, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(refresh));
            }
            else
            {
                bundle = cached;
            }

            return bundle;
        }

        private MessageBundle getNestedBundle(GadgetSpec spec, Locale locale, bool ignoreCache)
        {
            MessageBundle parent = getParentBundle(spec, locale, ignoreCache);
            MessageBundle child;
            LocaleSpec localeSpec = spec.getModulePrefs().getLocale(locale);
            if (localeSpec == null)
            {
                return parent ?? MessageBundle.EMPTY;
            }
            Uri messages = localeSpec.getMessages();
            if (messages == null || messages.ToString().Length == 0)
            {
                child = localeSpec.getMessageBundle();
            }
            else 
            {
                child = fetchBundle(localeSpec, ignoreCache);
            }
            return new MessageBundle(parent, child);
        }

        private MessageBundle getParentBundle(GadgetSpec spec, Locale locale, bool ignoreCache)
        {
            if (locale.getLanguage().ToLower().Equals("all"))
            {
                // Top most locale already.
                return null;
            }

            if (locale.getCountry().ToLower().Equals("all"))
            {
                return getBundle(spec, ALL_ALL, ignoreCache);
            }

            return getBundle(spec, new Locale(locale.getLanguage(), "ALL"), ignoreCache);
        }
        protected MessageBundle fetchBundle(LocaleSpec locale, bool ignoreCache)
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
            return bundle;
        }
    }
}