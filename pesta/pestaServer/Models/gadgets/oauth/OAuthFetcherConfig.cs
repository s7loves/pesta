﻿#region License, Terms and Conditions
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

using Pesta;
using Pesta.Engine.common.crypto;

namespace pestaServer.Models.gadgets.oauth
{
    /// <summary>
    /// Summary description for OAuthFetcherConfig
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class OAuthFetcherConfig
    {
        private BlobCrypter stateCrypter;
        private GadgetOAuthTokenStore tokenStore;
        public static readonly OAuthFetcherConfig Instance = new OAuthFetcherConfig();
        protected OAuthFetcherConfig()
        {
            byte[] masterKey = Crypto.getRandomBytes(BasicBlobCrypter.MASTER_KEY_MIN_LEN);
            this.stateCrypter = new BasicBlobCrypter(masterKey);
            this.tokenStore = new GadgetOAuthTokenStore(BasicOAuthStore.Instance, DefaultGadgetSpecFactory.Instance);
        }

        /**
        * Used to encrypt state stored on the client.
        */
        public BlobCrypter getStateCrypter()
        {
            return stateCrypter;
        }

        /**
        * Persistent token storage.
        */
        public GadgetOAuthTokenStore getTokenStore()
        {
            return tokenStore;
        }
    }
}