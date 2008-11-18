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
using System.Collections.Generic;

namespace Pesta
{
    /// <summary>
    /// Summary description for BasicSecurityToken
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BasicSecurityToken : SecurityToken
    {
        /** serialized form of the token */
        private readonly String token;

        /** data from the token */
        private readonly Dictionary<String, String> tokenData;

        /** tool to use for signing and encrypting the token */
        private BlobCrypter crypter = new BasicBlobCrypter(INSECURE_KEY);

        private static readonly byte[] INSECURE_KEY = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private static readonly String OWNER_KEY = "o";
        private static readonly String APP_KEY = "a";
        private static readonly String VIEWER_KEY = "v";
        private static readonly String DOMAIN_KEY = "d";
        private static readonly String APPURL_KEY = "u";
        private static readonly String MODULE_KEY = "m";

        /**
        * {@inheritDoc}
        */
        public String toSerialForm()
        {
            return token;
        }

        /**
        * Generates a token from an input string
        * @param token String form of token
        * @param maxAge max age of the token (in seconds)
        * @throws BlobCrypterException
        */
        public BasicSecurityToken(String token, int maxAge)
        {
            this.token = token;
            this.tokenData = crypter.unwrap(token, maxAge);
        }

        public BasicSecurityToken(String owner, String viewer, String app,
                    String domain, String appUrl, String moduleId)
        {
            tokenData = new Dictionary<String, String>();
            putNullSafe(OWNER_KEY, owner);
            putNullSafe(VIEWER_KEY, viewer);
            putNullSafe(APP_KEY, app);
            putNullSafe(DOMAIN_KEY, domain);
            putNullSafe(APPURL_KEY, appUrl);
            putNullSafe(MODULE_KEY, moduleId);
            token = crypter.wrap(tokenData);
        }

        private void putNullSafe(String key, String value)
        {
            if (value != null)
            {
                tokenData.Add(key, value);
            }
        }

        /**
        * {@inheritDoc}
        */
        public String getAppId()
        {
            return tokenData[APP_KEY];
        }

        /**
        * {@inheritDoc}
        */
        public String getDomain()
        {
            return tokenData[DOMAIN_KEY];
        }

        /**
        * {@inheritDoc}
        */
        public String getOwnerId()
        {
            return tokenData[OWNER_KEY];
        }

        /**
        * {@inheritDoc}
        */
        public String getViewerId()
        {
            return tokenData[VIEWER_KEY];
        }

        /**
        * {@inheritDoc}
        */
        public String getAppUrl()
        {
            return tokenData[APPURL_KEY];
        }

        /**
        * {@inheritDoc}
        */
        public long getModuleId()
        {
            return long.Parse(tokenData[MODULE_KEY]);
        }

        /**
        * {@inheritDoc}
        */
        public String getUpdatedToken()
        {
            return null;
        }

        /**
        * {@inheritDoc}
        */
        public String getTrustedJson()
        {
            return null;
        }

        /**
        * {@inheritDoc}
        */
        public bool isAnonymous()
        {
            return false;
        }
    } 
}
