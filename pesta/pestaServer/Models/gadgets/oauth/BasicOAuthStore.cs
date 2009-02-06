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
    /// Summary description for BasicOAuthStore
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
        protected BasicOAuthStore()
        {
            consumerInfos = new Dictionary<BasicOAuthStoreConsumerIndex, BasicOAuthStoreConsumerKeyAndSecret>();
            tokens = new Dictionary<BasicOAuthStoreTokenIndex, TokenInfo>();
            loadConsumers();
        }

        public void initFromConfigString(String oauthConfigStr)
        {
            try
            {
                JsonObject oauthConfigs = (JsonObject)JsonConvert.Import(oauthConfigStr);
                foreach (DictionaryEntry url in oauthConfigs)
                {
                    Uri gadgetUri = new Uri(url.Key.ToString());
                    JsonObject oauthConfig = (JsonObject)url.Value;
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

        private void storeConsumerInfos(Uri gadgetUri, JsonObject oauthConfig)
        {
            foreach (String serviceName in oauthConfig.Names)
            {
                JsonObject consumerInfo = (JsonObject)oauthConfig[serviceName];
                storeConsumerInfo(gadgetUri, serviceName, consumerInfo);
            }
        }

        private void storeConsumerInfo(Uri gadgetUri, String serviceName, JsonObject consumerInfo)
        {
            realStoreConsumerInfo(gadgetUri, serviceName, consumerInfo);
        }

        private void realStoreConsumerInfo(Uri gadgetUri, String serviceName, JsonObject consumerInfo)
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
            setConsumerKeyAndSecret(index, kas);
        }

        public void setDefaultKey(BasicOAuthStoreConsumerKeyAndSecret _defaultKey)
        {
            defaultKey = _defaultKey;
        }

        public void setConsumerKeyAndSecret(BasicOAuthStoreConsumerIndex providerKey, BasicOAuthStoreConsumerKeyAndSecret keyAndSecret)
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

        private static BasicOAuthStoreTokenIndex makeBasicOAuthStoreTokenIndex(ISecurityToken securityToken, String serviceName, String tokenName)
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
            BasicOAuthStoreTokenIndex tokenKey = makeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            return tokens.ContainsKey(tokenKey)?tokens[tokenKey]:null;
        }

        public override void setTokenInfo(ISecurityToken securityToken, ConsumerInfo consumerInfo,
                                          String serviceName, String tokenName, TokenInfo tokenInfo)
        {
            ++accessTokenAddCount;
            BasicOAuthStoreTokenIndex tokenKey = makeBasicOAuthStoreTokenIndex(securityToken, serviceName, tokenName);
            tokens.Add(tokenKey, tokenInfo);
        }

        public override void removeToken(ISecurityToken securityToken, ConsumerInfo consumerInfo, String serviceName, String tokenName)
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

        private void loadConsumers()
        {
            String oauthConfigString = ResourceLoader.getContent(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + OAUTH_CONFIG));
            initFromConfigString(oauthConfigString);
        }
    }
}