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
using System.Web;

namespace Pesta
{
    /// <summary>
    /// Summary description for OAuthFetcherFactory
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthFetcherFactory
    {
        protected OAuthFetcherConfig fetcherConfig;
        public static readonly OAuthFetcherFactory Instance = new OAuthFetcherFactory();
        /**
        * Creates an OAuthFetcherFactory based on prepared crypter and token store.
        *
        * @param fetcherConfig configuration
        */
        protected OAuthFetcherFactory()
        {
            byte[] masterKey = Crypto.GetRandomBytes(BasicBlobCrypter.MASTER_KEY_MIN_LEN);
            this.fetcherConfig = new OAuthFetcherConfig(
                        new BasicBlobCrypter(masterKey),
                        new GadgetOAuthTokenStore(new BasicOAuthStore(), BasicGadgetSpecFactory.Instance));
        }

        /**
        * Produces an OAuthFetcher that will sign requests and delegate actual
        * network retrieval to the {@code nextFetcher}
        *
        * @param nextFetcher The fetcher that will fetch real content
        * @param request request that will be sent through the fetcher
        * @return The oauth fetcher.
        * @throws GadgetException
        */
        public OAuthFetcher getOAuthFetcher(HttpFetcher nextFetcher, sRequest request)
        {
            OAuthFetcher fetcher = new OAuthFetcher(fetcherConfig, nextFetcher, request);
            return fetcher;
        }
    } 
}
