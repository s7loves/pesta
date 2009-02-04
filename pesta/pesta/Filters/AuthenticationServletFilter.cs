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
using System.Net;
using System.Web;
using System.Collections.Generic;
using Pesta.Engine.auth;
using Pesta.Engine.social.core.oauth;
using Pesta.Engine.social.oauth;

namespace Pesta.Filters
{
    /// <summary>
    /// Summary description for AuthenticationServletFilter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class AuthenticationServletFilter : IHttpModule
    {
        private List<IAuthenticationHandler> handlers = new List<IAuthenticationHandler> 
                                                           { 
                                                               new UrlParameterAuthenticationHandler(),
                                                               new AnonymousAuthenticationHandler(),
                                                               new OAuthConsumerRequestAuthenticationHandler(new SampleContainerOAuthLookupService())
                                                           };
          // At some point change this to a container specific realm
        private const String realm = "shindig";

        public void Init(HttpApplication context)
        {
            context.BeginRequest +=
                (new EventHandler(this.context_BeginRequest));
        }

        private void context_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication app = source as HttpApplication;
            if (app != null)
            {
                HttpRequest request = app.Context.Request;
                HttpResponse response = app.Context.Response;
                try
                {

                    foreach (IAuthenticationHandler handler in handlers)
                    {
                        ISecurityToken token = handler.getSecurityTokenFromRequest(request);
                        if (token != null)
                        {
                            new AuthInfo(app.Context, request.RawUrl).setAuthType(handler.getName()).setSecurityToken(
                                token);
                            return;
                        }
                        else
                        {
                            String authHeader = handler.getWWWAuthenticateHeader(realm);
                            if (authHeader != null)
                            {
                                response.AddHeader("WWW-Authenticate", authHeader);
                            }
                        }
                    }
                }
                catch (IAuthenticationHandler.InvalidAuthenticationException iae)
                {
                    if (iae.getAdditionalHeaders() != null)
                    {
                        foreach (var entry in iae.getAdditionalHeaders())
                        {
                            response.AddHeader(entry.Key, entry.Value);
                        }
                    }
                    if (iae.getRedirect() != null)
                    {
                        response.Redirect(iae.getRedirect());
                    }
                    else
                    {
                        response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        response.StatusDescription = iae.Message;
                    }
                }
            }
        }

        void IHttpModule.Dispose()
        {
        }
    }
}