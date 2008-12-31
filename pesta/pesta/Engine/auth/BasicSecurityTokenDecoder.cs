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

namespace Pesta
{
    /// <summary>
    /// Summary description for BasicSecurityTokenDecoder
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BasicSecurityTokenDecoder : SecurityTokenDecoder
    {
        private static readonly int OWNER_INDEX = 0;
        private static readonly int VIEWER_INDEX = 1;
        private static readonly int APP_ID_INDEX = 2;
        private static readonly int CONTAINER_INDEX = 3;
        private static readonly int APP_URL_INDEX = 4;
        private static readonly int MODULE_ID_INDEX = 5;
        private static readonly int TOKEN_COUNT = MODULE_ID_INDEX + 1;


        /**
        * Creates a signer with 24 hour token expiry
        */
        public static readonly BasicSecurityTokenDecoder Instance = new BasicSecurityTokenDecoder();
        protected BasicSecurityTokenDecoder()
        {
        }


        /**
        * Encodes a token using the a plaintext dummy format.
        */
        public String encodeToken(SecurityToken token)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(HttpUtility.UrlEncode(token.getOwnerId(), Encoding.UTF8)).Append(':')
                    .Append(HttpUtility.UrlEncode(token.getViewerId(), Encoding.UTF8)).Append(':')
                    .Append(HttpUtility.UrlEncode(token.getAppId(), Encoding.UTF8)).Append(':')
                    .Append(HttpUtility.UrlEncode(token.getDomain(), Encoding.UTF8)).Append(':')
                    .Append(HttpUtility.UrlEncode(token.getAppUrl(), Encoding.UTF8)).Append(':')
                    .Append(token.getModuleId().ToString());
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /**
        * {@inheritDoc}
        *
        * Returns a token with some faked out values.
        */
        public override SecurityToken createToken(Dictionary<String, String> parameters)
        {
            String token = parameters[SECURITY_TOKEN_NAME];
            if (token == null || token.Trim().Length == 0)
            {
                // No token is present, assume anonymous access
                return new AnonymousSecurityToken();
            }

            try
            {
                String[] tokens = token.Split(':');
                if (tokens.Length != TOKEN_COUNT && PestaSettings.AllowPlaintextToken.ToLower().Equals("true"))
                {
                    //throw new SecurityTokenException("Malformed security token");
                    return BasicSecurityToken.createFromToken(token, int.Parse(PestaSettings.TokenMaxAge));
                }
                else
                {
                    return new BasicSecurityToken(
                        HttpUtility.UrlDecode(tokens[OWNER_INDEX], Encoding.UTF8),
                        HttpUtility.UrlDecode(tokens[VIEWER_INDEX], Encoding.UTF8),
                        HttpUtility.UrlDecode(tokens[APP_ID_INDEX], Encoding.UTF8),
                        HttpUtility.UrlDecode(tokens[CONTAINER_INDEX], Encoding.UTF8),
                        HttpUtility.UrlDecode(tokens[APP_URL_INDEX], Encoding.UTF8),
                        HttpUtility.UrlDecode(tokens[MODULE_ID_INDEX], Encoding.UTF8));
                }
            }
            catch (BlobCrypterException e)
            {
                throw new SecurityTokenException(e);
            }
            catch (Exception e)
            {
                throw new SecurityTokenException(e);
            }
        }


    } 
}
