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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Pesta.Engine.auth;
using Pesta.Engine.common;
using Pesta.Interop.oauth;
using Pesta.Interop.oauth.signature;

namespace pestaServer.Models.gadgets.oauth
{
    /// <summary>
    /// Simple implementation of the {@link OAuthStore} interface. We use a
    /// in-memory hash map. If initialized with a private key, then the store will
    /// return an OAuthAccessor in {@code getOAuthAccessor} that uses that private
    /// key if no consumer key and secret could be found.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BasicOAuthStore : OAuthStore
    {
        private const string CONSUMER_SECRET_KEY = "consumer_secret";
        private const string CERTIFICATE_LOC = "certificate";
        private const string CERTIFICATE_PASS = "certificate_pass";
        private const string CONSUMER_KEY_KEY = "consumer_key";
        private const string KEY_TYPE_KEY = "key_type";
        private const string OAUTH_CONFIG = "config/oauth.json";

        /**
         * HashMap of provider and consumer information. Maps BasicOAuthStoreConsumerIndexs (i.e.
         * nickname of a service provider and the gadget that uses that nickname) to
         * {@link BasicOAuthStoreConsumerKeyAndSecret}s.
         */
        private readonly Dictionary<BasicOAuthStoreConsumerIndex, BasicOAuthStoreConsumerKeyAndSecret> consumerInfos;

        /**
        * HashMap of token information. Maps BasicOAuthStoreTokenIndexs (i.e. gadget id, token
        * nickname, module id, etc.) to TokenInfos (i.e. access token and token
        * secrets).
        */
        private readonly Dictionary<BasicOAuthStoreTokenIndex, TokenInfo> tokens;

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

        private BasicOAuthStore()
        {
            consumerInfos = new Dictionary<BasicOAuthStoreConsumerIndex, BasicOAuthStoreConsumerKeyAndSecret>();
            tokens = new Dictionary<BasicOAuthStoreTokenIndex, TokenInfo>();
            LoadConsumers();
        }

        private void InitFromConfigString(String oauthConfigStr)
        {
            try
            {
                JsonObject oauthConfigs = (JsonObject)JsonConvert.Import(oauthConfigStr);
                foreach (DictionaryEntry url in oauthConfigs)
                {
                    Uri gadgetUri = new Uri(url.Key.ToString());
                    JsonObject oauthConfig = (JsonObject)url.Value;
                    StoreConsumerInfos(gadgetUri, oauthConfig);
                }
            }
            catch (Exception e)
            {
                throw new GadgetException(GadgetException.Code.OAUTH_STORAGE_ERROR, e);
            }
        }

        private void StoreConsumerInfos(Uri gadgetUri, JsonObject oauthConfig)
        {
            foreach (String serviceName in oauthConfig.Names)
            {
                JsonObject consumerInfo = (JsonObject)oauthConfig[serviceName];
                StoreConsumerInfo(gadgetUri, serviceName, consumerInfo);
            }
        }

        private void StoreConsumerInfo(Uri gadgetUri, String serviceName, JsonObject consumerInfo)
        {
            RealStoreConsumerInfo(gadgetUri, serviceName, consumerInfo);
        }

        private void RealStoreConsumerInfo(Uri gadgetUri, String serviceName, JsonObject consumerInfo)
        {
            String consumerKey = consumerInfo[CONSUMER_KEY_KEY].ToString();
            String keyTypeStr = consumerInfo[KEY_TYPE_KEY].ToString();
            BasicOAuthStoreConsumerKeyAndSecret.KeyType keyType;
            BasicOAuthStoreConsumerKeyAndSecret kas;
            if (keyTypeStr.Equals("RSA_PRIVATE"))
            {
                String certName = consumerInfo[CERTIFICATE_LOC].ToString();
                String certPass = consumerInfo[CERTIFICATE_PASS].ToString();
                keyType = BasicOAuthStoreConsumerKeyAndSecret.KeyType.RSA_PRIVATE;
                kas = new BasicOAuthStoreConsumerKeyAndSecret(consumerKey, null, keyType, certName, certPass);
            }
            else
            {
                String consumerSecret = consumerInfo[CONSUMER_SECRET_KEY].ToString();
                keyType = BasicOAuthStoreConsumerKeyAndSecret.KeyType.HMAC_SYMMETRIC;
                kas = new BasicOAuthStoreConsumerKeyAndSecret(consumerKey, consumerSecret, keyType, null, null);
            }
            
            BasicOAuthStoreConsumerIndex index = new BasicOAuthStoreConsumerIndex();
            index.setGadgetUri(gadgetUri.ToString());
            index.setServiceName(serviceName);
            SetConsumerKeyAndSecret(index, kas);
        }

        public void SetDefaultKey(BasicOAuthStoreConsumerKeyAndSecret key)
        {
            defaultKey = key;
        }

        public void SetConsumerKeyAndSecret(BasicOAuthStoreConsumerIndex providerKey, BasicOAuthStoreConsumerKeyAndSecret keyAndSecret)
        {
            consumerInfos.Add(providerKey, keyAndSecret);
        }

        public override ConsumerInfo getConsumerKeyAndSecret(ISecurityToken securityToken, String serviceName, OAuthServiceProvider provider)
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
            OAuthConsumer consumer;
            if (cks.keyType == BasicOAuthStoreConsumerKeyAndSecret.KeyType.RSA_PRIVATE)
            {
                consumer = new OAuthConsumer(null, cks.ConsumerKey, null, provider);
                consumer.setProperty(OAuth.OAUTH_SIGNATURE_METHOD, OAuth.RSA_SHA1);
                consumer.setProperty(RSA_SHA1.X509_CERTIFICATE, cks.CertName);
                consumer.setProperty(RSA_SHA1.X509_CERTIFICATE_PASS, cks.CertPass);
            }
            else
            {
                consumer = new OAuthConsumer(null, cks.ConsumerKey, cks.ConsumerSecret, provider);
                consumer.setProperty(OAuth.OAUTH_SIGNATURE_METHOD, OAuth.HMAC_SHA1);
            }
            return new ConsumerInfo(consumer, cks.ConsumerKey);
        }

