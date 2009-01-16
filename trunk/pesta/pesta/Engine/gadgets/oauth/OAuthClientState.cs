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

namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Summary description for OAuthClientState
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class OAuthClientState
    {
        /**
        * Maximum age for our client state; if this is exceeded we start over. One
        * hour is a fairly arbitrary time limit here.
        */
        private const int CLIENT_STATE_MAX_AGE_SECS = 3600;

        // Our client state is encrypted key/value pairs.  These are the key names.
        private const string REQ_TOKEN_KEY = "r";
        private const string REQ_TOKEN_SECRET_KEY = "rs";
        private const string ACCESS_TOKEN_KEY = "a";
        private const string ACCESS_TOKEN_SECRET_KEY = "as";
        private const string OWNER_KEY = "o";
        private const String SESSION_HANDLE_KEY = "sh";
        private const String ACCESS_TOKEN_EXPIRATION_KEY = "e";

        /** Name/value pairs */
        private readonly Dictionary<string, string> state;

        /** Crypter to use when sending these to the client */
        private readonly BlobCrypter crypter;

        /**
        * Create a new, empty client state blob.
        * 
        * @param crypter
        */
        public OAuthClientState(BlobCrypter crypter)
        {
            state = new Dictionary<string, string>();
            this.crypter = crypter;
        }

        /**
        * Initialize client state based on an encrypted blob passed by the
        * client.
        * 
        * @param crypter
        * @param stateBlob
        */
        public OAuthClientState(BlobCrypter crypter, String stateBlob)
        {
            this.crypter = crypter;
            Dictionary<string, string> _state = null;
            if (!String.IsNullOrEmpty(stateBlob))
            {
                try
                {
                    _state = crypter.unwrap(stateBlob, CLIENT_STATE_MAX_AGE_SECS);
                }
                catch (BlobCrypterException)
                {
                    // Probably too old, pretend we never saw it at all.
                }
            }
            state = _state ?? new Dictionary<string, string>();
        }

        /**
        * @return true if there is no state to store with the client.
        */
        public bool isEmpty()
        {
            // Might contain just a timestamp
            return (state.Count == 0 || (state.Count == 1 && state.ContainsKey("t")));
        }

        /**
        * @return an encrypted blob of state to store with the client.
        * @throws BlobCrypterException
        */
        public String getEncryptedState()
        {
            return crypter.wrap(state);
        }

        /**
        * OAuth request token
        */
        public String getRequestToken()
        {
            return state.ContainsKey(REQ_TOKEN_KEY) ? state[REQ_TOKEN_KEY] : null;
        }

        public void setRequestToken(String requestToken)
        {
            setNullCheck(REQ_TOKEN_KEY, requestToken);
        }

        /**
        * OAuth request token secret
        */
        public String getRequestTokenSecret()
        {
            return state.ContainsKey(REQ_TOKEN_SECRET_KEY)? state[REQ_TOKEN_SECRET_KEY]:null;
        }

        public void setRequestTokenSecret(String requestTokenSecret)
        {
            setNullCheck(REQ_TOKEN_SECRET_KEY, requestTokenSecret);
        }

        /**
        * OAuth access token.
        */
        public String getAccessToken()
        {
            return state.ContainsKey(ACCESS_TOKEN_KEY)?state[ACCESS_TOKEN_KEY]:null;
        }

        public void setAccessToken(String accessToken)
        {
            setNullCheck(ACCESS_TOKEN_KEY, accessToken);
        }

        /**
        * OAuth access token secret.
        */
        public String getAccessTokenSecret()
        {
            return state.ContainsKey(ACCESS_TOKEN_SECRET_KEY)?state[ACCESS_TOKEN_SECRET_KEY]:null;
        }

        public void setAccessTokenSecret(String accessTokenSecret)
        {
            setNullCheck(ACCESS_TOKEN_SECRET_KEY, accessTokenSecret);
        }

        /**
   * Session handle (http://oauth.googlecode.com/svn/spec/ext/session/1.0/drafts/1/spec.html)
   */
        public String getSessionHandle()
        {
            return state.ContainsKey(SESSION_HANDLE_KEY) ? state[SESSION_HANDLE_KEY] : null;
        }

        public void setSessionHandle(String sessionHandle)
        {
            setNullCheck(SESSION_HANDLE_KEY, sessionHandle);
        }

        /**
         * Expiration of access token
         * (http://oauth.googlecode.com/svn/spec/ext/session/1.0/drafts/1/spec.html)
         */
        public long getTokenExpireMillis()
        {
            String expiration = state.ContainsKey(ACCESS_TOKEN_EXPIRATION_KEY) ? state[ACCESS_TOKEN_EXPIRATION_KEY] : null; 
            if (expiration == null)
            {
                return 0;
            }
            return long.Parse(expiration);
        }

        public void setTokenExpireMillis(long expirationMillis)
        {
            setNullCheck(ACCESS_TOKEN_EXPIRATION_KEY, expirationMillis.ToString());
        }

        /**
        * Owner of the OAuth token.
        */
        public String getOwner()
        {
            return state.ContainsKey(OWNER_KEY)?state[OWNER_KEY]:null;
        }

        public void setOwner(String owner)
        {
            setNullCheck(OWNER_KEY, owner);
        }

        private void setNullCheck(String key, String value)
        {
            if (value == null)
            {
                state.Remove(key);
            }
            else
            {
                state.Add(key, value);
            }
        }
    }
}