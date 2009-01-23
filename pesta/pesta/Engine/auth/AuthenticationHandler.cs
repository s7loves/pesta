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
    /// <summary>
    /// Summary description for AuthenticationHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public interface AuthenticationHandler
    {
        /**
       * @return The name of the authentication handler, used for debugging.
       */
        String getName();

        /**
         * Produce a security token extracted from the HTTP request.
         *
         * @param request The request to extract a token from.
         * @return A valid security token for the request, or null if it wasn't possible to authenticate.
         */
        ISecurityToken getSecurityTokenFromRequest(HttpRequest request);
    }
}