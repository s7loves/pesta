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
using Pesta;
using Pesta.Engine.common.crypto;

namespace pestaServer.Models.gadgets.oauth
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class OAuthModule
    {
        private const String OAUTH_CONFIG = "config/oauth.json";
        private const String OAUTH_SIGNING_KEY_FILE = "shindig.signing.key-file";
        private const String OAUTH_SIGNING_KEY_NAME = "shindig.signing.key-name";

        public class OAuthCrypterProvider 
        {
            private readonly BlobCrypter crypter;

            public OAuthCrypterProvider(String stateCrypterPath)
            {
                if (String.IsNullOrEmpty(stateCrypterPath)) 
                {
                    crypter = new BasicBlobCrypter(Crypto.getRandomBytes(BasicBlobCrypter.MASTER_KEY_MIN_LEN));
                } 
                else 
                {
                    crypter = new BasicBlobCrypter(stateCrypterPath);
                }
            }

            public BlobCrypter get() 
            {
                return crypter;
            }
        }
    }
}
