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
using System.Linq;
using Pesta.Engine.auth;
using Pesta.Interop.oauth;
using pestaServer.Models.gadgets.spec;

namespace pestaServer.Models.gadgets.oauth
{
    /// <summary>
    /// Higher-level interface that allows callers to store and retrieve
    /// OAuth-related data directly from {@code GadgetSpec}s, {@code GadgetContext}s,
    /// etc. See {@link OAuthStore} for a more detailed explanation of the OAuth
    /// Data Store.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetOAuthTokenStore
    {
        private readonly OAuthStore store;
        private readonly IGadgetSpecFactory specFactory;

        /**
         * Public constructor.
         *
         * @param store an {@link OAuthStore} that can store and retrieve OAuth
         *              tokens, as well as information about service providers.
         */

        public GadgetOAuthTokenStore(OAuthStore store, IGadgetSpecFactory specFactory)
        {
            this.store = store;
            this.specFactory = specFactory;
        }

        /**
         * Retrieve an AccessorInfo and OAuthAccessor that are ready for signing OAuthMessages.  To do
         * this, we need to figure out:
         * 
         * - what consumer key/secret to use for signing.
         * - if an access token should be used for the request, and if so what it is.   *   
         * - the OAuth request/authorization/access URLs.
         * - what HTTP method to use for request token and access token requests
         * - where the OAuth parameters are located.
         * 
         * Note that most of that work gets skipped for signed fetch, we just look up the consumer key
         * and secret for that.  Signed fetch always sticks the parameters in the query string.
         */
        public AccessorInfo getOAuthAccessor(ISecurityToken securityToken,
                                             OAuthArguments arguments, OAuthClientState clientState, OAuthResponseParams responseParams)
        {
            AccessorInfoBuilder accessorBuilder = new AccessorInfoBuilder();

            // Does the gadget spec tell us any details about the service provider, like where to put the
            // OAuth parameters and what methods to use for their URLs?
            OAuthServiceProvider provider = null;
            if (arguments.mayUseToken())
            {
                provider = lookupSpecInfo(securityToken, arguments, accessorBuilder, responseParams);
            }
            else
            {
                // This is plain old signed fetch.
                accessorBuilder.setParameterLocation(AccessorInfo.OAuthParamLocation.URI_QUERY);
            }

            // What consumer key/secret should we use?
            OAuthStore.ConsumerInfo consumer;
            try
            {
                consumer = store.getConsumerKeyAndSecret(
                    securityToken, arguments.getServiceName(), provider);
                accessorBuilder.setConsumer(consumer);
            }
            catch (GadgetException e)
            {
                throw responseParams.oauthRequestException(OAuthError.UNKNOWN_PROBLEM,
                                                           "Unable to retrieve consumer key", e);
            }


            // Should we use the OAuth access token?  We never do this unless the client allows it, and
            // if owner == viewer.
            if (arguments.mayUseToken()
                && securityToken.getOwnerId() != null
                && securityToken.getViewerId().Equals(securityToken.getOwnerId()))
            {
                lookupToken(securityToken, consumer, arguments, clientState, accessorBuilder, responseParams);
            }

            return accessorBuilder.create(responseParams);
        }

        /**
         * Lookup information contained in the gadget spec.
         */
        private OAuthServiceProvider lookupSpecInfo(ISecurityToken securityToken, OAuthArguments arguments,
                                                    AccessorInfoBuilder accessorBuilder, OAuthResponseParams responseParams)
        {
            GadgetSpec spec = findSpec(securityToken, arguments, responseParams);
            OAuthSpec oauthSpec = spec.getModulePrefs().getOAuthSpec();
            if (oauthSpec == null)
            {
                throw responseParams.oauthRequestException(OAuthError.BAD_OAUTH_CONFIGURATION,
                                                           "Failed to retrieve OAuth URLs, spec for gadget " +
                                                           securityToken.getAppUrl() + " does not contain OAuth element.");
            }
            OAuthService service = oauthSpec.getServices()[arguments.getServiceName()];
            if (service == null)
            {
                throw responseParams.oauthRequestException(OAuthError.BAD_OAUTH_CONFIGURATION,
                                                           "Failed to retrieve OAuth URLs, spec for gadget does not contain OAuth service " +
                                                           arguments.getServiceName() + ".  Known services: " +
                                                           String.Join(",",oauthSpec.getServices().Keys.AsEnumerable().ToArray()) + '.');

            }
            // In theory some one could specify different parameter locations for request token and
            // access token requests, but that's probably not useful.  We just use the request token
            // rules for everything.
            accessorBuilder.setParameterLocation(getStoreLocation(service.getRequestUrl().location, responseParams));
            accessorBuilder.setMethod(getStoreMethod(service.getRequestUrl().method, responseParams));
            OAuthServiceProvider provider = new OAuthServiceProvider(
                service.getRequestUrl().url.ToString(),
                service.getAuthorizationUrl().ToString(),
                service.getAccessUrl().url.ToString());
            return provider;
        }

        /**
         * Figure out the OAuth token that should be used with this request.  We check for this in three
         * places.  In order of priority:
         * 
         * 1) From information we cached on the client.
         *    We encrypt the token and cache on the client for performance.
         *    
         * 2) From information we have in our persistent state.
         *    We persist the token server-side so we can look it up if necessary.
         *    
         * 3) From information the gadget developer tells us to use (a preapproved request token.)
         *    Gadgets can be initialized with preapproved request tokens.  If the user tells the service
         *    provider they want to add a gadget to a gadget container site, the service provider can
         *    create a preapproved request token for that site and pass it to the gadget as a user
         *    preference.
         * @throws GadgetException 
         */
        private void lookupToken(ISecurityToken securityToken, OAuthStore.ConsumerInfo consumerInfo,
                                 OAuthArguments arguments, OAuthClientState clientState, AccessorInfoBuilder accessorBuilder, OAuthResponseParams responseParams)
        {
            if (clientState.getRequestToken() != null)
            {
                // We cached the request token on the client.
                accessorBuilder.setRequestToken(clientState.getRequestToken());
                accessorBuilder.setTokenSecret(clientState.getRequestTokenSecret());
            }
            else if (clientState.getAccessToken() != null)
            {
                // We cached the access token on the client
                accessorBuilder.setAccessToken(clientState.getAccessToken());
                accessorBuilder.setTokenSecret(clientState.getAccessTokenSecret());
                accessorBuilder.setSessionHandle(clientState.getSessionHandle());
                accessorBuilder.setTokenExpireMillis(clientState.getTokenExpireMillis());
            }
            else
            {
                // No useful client-side state, check persistent storage
                OAuthStore.TokenInfo tokenInfo;
                try
                {
                    tokenInfo = store.getTokenInfo(securityToken, consumerInfo,
                                                   arguments.getServiceName(), arguments.getTokenName());
                }
                catch (GadgetException e)
                {
                    throw responseParams.oauthRequestException(OAuthError.UNKNOWN_PROBLEM,
                                                               "Unable to retrieve access token", e);
                }
                if (tokenInfo != null && tokenInfo.getAccessToken() != null)
                {
                    // We have an access token in persistent storage, use that.
                    accessorBuilder.setAccessToken(tokenInfo.getAccessToken());
                    accessorBuilder.setTokenSecret(tokenInfo.getTokenSecret());
                    accessorBuilder.setSessionHandle(tokenInfo.getSessionHandle());
                    accessorBuilder.setTokenExpireMillis(tokenInfo.getTokenExpireMillis());
                }
                else
                {
                    // We don't have an access token yet, but the client sent us a (hopefully) preapproved
                    // request token.
                    accessorBuilder.setRequestToken(arguments.getRequestToken());
                    accessorBuilder.setTokenSecret(arguments.getRequestTokenSecret());
                }
            }
        }

        private static AccessorInfo.OAuthParamLocation getStoreLocation(OAuthService.Location location, OAuthResponseParams responseParams)
        {
            if (location == OAuthService.Location.HEADER)
            {
                return AccessorInfo.OAuthParamLocation.AUTH_HEADER;
            }
            if (location == OAuthService.Location.URL)
            {
                return AccessorInfo.OAuthParamLocation.URI_QUERY;
            }
            if (location == OAuthService.Location.BODY)
            {
                return AccessorInfo.OAuthParamLocation.POST_BODY;
            }
            throw responseParams.oauthRequestException(OAuthError.INVALID_REQUEST,
                                                       "Unknown parameter location " + location);
        }

        private static AccessorInfo.HttpMethod getStoreMethod(OAuthService.Method method, OAuthResponseParams responseParams)
        {
            if (method == OAuthService.Method.GET)
            {
                return AccessorInfo.HttpMethod.GET;
            }
            if (method == OAuthService.Method.POST)
            {
                return AccessorInfo.HttpMethod.POST;
            }
            throw responseParams.oauthRequestException(OAuthError.INVALID_REQUEST, "Unknown method " + method);

        }

        private GadgetSpec findSpec(ISecurityToken securityToken, OAuthArguments arguments, OAuthResponseParams responseParams)
        {
            try
            {
                return specFactory.getGadgetSpec(new Uri(securityToken.getAppUrl()),
                                                 arguments.getBypassSpecCache());
            }
            catch (UriFormatException e)
            {
                throw responseParams.oauthRequestException(OAuthError.UNKNOWN_PROBLEM,
                                                           "Could not fetch gadget spec, gadget URI invalid.", e);
            }
            catch (GadgetException e)
            {
                throw responseParams.oauthRequestException(OAuthError.UNKNOWN_PROBLEM,
                                                           "Could not fetch gadget spec", e);
            }
        }

        /**
         * Store an access token for the given user/gadget/service/token name
         */
        public void storeTokenKeyAndSecret(ISecurityToken securityToken, OAuthStore.ConsumerInfo consumerInfo,
                                           OAuthArguments arguments, OAuthStore.TokenInfo tokenInfo, OAuthResponseParams responseParams)
        {
            try
            {
                store.setTokenInfo(securityToken, consumerInfo, arguments.getServiceName(),
                                   arguments.getTokenName(), tokenInfo);
            }
            catch (GadgetException e)
            {
                throw responseParams.oauthRequestException(OAuthError.UNKNOWN_PROBLEM,
                                                           "Unable to store access token", e);
            }

        }

        /**
         * Remove an access token for the given user/gadget/service/token name
         */
        public void removeToken(ISecurityToken securityToken, OAuthStore.ConsumerInfo consumerInfo, OAuthArguments arguments, OAuthResponseParams responseParams)
        {
            try
            {
                store.removeToken(securityToken, consumerInfo, arguments.getServiceName(),
                                  arguments.getTokenName());
            }
            catch (GadgetException e)
            {
                throw responseParams.oauthRequestException(OAuthError.UNKNOWN_PROBLEM,
                                                           "Unable to remove access token", e);
            }

        }
    }
}