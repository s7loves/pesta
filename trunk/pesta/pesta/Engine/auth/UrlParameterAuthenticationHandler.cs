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
using System.Text;
using System.Web;

namespace Pesta.Engine.auth
{
    /// <summary>
    /// Summary description for UrlParameterAuthenticationHandler
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class UrlParameterAuthenticationHandler : IAuthenticationHandler
    {
        public static readonly String AUTH_URL_PARAMETER = "SecurityTokenUrlParameter";
        private const String TOKEN_PARAM = "st";

        private readonly SecurityTokenDecoder securityTokenDecoder;

        public UrlParameterAuthenticationHandler()
        {
            securityTokenDecoder = BasicSecurityTokenDecoder.Instance;
        }

        public override String getName()
        {
            return AUTH_URL_PARAMETER;
        }

        public override ISecurityToken getSecurityTokenFromRequest(HttpRequest request)
        {
            Dictionary<String, String> parameters = getMappedParameters(request);
            try 
            {
                if (!parameters.ContainsKey(SecurityTokenDecoder.SECURITY_TOKEN_NAME))
                {
                    return null;
                }

                return securityTokenDecoder.createToken(parameters);
            } 
            catch (SecurityTokenException e) 
            {
                throw new InvalidAuthenticationException("Malformed security token " + parameters[SecurityTokenDecoder.SECURITY_TOKEN_NAME], e);
            }
        }

        public override String getWWWAuthenticateHeader(String realm) 
        {
            return null;
        }

        protected SecurityTokenDecoder getSecurityTokenDecoder() 
        {
            return securityTokenDecoder;
        }

        protected Dictionary<String, String> getMappedParameters(HttpRequest request)
        {
            String token = request.Params[TOKEN_PARAM];
            if (!String.IsNullOrEmpty(token) && token.Split(':').Length != BasicSecurityTokenDecoder.TOKEN_COUNT)
            {
                token = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(token)));
            }
            return new Dictionary<string, string> {{SecurityTokenDecoder.SECURITY_TOKEN_NAME, token}};
        }

    }
}