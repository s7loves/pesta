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
using System.Configuration;
using System.Web;

namespace Pesta
{
    /// <summary>
    /// Summary description for AnonymousAuthenticationHandler
    /// </summary>
    /// <remarks>
    /// <para>
    /// Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class AnonymousAuthenticationHandler : AuthenticationHandler
    {
        public static readonly String ALLOW_UNAUTHENTICATED = "gadget.allowUnauthenticated";
        public static readonly String AUTH_UNAUTHENTICATED = "Unauthenticated";
        private readonly bool allowUnauthenticated;

        public AnonymousAuthenticationHandler()
        {
            this.allowUnauthenticated = ContainerConfig.getConfigurationValue(ALLOW_UNAUTHENTICATED).ToLower().Equals("true");
        }

        public String getName()
        {
            return AUTH_UNAUTHENTICATED;
        }

        public SecurityToken getSecurityTokenFromRequest(HttpRequest request)
        {
            if (allowUnauthenticated)
            {
                return new AnonymousSecurityToken();
            }
            return null;
        }
    } 
}
