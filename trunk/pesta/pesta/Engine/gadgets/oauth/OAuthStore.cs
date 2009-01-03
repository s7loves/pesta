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
using Pesta.Engine.auth;
using Pesta.Interop.oauth;


namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Summary description for OAuthStore
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public abstract class OAuthStore
    {
        /**
       * Information about an OAuth consumer.
       */
        public class ConsumerInfo
        {
            private readonly OAuthConsumer consumer;
            private readonly String keyName;

            /**
            * @param consumer the OAuth consumer
            * @param keyName the name of the key to use for this consumer (passed on query parameters to
            * help with key rotation.)
            */
            public ConsumerInfo(OAuthConsumer consumer, String keyName)
            {
                this.consumer = consumer;
                this.keyName = keyName;
            }

            public OAuthConsumer getConsumer()
            {
                return consumer;
            }

            public String getKeyName()
            {
                return keyName;
            }
        }

        /**
        * Information about an access token.
        */
        public class TokenInfo
        {
            private readonly String accessToken;
            private readonly String tokenSecret;
            public TokenInfo(String token, String secret)
            {
                accessToken = token;
                tokenSecret = secret;
            }
            public String getAccessToken()
            {
                return accessToken;
            }
            public String getTokenSecret()
            {
                return tokenSecret;
            }
        }

        /**
        * Retrieve OAuth consumer to use for requests.  The returned consumer is ready to use for signed
        * fetch requests.
        * 
        * @param securityToken token for user/gadget making request.
        * @param serviceName gadget's nickname for the service being accessed.
        * @param provider OAuth service provider info to be inserted into the returned consumer.
        * 
        * @throws GadgetException if no OAuth consumer can be found (e.g. no consumer key can be used.)
        */
        public abstract ConsumerInfo getConsumerKeyAndSecret(SecurityToken securityToken, String serviceName,
                                                             OAuthServiceProvider provider);

        /**
        * Retrieve OAuth access token to use for the request.
        * @param securityToken token for user/gadget making request.
        * @param consumerInfo OAuth consumer that will be used for the request.
        * @param serviceName gadget's nickname for the service being accessed.
        * @param tokenName gadget's nickname for the token to use.
        * @return the token and secret, or null if none exist
        * @throws GadgetException if an error occurs during lookup
        */
        public abstract TokenInfo getTokenInfo(SecurityToken securityToken, ConsumerInfo consumerInfo,
                                               String serviceName, String tokenName);

        /**
        * Set the access token for the given user/gadget/service/token
        */
        public abstract void setTokenInfo(SecurityToken securityToken, ConsumerInfo consumerInfo, String serviceName,
                                          String tokenName, TokenInfo tokenInfo);

        /**
        * Remove the access token for the given user/gadget/service/token
        */
        public abstract void removeToken(SecurityToken securityToken, ConsumerInfo consumerInfo,
                                         String serviceName, String tokenName);
    }
}