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
        private static int CLIENT_STATE_MAX_AGE_SECS = 3600;

        // Our client state is encrypted key/value pairs.  These are the key names.
        private static String REQ_TOKEN_KEY = "r";
        private static String REQ_TOKEN_SECRET_KEY = "rs";
        private static String ACCESS_TOKEN_KEY = "a";
        private static String ACCESS_TOKEN_SECRET_KEY = "as";
        private static String OWNER_KEY = "o";

        /** Name/value pairs */
        private Dictionary<string, string> state;

        /** Crypter to use when sending these to the client */
        private BlobCrypter crypter;

        /**
        * Create a new, empty client state blob.
        * 
        * @param crypter
        */
        public OAuthClientState(BlobCrypter crypter)
        {
            this.state = new Dictionary<string, string>();
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
            Dictionary<string, string> state = null;
            if (!String.IsNullOrEmpty(stateBlob))
            {
                try
                {
                    state = crypter.unwrap(stateBlob, CLIENT_STATE_MAX_AGE_SECS);
                }
                catch (BlobCrypterException e)
                {
                    // Probably too old, pretend we never saw it at all.
                }
            }
            this.state = state ?? new Dictionary<string, string>();
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
            return state[REQ_TOKEN_KEY];
        }

        public void setRequestToken(String requestToken)
        {
            state.Add(REQ_TOKEN_KEY, requestToken);
        }

        /**
        * OAuth request token secret
        */
        public String getRequestTokenSecret()
        {
            return state[REQ_TOKEN_SECRET_KEY];
        }

        public void setRequestTokenSecret(String requestTokenSecret)
        {
            state.Add(REQ_TOKEN_SECRET_KEY, requestTokenSecret);
        }

        /**
        * OAuth access token.
        */
        public String getAccessToken()
        {
            return state[ACCESS_TOKEN_KEY];
        }

        public void setAccessToken(String accessToken)
        {
            state.Add(ACCESS_TOKEN_KEY, accessToken);
        }

        /**
        * OAuth access token secret.
        */
        public String getAccessTokenSecret()
        {
            return state[ACCESS_TOKEN_SECRET_KEY];
        }

        public void setAccessTokenSecret(String accessTokenSecret)
        {
            state.Add(ACCESS_TOKEN_SECRET_KEY, accessTokenSecret);
        }

        /**
        * Owner of the OAuth token.
        */
        public String getOwner()
        {
            return state[OWNER_KEY];
        }

        public void setOwner(String owner)
        {
            state.Add(OWNER_KEY, owner);
        }
    }
}