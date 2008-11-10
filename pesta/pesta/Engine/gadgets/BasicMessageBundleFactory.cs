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
    public class BasicMessageBundleFactory : AbstractMessageBundleFactory
    {
        private HttpFetcher fetcher;

        public readonly static BasicMessageBundleFactory Instance = new BasicMessageBundleFactory();
        protected BasicMessageBundleFactory()
        {
            this.fetcher = BasicHttpFetcher.Instance;
        }

        protected override MessageBundle fetchBundle(LocaleSpec locale, bool ignoreCache)
        {
            object sync = new object();
            if (ignoreCache)
            {
                return fetchAndCacheBundle(locale, ignoreCache);
            }
            MessageBundle cached = null;
            lock (sync)
            {
                cached = HttpRuntime.Cache[locale.getMessages().ToString()] as MessageBundle;
            }

            if (cached == null)
            {
                try
                {
                    return fetchAndCacheBundle(locale, ignoreCache);
                }
                catch (GadgetException e)
                {
                    if (cached == null)
                        throw e;
                }
            }
            return cached;
        }

        private MessageBundle fetchAndCacheBundle(LocaleSpec locale, bool ignoreCache)
        {
            Uri url = locale.getMessages();
            sRequest request = new sRequest(url).SetIgnoreCache(ignoreCache);
            sResponse response = fetcher.fetch(request);
            if (response == null || response.getHttpStatusCode() != HttpStatusCode.OK)
            {
                throw new GadgetException(GadgetException.Code.FAILED_TO_RETRIEVE_CONTENT,
                    "Unable to retrieve message bundle xml. HTTP error " +
                    response.getHttpStatusCode());
            }

            MessageBundle bundle = new MessageBundle(locale, response.responseString);
            HttpRuntime.Cache.Insert(url.ToString(), bundle, null,
                response.getCacheExpiration() ?? DateTime.Now.AddMinutes(5),
                System.Web.Caching.Cache.NoSlidingExpiration);

            return bundle;
        }
    } 
}
