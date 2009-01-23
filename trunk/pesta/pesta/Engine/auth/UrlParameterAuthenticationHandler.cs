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
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace Pesta.Engine.auth
{
    /// <summary>
    /// Summary description for UrlParameterAuthenticationHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class UrlParameterAuthenticationHandler : AuthenticationHandler
    {
        public static readonly String AUTH_URL_PARAMETER = "SecurityTokenUrlParameter";

        private readonly SecurityTokenDecoder securityTokenDecoder;

        public UrlParameterAuthenticationHandler()
        {
            this.securityTokenDecoder = BasicSecurityTokenDecoder.Instance;
        }

        public String getName()
        {
            return AUTH_URL_PARAMETER;
        }

        public ISecurityToken getSecurityTokenFromRequest(HttpRequest request)
        {
            try
            {
                String token = request.Params["st"];
                if (token == null)
                {
                    return null;
                }
                if (token.Split(':').Length != 6)
                {
                    token = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(token)));
                }

                Dictionary<String, String> parameters = new Dictionary<string, string> { { SecurityTokenDecoder.SECURITY_TOKEN_NAME, token } };
                return securityTokenDecoder.createToken(parameters);
            }
            catch (SecurityTokenException e)
            {
                return null;
            }
        }
    }
}