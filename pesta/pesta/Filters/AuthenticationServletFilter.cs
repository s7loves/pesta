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
        private List<AuthenticationHandler> handlers = new List<AuthenticationHandler> 
                                                           { 
                                                               new UrlParameterAuthenticationHandler(),
                                                               new AnonymousAuthenticationHandler(),
                                                               new OAuthConsumerRequestAuthenticationHandler(new SampleContainerOAuthLookupService())
                                                           };

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
                foreach (AuthenticationHandler handler in handlers)
                {
                    ISecurityToken token = handler.getSecurityTokenFromRequest(request);
                    if (token != null)
                    {
                        new AuthInfo(app.Context, request.RawUrl).setAuthType(handler.getName()).setSecurityToken(token);
                        return;
                    }
                }
            }
        }

        void IHttpModule.Dispose()
        {
        }
    }
}