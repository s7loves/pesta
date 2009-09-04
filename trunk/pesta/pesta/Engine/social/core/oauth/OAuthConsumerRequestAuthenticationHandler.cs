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
using System.Web;
using Pesta.Engine.auth;
using Pesta.Engine.social.oauth;
using Pesta.Libraries.OAuth;
using Pesta.Libraries.OAuth.server;

namespace Pesta.Engine.social.core.oauth
{
    public class OAuthConsumerRequestAuthenticationHandler : IAuthenticationHandler
    {
        public static readonly String AUTH_OAUTH_CONSUMER_REQUEST = "OAuth-ConsumerRequest";
        public static readonly String REQUESTOR_ID_PARAM = "xoauth_requestor_id";
        private readonly IOAuthLookupService service;

        public OAuthConsumerRequestAuthenticationHandler(IOAuthLookupService service)
        {
            this.service = service;
        }

        public override String getName()
        {
            return AUTH_OAUTH_CONSUMER_REQUEST;
        }

        public override ISecurityToken getSecurityTokenFromRequest(HttpRequest request)
        {
            OAuthMessage requestMessage = OAuthServlet.getMessage(request, null);

            String containerKey = getParameter(requestMessage, OAuth.OAUTH_CONSUMER_KEY);
            String containerSignature = getParameter(requestMessage, OAuth.OAUTH_SIGNATURE);
            String userId = request.Params[REQUESTOR_ID_PARAM].Trim();

            if (containerKey == null || containerSignature == null || string.IsNullOrEmpty(userId))
            {
                // This isn't a proper OAuth request
                return null;
            }

            try
            {
                if (service.thirdPartyHasAccessToUser(requestMessage, containerKey, userId)) 
                {
                    return service.getSecurityToken(containerKey, userId);
                }
                throw new InvalidAuthenticationException("Access for app not allowed",null);
            }
            catch (OAuthException oae) 
            {
                throw new InvalidAuthenticationException(oae.Message, oae);
            }
        }

        public override String getWWWAuthenticateHeader(String realm) 
        {
            return String.Format("OAuth realm=\"{0}\"", realm);
        }

        private String getParameter(OAuthMessage requestMessage, String key)
        {
            try
            {
                return requestMessage.getParameter(key);
            }
            catch
            {
                return null;
            }
        }
    }
}