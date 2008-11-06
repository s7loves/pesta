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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;



namespace Pesta
{
    /// <summary>
    /// Summary description for OAuthFetcherConfig
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthFetcherConfig
    {
        private BlobCrypter stateCrypter;
        private GadgetOAuthTokenStore tokenStore;

        public OAuthFetcherConfig(BlobCrypter stateCrypter, GadgetOAuthTokenStore tokenStore)
        {
            this.stateCrypter = stateCrypter;
            this.tokenStore = tokenStore;
        }

        /**
        * Used to encrypt state stored on the client.
        */
        public BlobCrypter getStateCrypter()
        {
            return stateCrypter;
        }

        /**
        * Persistent token storage.
        */
        public GadgetOAuthTokenStore getTokenStore()
        {
            return tokenStore;
        }
    } 
}