        private static BasicOAuthStoreTokenIndex MakeBasicOAuthStoreTokenIndex(ISecurityToken securityToken, String serviceName, String tokenName)
        {
            BasicOAuthStoreTokenIndex tokenKey = new BasicOAuthStoreTokenIndex();
            tokenKey.setGadgetUri(securityToken.getAppUrl());
            tokenKey.setModuleId(securityToken.getModuleId());
            tokenKey.setServiceName(serviceName);
            tokenKey.setTokenName(tokenName);
            tokenKey.setUserId(securityToken.getViewerId());
            return tokenKey;
        }

        public override TokenInfo getTokenInfo(ISecurityToken securityToken, ConsumerInfo consumerInfo,
                                               String serviceName, String tokenName)
        {
            ++accessTokenLookupCount;
            BasicOAuthStoreTokenIndex tokenKey = MakeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            return tokens.ContainsKey(tokenKey)?tokens[tokenKey]:null;
        }

        public override void setTokenInfo(ISecurityToken securityToken, ConsumerInfo consumerInfo,
                                          String serviceName, String tokenName, TokenInfo tokenInfo)
        {
            ++accessTokenAddCount;
            BasicOAuthStoreTokenIndex tokenKey = MakeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            tokens.Add(tokenKey, tokenInfo);
        }

        public override void removeToken(ISecurityToken securityToken, ConsumerInfo consumerInfo, String serviceName, String tokenName)
        {
            ++accessTokenRemoveCount;
            BasicOAuthStoreTokenIndex tokenKey = MakeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            tokens.Remove(tokenKey);
        }

        public int GetConsumerKeyLookupCount()
        {
            return consumerKeyLookupCount;
        }

        public int GetAccessTokenLookupCount()
        {
            return accessTokenLookupCount;
        }

        public int GetAccessTokenAddCount()
        {
            return accessTokenAddCount;
        }

        public int GetAccessTokenRemoveCount()
        {
            return accessTokenRemoveCount;
        }

        private void LoadConsumers()
        {
            String oauthConfigString = ResourceLoader.GetContent(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + OAUTH_CONFIG));
            InitFromConfigString(oauthConfigString);
        }
    }
}