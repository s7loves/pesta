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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace Pesta
{

    /// <summary>
    /// Implements both signed fetch and full OAuth for gadgets, as well as a combination of the two that
    /// is necessary to build OAuth enabled gadgets for social sites.
    /// Signed fetch sticks identity information in the query string, signed either with the container's
    /// private key, or else with a secret shared between the container and the gadget.
    /// Full OAuth redirects the user to the OAuth service provider site to obtain the user's permission
    /// to access their data.  Read the example in the appendix to the OAuth spec for a summary of how
    /// this works (The spec is at http://oauth.net/core/1.0/).
    /// The combination protocol works by sending identity information in all requests, and allows the
    /// OAuth dance to happen as well when owner == viewer.  This lets OAuth service providers build up
    /// an identity mapping from ids on social network sites to their own local ids.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthFetcher : ChainedContentFetcher
    {
        // Maximum number of attempts at the protocol before giving up.
        private const int MAX_ATTEMPTS = 2;

        // names of additional OAuth parameters we include in outgoing requests
        // TODO(beaton): can we do away with this bit in favor of the opensocial param?
        public const String XOAUTH_APP_URL = "xoauth_app_url";

        protected internal const String OPENSOCIAL_OWNERID = "opensocial_owner_id";

        protected internal const String OPENSOCIAL_VIEWERID = "opensocial_viewer_id";

        protected internal const String OPENSOCIAL_APPID = "opensocial_app_id";

        // TODO(beaton): figure out if this is the name in the 0.8 spec.
        protected internal const String OPENSOCIAL_APPURL = "opensocial_app_url";

        protected internal const String XOAUTH_PUBLIC_KEY = "xoauth_signature_publickey";

        protected static internal readonly Regex ALLOWED_PARAM_NAME = new Regex("[-:\\w~!@$*()_\\[\\]:,./]+", RegexOptions.Compiled);

        /// <summary>
        /// State information from client
        /// </summary>
        ///
        protected internal readonly OAuthClientState clientState;

        /// <summary>
        /// Configuration options for the fetcher.
        /// </summary>
        ///
        protected internal readonly OAuthFetcherConfig fetcherConfig;

        /// <summary>
        /// OAuth specific stuff to include in the response.
        /// </summary>
        ///
        protected internal readonly OAuthResponseParams responseParams;

        /// <summary>
        /// The accessor we use for signing messages. This also holds metadata about
        /// the service provider, such as their URLs and the keys we use to access
        /// those URLs.
        /// </summary>
        ///
        private AccessorInfo accessorInfo;

        /// <summary>
        /// The request the client really wants to make.
        /// </summary>
        ///
        private sRequest realRequest;


        /**
       * @param fetcherConfig configuration options for the fetcher
       * @param nextFetcher fetcher to use for actually making requests
       * @param request request that will be sent through the fetcher
       */
        public OAuthFetcher(OAuthFetcherConfig fetcherConfig, HttpFetcher nextFetcher, sRequest request)
            : base(nextFetcher)
        {
            this.fetcherConfig = fetcherConfig;
            this.clientState = new OAuthClientState(fetcherConfig.getStateCrypter(), request.OAuthArguments.OrigClientState);
            this.responseParams = new OAuthResponseParams(fetcherConfig.getStateCrypter());
        }

        /**
        * Retrieves metadata from our persistent store.
        *
        * TODO(beaton): can we fix this so it avoids hitting the persistent data
        * store when a client makes multiple requests with an approved access token?
        *
        * @throws GadgetException
        */
        private void lookupOAuthMetadata()
        {
            accessorInfo = fetcherConfig.getTokenStore().getOAuthAccessor(realRequest.SecurityToken, realRequest.OAuthArguments, clientState);
        }

        public override sResponse fetch(sRequest request)
        {
            this.realRequest = request;
            HttpCacheKey cacheKey = makeCacheKey();
            sResponse response = HttpCache.getResponse(cacheKey, request);
            if (response != null)
            {
                return response;
            }

            try
            {
                lookupOAuthMetadata();
            }
            catch (GadgetException e)
            {
                responseParams.setError(OAuthError.BAD_OAUTH_CONFIGURATION);
                //return buildErrorResponse(e);
                return null;
            }

            int attempts = 0;
            bool retry;
            do
            {
                retry = false;
                ++attempts;
                try
                {
                    response = attemptFetch();
                }
                catch (OAuthProtocolException pe)
                {
                    retry = handleProtocolException(pe, attempts);
                    if (!retry)
                    {
                        response = null;
                    }
                }
            } while (retry);

            if (response == null)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR,
                            "No response for OAuth fetch to " + realRequest.Uri);
            }
            return HttpCache.addResponse(cacheKey, request, response);
        }

        // Builds up a cache key.  Full OAuth and signed fetch have slightly different cache semantics
        // that both need to be accounted for here.  For signed fetch, we need to remember what identity
        // information we passed along (owner only?  viewer only?  both?).  For OAuth, we need to
        // remember whose OAuth token we used.  We only use the OAuth token when owner == viewer, and
        // it's possible we won't do it even then.
        private HttpCacheKey makeCacheKey()
        {
            HttpCacheKey key = new HttpCacheKey(realRequest);
            SecurityToken st = realRequest.SecurityToken;
            key.set("authentication", "oauth");
            if (realRequest.OAuthArguments.SignOwner)
            {
                key.set("owner", st.getOwnerId());
            }
            if (realRequest.OAuthArguments.SignViewer)
            {
                key.set("viewer", st.getViewerId());
            }
            if (st.getOwnerId() != null
                && st.getOwnerId().Equals(st.getViewerId())
                && realRequest.OAuthArguments.MayUseToken())
            {
                key.set("tokenOwner", st.getOwnerId());
            }
            key.set("gadget", st.getAppUrl());
            key.set("instance", st.getModuleId().ToString());
            key.set("service", realRequest.OAuthArguments.ServiceName);
            key.set("token", realRequest.OAuthArguments.TokenName);
            return key;
        }

        private sResponse buildErrorResponse(Exception e)
        {
            if (responseParams.getError() == null)
            {
                responseParams.setError(OAuthError.UNKNOWN_PROBLEM);
            }
            //if (responseParams.getErrorText() == null && (e is UserVisibleOAuthException)) 
            //{
            //    responseParams.setErrorText(e.getMessage());
            //}
            //return buildNonDataResponse(HttpStatusCode.Forbidden);
            return null;
        }

        private bool handleProtocolException(OAuthProtocolException pe, int attempts)
        {
            if (pe.startFromScratch)
            {
                fetcherConfig.getTokenStore().removeToken(realRequest.SecurityToken,
                accessorInfo.getConsumer(), realRequest.OAuthArguments);
                accessorInfo.getAccessor().accessToken = null;
                accessorInfo.getAccessor().requestToken = null;
                accessorInfo.getAccessor().tokenSecret = null;
            }
            return (attempts < MAX_ATTEMPTS && pe.canRetry);
        }

        private sResponse attemptFetch()
        {
            if (needApproval())
            {
                // This is section 6.1 of the OAuth spec.
                checkCanApprove();
                fetchRequestToken();
                // This is section 6.2 of the OAuth spec.
                buildClientApprovalState();
                buildAznUrl();
                // break out of the content fetching chain, we need permission from
                // the user to do this
                return buildOAuthApprovalResponse();
            }
            else if (needAccessToken())
            {
                // This is section 6.3 of the OAuth spec
                checkCanApprove();
                exchangeRequestToken();
                saveAccessToken();
                buildClientAccessState();
            }
            return fetchData();
        }

        /**
        * Do we need to get the user's approval to access the data?
        */
        private bool needApproval()
        {
            return (realRequest.OAuthArguments.MustUseToken()
                        && accessorInfo.getAccessor().requestToken == null
                        && accessorInfo.getAccessor().accessToken == null);
        }

        /**
        * Make sure the user is authorized to approve access tokens.  At the moment
        * we restrict this to page owner's viewing their own pages.
        *
        * @throws GadgetException
        */
        private void checkCanApprove()
        {
            String pageOwner = realRequest.SecurityToken.getOwnerId();
            String pageViewer = realRequest.SecurityToken.getViewerId();
            String stateOwner = clientState.getOwner();
            if (!pageOwner.Equals(pageViewer))
            {
                throw new UserVisibleOAuthException("Only page owners can grant OAuth approval");
            }
            if (stateOwner != null && !stateOwner.Equals(pageOwner))
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR,
                        "Client state belongs to a different person.");
            }
        }

        private void fetchRequestToken()
        {
            try
            {
                OAuthAccessor accessor = accessorInfo.getAccessor();
                sRequest request = new sRequest(Uri.parse(accessor.consumer.serviceProvider.requestTokenURL));
                request.setMethod(accessorInfo.getHttpMethod().ToString());
                if (accessorInfo.getHttpMethod().CompareTo(AccessorInfo.HttpMethod.POST) == 0)
                {
                    request.setHeader("Content-Type", OAuth.FORM_ENCODED);
                }

                sRequest signed = sanitizeAndSign(request, null);

                OAuthMessage reply = sendOAuthMessage(signed);

                reply.requireParameters(new string[] { OAuth.OAUTH_TOKEN, OAuth.OAUTH_TOKEN_SECRET });
                accessor.requestToken = reply.getParameter(OAuth.OAUTH_TOKEN);
                accessor.tokenSecret = reply.getParameter(OAuth.OAUTH_TOKEN_SECRET);
            }
            catch (OAuthException e)
            {
                throw new UserVisibleOAuthException(e.Message, e);
            }
            catch (IOException e)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR, e);
            }
        }

        /**
        * Strip out any owner or viewer identity information passed by the client.
        * 
        * @throws RequestSigningException
        */
        private List<OAuth.Parameter> sanitize(List<OAuth.Parameter> parameters)
        {
            List<OAuth.Parameter> list = new List<OAuth.Parameter>();
            foreach (OAuth.Parameter p in parameters)
            {
                String name = p.Key;
                if (allowParam(name))
                {
                    list.Add(p);
                }
                else
                {
                    throw new RequestSigningException("invalid parameter name " + name);
                }
            }
            return list;
        }

        private bool allowParam(String paramName)
        {
            String canonParamName = paramName.ToLower();
            return (!(canonParamName.StartsWith("oauth") ||
                    canonParamName.StartsWith("xoauth") ||
                    canonParamName.StartsWith("opensocial")) &&
                    ALLOWED_PARAM_NAME.IsMatch(canonParamName));
        }

        /**
        * Add identity information, such as owner/viewer/gadget.
        */
        private void addIdentityParams(List<OAuth.Parameter> parameters)
        {
            String owner = realRequest.SecurityToken.getOwnerId();
            if (owner != null && realRequest.OAuthArguments.SignOwner)
            {
                parameters.Add(new OAuth.Parameter(OPENSOCIAL_OWNERID, owner));
            }

            String viewer = realRequest.SecurityToken.getViewerId();
            if (viewer != null && realRequest.OAuthArguments.SignViewer)
            {
                parameters.Add(new OAuth.Parameter(OPENSOCIAL_VIEWERID, viewer));
            }

            String app = realRequest.SecurityToken.getAppId();
            if (app != null)
            {
                parameters.Add(new OAuth.Parameter(OPENSOCIAL_APPID, app));
            }

            String appUrl = realRequest.SecurityToken.getAppUrl();
            if (appUrl != null)
            {
                parameters.Add(new OAuth.Parameter(OPENSOCIAL_APPURL, appUrl));
            }

            if (accessorInfo.getConsumer().getConsumer().consumerKey == null)
            {
                parameters.Add(new OAuth.Parameter(OAuth.OAUTH_CONSUMER_KEY, realRequest.SecurityToken.getDomain()));
            }
        }

        /**
        * Add signature type to the message.
        */
        private void addSignatureParams(List<OAuth.Parameter> parameters)
        {
            if (accessorInfo.getConsumer().getKeyName() != null)
            {
                parameters.Add(new OAuth.Parameter(XOAUTH_PUBLIC_KEY, accessorInfo.getConsumer().getKeyName()));
            }
        }

        private String getAuthorizationHeader(List<OAuth.Parameter> oauthParams)
        {
            StringBuilder result = new StringBuilder("OAuth ");
            bool first = true;
            foreach (var parameter in oauthParams)
            {
                if (!first)
                {
                    result.Append(", ");
                }
                else
                {
                    first = false;
                }
                result.Append(OAuth.percentEncode(parameter.Key))
                    .Append("=\"")
                    .Append(OAuth.percentEncode(parameter.Value))
                    .Append('"');
            }
            return result.ToString();
        }


        /*
        Start with an HttpRequest.
        Throw if there are any attacks in the query.
        Throw if there are any attacks in the post body.
        Build up OAuth parameter list
        Sign it.
        Add OAuth parameters to new request
        Send it.
        */
        public sRequest sanitizeAndSign(sRequest basereq, List<OAuth.Parameter> parameters)
        {
            if (parameters == null)
            {
                parameters = new List<OAuth.Parameter>();
            }
            UriBuilder target = new UriBuilder(basereq.Uri);
            String query = target.getQuery();
            target.setQuery(null);
            parameters.AddRange(sanitize(OAuth.decodeForm(query)));
            if (OAuth.isFormEncoded(basereq.getHeader("Content-Type")))
            {
                parameters.AddRange(sanitize(OAuth.decodeForm(basereq.getPostBodyAsString())));
            }

            addIdentityParams(parameters);

            addSignatureParams(parameters);

            try
            {
                OAuthMessage signed = accessorInfo.getAccessor().newRequestMessage(
                                                basereq.getMethod(), target.ToString(), parameters);
                sRequest oauthHttpRequest = createHttpRequest(basereq, selectOAuthParams(signed));
                // Following 302s on OAuth responses is unlikely to be productive.
                oauthHttpRequest.FollowRedirects = false;
                return oauthHttpRequest;
            }
            catch (Exception e)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR, e);
            }
        }

        private sRequest createHttpRequest(sRequest basereq, List<OAuth.Parameter> oauthParams)
        {
            AccessorInfo.OAuthParamLocation paramLocation = accessorInfo.getParamLocation();

            // paramLocation could be overriden by a run-time parameter to fetchRequest

            sRequest result = new sRequest(basereq);

            // If someone specifies that OAuth parameters go in the body, but then sends a request for
            // data using GET, we've got a choice.  We can throw some type of error, since a GET request
            // can't have a body, or we can stick the parameters somewhere else, like, say, the header.
            // We opt to put them in the header, since that stands some chance of working with some
            // OAuth service providers.
            if (paramLocation == AccessorInfo.OAuthParamLocation.POST_BODY &&
                !result.getMethod().Equals("POST"))
            {
                paramLocation = AccessorInfo.OAuthParamLocation.AUTH_HEADER;
            }

            switch (paramLocation)
            {
                case AccessorInfo.OAuthParamLocation.AUTH_HEADER:
                    result.addHeader("Authorization", getAuthorizationHeader(oauthParams));
                    break;

                case AccessorInfo.OAuthParamLocation.POST_BODY:
                    String contentType = result.getHeader("Content-Type");
                    if (!OAuth.isFormEncoded(contentType))
                    {
                        throw new UserVisibleOAuthException(
                                "OAuth param location can only be post_body if post body if of " +
                                "type x-www-form-urlencoded");
                    }
                    String oauthData = OAuth.formEncode(oauthParams);
                    if (result.getPostBodyLength() == 0)
                    {
                        result.setPostBody(Encoding.UTF8.GetBytes(oauthData));
                    }
                    else
                    {
                        result.setPostBody(Encoding.UTF8.GetBytes(result.getPostBodyAsString() + '&' + oauthData));
                    }
                    break;

                case AccessorInfo.OAuthParamLocation.URI_QUERY:
                    result.Uri = Uri.parse(OAuth.addParameters(result.Uri.ToString(), oauthParams));
                    break;
            }
            return result;
        }

        /**
        * Sends OAuth request token and access token messages.
        * @throws GadgetException 
        * @throws IOException 
        * @throws OAuthProtocolException 
        */
        private OAuthMessage sendOAuthMessage(sRequest request)
        {
            sResponse response = nextFetcher.fetch(request);
            checkForProtocolProblem(response);
            OAuthMessage reply = new OAuthMessage(null, null, null);

            reply.addParameters(OAuth.decodeForm(response.responseString));
            reply = parseAuthHeader(reply, response);
            return reply;
        }

        /**
        * Parse OAuth WWW-Authenticate header and either add them to an existing
        * message or create a new message.
        *
        * @param msg
        * @param resp
        * @return the updated message.
        */
        private OAuthMessage parseAuthHeader(OAuthMessage msg, sResponse resp)
        {
            if (msg == null)
            {
                msg = new OAuthMessage(null, null, null);
            }

            foreach (String auth in resp.getHeaders("WWW-Authenticate"))
            {
                msg.addParameters(OAuthMessage.decodeAuthorization(auth));
            }

            return msg;
        }

        /**
        * Builds the data we'll cache on the client while we wait for approval.
        */
        private void buildClientApprovalState()
        {
            OAuthAccessor accessor = accessorInfo.getAccessor();
            responseParams.getNewClientState().setRequestToken(accessor.requestToken);
            responseParams.getNewClientState().setRequestTokenSecret(accessor.tokenSecret);
            responseParams.getNewClientState().setOwner(realRequest.SecurityToken.getOwnerId());
        }

        /**
        * Builds the URL the client needs to visit to approve access.
        */
        private void buildAznUrl()
        {
            // At some point we can be clever and use a callback URL to improve
            // the user experience, but that's too complex for now.
            OAuthAccessor accessor = accessorInfo.getAccessor();
            StringBuilder azn = new StringBuilder(
                                    accessor.consumer.serviceProvider.userAuthorizationURL);
            if (azn.ToString().IndexOf("?") == -1)
            {
                azn.Append('?');
            }
            else
            {
                azn.Append('&');
            }
            azn.Append(OAuth.OAUTH_TOKEN);
            azn.Append('=');
            azn.Append(OAuth.percentEncode(accessor.requestToken));
            responseParams.setAznUrl(azn.ToString());
        }

        private sResponse buildOAuthApprovalResponse()
        {
            //return buildNonDataResponse(HttpStatusCode.OK);
            return null;
        }

        /**
        * Do we need to exchange a request token for an access token?
        */
        private bool needAccessToken()
        {
            return (realRequest.OAuthArguments.MustUseToken()
                && accessorInfo.getAccessor().requestToken != null
                && accessorInfo.getAccessor().accessToken == null);
        }

        /**
        * Implements section 6.3 of the OAuth spec.
        * @throws OAuthProtocolException
        */
        private void exchangeRequestToken()
        {
            try
            {
                OAuthAccessor accessor = accessorInfo.getAccessor();
                sRequest request = new sRequest( Uri.parse(accessor.consumer.serviceProvider.accessTokenURL));
                request.setMethod(accessorInfo.getHttpMethod().ToString());
                if (accessorInfo.getHttpMethod() == AccessorInfo.HttpMethod.POST)
                {
                    request.setHeader("Content-Type", OAuth.FORM_ENCODED);
                }

                List<OAuth.Parameter> msgParams = new List<OAuth.Parameter>();
                msgParams.Add(new OAuth.Parameter(OAuth.OAUTH_TOKEN, accessor.requestToken));

                sRequest signed = sanitizeAndSign(request, msgParams);

                OAuthMessage reply = sendOAuthMessage(signed);

                reply.requireParameters(new string[] { OAuth.OAUTH_TOKEN, OAuth.OAUTH_TOKEN_SECRET });
                accessor.accessToken = reply.getParameter(OAuth.OAUTH_TOKEN);
                accessor.tokenSecret = reply.getParameter(OAuth.OAUTH_TOKEN_SECRET);
            }
            catch (OAuthException e)
            {
                throw new UserVisibleOAuthException(e.Message, e);
            }
            catch (IOException e)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR, e);
            }
        }

        /**
        * Save off our new token and secret to the persistent store.
        *
        * @throws GadgetException
        */
        private void saveAccessToken()
        {
            OAuthAccessor accessor = accessorInfo.getAccessor();
            OAuthStore.TokenInfo tokenInfo = new OAuthStore.TokenInfo(accessor.accessToken, accessor.tokenSecret);
            fetcherConfig.getTokenStore().storeTokenKeyAndSecret(realRequest.SecurityToken,
            accessorInfo.getConsumer(), realRequest.OAuthArguments, tokenInfo);
        }

        /**
        * Builds the data we'll cache on the client while we make requests.
        */
        private void buildClientAccessState()
        {
            OAuthAccessor accessor = accessorInfo.getAccessor();
            responseParams.getNewClientState().setAccessToken(accessor.accessToken);
            responseParams.getNewClientState().setAccessTokenSecret(accessor.tokenSecret);
            responseParams.getNewClientState().setOwner(realRequest.SecurityToken.getOwnerId());
        }

        /**
        * Get honest-to-goodness user data.
        *
        * @throws OAuthProtocolException if the service provider returns an OAuth
        * related error instead of user data.
        */
        private sResponse fetchData()
        {
            try
            {
                sRequest signed = sanitizeAndSign(realRequest, null);
                sResponse response = nextFetcher.fetch(signed);
                checkForProtocolProblem(response);

                // Track metadata on the response
                HttpResponseBuilder builder = new HttpResponseBuilder(response);
                responseParams.addToResponse(builder);
                //return builder.create();
                return null;
            }
            catch (Exception e)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR, e);
            }
        }

        /**
        * Look for an OAuth protocol problem.  For cases where no access token is in play 
        * @param response
        * @throws OAuthProtocolException
        * @throws IOException
        */
        private void checkForProtocolProblem(sResponse response)
        {
            if (isFullOAuthError(response))
            {
                OAuthMessage message = parseAuthHeader(null, response);
                if (message.getParameter(OAuthProblemException.OAUTH_PROBLEM) != null)
                {
                    // SP reported extended error information
                    throw new OAuthProtocolException(message);
                }
                // No extended information, guess based on HTTP response code.
                throw new OAuthProtocolException((int)response.getHttpStatusCode());
            }
        }

        /**
        * Check if a response might be due to an OAuth protocol error.  We don't want to intercept
        * errors for signed fetch, we only care about places where we are dealing with OAuth request
        * and/or access tokens.
        */
        private bool isFullOAuthError(sResponse response)
        {
            // Is this an error in the 4xx range?  If so, it might be OAuth related.
            if ((int)response.getHttpStatusCode() < 400 || (int)response.getHttpStatusCode() >= 500)
            {
                return false;
            }
            // If the client forced us to use full OAuth, this might be OAuth related.
            if (realRequest.OAuthArguments.MustUseToken())
            {
                return true;
            }
            // If we're using an access token, this might be OAuth related.
            if (accessorInfo.getAccessor().accessToken != null)
            {
                return true;
            }
            // Not OAuth related.
            return false;
        }

        /**
        * Extracts only those parameters from an OAuthMessage that are OAuth-related.
        * An OAuthMessage may hold a whole bunch of non-OAuth-related parameters
        * because they were all needed for signing. But when constructing a request
        * we need to be able to extract just the OAuth-related parameters because
        * they, and only they, may have to be put into an Authorization: header or
        * some such thing.
        *
        * @param message the OAuthMessage object, which holds non-OAuth parameters
        * such as foo=bar (which may have been in the original URI query part, or
        * perhaps in the POST body), as well as OAuth-related parameters (such as
        * oauth_timestamp or oauth_signature).
        *
        * @return a list that contains only the oauth_related parameters.
        *
        * @throws IOException
        */
        private List<OAuth.Parameter> selectOAuthParams(OAuthMessage message)
        {
            List<OAuth.Parameter> result = new List<OAuth.Parameter>();
            foreach (var param in message.getParameters())
            {
                if (isContainerInjectedParameter(param.Key))
                {
                    result.Add(param);
                }
            }
            return result;
        }

        private bool isContainerInjectedParameter(String key)
        {
            key = key.ToLower();
            return key.StartsWith("oauth") || key.StartsWith("xoauth") || key.StartsWith("opensocial");
        }
    } 
}
