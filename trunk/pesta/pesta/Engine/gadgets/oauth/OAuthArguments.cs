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
using System.Web;
using System;
using Pesta.Engine.gadgets.spec;


namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Arguments to an OAuth fetch sent by the client.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
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
        private UseToken useToken;

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


        /** Arbitrary name/value pairs associated with the request */
        private readonly Dictionary<String, String> requestOptions = new Dictionary<string, string>();

        /// <summary>
        /// Parse OAuthArguments from parameters to the makeRequest servlet.
        /// </summary>
        ///
        /// <param name="auth">authentication type for the request</param>
        /// <param name="request">servlet request</param>
        /// @throws GadgetExceptionif any parameters are invalid.
        public OAuthArguments(AuthType auth, HttpRequest request)
        {
            signViewer = false;
            signOwner = false;
            bypassSpecCache = false;
            origClientState = null;
            requestTokenSecret = null;
            requestToken = null;
            tokenName = "";
            serviceName = "";
            useToken = UseToken.ALWAYS;
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
            signViewer = false;
            signOwner = false;
            bypassSpecCache = false;
            origClientState = null;
            requestTokenSecret = null;
            requestToken = null;
            tokenName = "";
            serviceName = "";
            useToken = UseToken.ALWAYS;
            Dictionary<string, string> attrs = info.getAttributes();
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
            foreach (var pair in info.getAttributes())
            {
                requestOptions.Add(pair.Key,pair.Value);
            }
        }


        /// <returns>the named attribute from the Preload tag attributes, or default</returns>
        private static String GetAuthInfoParam(IDictionary<string, string> attrs, String name, String def)
        {
            String val;
            if (!attrs.TryGetValue(name, out val))
            {
                val = def;
            }
            return val;
        }


        /// <returns>the named parameter from the request, or default if the named</returns>
        private static String GetRequestParam(HttpRequest request, String name, String def)
        {
            String val = request.Params[name] ?? def;
            return val;
        }

        /// <summary>
        /// Figure out what the client wants us to do with the OAuth access token.
        /// </summary>
        ///
        private static UseToken ParseUseToken(AuthType auth, String useTokenStr)
        {
            if (useTokenStr.Length == 0)
            {
                // OAuth defaults to always using it.
                return auth == AuthType.SIGNED ? UseToken.NEVER : UseToken.ALWAYS;
            }
            useTokenStr = useTokenStr.ToLower();
            if ("always".Equals(useTokenStr))
            {
                return UseToken.ALWAYS;
            }
            if ("if_available".Equals(useTokenStr))
            {
                return UseToken.IF_AVAILABLE;
            }
            if ("never".Equals(useTokenStr))
            {
                return UseToken.NEVER;
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
            signViewer = false;
            signOwner = false;
            bypassSpecCache = false;
            origClientState = null;
            requestTokenSecret = null;
            requestToken = null;
            tokenName = "";
            serviceName = "";
            useToken = UseToken.ALWAYS;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        ///
        public OAuthArguments(OAuthArguments orig)
        {
            signViewer = false;
            signOwner = false;
            bypassSpecCache = false;
            origClientState = null;
            requestTokenSecret = null;
            requestToken = null;
            tokenName = "";
            serviceName = "";
            useToken = UseToken.ALWAYS;
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

        public bool mustUseToken()
        {
            return (useToken == UseToken.ALWAYS);
        }

        public bool mayUseToken()
        {
            return (useToken == UseToken.IF_AVAILABLE || useToken == UseToken.ALWAYS);
        }

        public UseToken getUseToken()
        {
            return useToken;
        }

        public void setUseToken(UseToken _useToken)
        {
            useToken = _useToken;
        }

        public String getServiceName()
        {
            return serviceName;
        }

        public void setServiceName(String _serviceName)
        {
            serviceName = _serviceName;
        }

        public String getTokenName()
        {
            return tokenName;
        }

        public void setTokenName(String _tokenName)
        {
            tokenName = _tokenName;
        }

        public String getRequestToken()
        {
            return requestToken;
        }

        public void setRequestToken(String _requestToken)
        {
            requestToken = _requestToken;
        }

        public String getRequestTokenSecret()
        {
            return requestTokenSecret;
        }

        public void setRequestTokenSecret(String _requestTokenSecret)
        {
            requestTokenSecret = _requestTokenSecret;
        }

        public String getOrigClientState()
        {
            return origClientState;
        }

        public void setOrigClientState(String _origClientState)
        {
            origClientState = _origClientState;
        }

        public bool getBypassSpecCache()
        {
            return bypassSpecCache;
        }

        public void setBypassSpecCache(bool _bypassSpecCache)
        {
            bypassSpecCache = _bypassSpecCache;
        }

        public bool getSignOwner()
        {
            return signOwner;
        }

        public void setSignOwner(bool _signOwner)
        {
            signOwner = _signOwner;
        }

        public bool getSignViewer()
        {
            return signViewer;
        }

        public void setSignViewer(bool _signViewer)
        {
            signViewer = _signViewer;
        }

        public void setRequestOption(String name, String value)
        {
            requestOptions.Add(name, value);
        }

        public void removeRequestOption(String name)
        {
            requestOptions.Remove(name);
        }

        public String getRequestOption(String name)
        {
            return requestOptions[name];
        }
    }
}