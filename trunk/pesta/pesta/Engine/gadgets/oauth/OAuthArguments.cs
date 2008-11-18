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
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Net;
using System;


namespace Pesta
{

    /// <summary>
    /// Arguments to an OAuth fetch sent by the client.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthArguments
    {
        private const String SERVICE_PARAM = "OAUTH_SERVICE_NAME";
        private const String TOKEN_PARAM = "OAUTH_TOKEN_NAME";
        private const String REQUEST_TOKEN_PARAM = "OAUTH_REQUEST_TOKEN";
        private const String REQUEST_TOKEN_SECRET_PARAM = "OAUTH_REQUEST_TOKEN_SECRET";
        private const String USE_TOKEN_PARAM = "OAUTH_USE_TOKEN";
        private const String CLIENT_STATE_PARAM = "oauthState";
        private const String BYPASS_SPEC_CACHE_PARAM = "bypassSpecCache";
        private const String SIGN_OWNER_PARAM = "signOwner";
        private const String SIGN_VIEWER_PARAM = "signViewer";

        /**
         * Should the OAuth access token be used?
         */
        public enum UseToken
        {
            /** Do not use the OAuth access token */
            NEVER,
            /**
             * Use the access token if it exists already, but don't prompt for
             * permission
             */
            IF_AVAILABLE,
            /** Use the access token if it exists, and prompt if it doesn't */
            ALWAYS,
        }

        /// <summary>
        /// Should we attempt to use an access token for the request
        /// </summary>
        ///
        private OAuthArguments.UseToken useToken;

        /// <summary>
        /// OAuth service nickname. Signed fetch uses the empty string
        /// </summary>
        ///
        private String serviceName;

        /// <summary>
        /// OAuth token nickname. Signed fetch uses the empty string
        /// </summary>
        ///
        private String tokenName;

        /// <summary>
        /// Request token the client wants us to use, may be null
        /// </summary>
        ///
        private String requestToken;

        /// <summary>
        /// Token secret that goes with the request token
        /// </summary>
        ///
        private String requestTokenSecret;

        /// <summary>
        /// Encrypted state blob stored on the client
        /// </summary>
        ///
        private String origClientState;

        /// <summary>
        /// Whether we should bypass the gadget spec cache
        /// </summary>
        ///
        private bool bypassSpecCache;

        /// <summary>
        /// Include information about the owner?
        /// </summary>
        ///
        private bool signOwner;

        /// <summary>
        /// Include information about the viewer?
        /// </summary>
        ///
        private bool signViewer;

        /// <summary>
        /// Parse OAuthArguments from parameters to the makeRequest servlet.
        /// </summary>
        ///
        /// <param name="auth">authentication type for the request</param>
        /// <param name="request">servlet request</param>
        /// @throws GadgetExceptionif any parameters are invalid.
        public OAuthArguments(AuthType auth, HttpRequest request)
        {
            this.signViewer = false;
            this.signOwner = false;
            this.bypassSpecCache = false;
            this.origClientState = null;
            this.requestTokenSecret = null;
            this.requestToken = null;
            this.tokenName = "";
            this.serviceName = "";
            this.useToken = OAuthArguments.UseToken.ALWAYS;
            useToken = ParseUseToken(auth, GetRequestParam(request,
                    USE_TOKEN_PARAM, ""));
            serviceName = GetRequestParam(request, SERVICE_PARAM, "");
            tokenName = GetRequestParam(request, TOKEN_PARAM, "");
            requestToken = GetRequestParam(request, REQUEST_TOKEN_PARAM, null);
            requestTokenSecret = GetRequestParam(request,
                    REQUEST_TOKEN_SECRET_PARAM, null);
            origClientState = GetRequestParam(request, CLIENT_STATE_PARAM, null);
            bypassSpecCache = "1".Equals(GetRequestParam(request,
                    BYPASS_SPEC_CACHE_PARAM, null));
            signOwner = Boolean.Parse(GetRequestParam(request,
                            SIGN_OWNER_PARAM, "true"));
            signViewer = Boolean.Parse(GetRequestParam(request,
                            SIGN_VIEWER_PARAM, "true"));
        }

        public OAuthArguments(RequestAuthenticationInfo info)
        {
            this.signViewer = false;
            this.signOwner = false;
            this.bypassSpecCache = false;
            this.origClientState = null;
            this.requestTokenSecret = null;
            this.requestToken = null;
            this.tokenName = "";
            this.serviceName = "";
            this.useToken = OAuthArguments.UseToken.ALWAYS;
            Dictionary<string, string> attrs = new Dictionary<string, string>();
            attrs = info.getAttributes();
            useToken = ParseUseToken(info.getAuthType(), GetAuthInfoParam(attrs, USE_TOKEN_PARAM, ""));
            serviceName = GetAuthInfoParam(attrs, SERVICE_PARAM, "");
            tokenName = GetAuthInfoParam(attrs, TOKEN_PARAM, "");
            requestToken = GetAuthInfoParam(attrs, REQUEST_TOKEN_PARAM, null);
            requestTokenSecret = GetAuthInfoParam(attrs,
                    REQUEST_TOKEN_SECRET_PARAM, null);
            origClientState = null;
            bypassSpecCache = false;
            signOwner = info.isSignOwner();
            signViewer = info.isSignViewer();
        }


        /// <returns>the named attribute from the Preload tag attributes, or default</returns>
        private static String GetAuthInfoParam(Dictionary<string, string> attrs, String name, String def)
        {
            String val = null;
            if (!attrs.TryGetValue(name, out val))
            {
                val = def;
            }
            return val;
        }


        /// <returns>the named parameter from the request, or default if the named</returns>
        private static String GetRequestParam(HttpRequest request, String name, String def)
        {
            String val = request.Params[name];
            if (val == null)
            {
                val = def;
            }
            return val;
        }

        /// <summary>
        /// Figure out what the client wants us to do with the OAuth access token.
        /// </summary>
        ///
        private static OAuthArguments.UseToken ParseUseToken(AuthType auth, String useTokenStr)
        {
            if (useTokenStr.Length == 0)
            {
                if (auth == AuthType.SIGNED)
                {
                    // signed fetch defaults to not using the token
                    return OAuthArguments.UseToken.NEVER;
                }
                else
                {
                    // OAuth defaults to always using it.
                    return OAuthArguments.UseToken.ALWAYS;
                }
            }
            useTokenStr = useTokenStr.ToLower();
            if ("always".Equals(useTokenStr))
            {
                return OAuthArguments.UseToken.ALWAYS;
            }
            if ("if_available".Equals(useTokenStr))
            {
                return OAuthArguments.UseToken.IF_AVAILABLE;
            }
            if ("never".Equals(useTokenStr))
            {
                return OAuthArguments.UseToken.NEVER;
            }
            throw new Exception("Unknown use token value " + useTokenStr);
        }

        /// <summary>
        /// Create an OAuthArguments object with all default values. The details can
        /// be filled in later using the setters.
        /// Be careful using this in anything except test code. If you find yourself
        /// wanting to use this method in real code, consider writing a new
        /// constructor instead.
        /// </summary>
        ///
        public OAuthArguments()
        {
            this.signViewer = false;
            this.signOwner = false;
            this.bypassSpecCache = false;
            this.origClientState = null;
            this.requestTokenSecret = null;
            this.requestToken = null;
            this.tokenName = "";
            this.serviceName = "";
            this.useToken = OAuthArguments.UseToken.ALWAYS;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        ///
        public OAuthArguments(OAuthArguments orig)
        {
            this.signViewer = false;
            this.signOwner = false;
            this.bypassSpecCache = false;
            this.origClientState = null;
            this.requestTokenSecret = null;
            this.requestToken = null;
            this.tokenName = "";
            this.serviceName = "";
            this.useToken = OAuthArguments.UseToken.ALWAYS;
            useToken = orig.useToken;
            serviceName = orig.serviceName;
            tokenName = orig.tokenName;
            requestToken = orig.requestToken;
            requestTokenSecret = orig.requestTokenSecret;
            origClientState = orig.origClientState;
            bypassSpecCache = orig.bypassSpecCache;
            signOwner = orig.signOwner;
            signViewer = orig.signViewer;
        }

        public bool MustUseToken()
        {
            return (useToken == OAuthArguments.UseToken.ALWAYS);
        }

        public bool MayUseToken()
        {
            return (useToken == OAuthArguments.UseToken.IF_AVAILABLE || useToken == OAuthArguments.UseToken.ALWAYS);
        }


        public String ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                this.serviceName = value;
            }
        }


        public String TokenName
        {
            get
            {
                return tokenName;
            }
            set
            {
                this.tokenName = value;
            }
        }


        public String RequestToken
        {
            get
            {
                return requestToken;
            }
            set
            {
                this.requestToken = value;
            }
        }


        public String RequestTokenSecret
        {
            get
            {
                return requestTokenSecret;
            }
            set
            {
                this.requestTokenSecret = value;
            }
        }


        public String OrigClientState
        {
            get
            {
                return origClientState;
            }
            set
            {
                this.origClientState = value;
            }
        }


        public bool BypassSpecCache
        {
            get
            {
                return bypassSpecCache;
            }
            set
            {
                this.bypassSpecCache = value;
            }
        }


        public bool SignOwner
        {
            get
            {
                return signOwner;
            }
            set
            {
                this.signOwner = value;
            }
        }


        public bool SignViewer
        {
            get
            {
                return signViewer;
            }
            set
            {
                this.signViewer = value;
            }
        }

    } 
}

