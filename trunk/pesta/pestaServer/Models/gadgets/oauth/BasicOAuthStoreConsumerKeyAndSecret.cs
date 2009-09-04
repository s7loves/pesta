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

namespace pestaServer.Models.gadgets.oauth
{
    /// @author beaton@google.com (Your Name Here)
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class BasicOAuthStoreConsumerKeyAndSecret
    {

        public enum KeyType
        {
            HMAC_SYMMETRIC, RSA_PRIVATE
        }

        /// <summary>
        /// Value for oauthConsumer_key
        /// </summary>
        ///
        private readonly String consumerKey;

        /// <summary>
        /// HMAC secret, or RSA private key, depending on keyType
        /// </summary>
        ///
        private readonly String consumerSecret;

        /// <summary>
        /// Type of key
        /// </summary>
        ///
        public readonly KeyType keyType;

        private readonly String certName;
        private readonly String certPass;
        public BasicOAuthStoreConsumerKeyAndSecret(String key, String secret,
                                                   KeyType type, String name, String pass)
        {
            consumerKey = key;
            consumerSecret = secret;
            keyType = type;
            certName = name;
            certPass = pass;
        }

        public String ConsumerKey
        {
            get
            {
                return consumerKey;
            }
        }
        
        public String ConsumerSecret
        {
            get
            {
                return consumerSecret;
            }
        }

        public String CertName
        {
            get
            {
                return certName;
            }
        }
        public String CertPass
        {
            get
            {
                return certPass;
            }
        }

    }
}