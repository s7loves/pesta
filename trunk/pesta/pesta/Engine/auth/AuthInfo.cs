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

namespace Pesta.Engine.auth
{
    /// <summary> Class to get authorization information on a servlet request.
    /// 
    /// Information is set by adding an AuthentiationServletFilter, and there
    /// is no way to set in a public API. This can be added in the future for testing
    /// purposes.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class AuthInfo
    {
        HttpContext context;
        string url;
        /**
        * Create AuthInfo from a given HttpServletRequest
        * @param req
        */

        public AuthInfo(HttpContext context, string url)
        {
            this.context = context;
            this.url = url;
        }
        /**
        * Constants for request attribute keys
        * 
        * This is only public for testing.
        */
        public enum Attribute
        {
            /** The security token */
            SECURITY_TOKEN,
            /** The named auth type */
            AUTH_TYPE
        }

        /**
        * Get the security token for this request.
        *
        * @return The security token
        */
        public ISecurityToken getSecurityToken()
        {
            return context.Items[url + Attribute.SECURITY_TOKEN.ToString()] as ISecurityToken;
        }

        /**
        * Get the hosted domain for this request.
        *
        * @return The domain, or {@code null} if no domain was found
        */
        public String getAuthType()
        {
            return context.Items[url + Attribute.AUTH_TYPE.ToString()] as String;
        }

        /**
        * Set the security token for the request.
        *
        * @param token The security token
        * @return This object
        */
        public AuthInfo setSecurityToken(ISecurityToken token)
        {
            context.Items[url + Attribute.SECURITY_TOKEN.ToString()] = token;
            return this;
        }

        /**
        * Set the auth type for the request.
        *
        * @param authType The named auth type
        * @return This object
        */
        public AuthInfo setAuthType(String authType)
        {
            context.Items[url + Attribute.AUTH_TYPE.ToString()] = authType;
            return this;
        }

    }
}