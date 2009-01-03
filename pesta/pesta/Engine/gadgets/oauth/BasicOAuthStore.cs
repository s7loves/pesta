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
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Pesta.Engine.auth;
using Pesta.Interop.oauth;
using Pesta.Interop.oauth.signature;
using URI = System.Uri;

namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Summary description for BasicOAuthStore
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class BasicOAuthStore : OAuthStore
    {
        private static String CONSUMER_SECRET_KEY = "consumer_secret";
        private static String CONSUMER_KEY_KEY = "consumer_key";
        private static String KEY_TYPE_KEY = "key_type";

        /**
         * HashMap of provider and consumer information. Maps BasicOAuthStoreConsumerIndexs (i.e.
         * nickname of a service provider and the gadget that uses that nickname) to
         * {@link BasicOAuthStoreConsumerKeyAndSecret}s.
         */
        private Dictionary<BasicOAuthStoreConsumerIndex, BasicOAuthStoreConsumerKeyAndSecret> consumerInfos;

        /**
        * HashMap of token information. Maps BasicOAuthStoreTokenIndexs (i.e. gadget id, token
        * nickname, module id, etc.) to TokenInfos (i.e. access token and token
        * secrets).
        */
        private Dictionary<BasicOAuthStoreTokenIndex, TokenInfo> tokens;

        /**
        * Key to use when no other key is found.
        */
        private BasicOAuthStoreConsumerKeyAndSecret defaultKey;

        /** Number of times we looked up a consumer key */
        private int consumerKeyLookupCount;

        /** Number of times we looked up an access token */
        private int accessTokenLookupCount;

        /** Number of times we added an access token */
        private int accessTokenAddCount;

        /** Number of times we removed an access token */
        private int accessTokenRemoveCount;
        public readonly static BasicOAuthStore Instance = new BasicOAuthStore();
        protected BasicOAuthStore()
        {
            consumerInfos = new Dictionary<BasicOAuthStoreConsumerIndex, BasicOAuthStoreConsumerKeyAndSecret>();
            tokens = new Dictionary<BasicOAuthStoreTokenIndex, TokenInfo>();
        }

        public void initFromConfigString(String oauthConfigStr)
        {
            try
            {
                JsonObject oauthConfigs = JsonConvert.Import(oauthConfigStr) as JsonObject;
                foreach (String url in oauthConfigs)
                {
                    URI gadgetUri = new URI(url);
                    JsonObject oauthConfig = oauthConfigs[url] as JsonObject;
                    storeConsumerInfos(gadgetUri, oauthConfig);
                }
            }
            catch (JsonException e)
            {
                throw new GadgetException(GadgetException.Code.OAUTH_STORAGE_ERROR, e);
            }
            catch (UriFormatException e)
            {
                throw new GadgetException(GadgetException.Code.OAUTH_STORAGE_ERROR, e);
            }
        }

        private void storeConsumerInfos(URI gadgetUri, JsonObject oauthConfig)
        {
            foreach (String serviceName in oauthConfig.Names)
            {
                JsonObject consumerInfo = oauthConfig[serviceName] as JsonObject;
                storeConsumerInfo(gadgetUri, serviceName, consumerInfo);
            }
        }

        private void storeConsumerInfo(URI gadgetUri, String serviceName, JsonObject consumerInfo)
        {
            realStoreConsumerInfo(gadgetUri, serviceName, consumerInfo);
        }

        private void realStoreConsumerInfo(URI gadgetUri, String serviceName, JsonObject consumerInfo)
        {
            String consumerSecret = consumerInfo[CONSUMER_SECRET_KEY] as String;
            String consumerKey = consumerInfo[CONSUMER_KEY_KEY] as String;
            String keyTypeStr = consumerInfo[KEY_TYPE_KEY] as String;
            BasicOAuthStoreConsumerKeyAndSecret.KeyType keyType = BasicOAuthStoreConsumerKeyAndSecret.KeyType.HMAC_SYMMETRIC;

            if (keyTypeStr.Equals("RSA_PRIVATE"))
            {
                keyType = BasicOAuthStoreConsumerKeyAndSecret.KeyType.RSA_PRIVATE;
                consumerSecret = convertFromOpenSsl(consumerSecret);
            }

            BasicOAuthStoreConsumerKeyAndSecret kas = new BasicOAuthStoreConsumerKeyAndSecret(consumerKey, consumerSecret, keyType, null);

            BasicOAuthStoreConsumerIndex index = new BasicOAuthStoreConsumerIndex();
            index.setGadgetUri(gadgetUri.ToString());
            index.setServiceName(serviceName);
            setConsumerKeyAndSecret(index, kas);
        }

        // Support standard openssl keys by stripping out the headers and blank lines
        public static String convertFromOpenSsl(String privateKey)
        {
            return privateKey.Replace("-----[A-Z ]*-----", "").Replace("\n", "");
        }

        public void setDefaultKey(BasicOAuthStoreConsumerKeyAndSecret defaultKey)
        {
            this.defaultKey = defaultKey;
        }

        public void setConsumerKeyAndSecret(BasicOAuthStoreConsumerIndex providerKey, BasicOAuthStoreConsumerKeyAndSecret keyAndSecret)
        {
            consumerInfos.Add(providerKey, keyAndSecret);
        }

        public override ConsumerInfo getConsumerKeyAndSecret(SecurityToken securityToken, String serviceName, OAuthServiceProvider provider)
        {
            ++consumerKeyLookupCount;
            BasicOAuthStoreConsumerIndex pk = new BasicOAuthStoreConsumerIndex();
            pk.setGadgetUri(securityToken.getAppUrl());
            pk.setServiceName(serviceName);
            BasicOAuthStoreConsumerKeyAndSecret cks = consumerInfos.ContainsKey(pk) ? consumerInfos[pk] : defaultKey;
            if (cks == null)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR,
                                          "No key for gadget " + securityToken.getAppUrl() + " and service " + serviceName);
            }
            OAuthConsumer consumer = null;
            if (cks.keyType == BasicOAuthStoreConsumerKeyAndSecret.KeyType.RSA_PRIVATE)
            {
                consumer = new OAuthConsumer(null, cks.ConsumerKey, null, provider);
                // The oauth.net java code has lots of magic.  By setting this property here, code thousands
                // of lines away knows that the consumerSecret value in the consumer should be treated as
                // an RSA private key and not an HMAC key.
                consumer.setProperty(OAuth.OAUTH_SIGNATURE_METHOD, OAuth.RSA_SHA1);
                consumer.setProperty(RSA_SHA1.PRIVATE_KEY, cks.ConsumerSecret);
            }
            else
            {
                consumer = new OAuthConsumer(null, cks.ConsumerKey, cks.ConsumerSecret, provider);
                consumer.setProperty(OAuth.OAUTH_SIGNATURE_METHOD, OAuth.HMAC_SHA1);
            }
            return new ConsumerInfo(consumer, cks.KeyName);
        }

        private BasicOAuthStoreTokenIndex makeBasicOAuthStoreTokenIndex(SecurityToken securityToken, String serviceName, String tokenName)
        {
            BasicOAuthStoreTokenIndex tokenKey = new BasicOAuthStoreTokenIndex();
            tokenKey.setGadgetUri(securityToken.getAppUrl());
            tokenKey.setModuleId(securityToken.getModuleId());
            tokenKey.setServiceName(serviceName);
            tokenKey.setTokenName(tokenName);
            tokenKey.setUserId(securityToken.getViewerId());
            return tokenKey;
        }

        public override TokenInfo getTokenInfo(SecurityToken securityToken, ConsumerInfo consumerInfo,
                                               String serviceName, String tokenName)
        {
            ++accessTokenLookupCount;
            BasicOAuthStoreTokenIndex tokenKey = makeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            return tokens[tokenKey];
        }

        public override void setTokenInfo(SecurityToken securityToken, ConsumerInfo consumerInfo,
                                          String serviceName, String tokenName, TokenInfo tokenInfo)
        {
            ++accessTokenAddCount;
            BasicOAuthStoreTokenIndex tokenKey = makeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            tokens.Add(tokenKey, tokenInfo);
        }

        public override void removeToken(SecurityToken securityToken, ConsumerInfo consumerInfo, String serviceName, String tokenName)
        {
            ++accessTokenRemoveCount;
            BasicOAuthStoreTokenIndex tokenKey = makeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            tokens.Remove(tokenKey);
        }

        public int getConsumerKeyLookupCount()
        {
            return consumerKeyLookupCount;
        }

        public int getAccessTokenLookupCount()
        {
            return accessTokenLookupCount;
        }

        public int getAccessTokenAddCount()
        {
            return accessTokenAddCount;
        }

        public int getAccessTokenRemoveCount()
        {
            return accessTokenRemoveCount;
        }
    }
}