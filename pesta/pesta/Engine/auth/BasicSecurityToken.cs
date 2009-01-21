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
using Pesta.Engine.common.crypto;

namespace Pesta.Engine.auth
{
    /// <summary>
    /// Summary description for BasicSecurityToken
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class BasicSecurityToken : ISecurityToken
    {
        /** serialized form of the token */
        private readonly String token;

        /** data from the token */
        private readonly Dictionary<String, String> tokenData;

        /** tool to use for signing and encrypting the token */
        private readonly BlobCrypter crypter = new BasicBlobCrypter(INSECURE_KEY);

        private static readonly byte[] INSECURE_KEY = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private const string OWNER_KEY = "o";
        private const string APP_KEY = "a";
        private const string VIEWER_KEY = "v";
        private const string DOMAIN_KEY = "d";
        private const string APPURL_KEY = "u";
        private const string MODULE_KEY = "m";

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
            tokenData = crypter.unwrap(token, maxAge);
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

        static public BasicSecurityToken createFromToken(string token, int maxAge) 
        {
            return new BasicSecurityToken(token, maxAge);
        }

        /**
        * Generates a token from an input array of values
        * @param owner owner of this gadget
        * @param viewer viewer of this gadget
        * @param app application id
        * @param domain domain of the container
        * @param appUrl url where the application lives
        * @param moduleId module id of this gadget 
        * @throws BlobCrypterException 
        */
        static public BasicSecurityToken createFromValues(string _owner, string _viewer, string _app, string _domain, string _appUrl, string _moduleId) 
        {
            return new BasicSecurityToken(_owner, _viewer, _app, _domain, _appUrl, _moduleId);
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